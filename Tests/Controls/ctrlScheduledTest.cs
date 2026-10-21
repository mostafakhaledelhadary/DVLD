using DVLD.Properties;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD
{
    public partial class ctrlScheduledTest : UserControl
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
                        pbTestType.Image = Resources.Vision_512;
                        gbTestType.Text = "Vision Test";
                        break;

                    case clsTestType.enTestType.WrittenTest:
                        pbTestType.Image = Resources.Written_Test_512;
                        gbTestType.Text = "Written Test";
                        break;

                    case clsTestType.enTestType.StreetTest:
                        pbTestType.Image = Resources.driving_test_512;
                        gbTestType.Text = "Street Test";
                        break;
                }
            }
        }

        private int _TestID = -1;

        public int TestID 
        {
            get { return _TestID; }
            set
            {
                _TestID = value;

                if (TestID != -1)
                    lblTestID.Text = TestID.ToString();
                else
                    lblTestID.Text = "Not taken yet";
            }
        }

        public ctrlScheduledTest()
        {
            InitializeComponent();
        }

        public void LoadData(int TestAppointmentID)
        {
            _TestAppointmentID = TestAppointmentID;

            _TestAppointmentInfo = clsTestAppointment.FindByTestAppointmentID(TestAppointmentID);

            if(_TestAppointmentInfo == null)
            {
                MessageBox.Show("Test Appointment is not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // بيخلي الكنترول يحدد نوع الاختبار تلقائياً من الموعد المحفوظ
            TestTypeID = (clsTestType.enTestType)_TestAppointmentInfo.TestTypeID;

            _LocalDrivingLicenseApplicationID = _TestAppointmentInfo.LocalDrivingLicenseApplicationID;

            _LocalDrivingLicenseApplicationInfo =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingLicenseApplicationID(_LocalDrivingLicenseApplicationID);

            if(_LocalDrivingLicenseApplicationInfo == null)
            {
                MessageBox.Show("Local Driving License Application is not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblLocalDrivingLicenseApplicationID.Text = _LocalDrivingLicenseApplicationID.ToString();
            lblLicenseClass.Text = _LocalDrivingLicenseApplicationInfo.LicenseClassInfo.ClassName;
            lblName.Text = _LocalDrivingLicenseApplicationInfo.ApplicantFullName;
            lblTrial.Text = _LocalDrivingLicenseApplicationInfo.GetTestTrials(TestTypeID).ToString();
            lblDate.Text = _TestAppointmentInfo.AppointmentDate.ToShortDateString();
            lblTestFees.Text =_TestAppointmentInfo.PaidFees.ToString();
            TestID = _TestAppointmentInfo.TestID;
        }
    }
}
