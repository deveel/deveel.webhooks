using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Deveel.Webhooks {
	public sealed class WebhookReceiverBuilder {
		public WebhookReceiverBuilder(Type webhookType, IServiceCollection services) {
			if (webhookType == null)
				throw new ArgumentNullException(nameof(webhookType));

			if (!webhookType.IsClass || webhookType.IsAbstract)
				throw new ArgumentException("The webhook type must be a non-abstract class");

			WebhookType = webhookType;
			Services = services ?? throw new ArgumentNullException(nameof(services));

			RegisterReceiverMiddleware();
			RegisterVerifierMiddleware();
			RegisterDefaultReceiver();

			UseJsonParser();
		}

		public Type WebhookType { get; }

		public IServiceCollection Services { get; }

		private void RegisterReceiverMiddleware() {
			var middlewareType = typeof(WebhookRceiverMiddleware<>).MakeGenericType(WebhookType);
			Services.AddScoped(middlewareType);
		}

		private void RegisterVerifierMiddleware() {
			var middlewareType = typeof(WebhookRequestVerfierMiddleware<>).MakeGenericType(WebhookType);
			Services.AddScoped(middlewareType);
		}

		private void RegisterDefaultReceiver() {
			var receiverType = typeof(IWebhookReceiver<>).MakeGenericType(WebhookType);
			var defaultReceiverType = typeof(WebhookReceiver<>).MakeGenericType(WebhookType);

			Services.TryAddScoped(receiverType, defaultReceiverType);
			Services.TryAddScoped(defaultReceiverType, defaultReceiverType);
		}

		public WebhookReceiverBuilder UseReceiver<TReceiver>() {
			var receiverType = typeof(IWebhookReceiver<>).MakeGenericType(WebhookType);
			if (!receiverType.IsAssignableFrom(typeof(TReceiver)))
				throw new ArgumentException($"The type '{typeof(TReceiver)}' must be assignable from '{receiverType}'");

			Services.RemoveAll(receiverType);
			Services.AddScoped(receiverType, typeof(TReceiver));

			if (typeof(TReceiver).IsClass && !typeof(TReceiver).IsAbstract)
				Services.AddScoped(typeof(TReceiver), typeof(TReceiver));

			return this;
		}

		public WebhookReceiverBuilder AddHandler<THandler>() {
			var handlerType = typeof(IWebhookHandler<>).MakeGenericType(WebhookType);
			if (!handlerType.IsAssignableFrom(typeof(THandler)))
				throw new ArgumentException($"The type '{typeof(THandler)}' must be assignable from '{handlerType}'");

			Services.AddScoped(handlerType, typeof(THandler));

			if (typeof(THandler).IsClass && !typeof(THandler).IsAbstract)
				Services.AddScoped(typeof(THandler), typeof(THandler));

			return this;
		}

		public WebhookReceiverBuilder ConfigureOptions<TOptions>(string sectionName) where TOptions : class {
			var optionType = typeof(WebhookReceiverOptions<>).MakeGenericType(WebhookType);
			if (!optionType.IsAssignableFrom(typeof(TOptions)))
				throw new ArgumentException($"The options type '{typeof(TOptions)}' is not assignable from '{optionType}'");

			// TODO: Validate the configured options
			Services.AddOptions<TOptions>()
				.BindConfiguration(sectionName);

			return this;
		}

		public WebhookReceiverBuilder ConfigureOptions<TOptions>(Action<TOptions> configure) where TOptions : class {
			var optionType = typeof(WebhookReceiverOptions<>).MakeGenericType(WebhookType);
			if (!optionType.IsAssignableFrom(typeof(TOptions)))
				throw new ArgumentException($"The options type '{typeof(TOptions)}' is not assignable to '{optionType}'");

			// TODO: Validate the configured options
			Services.AddOptions<TOptions>()
				.Configure(configure);

			return this;
		}

		public WebhookReceiverBuilder UseJsonParser<TParser>(ServiceLifetime lifetime = ServiceLifetime.Singleton) {
			var parserType = typeof(IWebhookJsonParser<>).MakeGenericType(WebhookType);

			if (!parserType.IsAssignableFrom(typeof(TParser)))
				throw new ArgumentException($"The type '{typeof(TParser)}' is not assignable to '{parserType}'");

			Services.RemoveAll(parserType);
			Services.Add(new ServiceDescriptor(parserType, typeof(TParser), lifetime));

			if (typeof(TParser).IsClass && !typeof(TParser).IsAbstract)
				Services.Add(new ServiceDescriptor(typeof(TParser), typeof(TParser), lifetime));

			return this;
		}

		public WebhookReceiverBuilder UseJsonParser<TParser>(TParser parser)
			where TParser : class {
			var parserType = typeof(IWebhookJsonParser<>).MakeGenericType(WebhookType);

			if (!parserType.IsAssignableFrom(typeof(TParser)))
				throw new ArgumentException($"The type '{typeof(TParser)}' is not assignable to '{parserType}'");

			Services.RemoveAll(parserType);
			Services.AddSingleton(parserType, parser);

			Services.AddSingleton(parser);

			return this;
		}

		public WebhookReceiverBuilder UseJsonParser(JsonSerializerOptions options = null) {
			var parserType = typeof(IWebhookJsonParser<>).MakeGenericType(WebhookType);
			var jsonParserType = typeof(SystemTextWebhookJsonParser<>).MakeGenericType(WebhookType);
			var parser = Activator.CreateInstance(jsonParserType, new[] {options});

			Services.RemoveAll(parserType);
			Services.AddSingleton(parserType, parser);
			Services.AddSingleton(jsonParserType, parser);

			return this;
		}

		public WebhookReceiverBuilder UseNewtonsoftJsonParser(JsonSerializerSettings settings = null) {
			var parserType = typeof(IWebhookJsonParser<>).MakeGenericType(WebhookType);
			var jsonParserType = typeof(NewtonsoftWebhookJsonParser<>).MakeGenericType(WebhookType);
			var parser = Activator.CreateInstance(jsonParserType, new[] { settings });

			Services.RemoveAll(parserType);
			Services.AddSingleton(parserType, parser);
			Services.AddSingleton(jsonParserType, parser);

			return this;
		}

		public WebhookReceiverBuilder UseJsonParser<TWebhook>(Func<Stream, CancellationToken, Task<TWebhook>> parser)
			where TWebhook : class {
			if (typeof(TWebhook) != WebhookType)
				throw new ArgumentException($"The parser must return webhooks of type '{WebhookType}'");

			var parserType = typeof(IWebhookJsonParser<>).MakeGenericType(WebhookType);

			Services.RemoveAll(parserType);
			Services.AddSingleton(parserType, new DelegatedJsonParser<TWebhook>(parser));

			return this;
		}

		public WebhookReceiverBuilder UseJsonParser<TWebhook>(Func<string, TWebhook> parser)
			where TWebhook : class {
			if (typeof(TWebhook) != WebhookType)
				throw new ArgumentException($"The parser must return webhooks of type '{WebhookType}'");

			var parserType = typeof(IWebhookJsonParser<>).MakeGenericType(WebhookType);

			Services.RemoveAll(parserType);
			Services.AddSingleton(parserType, new DelegatedJsonParser<TWebhook>(parser));

			return this;
		}

		public WebhookReceiverBuilder UseSigner<TSigner>() where TSigner : class, IWebhookSigner {
			var signerType = typeof(IWebhookSigner<>).MakeGenericType(WebhookType);
			
			if (!signerType.IsAssignableFrom(typeof(TSigner))) {
				var signer = (IWebhookSigner) Activator.CreateInstance(typeof(TSigner));
				var wrapperType = typeof(WebhookSignerWrapper<>).MakeGenericType(WebhookType);
				var wrapper = Activator.CreateInstance(wrapperType, new[] { signer });
				Services.AddSingleton(signerType, wrapper);
			} else {
				Services.AddSingleton(signerType, typeof(TSigner));
			}

			var providerType = typeof(IWebhookSignerProvider<>).MakeGenericType(WebhookType);
			var defaultProviderType = typeof(DefaultWebhookSignerProvider<>).MakeGenericType(WebhookType);
			Services.TryAddSingleton(providerType, defaultProviderType);

			return this;
		}

		public WebhookReceiverBuilder UseSigner<TSigner>(TSigner provider)
			where TSigner : class, IWebhookSigner {
			var signerType = typeof(IWebhookSigner<>).MakeGenericType(WebhookType);

			if (!signerType.IsAssignableFrom(typeof(TSigner))) {
				var wrapperType = typeof(WebhookSignerWrapper<>).MakeGenericType(WebhookType);
				var wrapper = Activator.CreateInstance(wrapperType, new[] { provider });
				Services.AddSingleton(signerType, wrapper);
			} else {
				Services.AddSingleton(signerType, typeof(TSigner));
			}

			var providerType = typeof(IWebhookSignerProvider<>).MakeGenericType(WebhookType);
			var defaultProviderType = typeof(DefaultWebhookSignerProvider<>).MakeGenericType(WebhookType);
			Services.TryAddSingleton(providerType, defaultProviderType);

			return this;
		}

		class DefaultWebhookSignerProvider<TWebhook> : IWebhookSignerProvider<TWebhook>
			where TWebhook : class {
			private readonly IDictionary<string, IWebhookSigner> signers;

			public DefaultWebhookSignerProvider(IEnumerable<IWebhookSigner<TWebhook>> signers) {
				this.signers = new Dictionary<string, IWebhookSigner>(StringComparer.OrdinalIgnoreCase);

				if (signers != null) {
					foreach (var signer in signers) {
						foreach (var alg in signer.Algorithms) {
							this.signers[alg] = signer;
						}
					}
				}
			}

			public IWebhookSigner GetSigner(string algorithm) {
				if (!signers.TryGetValue(algorithm, out var signer))
					return null;

				return signer;
			}
		}

		#region WebhookSignatureProviderWrapper

		class WebhookSignerWrapper<TWebhook> : IWebhookSigner<TWebhook> where TWebhook : class {
			private readonly IWebhookSigner signer;

			public WebhookSignerWrapper(IWebhookSigner signer) {
				this.signer = signer;
			}

			public string[] Algorithms => signer.Algorithms;

			public string SignWebhook(string jsonBody, string secret) => signer.SignWebhook(jsonBody, secret);
		}

		#endregion

		#region DelegatedJsonParser

		class DelegatedJsonParser<TWebhook> : IWebhookJsonParser<TWebhook> where TWebhook : class {
			private readonly Func<Stream, CancellationToken, Task<TWebhook>> streamParser;
			private readonly Func<string, TWebhook> syncStringParser;

			public DelegatedJsonParser(Func<string, TWebhook> syncStringParser) {
				this.syncStringParser = syncStringParser;
			}

			public DelegatedJsonParser(Func<Stream, CancellationToken, Task<TWebhook>> parser) {
				this.streamParser = parser;
			}

			public async Task<TWebhook> ParseWebhookAsync(Stream utf8Stream, CancellationToken cancellationToken = default) {
				if (streamParser != null) {
					return await streamParser(utf8Stream, cancellationToken);
				} else if (syncStringParser != null) {
					using var reader = new StreamReader(utf8Stream, Encoding.UTF8);
					var json = await reader.ReadToEndAsync();

					return syncStringParser(json);
				}

				throw new NotSupportedException();
			}
		}

		#endregion
	}
}
