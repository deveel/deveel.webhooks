using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Deveel.Facebook {
	public class UrlPayload : TemplateAttachmentPayload {
		[JsonConstructor]
		public UrlPayload(string url) {
			if (string.IsNullOrWhiteSpace(url)) 
				throw new ArgumentException($"'{nameof(url)}' cannot be null or whitespace.", nameof(url));

			Url = url;
		}

		[JsonPropertyName("url")]
		public string Url { get; }
	}
}
