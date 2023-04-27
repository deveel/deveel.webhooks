using System.Runtime.Serialization;

namespace Deveel.Facebook {
    /// <summary>
    /// Enumerates the types of buttons that can be added to a message.
    /// </summary>
    public enum ButtonType {
		/// <summary>
		/// The button is a link to an URL available on the web.
		/// </summary>
		[EnumMember(Value = ButtonTypeNames.WebUrl)]
		WebUrl,

		/// <summary>
		/// The button is a postback that can be used to
		/// send a data back to the application.
		/// </summary>
		[EnumMember(Value = ButtonTypeNames.Postback)]
		Postback,

		/// <summary>
		/// The button triggers a phone call (from within the device of the user)
		/// to a given number.
		/// </summary>
		[EnumMember(Value = ButtonTypeNames.PhoneNumber)]
		PhoneNumber,

		/// <summary>
		/// The button starts a login flow to an external application's
		/// identity manager
		/// </summary>
		[EnumMember(Value = ButtonTypeNames.AccountLink)]
		AccountLink,

		/// <summary>
		/// The button starts a logout flow to an external application's
		/// identity management system.
		/// </summary>
		[EnumMember(Value = ButtonTypeNames.AccountUnlink)]
		AccountUnlink,

		/// <summary>
		/// The button triggers the launch of an Instant Game associated to
		/// a page.
		/// </summary>
		[EnumMember(Value = ButtonTypeNames.GamePlay)]
		GamePlay
	}
}
