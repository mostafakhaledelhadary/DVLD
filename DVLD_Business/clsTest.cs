using System;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsTest
    {
        private enum enMode : byte {AddNew = 0, Update = 1}
        private enMode _Mode = enMode.AddNew;
        public int TestID { get; set; }
        public int TestAppointmentID { get; set; }
        public clsTestAppointment TestAppointmentInfo;
        public bool TestResult { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }
        public clsUser UserInfo;

        public clsTest()
        {
            TestID = -1;
            TestAppointmentID = -1;
            TestResult = false;
            Notes = "";
            CreatedByUserID = -1;

            _Mode = enMode.AddNew;
        }

        private clsTest(int TestID, int TestAppointmentID, bool TestResult, string Notes, int CreatedByUserID)
        {
            this.TestID = TestID;
            this.TestAppointmentID = TestAppointmentID;
            TestAppointmentInfo = clsTestAppointment.FindByTestAppointmentID(TestAppointmentID);
            this.TestResult = TestResult;
            this.Notes = Notes;
            this.CreatedByUserID = CreatedByUserID;
            UserInfo = clsUser.FindByUserID(CreatedByUserID);

            _Mode = enMode.Update;
        }

        private bool _AddNewTest()
        {
            TestID = clsTestData.AddNewTest(TestAppointmentID, TestResult, Notes, CreatedByUserID);

            return (TestID != -1);
        }

        private bool _UpdateTest()
        {
            return clsTestData.UpdateTest(TestID, TestAppointmentID, TestResult, Notes, CreatedByUserID);
        }

        public static clsTest Find(int TestID)
        {
            int TestAppointmentID = -1, CreatedByUserID = -1;
            bool TestResult = false;
            string Notes = "";

            if (clsTestData.GetTestInfoByTestID(TestID, ref TestAppointmentID, ref TestResult, ref Notes, ref CreatedByUserID))
            {
                return new clsTest(TestID, TestAppointmentID, TestResult, Notes, CreatedByUserID);
            }
            else
                return null;
        }

        public static byte GetNumberOfPassedTests(int LocalDrivingLicenseApplicationID)
        {
            return clsTestData.GetNumberOfPassedTests(LocalDrivingLicenseApplicationID);
        }

        public static bool DoesPersonPassInTest(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return clsTestData.DoesPersonPassInTest(LocalDrivingLicenseApplicationID, (byte)TestTypeID);
        }

        public static byte GetTestTrials(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            return clsTestData.GetTestTrials(LocalDrivingLicenseApplicationID, (byte)TestTypeID);
        }

        public static clsTest FindLastTestByPersonIDAndLicenseClassID(int ApplicantPersonID, int LicenseClassID, clsTestType.enTestType TestTypeID)
        {
            int TestID = -1, TestAppointmentID = -1, CreatedByUserID = -1;
            bool TestResult = false;
            string Notes = "";

            if (clsTestData.GetLastTestByPersonIDAndLicenseClassID(ApplicantPersonID, LicenseClassID, (byte)TestTypeID,
                ref TestID, ref TestAppointmentID, ref TestResult, ref Notes, ref CreatedByUserID))
            {
                return new clsTest(TestID, TestAppointmentID, TestResult, Notes, CreatedByUserID);
            }
            else
                return null;
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewTest())
                        {
                            //تقفل الموعد تلقائيًا أول ما الاختبار يتحفظ بنجاح
                            clsTestAppointment.LockAppointment(TestAppointmentID);
                            _Mode = enMode.Update;
                            return true;
                        }
                        else
                            return false;
                    }

                case enMode.Update:
                    return _UpdateTest();

                default:
                    return false;
            }
        }
    }
}
