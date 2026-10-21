using System;
using System.Data;
using System.Runtime.CompilerServices;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsUser
    {
        private enum enMode : byte { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;

        public int UserID { get; set; }
        public int PersonID { get; set; }
        public clsPerson PersonInfo;

        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }



        public clsUser()
        {
            UserID = -1;
            PersonID = -1;
            Username = "";
            Password = "";
            IsActive = true;
            _Mode = enMode.AddNew;
        }

        private clsUser(int UserID, int PersonID, string Username, string Password, bool IsActive)
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.Username = Username;
            this.Password = Password;
            this.IsActive = IsActive;
            PersonInfo = clsPerson.Find(PersonID);
            _Mode = enMode.Update;
        }

        private bool _AddNewUser()
        {
            UserID = clsUserData.AddNewUser(PersonID, Username, Password, IsActive);

            return (UserID != -1);
        }

        private bool _UpdateUser()
        {
            return clsUserData.UpdateUser(UserID, PersonID, Username, Password, IsActive);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewUser())
                        {
                            _Mode = enMode.Update;
                            return true;
                        }
                        else
                            return false;
                    }

                case enMode.Update:
                    {
                        return _UpdateUser();
                    }

                default: return false;
            }
        }

        public static clsUser FindByUserID(int UserID)
        {
            int PersonID = -1;
            string Username = "", Password = "";
            bool IsActive = false;

            if (clsUserData.GetUserInfoByUserID(UserID, ref PersonID, ref Username, ref Password, ref IsActive))
                return new clsUser(UserID, PersonID, Username, Password, IsActive);
            else
                return null;
        }

        public static clsUser FindByPersonID(int PersonID)
        {
            int UserID = -1;
            string Username = "", Password = "";
            bool IsActive = false;

            if (clsUserData.GetUserInfoByPersonID(PersonID, ref UserID, ref Username, ref Password, ref IsActive))
                return new clsUser(UserID, PersonID, Username, Password, IsActive);
            else
                return null;
        }

        public static clsUser FindByUsernameAndPassword(string Username, string Password)
        {
            int UserID = -1, PersonID = -1;
            bool IsActive = false;

            if (clsUserData.GetUserInfoByUsernameAndPassword(Username, Password, ref UserID, ref PersonID, ref IsActive))
                return new clsUser(UserID, PersonID, Username, Password, IsActive);
            else
                return null;
        }

        public static bool DeleteUser(int UserID)
        {
            return clsUserData.DeleteUser(UserID);
        }

        public static bool IsUserExists(int UserID)
        {
            return clsUserData.IsUserExists(UserID);
        }

        public static bool IsUserExists(string Username)
        {
            return clsUserData.IsUserExists(Username);
        }

        public static bool IsPersonIsAUser(int PersonID)
        {
            return clsUserData.IsPersonIsAUser(PersonID);
        }

        public static DataTable GetAllUsers()
        {
            return clsUserData.GetAllUsers();
        }
    }
}
