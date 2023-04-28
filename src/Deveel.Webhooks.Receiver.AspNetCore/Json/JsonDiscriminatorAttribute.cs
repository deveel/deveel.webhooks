using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deveel.Json {
	/// <summary>
	/// Indiates the property that is used as a discriminator
	/// in a polymorphic class.
	/// </summary>
	/// <remarks>
	/// The name of the discriminator property must be the same
	/// as used by the JSON object, and it must be a string.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class JsonDiscriminatorAttribute : Attribute {
		/// <summary>
		/// Constructs the attribute with the given JSON property name.
		/// </summary>
		/// <param name="propertyName">
		/// The name of the JSON property that is used as a discriminator.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown when the given property name is null or whitespace.
		/// </exception>
		public JsonDiscriminatorAttribute(string propertyName) {
			if (string.IsNullOrWhiteSpace(propertyName)) 
				throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace.", nameof(propertyName));

			PropertyName = propertyName;
		}

		/// <summary>
		/// Gets the name of the JSON property that is used as a discriminator
		/// </summary>
		public string PropertyName { get; }
	}
}
