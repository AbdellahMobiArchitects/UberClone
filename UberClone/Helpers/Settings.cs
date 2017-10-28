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


        #region User Constants

        #region user_id Constant
        private const string user_id_Key = "user_id_Key";
        private static readonly string User_id_default = string.Empty;

        public static string User_ID
        {
            get
            {
                return AppSettings.GetValueOrDefault(user_id_Key, User_id_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(user_id_Key, value);
            }
        }
        #endregion

        #region Username Constant
        private const string username_Key = "username_Key";
        private static readonly string username_default = string.Empty;

        public static string Username
        {
            get
            {
                return AppSettings.GetValueOrDefault(username_Key, username_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(username_Key, value);
            }
        }
        #endregion

        #region usertype Constant
        private const string usertype_Key = "usertype_Key";
        private static readonly string usertype_default = string.Empty;

        public static string Usertype
        {
            get
            {
                return AppSettings.GetValueOrDefault(usertype_Key, usertype_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(usertype_Key, value);
            }
        }
        #endregion

        #region user_longitude Constant
        private const string user_longitude_Key = "user_longitude_Key";
        private static readonly string user_longitude_default = string.Empty;

        public static string User_Longitude
        {
            get
            {
                return AppSettings.GetValueOrDefault(user_longitude_Key, user_longitude_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(user_longitude_Key, value);
            }
        }
        #endregion

        #region user_latitude Constant
        private const string user_latitude_Key = "user_latitude_Key";
        private static readonly string user_latitude_default = string.Empty;

        public static string User_Latitude
        {
            get
            {
                return AppSettings.GetValueOrDefault(user_latitude_Key, user_latitude_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(user_latitude_Key, value);
            }
        }
        #endregion

        #endregion

        #region Request Constants

        #region request_id Constant
        private const string request_id_Key = "request_id_key";
        private static readonly string request_id_default = string.Empty;

        public static string Request_ID
        {
            get
            {
                return AppSettings.GetValueOrDefault(request_id_Key, request_id_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(request_id_Key, value);
            }
        }
        #endregion

        #region requester_username Constant
        private const string requester_username_Key = "requester_username_Key";
        private static readonly string requester_username_default = string.Empty;

        public static string Requester_Username
        {
            get
            {
                return AppSettings.GetValueOrDefault(requester_username_Key, requester_username_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(requester_username_Key, value);
            }
        }
        #endregion

        #region requester_longitude Constant
        private const string requester_longitude_Key = "requester_longitude_Key";
        private static readonly string requester_longitude_default = string.Empty;

        public static string Requester_Longitude
        {
            get
            {
                return AppSettings.GetValueOrDefault(requester_longitude_Key, requester_longitude_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(requester_longitude_Key, value);
            }
        }
        #endregion

        #region requester_latitude Constant
        private const string requester_latitude_Key = "requester_latitude_Key";
        private static readonly string requester_latitude_default = string.Empty;

        public static string Requester_Latitude
        {
            get
            {
                return AppSettings.GetValueOrDefault(requester_latitude_Key, requester_latitude_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(requester_latitude_Key, value);
            }
        }
        #endregion

        #region driver_usename Constant
        private const string driver_usename_Key = "driver_usename_Key";
        private static readonly string driver_usename_default = string.Empty;

        public static string Driver_Username
        {
            get
            {
                return AppSettings.GetValueOrDefault(driver_usename_Key, driver_usename_default);
            }
            set
            {
                AppSettings.AddOrUpdateValue(driver_usename_Key, value);
            }
        }
        #endregion

        #endregion

        internal static void ClearUserLocalVars()
        {
            User_ID = null;
            Username = null;
            Usertype = null;
            User_Longitude = null;
            User_Latitude = null;
        }

        internal static void ClearRequestLocalVars()
        {
            Request_ID = null;
            Requester_Username = null;
            Requester_Longitude = null;
            Requester_Latitude = null;
            Driver_Username = null;
        }

    }
}