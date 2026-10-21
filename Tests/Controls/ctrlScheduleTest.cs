using DVLD.Properties;
using DVLD_Business;
using System;
using System.Windows.Forms;

namespace DVLD
{
    public partial class ctrlScheduleTest : UserControl
    {
        private int _LocalDrivingLicenseApplicationID = -1;
        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplicationInfo;
        private int _TestAppointmentID = -1;
        private clsTestAppointment _TestAppointmentInfo;
        private clsTestType.enTestType _TestTypeID = clsTestType.enTestType.VisionTest;
        public clsTestType.enTestType TestTypeID
        {
            get { return _TestTypeID; }

            set 
            {
                _TestTypeID = value;

                switch (_TestTypeID)
                {
                    case clsTestType.enTestType.VisionTest:
                        {
                            gbTestType.Text = "Vision Test";
                            pbTestType.Image = Resources.Vision_512;
                            break;
                        }

                    case clsTestType.enTestType.WrittenTest:
                        {
                            gbTestType.Text = "Written Test";
                            pbTestType.Image = Resources.Written_Test_512;
                            break;
                        }

                    case clsTestType.enTestType.StreetTest:
                        {
                            gbTestType.Text = "Street Test";
                            pbTestType.Image = Resources.driving_test_512;
                            break;
                        }
                }
            }
        }

        private enum enCreationMode {FirstTimeSchedule = 1, RetakeTestSchedule = 2}
        private enCreationMode _CreationMode = enCreationMode.FirstTimeSchedule;

        private enum enMode : byte {AddNew = 1, Update = 2}
        private enMode _Mode = enMode.AddNew;

        public ctrlScheduleTest()
        {
            InitializeComponent();
        }

        private bool _LoadTestAppointmentInfo()
        {
            _TestAppointmentInfo = clsTestAppointment.FindByTestAppointmentID(_TestAppointmentID);

            if(_TestAppointmentInfo == null)
            {
                MessageBox.Show("Test Appointment is not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // 3. ضبط الـ MinDate بناءً على موعد الحجز
            if (DateTime.Compare(_TestAppointmentInfo.AppointmentDate, DateTime.Now) < 0)
                dtpDate.MinDate = _TestAppointmentInfo.AppointmentDate;
            else
                dtpDate.MinDate = DateTime.Now;

            dtpDate.Value = _TestAppointmentInfo.AppointmentDate;

            if (_TestAppointmentInfo.RetakeTestApplicationID == -1)
            {
                lblTitle.Text = "Schedule Test";
                lblRetakeTestAppID.Text = "N/A";
                lblRetakeTestFees.Text = "0";
                gbRetakeTestInfo.Enabled = false;
            }
            else
            {
                lblTitle.Text = "Schedule Retake Test";
                lblRetakeTestAppID.Text = _TestAppointmentInfo.RetakeTestApplicationID.ToString();
                lblRetakeTestFees.Text = _TestAppointmentInfo.RetakeTestApplicationInfo.PaidFees.ToString();
                gbRetakeTestInfo.Enabled = true;
            }

            return true;
        }

        private bool _HandleAppointmentLockedConstraint()
        {
            if (_TestAppointmentInfo.IsLocked == true)
            {
                btnSave.Enabled = false;
                dtpDate.Enabled = false;
                lblUserMessage.Visible = true;
                lblUserMessage.Text = "Person already sat for this test, appointment locked.";
                return false;
            }
            else
                lblUserMessage.Visible = false;

            return true;
        }

        private bool _HandleActiveTestAppointmentConstraint()
        {
            if(_Mode == enMode.AddNew && clsLocalDrivingLicenseApplication.IsThereAnActiveScheduledTest(_LocalDrivingLicenseApplicationID, TestTypeID))
            {
                lblUserMessage.Text = "Person already has an active appointment for this test.";
                lblUserMessage.Visible = true;
                btnSave.Enabled = false;
                dtpDate.Enabled = false;
                return false;
            }

            return true;
        }

        private bool _HandleRetakeApplication()
        {
            if (_Mode == enMode.AddNew && _CreationMode == enCreationMode.RetakeTestSchedule && _TestAppointmentInfo.RetakeTestApplicationID == -1)
            {
                clsApplication NewApplication = new clsApplication
                {
                    ApplicationTypeID = (int)clsApplication.enApplicationType.RetakeTest,
                    ApplicantPersonID = _LocalDrivingLicenseApplicationInfo.ApplicantPersonID,
                    ApplicationStatus = clsApplication.enApplicationStatus.New,
                    LastStatusDate = DateTime.Now,
                    CreatedByUserID = clsGlobal.CurrentUser.UserID,
                    PaidFees = clsApplicationType.Find((int)clsApplication.enApplicationType.RetakeTest).Fees,
                    ApplicationDate = DateTime.Now
                };

                if (!NewApplication.Save())
                {
                    _TestAppointmentInfo.RetakeTestApplicationID = -1;
                    lblRetakeTestAppID.Text = "N/A";
                    MessageBox.Show("Failed to create application.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                _TestAppointmentInfo.RetakeTestApplicationID = NewApplication.ApplicationID;
                lblRetakeTestAppID.Text = NewApplication.ApplicationID.ToString();
            }

            return true;
        }

        private bool _HandleAlreadyPassedTestConstraint()
        {
            if (_LocalDrivingLicenseApplicationInfo.DoesPassTestType(_TestTypeID))
            {
                lblUserMessage.Text = "Person already passed this test, test is locked.";
                lblUserMessage.Visible = true;
                btnSave.Enabled = false;
                dtpDate.Enabled = false;
                return false;
            }

            return true;
        }

        private bool _HandlePrviousTestConstraint()
        {
            //we need to make sure that this person passed the prvious required test before apply to the new test.
            //person cannno apply for written test unless s/he passes the vision test.
            //person cannot apply for street test unless s/he passes the written test.

            switch (TestTypeID)
            {
                case clsTestType.enTestType.VisionTest:
                    //in this case no required prvious test to pass.
                    lblUserMessage.Visible = false;

                    return true;

                case clsTestType.enTestType.WrittenTest:
                    //Written Test, you cannot sechdule it before person passes the vision test.
                    //we check if pass vision test 1.
                    if (!_LocalDrivingLicenseApplicationInfo.DoesPassTestType(clsTestType.enTestType.VisionTest))
                    {
                        lblUserMessage.Text = "Cannot Schedule, Vision Test should be passed first";
                        lblUserMessage.Visible = true;
                        btnSave.Enabled = false;
                        dtpDate.Enabled = false;
                        return false;
                    }
                    else
                    {
                        lblUserMessage.Visible = false;
                        btnSave.Enabled = true;
                        dtpDate.Enabled = true;
                    }

                    return true;

                case clsTestType.enTestType.StreetTest:

                    //Street Test, you cannot sechdule it before person passes the written test.
                    //we check if pass Written 2.
                    if (!_LocalDrivingLicenseApplicationInfo.DoesPassTestType(clsTestType.enTestType.WrittenTest))
                    {
                        lblUserMessage.Text = "Cannot Schedule, Written Test should be passed first";
                        lblUserMessage.Visible = true;
                        btnSave.Enabled = false;
                        dtpDate.Enabled = false;
                        return false;
                    }
                    else
                    {
                        lblUserMessage.Visible = false;
                        btnSave.Enabled = true;
                        dtpDate.Enabled = true;
                    }

                    return true;
            }

            return true;
        }

        public void LoadData(int LocalDrivingLicenseApplicationID, int TestAppointmentID = -1)
        {
            _Mode = (TestAppointmentID == -1) ? enMode.AddNew : enMode.Update;

            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestAppointmentID = TestAppointmentID;

            _LocalDrivingLicenseApplicationInfo =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingLicenseApplicationID(LocalDrivingLicenseApplicationID);

            if(_LocalDrivingLicenseApplicationInfo == null)
            {
                MessageBox.Show("Local Driving License Application is not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_LocalDrivingLicenseApplicationInfo.DoesAttendTestType(_TestTypeID) &&
               !_LocalDrivingLicenseApplicationInfo.DoesPassTestType(_TestTypeID))
                _CreationMode = enCreationMode.RetakeTestSchedule;
            else
                _CreationMode = enCreationMode.FirstTimeSchedule;

            if(_CreationMode == enCreationMode.RetakeTestSchedule)
            {
                lblTitle.Text = "Schedule Retake Test";
                gbRetakeTestInfo.Enabled = true;
                lblRetakeTestFees.Text = clsApplicationType.Find((int)clsApplication.enApplicationType.RetakeTest).Fees.ToString();
            }
            else
            {
                lblTitle.Text = "Schedule Test";
                gbRetakeTestInfo.Enabled = false;
                lblUserMessage.Visible = false;
                lblRetakeTestFees.Text = "0";
                lblRetakeTestAppID.Text = "N/A";
            }

            lblLocalDrivingLicenseApplicationID.Text = LocalDrivingLicenseApplicationID.ToString();
            lblLicenseClass.Text = _LocalDrivingLicenseApplicationInfo.LicenseClassInfo.ClassName;
            lblApplicantName.Text = _LocalDrivingLicenseApplicationInfo.ApplicantFullName;
            lblTrial.Text = _LocalDrivingLicenseApplicationInfo.GetTestTrials(_TestTypeID).ToString();
            lblTestFees.Text = clsTestType.Find(_TestTypeID).Fees.ToString();

            if (_Mode == enMode.AddNew)
            {
                _TestAppointmentInfo = new clsTestAppointment();
                dtpDate.Value = DateTime.Now;
                dtpDate.MinDate = DateTime.Now;
            }
            else
            {
                if (!_LoadTestAppointmentInfo())
                    return;
            }

            lblTotalFees.Text = (Convert.ToSingle(lblTestFees.Text) + Convert.ToSingle(lblRetakeTestFees.Text)).ToString();

            if (!_HandleAppointmentLockedConstraint())
                return;

            if (!_HandleActiveTestAppointmentConstraint())
                return;

            if (!_HandlePrviousTestConstraint())
                return;

            if (!_HandleAlreadyPassedTestConstraint())
                return;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save data?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            if (!_HandleRetakeApplication())
                return;

            _TestAppointmentInfo.LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationID;
            _TestAppointmentInfo.TestTypeID = _TestTypeID;
            _TestAppointmentInfo.AppointmentDate = dtpDate.Value;
            _TestAppointmentInfo.PaidFees = Convert.ToSingle(lblTestFees.Text);
            _TestAppointmentInfo.CreatedByUserID = clsGlobal.CurrentUser.UserID;
            _TestAppointmentInfo.IsLocked = false;

            if (_CreationMode == enCreationMode.FirstTimeSchedule)
            {
                _TestAppointmentInfo.RetakeTestApplicationID = -1;
            }

            if (_TestAppointmentInfo.Save())
            {
                _Mode = enMode.Update;
                MessageBox.Show("Data saved successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Failed to save data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}
