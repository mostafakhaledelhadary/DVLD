using DVLD_DataAccess;
using System;
using System.Data;
using static DVLD_Business.clsApplication;

namespace DVLD_Business
{
    public class clsLicense
    {
        public enum enIssueReason : byte
        {
            FirstTime = 1, Renew = 2, ReplacementForDamaged = 3, ReplacementForLost = 4
        }
        
        private enum enMode {AddNew = 1, Update = 2}
        private enMode _Mode = enMode.AddNew;

        public int LicenseID { get; set; }
        public int ApplicationID { get; set; }
        public clsApplication ApplicationInfo;

        public int DriverID { get; set; }
        public clsDriver DriverInfo;

        public int LicenseClassID { get; set; }
        public clsLicenseClass LicenseClassInfo;

        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; }
        public float PaidFees { get; set; }
        public bool IsActive { get; set; }
        public enIssueReason IssueReason { get; set; }
        public string IssueReasonText
        {
            get
            {
                switch (IssueReason)
                {
                    case enIssueReason.FirstTime: return "First Time";
                    case enIssueReason.ReplacementForLost: return "Replaced For Lost";
                    case enIssueReason.ReplacementForDamaged: return "Replaced For Damaged";
                    case enIssueReason.Renew: return "Renew";
                    default: return "Unknown";
                }
            }
        }

        public int CreatedByUserID { get; set; }
        public clsUser UserInfo;

        public bool IsDetained
        {
            get { return clsDetainedLicense.IsLicenseDetained(LicenseID); }
        }

        public clsDetainedLicense DetainedLicenseInfo
        {
            get
            {
                return (IsDetained) ? clsDetainedLicense.FindDetainedLicense(LicenseID) : null;
            }
        }

        public clsLicense()
        {
            ApplicationID = -1;
            DriverID = -1;
            LicenseClassID = -1;
            IssueDate = DateTime.Now;
            ExpirationDate = DateTime.Now;
            Notes = "";
            PaidFees = 0;
            IsActive = true;
            IssueReason = enIssueReason.FirstTime;
            CreatedByUserID = -1;

            _Mode = enMode.AddNew;
        }

        private clsLicense(int LicenseID, int ApplicationID, int DriverID, int LicenseClassID, DateTime IssueDate,
            DateTime ExpirationDate, string Notes, float PaidFees, bool IsActive, enIssueReason IssueReason, int CreatedByUserID)
        {
            this.LicenseID = LicenseID;
            this.ApplicationID = ApplicationID;
            ApplicationInfo = clsApplication.FindByApplicationID(ApplicationID);
            this.DriverID = DriverID;
            DriverInfo = clsDriver.FindByDriverID(DriverID);
            this.LicenseClassID = LicenseClassID;
            LicenseClassInfo = clsLicenseClass.Find(LicenseClassID);
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.Notes = Notes;
            this.PaidFees = PaidFees;
            this.IsActive = IsActive;
            this.IssueReason = IssueReason;
            this.CreatedByUserID = CreatedByUserID;
            UserInfo = clsUser.FindByUserID(CreatedByUserID);
        }

        private bool _AddNewLicense()
        {
            LicenseID = clsLicenseData.AddNewLicense(ApplicationID, DriverID, LicenseClassID, IssueDate,
                ExpirationDate, Notes, PaidFees, IsActive, (byte)IssueReason, CreatedByUserID);

            return (LicenseID != -1);
        }

        private bool _UpdateLicense()
        {
            return clsLicenseData.UpdateLicense(LicenseID, ApplicationID, DriverID, LicenseClassID, IssueDate,
                ExpirationDate, Notes, PaidFees, IsActive, (byte)IssueReason, CreatedByUserID);
        }

        public static clsLicense FindByLicenseID(int LicenseID)
        {
            int ApplicationID = -1, DriverID = -1, LicenseClassID = -1, CreatedByUserID = -1;
            DateTime IssueDate = DateTime.Now, ExpirationDate = DateTime.Now;
            string Notes = "";
            float PaidFees = 0;
            bool IsActive = false;
            byte IssueReason = 0;

            if (clsLicenseData.GetLicenseInfoByLicenseID(LicenseID, ref ApplicationID, ref DriverID, ref LicenseClassID, ref IssueDate,
                ref ExpirationDate, ref Notes, ref PaidFees, ref IsActive, ref IssueReason, ref CreatedByUserID))
            {
                return new clsLicense(LicenseID, ApplicationID, DriverID, LicenseClassID, IssueDate, ExpirationDate, Notes,
                    PaidFees, IsActive, (enIssueReason)IssueReason, CreatedByUserID);
            }
            else
                return null;
        }

        public static int GetActiveLicenseIDByPersonID(int PersonID, int LicenseClassID)
        {
            return clsLicenseData.GetActiveLicenseIDByPersonID(PersonID, LicenseClassID);
        }

        public static bool IsLicenseIssued(int PersonID, int LicenseClassID)
        {
            return clsLicenseData.IsLicenseIssued(PersonID, LicenseClassID);
        }

        public bool DeactivateCurrentLicense()
        {
            return clsLicenseData.DeactivateCurrentLicense(LicenseID);
        }

        public static DataTable GetDriverLicenses(int DriverID)
        {
            return clsLicenseData.GetAllLocalLicensesForDriver(DriverID);
        }

        public clsLicense RenewLicense(string Notes, int CreatedByUserID)
        {
            if (!IsLicenseExpired())
                return null;

            if (!IsActive)
                return null;

            if (IsDetained)
                return null;

            clsApplicationType AppType = clsApplicationType.Find((int)clsApplication.enApplicationType.RenewDrivingLicense);

            clsApplication NewApplication = new clsApplication
            {
                ApplicantPersonID = ApplicationInfo.ApplicantPersonID,
                ApplicationDate = DateTime.Now,
                ApplicationTypeID = Convert.ToInt32(clsApplication.enApplicationType.RenewDrivingLicense),
                ApplicationStatus = clsApplication.enApplicationStatus.Completed,
                LastStatusDate = DateTime.Now,
                PaidFees = (AppType != null) ? AppType.Fees : 0,
                CreatedByUserID = CreatedByUserID
            };

            if (!NewApplication.Save()) return null;

                clsLicense NewLicense = new clsLicense
            {
                ApplicationID = NewApplication.ApplicationID,
                DriverID = DriverID,
                LicenseClassID = LicenseClassID,
                IssueDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddYears(LicenseClassInfo.DefaultValidityLength),
                Notes = Notes,
                PaidFees = LicenseClassInfo.ClassFees,
                IsActive = true,
                IssueReason = enIssueReason.Renew,
                CreatedByUserID = CreatedByUserID
            };

            if(!NewLicense.Save()) return null;

            DeactivateCurrentLicense();

            return NewLicense;
        }

        public clsLicense Replace(enIssueReason IssueReason, int CreatedByUserID)
        {
            if(!IsActive) return null;
            if(IsLicenseExpired()) return null;
            if(IsDetained) return null;

            int AppTypeID = (IssueReason == enIssueReason.ReplacementForDamaged) ?
                (int)clsApplication.enApplicationType.ReplaceDamagedDrivingLicense :
                (int)clsApplication.enApplicationType.ReplaceLostDrivingLicense;

            clsApplicationType AppType = clsApplicationType.Find(AppTypeID);

            clsApplication NewApplication = new clsApplication
            {
                ApplicantPersonID = ApplicationInfo.ApplicantPersonID,
                ApplicationDate = DateTime.Now,
                ApplicationTypeID = AppTypeID,
                ApplicationStatus = clsApplication.enApplicationStatus.Completed,
                LastStatusDate = DateTime.Now,
                PaidFees = (AppType != null) ? AppType.Fees : 0,
                CreatedByUserID = CreatedByUserID
            };

            if(!NewApplication.Save()) return null;

            clsLicense NewLicense = new clsLicense
            {
                ApplicationID = NewApplication.ApplicationID,
                DriverID = DriverID,
                LicenseClassID = LicenseClassID,
                IssueDate = DateTime.Now,
                ExpirationDate = ExpirationDate,
                Notes = Notes,
                PaidFees = 0,
                IsActive = true,
                IssueReason = IssueReason,
                CreatedByUserID = CreatedByUserID
            };

            if(!NewLicense.Save()) return null;

            DeactivateCurrentLicense();

            return NewLicense;
        }

        public int Detain(float FineFees, int CreatedByUserID)
        {
            if (!IsActive) return -1;
            if (IsDetained) return -1;

            clsDetainedLicense DetainedLicense = new clsDetainedLicense
            {
                LicenseID = LicenseID,
                DetainDate = DateTime.Now,
                FineFees = FineFees,
                CreatedByUserID = CreatedByUserID,
                IsReleased = false,
                ReleaseDate = DateTime.MinValue,
                ReleasedByUserID = -1,
                ReleaseApplicationID = -1
            };

            if (!DetainedLicense.Save()) return -1;

            return DetainedLicense.DetainID;
        }

        public bool ReleaseDetainedLicense(int UserID, ref int ReleaseApplicationID)
        {
            if (!IsDetained) return false;

            clsApplicationType AppType = clsApplicationType.Find((int)clsApplication.enApplicationType.ReleaseDetainedDrivingLicense);

            clsApplication NewApplication = new clsApplication
            {
                ApplicantPersonID = ApplicationInfo.ApplicantPersonID,
                ApplicationDate = DateTime.Now,
                ApplicationTypeID = Convert.ToInt32(clsApplication.enApplicationType.ReleaseDetainedDrivingLicense),
                ApplicationStatus = clsApplication.enApplicationStatus.Completed,
                LastStatusDate = DateTime.Now,
                PaidFees = (AppType != null) ? AppType.Fees : 0,
                CreatedByUserID = UserID
            };

            if (!NewApplication.Save()) return false;

            ReleaseApplicationID = NewApplication.ApplicationID;

            if (DetainedLicenseInfo == null) return false;

            if (!DetainedLicenseInfo.ReleaseDetainedLicense(UserID, NewApplication.ApplicationID)) return false;

            return true;
        }

        public bool IsLicenseExpired()
        {
            return (ExpirationDate < DateTime.Now);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewLicense())
                        {
                            _Mode = enMode.Update;
                            return true;
                        }
                        else
                            return false;
                    }

                case enMode.Update:
                    return _UpdateLicense();

                default:
                    return false;
            }
        }
    }
}