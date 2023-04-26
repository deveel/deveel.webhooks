using System;

namespace Deveel.Webhooks {
	/// <summary>
	/// Enumerates the possible execution modes for webhook handles
	/// when received and processed.
	/// </summary>
	public enum HandlerExecutionMode {
		/// <summary>
		/// The handlers are executed sequentially, one at a time.
		/// </summary>
		Sequential,

		/// <summary>
		/// The handlers are executed in parallel, all at the same time.
		/// </summary>
		Parallel
	}
}
