using DVLD_Business;
using System;
using Microsoft.Win32;

namespace DVLD
{
    public class clsGlobal
    {
        public static clsUser CurrentUser;
        private const string _RegistryKeyPath = @"HKEY_CURRENT_USER\SOFTWARE\DVLD";
        private const string _RegistrySubKeyPath = @"SOFTWARE\DVLD";
        private const string _UsernameValue = "Username";
        private const string _PasswordValue = "Password";

        public static bool RememberUsernameAndPassword(string Username, string Password)
        {
            try
            {
                Registry.SetValue(_RegistryKeyPath, _UsernameValue, Username, RegistryValueKind.String);
                Registry.SetValue(_RegistryKeyPath, _PasswordValue, Password, RegistryValueKind.String);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public static bool GetStoredCredentials(ref string Username, ref string Password)
        {
            try
            {
                Username = Registry.GetValue(_RegistryKeyPath, _UsernameValue, null) as string;
                Password = Registry.GetValue(_RegistryKeyPath, _PasswordValue, null) as string;

                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)) return true;
                else return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteStoredCredentials()
        {
            try
            {
                using(RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
                {
                    using(RegistryKey key = baseKey.OpenSubKey(_RegistrySubKeyPath, true))
                    {
                        if(key != null)
                        {
                            key.DeleteValue(_UsernameValue, false);
                            key.DeleteValue(_PasswordValue, false);
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}