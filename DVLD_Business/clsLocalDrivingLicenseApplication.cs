using System;
using System.Data;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsLocalDrivingLicenseApplication : clsApplication
    {
        private new enum enMode : byte { AddNew = 0, Update = 1 }
        private enMode _Mode;

        public int LocalDrivingLicenseApplicationID { get; set; }
        public int LicenseClassID { get; set; }
        public clsLicenseClass LicenseClassInfo;
        public string PersonFullName
        {
            get
            {
                return clsPerson.Find(ApplicantPersonID).FullName;
            }
        }

        public clsLocalDrivingLicenseApplication()
        {
            LocalDrivingLicenseApplicationID = -1;
            LicenseClassID = -1;

            _Mode = enMode.AddNew;
        }

        private clsLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID, int ApplicationID, int ApplicantPersonID,
            DateTime ApplicationDate, int ApplicationTypeID, enApplicationStatus ApplicationStatus, DateTime LastStatusDate,
            float PaidFees, int CreatedByUserID, int LicenseClassID)
        {
            this.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            this.ApplicationID = ApplicationID;
            this.ApplicantPersonID = ApplicantPersonID;
            PersonInfo = clsPerson.Find(ApplicantPersonID);
            this.ApplicationDate = ApplicationDate;
            this.ApplicationTypeID = ApplicationTypeID;
            ApplicationTypeInfo = clsApplicationType.Find(ApplicationTypeID);
            this.ApplicationStatus = ApplicationStatus;
            this.LastStatusDate = LastStatusDate;
            this.PaidFees = PaidFees;
            this.CreatedByUserID = CreatedByUserID;
            UserInfo = clsUser.FindByUserID(CreatedByUserID);
            this.LicenseClassID = LicenseClassID;
            LicenseClassInfo = clsLicenseClass.Find(LicenseClassID);

            _Mode = enMode.Update;
        }

        private bool _AddNewLocalDrivingLicenseApplication()
        {
            LocalDrivingLicenseApplicationID = clsLocalDrivingLicenseApplicationData.AddNewLocalDrivingLicenseApplication(ApplicationID,
                    LicenseClassID);
            
            return (LocalDrivingLicenseApplicationID != -1);
        }

        private bool _UpdateLocalDrivingLicenseApplication()
        {
            return clsLocalDrivingLicenseApplicationData.UpdateLocalDrivingLicenseApplication(LocalDrivingLicenseApplicationID, ApplicationID, LicenseClassID);
        }

        public static clsLocalDrivingLicenseApplication FindByLocalDrivingLicenseApplicationID(int LocalDrivingLicenseApplicationID)
        {
            int ApplicationID = -1, LicenseClassID = -1;

            bool IsFound = clsLocalDrivingLicenseApplicationData.GetLocalDrivingLicenseApplicationInfoByID(LocalDrivingLicenseApplicationID,
                ref ApplicationID, ref LicenseClassID);

            if (IsFound)
            {
                clsApplication Application = clsApplication.FindByApplicationID(ApplicationID);

                if (Application == null)
                    return null;

                return new clsLocalDrivingLicenseApplication(LocalDrivingLicenseApplicationID,
                    ApplicationID, Application.ApplicantPersonID, Application.ApplicationDate,
                    Application.ApplicationTypeID, (enApplicationStatus)Application.ApplicationStatus, Application.LastStatusDate,
                    Application.PaidFees, Application.CreatedByUserID, LicenseClassID);
            }
            else
                return null;
        }

        public static DataTable GetAllLocalDrivingLicenseApplications()
        {
            return clsLocalDrivingLicenseApplicationData.GetAllLocalDrivingLicenseApplications();
        }

        public new bool Delete()
        {
            bool IsLocalDrivingLicenseApplicaionDeleted = false;
            bool IsBaseApplicationDeleted = false;

            IsLocalDrivingLicenseApplicaionDeleted =
                clsLocalDrivingLicenseApplicationData.DeleteLocalDrivingLicenseApplication(LocalDrivingLicenseApplicationID);

            if (!IsLocalDrivingLicenseApplicaionDeleted)
                return false;

            IsBaseApplicationDeleted = base.Delete();

            return IsBaseApplicationDeleted;
        }

        public static bool IsLocalDrivingLicenseApplicationExists(int LocalDrivingLicenseApplicationID)
        {
            return clsLocalDrivingLicenseApplicationData.IsLocalDrivingLicenseApplicationExists(LocalDrivingLicenseApplicationID);
        }

        public new bool Save()
        {
            base.Mode = (clsApplication.enMode)_Mode;
            if (!base.Save())
                return false;

            switch (_Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewLocalDrivingLicenseApplication())
                        {
                            _Mode = enMode.Update;
                            return true;
                        }
                        else
                            return false;
                    }

                case enMode.Update:
                    {
                        return _UpdateLocalDrivingLicenseApplication();
                    }

                default:
                    return false;
            }
        }

        public int GetActiveLicenseID()
        {
            return clsLicense.GetActiveLicenseIDByPersonID(ApplicantPersonID, LicenseClassID);
        }

        public bool IsLicenseIssued()
        {
            return clsLicense.IsLicenseIssued(ApplicantPersonID, LicenseClassID);
        }

        public byte GetNumberOfPassedTests()
        {
            return clsTest.GetNumberOfPassedTests(LocalDrivingLicenseApplicationID);
        }

        public bool DoesPassTestType(clsTestType.enTestType TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.DoesPassTestType(LocalDrivingLicenseApplicationID, (byte)TestTypeID);
        }

        public bool DoesAttendTestType(clsTestType.enTestType TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.DoesAttendTestType(LocalDrivingLicenseApplicationID, (byte)TestTypeID);
        }

        public byte GetTestTrials(clsTestType.enTestType TestTypeID)
        {
            return clsTest.GetTestTrials(LocalDrivingLicenseApplicationID, TestTypeID);
        }

        public bool IsThereAnActiveScheduledTest(clsTestType.enTestType TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.IsThereAnActiveScheduledTest(LocalDrivingLicenseApplicationID, (byte)TestTypeID);
        }

        public static bool IsThereAnActiveScheduledTest(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return clsLocalDrivingLicenseApplicationData.IsThereAnActiveScheduledTest(LocalDrivingLicenseApplicationID, (byte)TestTypeID);
        }

        public clsTest GetLastTestForTestType(clsTestType.enTestType TestTypeID)
        {
            return clsTest.FindLastTestByPersonIDAndLicenseClassID(ApplicantPersonID, LicenseClassID, TestTypeID);
        }

        public int IssueLicenseForFirstTime(string Notes, int CreatedByUserID)
        {
            if (!IsPassedAllTests())
                return -1;

            if (GetActiveLicenseID() != -1)
                return -1;

            if (ApplicationStatus == enApplicationStatus.Completed || ApplicationStatus == enApplicationStatus.Cancelled)
                return -1;

            int DriverID = -1;

            clsDriver DriverInfo = clsDriver.FindByPersonID(ApplicantPersonID);

            if (DriverInfo == null)
            {
                DriverInfo = new clsDriver()
                {
                    PersonID = ApplicantPersonID,
                    CreatedByUserID = CreatedByUserID,
                    CreatedDate = DateTime.Now
                };

                if (DriverInfo.Save())
                    DriverID = DriverInfo.DriverID;
                else
                    return -1;
            }
            else
                DriverID = DriverInfo.DriverID;

            if (LicenseClassInfo == null)
                LicenseClassInfo = clsLicenseClass.Find(LicenseClassID);

            clsLicense License = new clsLicense()
            {
                ApplicationID = ApplicationID,
                DriverID = DriverID,
                LicenseClassID = LicenseClassID,
                IssueDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddYears(LicenseClassInfo.DefaultValidityLength),
                Notes = Notes,
                PaidFees = LicenseClassInfo.ClassFees,
                IsActive = true,
                IssueReason = clsLicense.enIssueReason.FirstTime,
                CreatedByUserID = CreatedByUserID
            };

            if (License.Save())
            {
                SetComplete();
                return License.LicenseID;
            }

            return -1;
        }

        public bool IsPassedAllTests()
        {
            return (clsTest.GetNumberOfPassedTests(LocalDrivingLicenseApplicationID) == 3);
        }
    }
}
