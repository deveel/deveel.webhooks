namespace Deveel.Webhooks {
	public class XmlSerializerOptions {
		public bool? IncludeNamespaces { get; set; }

		public IDictionary<string, string> Namespaces { get; set; } = new Dictionary<string, string>();

		public bool Indent { get; set; } = false;
	}
}
