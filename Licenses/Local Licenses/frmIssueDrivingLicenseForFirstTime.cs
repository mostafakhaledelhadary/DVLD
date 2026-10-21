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
    public partial class frmIssueDrivingLicenseForFirstTime : Form
    {
        private int _LocalDrivingLicenseApplicationID = -1;

        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplicationInfo;

        public frmIssueDrivingLicenseForFirstTime(int LocalDrivingLicenseApplicationID)
        {
            InitializeComponent();
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
        }

        private void frmIssueDrivingLicenseForFirstTime_Load(object sender, EventArgs e)
        {
            txtNotes.Focus();

            _LocalDrivingLicenseApplicationInfo =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingLicenseApplicationID(_LocalDrivingLicenseApplicationID);

            if(_LocalDrivingLicenseApplicationInfo == null)
            {
                MessageBox.Show($"Local Driving License with ID = {_LocalDrivingLicenseApplicationID} is not found!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (!_LocalDrivingLicenseApplicationInfo.IsPassedAllTests())
            {
                MessageBox.Show("You should pass all tests first!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            int LicenseID = _LocalDrivingLicenseApplicationInfo.GetActiveLicenseID();

            if(LicenseID != -1)
            {
                MessageBox.Show($"Person already has a license before and it's ID = {LicenseID}!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (_LocalDrivingLicenseApplicationInfo.ApplicationStatus == clsApplication.enApplicationStatus.Completed)
            {
                MessageBox.Show("This application is already completed and license was issued!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            ctrlLocalDrivingLicenseApplicationInfo1.LoadData(_LocalDrivingLicenseApplicationID);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIssueLicense_Click(object sender, EventArgs e)
        {
            int LicenseID = _LocalDrivingLicenseApplicationInfo.IssueLicenseForFirstTime(txtNotes.Text.Trim(), clsGlobal.CurrentUser.UserID);

            if(LicenseID != -1)
            {
                MessageBox.Show("License issued successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnIssueLicense.Enabled = false;
            }
            else
                MessageBox.Show("Failed to issue a license", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
