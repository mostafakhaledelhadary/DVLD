using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD
{
    public partial class frmReleaseDetainedLicense : Form
    {
        private int _LicenseID = -1;

        public frmReleaseDetainedLicense()
        {
            InitializeComponent();
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += _Validation;
        }

        public frmReleaseDetainedLicense(int LicenseID)
        {
            InitializeComponent();
            _LicenseID = LicenseID;
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += _Validation;
        }

        private void _ResetDefaultValues()
        {
            lblDetainID.Text = "N/A";
            lblDetainDate.Text = "00/00/0000";

            clsApplicationType AppType = clsApplicationType.Find((int)clsApplication.enApplicationType.ReleaseDetainedDrivingLicense);
            lblApplicationFees.Text = (AppType != null) ? AppType.Fees.ToString() : "0";

            lblTotalFees.Text = lblApplicationFees.Text;
            lblLicenseID.Text = "N/A";
            lblCreatedByUsername.Text = "????";
            lblFineFees.Text = "0";
            lblApplicationID.Text = "N/A";
            btnReleaseLicense.Enabled = false;
            llShowLicenseInfo.Enabled = false;
            llShowLicensesHistory.Enabled = false;
        }

        private void _FillDetainedLicenseData()
        {
            llShowLicensesHistory.Enabled = true;
            llShowLicenseInfo.Enabled = true;
            lblDetainID.Text = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedLicenseInfo.DetainID.ToString();
            lblDetainDate.Text = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedLicenseInfo.DetainDate.ToShortDateString();
            lblLicenseID.Text = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.LicenseID.ToString();
            lblFineFees.Text = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedLicenseInfo.FineFees.ToString();
            lblCreatedByUsername.Text = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DetainedLicenseInfo.CreatedByUserInfo.Username;
            lblTotalFees.Text = (Convert.ToSingle(lblApplicationFees.Text) + Convert.ToSingle(lblFineFees.Text)).ToString();
        }

        private void _ReleaseDetainedLicense()
        {
            int ReleaseApplicationID = -1;
            bool isReleased = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.ReleaseDetainedLicense(clsGlobal.CurrentUser.UserID, ref ReleaseApplicationID);

            if (!isReleased)
            {
                MessageBox.Show("Failed to release detained license", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Detained license released successfully",
                        "License released", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lblApplicationID.Text = ReleaseApplicationID.ToString();
                btnReleaseLicense.Enabled = false;
                ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
                llShowLicenseInfo.Enabled = true;
            }
        }

        private void _LoadDetainedLicenseInfo()
        {
            ctrlDriverLicenseInfoWithFilter1.LoadLicenseInfo(_LicenseID);
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            _Validation();
        }

        private void _Validation()
        {
            if(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo == null)
            {
                _ResetDefaultValues();
                return;
            }

            clsLicense SelectedLicenseInfo = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo;

            if (!SelectedLicenseInfo.IsDetained)
            {
                MessageBox.Show("Selected license is not detained, please choose another one", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ResetDefaultValues();
                btnReleaseLicense.Enabled = false;
                return;
            }

            _FillDetainedLicenseData();
            btnReleaseLicense.Enabled = true;
        }

        private void frmReleaseDetainedLicense_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if(_LicenseID != -1)
                _LoadDetainedLicenseInfo();
        }

        private void btnReleaseLicense_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to release this license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            _ReleaseDetainedLicense();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo == null)
                return;

            frmLicenseInfo frm = new frmLicenseInfo(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.LicenseID);
            frm.ShowDialog();
        }

        private void llShowLicensesHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo == null)
                return;

            frmPersonLicenseHistory frm = new frmPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.ApplicationInfo.ApplicantPersonID);
            frm.ShowDialog();
        }
    }
}