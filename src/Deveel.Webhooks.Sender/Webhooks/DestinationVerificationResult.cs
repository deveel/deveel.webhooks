// Copyright 2022-2024 Antonello Provenzano
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

namespace Deveel.Webhooks {
	/// <summary>
	/// A value type that represents the result of a verification of a destination
	/// </summary>
	public readonly struct DestinationVerificationResult {
		private DestinationVerificationResult(bool success, int? statusCode, string? errorMessage) {
			StatusCode = statusCode;
			Successful = success;
			ErrorMessage = errorMessage;
		}

		/// <summary>
		/// Gets the status code of the HTTP response if
		/// any was returned.
		/// </summary>
		public int? StatusCode { get; }

		/// <summary>
		/// Gets a value indicating whether the verification was successful.
		/// </summary>
		public bool Successful { get; }

		/// <summary>
		/// Gets the error message, if any, returned by the destination,
		/// or if was set by the service.
		/// </summary>
		public string? ErrorMessage { get; }

		/// <summary>
		/// Creates a new <see cref="DestinationVerificationResult"/> that
		/// indicates a successful verification.
		/// </summary>
		/// <param name="statusCode">
		/// A HTTP status code returned by the destination.
		/// </param>
		/// <returns></returns>
		public static DestinationVerificationResult Success(int? statusCode) 
			=> new DestinationVerificationResult(true, statusCode, null);

		/// <summary>
		/// Creates a new <see cref="DestinationVerificationResult"/> that
		/// indicates a failed verification.
		/// </summary>
		/// <param name="statusCode">
		/// A HTTP status code returned by the destination.
		/// </param>
		/// <param name="errorMessage">
		/// An optional error message returned by the destination.
		/// </param>
		/// <returns></returns>
		public static DestinationVerificationResult Failed(int? statusCode, string? errorMessage = null) 
			=> new DestinationVerificationResult(false, statusCode, errorMessage);
	}
}
