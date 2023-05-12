// Copyright 2022-2023 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Polly;

namespace Deveel.Webhooks {
	/// <summary>
	/// A base class for the clients that send HTTP requests to a destination.
	/// </summary>
	public abstract class WebhookSenderClient : IDisposable {
		private readonly IHttpClientFactory? httpClientFactory;

		private bool disposeClient;
		private HttpClient? httpClient;

		private bool disposed = false;

		/// <summary>
		/// Creates a new instance of the <see cref="WebhookSenderClient"/> class
		/// that uses the given HTTP client.
		/// </summary>
		/// <param name="httpClient">
		/// The HTTP client to use for sending the requests.
		/// </param>
		/// <param name="logger">
		/// A logger to use for logging the operations of the sender.
		/// </param>
		protected WebhookSenderClient(HttpClient httpClient, ILogger? logger) {
			this.httpClient = httpClient;
			disposeClient = (httpClient == null);

			Logger = logger ?? NullLogger.Instance;
		}

		/// <summary>
		/// Creates a new instance of the <see cref="WebhookSenderClient"/> class
		/// </summary>
		/// <param name="httpClientFactory">
		/// The factory of HTTP clients to use for sending the requests.
		/// </param>
		/// <param name="logger">
		/// A logger to use for logging the operations of the sender.
		/// </param>
		protected WebhookSenderClient(IHttpClientFactory? httpClientFactory, ILogger? logger) {
			this.httpClientFactory = httpClientFactory;
			Logger = logger ?? NullLogger.Instance;
		}

		
		/// <summary>
		/// Gets the timeout for each request sent.
		/// </summary>
		protected virtual TimeSpan? Timeout { get; }

		/// <summary>
		/// Gets the retry options for each request sent.
		/// </summary>
		protected virtual WebhookRetryOptions? Retry { get; }

		/// <summary>
		/// Gets the logger to use for logging the operations of the sender.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the name of the <see cref="HttpClient"/> to be obtained 
		/// from the <see cref="IHttpClientFactory"/>.
		/// </summary>
		protected virtual string? HttpClientName { get; }

		/// <summary>
		/// Throws an exception if the sender has been disposed
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Thrown when the sender has been disposed
		/// </exception>
		protected void ThrowIfDisposed() {
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		/// <summary>
		/// Create a HTTP client to use for sending the webhook
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		///   <item>
		///   When the sender was constructed with a <see cref="IHttpClientFactory"/>,
		///   this method will use it to create a new instance of <see cref="HttpClient"/>.
		///   </item>
		///   <item>
		///   When the sender was constructed with a <see cref="HttpClient"/>, this method
		///   returns the same instance.
		///   </item>
		///   <item>
		///   When neither a <see cref="IHttpClientFactory"/> or a <see cref="HttpClient"/>
		///   where provided, this method will create a new instance of <see cref="HttpClient"/>
		///   that will be disposed when the sender is disposed.
		///   </item>
		/// </list>
		/// </remarks>
		/// <returns>
		/// Returns an instance of <see cref="HttpClient"/> to use for sending the webhook,
		/// that can be already existing (when explicitly specified) or a new one (from the factory).
		/// </returns>
		/// <exception cref="ObjectDisposedException">
		/// Thrown when the sender has been disposed
		/// </exception>
		protected HttpClient GetOrCreateClient() {
			ThrowIfDisposed();

			if (httpClientFactory != null) {
				if (String.IsNullOrWhiteSpace(HttpClientName))
					return httpClientFactory.CreateClient();

				return httpClientFactory.CreateClient(HttpClientName);
			}

			if (httpClient == null) {
				httpClient = new HttpClient();
				disposeClient = true;
			}

			return httpClient;
		}

		/// <summary>
		/// Creates a retry policy for the given number of retries and the sleep time
		/// </summary>
		/// <param name="retryCount">
		/// The number of retries to perform
		/// </param>
		/// <param name="sleep">
		/// The time to sleep between retries
		/// </param>
		/// <returns></returns>
		protected IAsyncPolicy CreateRetryPolicy(int? retryCount, TimeSpan? sleep) {
			// TODO: Validate that the sum of the retry delays is less than the timeout
			var retryCountValue = (retryCount ?? Retry?.MaxRetries) ?? 0;
			var sleepValue = (sleep ?? Retry?.MaxDelay) ?? TimeSpan.FromMilliseconds(300);

			// the retry policy
			return Policy
				.Handle<HttpRequestException>()
				.Or<TaskCanceledException>()
				.Or<TimeoutException>()
				.WaitAndRetryAsync(retryCountValue, attempt => sleepValue);
		}

		/// <summary>
		/// Creates a timeout policy for a single try for the given time
		/// </summary>
		/// <typeparam name="T">
		/// The type of the result of the policy execution
		/// </typeparam>
		/// <param name="timeout">
		/// The timeout to apply
		/// </param>
		/// <returns>
		/// Returns a policy that will timeout the execution after the given time
		/// </returns>
		protected AsyncPolicy<T> CreateTryTimeoutPolicy<T>(TimeSpan? timeout) {
			// TODO: Validate that the timeout is not less than the retry timeout
			var timeoutValue = (timeout ?? Retry?.Timeout) ?? System.Threading.Timeout.InfiniteTimeSpan;
			return Policy.TimeoutAsync<T>(timeoutValue);
		}

		/// <summary>
		/// Creates a timeout policy for the given time
		/// </summary>
		/// <returns>
		/// Returns a policy that will timeout the execution after the given time
		/// </returns>
		protected AsyncPolicy CreateTimeoutPolicy() {
			// TODO: Validate that the timeout is not less than the retry timeout
			var timeOut = Timeout ?? System.Threading.Timeout.InfiniteTimeSpan;
			return Policy.TimeoutAsync(timeOut);
		}

		/// <summary>
		/// Sends the request to the given request through the HTTP channel.
		/// </summary>
		/// <param name="request">
		/// The HTTP request to be sent.
		/// </param>
		/// <param name="cancellationToken">
		/// A cancellation token that can be used to cancel the operation.
		/// </param>
		/// <returns>
		/// Returns a response message that was received from the remote destination.
		/// </returns>
		protected virtual Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			// we don't dispose the client, because it might be a singleton...
			// 1. if the client is coming from the IHttpClientFactory, it will be disposed by the factory
			// 2. if the client was set at the constructor, it will be disposed by the caller
			// 3. if the client was created by the sender, it will be disposed when the sender is disposed

			var client = GetOrCreateClient();
			return client.SendAsync(request, cancellationToken);
		}

		/// <summary>
		/// Deisposes the sender.
		/// </summary>
		/// <param name="disposing">
		/// Whether the method is called from the <see cref="Dispose()"/> method.
		/// </param>
		protected virtual void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing && disposeClient) {
					httpClient?.Dispose();
				}

				disposed = true;
			}
		}

		/// <inheritdoc/>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
