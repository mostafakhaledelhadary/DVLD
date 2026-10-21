using DVLD.Properties;
using DVLD_Business;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DVLD
{
    public partial class frmTestAppointments : Form
    {
        private int _LocalDrivingLicenseApplicationID;

        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplicationInfo;

        private clsTestType.enTestType _TestTypeID;

        public frmTestAppointments(int LocalDrivingLicenseApplicationID, clsTestType.enTestType TestTypeID)
        {
            InitializeComponent();
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestTypeID = TestTypeID;
        }

        private int _SelectedTestAppointmentID()
        {
            if (dgvAppointments.CurrentRow == null || dgvAppointments.CurrentRow.Index < 0)
                return -1;

            return Convert.ToInt32(dgvAppointments.CurrentRow.Cells["Appointment ID"].Value);
        }

        private void _LoadTestTypeImageAndTitle()
        {
            switch (_TestTypeID)
            {
                case clsTestType.enTestType.VisionTest:
                    this.Text = "Vision Test Appointments";
                    lblTestAppointments.Text = "Vision Test Appointments";
                    pbTestAppointments.Image = Resources.Vision_512;
                    break;

                case clsTestType.enTestType.WrittenTest:
                    this.Text = "Written Test Appointments";
                    lblTestAppointments.Text = "Written Test Appointments";
                    pbTestAppointments.Image = Resources.Written_Test_512;
                    break;

                case clsTestType.enTestType.StreetTest:
                    this.Text = "Street Test Appointments";
                    lblTestAppointments.Text = "Street Test Appointments";
                    pbTestAppointments.Image = Resources.driving_test_512;
                    break;
            }
        }

        private void _LoadData()
        {
            _LoadTestTypeImageAndTitle();

            _LocalDrivingLicenseApplicationInfo =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingLicenseApplicationID(_LocalDrivingLicenseApplicationID);

            if (_LocalDrivingLicenseApplicationInfo == null)
            {
                MessageBox.Show("Error: No Local Driving License Application found with ID = " + _LocalDrivingLicenseApplicationID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                ctrlLocalDrivingLicenseApplicationInfo1.LoadData(_LocalDrivingLicenseApplicationID);
                dgvAppointments.DataSource =
                    clsTestAppointment.GetAllTestAppointmentsForTestType(_LocalDrivingLicenseApplicationID, _TestTypeID);
                lblRecords.Text = dgvAppointments.Rows.Count.ToString();
            }

            catch(Exception ex)
            {
                MessageBox.Show("Failed while loading data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"Database Error: {ex.Message}");
            }

        }

        private void frmTestAppointments_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private void btnAddNewAppointment_Click(object sender, EventArgs e)
        {
            if (_LocalDrivingLicenseApplicationInfo.IsThereAnActiveScheduledTest(_TestTypeID))
            {
                MessageBox.Show("Person already has an active appointment for this test, you can't add another appointment.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_LocalDrivingLicenseApplicationInfo.DoesPassTestType(_TestTypeID))
            {
                MessageBox.Show("This person already passed this test before, you can only retake failed tests.",
                    "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            frmScheduleTest frm = new frmScheduleTest(_LocalDrivingLicenseApplicationID, _TestTypeID);
            frm.ShowDialog();
            _LoadData();
        }

        private void takeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_SelectedTestAppointmentID() == -1)
                return;

            frmTakeTest frm = new frmTakeTest(_TestTypeID, _SelectedTestAppointmentID());
            frm.ShowDialog();
            _LoadData();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_SelectedTestAppointmentID() == -1)
                return;

            frmScheduleTest frm = new frmScheduleTest(_LocalDrivingLicenseApplicationID, _TestTypeID, _SelectedTestAppointmentID());
            frm.ShowDialog();
            _LoadData();
        }
    }
}
