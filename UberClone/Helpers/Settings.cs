// Helpers/Settings.cs This file was automatically added when you installed the Settings Plugin. If you are not using a PCL then comment this file back in to use it.
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace UberClone.Helpers
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region Setting Constants

		private const string SettingsKey = "settings_key";
		private static readonly string SettingsDefault = string.Empty;

		#endregion


		public static string GeneralSettings
		{
			get
			{
				return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(SettingsKey, value);
			}
		}
        #region User_id
        private const string User_idKey = "User_idKey";
        private static readonly string User_idDefault = string.Empty;

        public static string User_id
        {
            get
            {
                return AppSettings.GetValueOrDefault(User_idKey, User_idDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(User_idKey, value);
            }
        }
        #endregion

        #region Username Constant
        private const string UsernameKey = "UsernameKey";
        private static readonly string UsernameDefault = string.Empty;

        public static string Username
        {
            get
            {
                return AppSettings.GetValueOrDefault(UsernameKey, UsernameDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UsernameKey, value);
            }
        }
        #endregion

        #region Usertype Constant
        private const string UsertypeKey = "UsertypeKey";
        private static readonly string UsertypeDefault = string.Empty;

        public static string Usertype
        {
            get
            {
                return AppSettings.GetValueOrDefault(UsertypeKey, UsertypeDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UsertypeKey, value);
            }
        }
        #endregion

        #region User_long Constant
        private const string User_longKey = "User_longKey";
        private static readonly string User_longDefault = string.Empty;

        public static string User_long
        {
            get
            {
                return AppSettings.GetValueOrDefault(User_longKey, User_longDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(User_longKey, value);
            }
        }
        #endregion

        #region User_Lat Constant
        private const string User_LatKey = "User_LatKey";
        private static readonly string User_LatDefault = string.Empty;

        public static string User_Lat
        {
            get
            {
                return AppSettings.GetValueOrDefault(User_LatKey, User_LatDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(User_LatKey, value);
            }
        }
        #endregion
        internal static void ClearAll()
        {
            User_id = null;
            Username = null;
            Usertype = null;
            User_long = null;
            User_Lat = null;
        }

    }
}