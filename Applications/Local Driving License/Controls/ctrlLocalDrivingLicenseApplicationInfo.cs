using DVLD_Business;
using System;
using System.Windows.Forms;

namespace DVLD
{
    public partial class ctrlLocalDrivingLicenseApplicationInfo : UserControl
    {
        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplicationInfo;
        private int _LocalDrivingLicenseApplicationID = -1;

        public int LocalDrivingLicenseApplicationID
        {
            get { return _LocalDrivingLicenseApplicationID; }
        }


        private bool _ShowLicenseInfoEnabled = true;
        public bool ShowLicenseInfoEnabled
        {
            get { return _ShowLicenseInfoEnabled; }
            set
            {
                _ShowLicenseInfoEnabled = value;
                llShowLicenseInfo.Enabled = _ShowLicenseInfoEnabled;
            }
        }

        private int _LicenseID = -1;
        public int LicenseID
        {
            get { return _LicenseID; }
            set { _LicenseID = value; }
        }

        public ctrlLocalDrivingLicenseApplicationInfo()
        {
            InitializeComponent();
        }
    
        private void _FillLocalDrivingLicenseApplicationInfo()
        {
            _LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationInfo.LocalDrivingLicenseApplicationID;

            lblLocalDrivingLicenseApplicationID.Text = _LocalDrivingLicenseApplicationInfo.LocalDrivingLicenseApplicationID.ToString();

            int LicenseID = _LocalDrivingLicenseApplicationInfo.GetActiveLicenseID();
            llShowLicenseInfo.Enabled = (LicenseID != -1);

            lblAppliedForLicense.Text = _LocalDrivingLicenseApplicationInfo.LicenseClassInfo.ClassName;

            int NumberOfPassedTests = _LocalDrivingLicenseApplicationInfo.GetNumberOfPassedTests();

            lblPassedTests.Text = $"{NumberOfPassedTests}/3";

            ctrlBasicApplicationInfo1.LoadData(_LocalDrivingLicenseApplicationInfo.ApplicationID);
        }

        private void _ResetLocalDrivingLicenseApplicationInfo()
        {
            _LocalDrivingLicenseApplicationID = -1;
            lblLocalDrivingLicenseApplicationID.Text = "N/A";
            llShowLicenseInfo.Enabled = false;
            lblAppliedForLicense.Text = "????";
            lblPassedTests.Text = "0/3";
            ctrlBasicApplicationInfo1.ResetApplicationInfo();
        }

        public void LoadData(int LocalDrivingLicenseApplicationID)
        {
            _LocalDrivingLicenseApplicationInfo =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingLicenseApplicationID(LocalDrivingLicenseApplicationID);

            if(_LocalDrivingLicenseApplicationInfo == null)
            {
                _ResetLocalDrivingLicenseApplicationInfo();
                MessageBox.Show("Local Driving Application is not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillLocalDrivingLicenseApplicationInfo();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmLicenseInfo frm = new frmLicenseInfo(_LicenseID);
            frm.ShowDialog();
        }
    }
}
