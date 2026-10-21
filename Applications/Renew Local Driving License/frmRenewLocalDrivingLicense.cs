using DVLD_Business;
using System;
using System.Windows.Forms;

namespace DVLD
{
    public partial class frmRenewLocalDrivingLicense : Form
    {
        private int _NewLicenseID = -1;

        public frmRenewLocalDrivingLicense()
        {
            InitializeComponent();
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += _Validation;
        }

        private void _ResetDefaultValues()
        {
            _NewLicenseID = -1;
            ctrlDriverLicenseInfoWithFilter1.FilterFocus();
            lblRenewLicenseApplicationID.Text = "N/A";
            lblApplicationDate.Text = DateTime.Now.ToShortDateString();
            lblIssueDate.Text = DateTime.Now.ToShortDateString();

            clsApplicationType AppType = clsApplicationType.Find((int)clsApplication.enApplicationType.RenewDrivingLicense);
            lblApplicationFees.Text = (AppType != null) ? AppType.Fees.ToString() : "0";

            lblLicenseFees.Text = "0";
            lblRenewedLicenseID.Text = "N/A";
            lblOldLicenseID.Text = "N/A";
            lblExpirationDate.Text = DateTime.Now.AddYears(10).ToShortDateString();
            lblUsername.Text = clsGlobal.CurrentUser.Username;
            lblTotalFees.Text = "0";
            txtNotes.Text = "";
            btnRenewLocalDrivingLicense.Enabled = false;
            llShowLicensesHistory.Enabled = false;
            llShowNewLicenseInfo.Enabled = false;
            txtNotes.Enabled = false;
        }

        private void _FillLicenseData(clsLicense SelectedLicenseInfo)
        {
            lblLicenseFees.Text = SelectedLicenseInfo.LicenseClassInfo.ClassFees.ToString();
            lblOldLicenseID.Text = SelectedLicenseInfo.LicenseID.ToString();
            lblExpirationDate.Text = DateTime.Now.AddYears(SelectedLicenseInfo.LicenseClassInfo.DefaultValidityLength).ToShortDateString();
            lblTotalFees.Text = (Convert.ToSingle(lblApplicationFees.Text) + Convert.ToSingle(lblLicenseFees.Text)).ToString();
            llShowLicensesHistory.Enabled = true;
        }

        private void _RenewLicense()
        {
            clsLicense NewLicense = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.RenewLicense(txtNotes.Text.Trim(),
                clsGlobal.CurrentUser.UserID);

            if(NewLicense == null)
            {
                MessageBox.Show("Failed to create new license", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show($"License renewed successfully with ID = {NewLicense.LicenseID}",
                    "License renewed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _NewLicenseID = NewLicense.LicenseID;
                lblRenewLicenseApplicationID.Text = NewLicense.ApplicationID.ToString();
                lblRenewedLicenseID.Text = NewLicense.LicenseID.ToString();
                btnRenewLocalDrivingLicense.Enabled = false;
                ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
                llShowNewLicenseInfo.Enabled = true;
                lblExpirationDate.Text = NewLicense.ExpirationDate.ToShortDateString();
                txtNotes.Enabled = false;
            }
        }

        private void _Validation()
        {
            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo == null)
            {
                _ResetDefaultValues();
                return;
            }

            clsLicense SelectedLicenseInfo = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo;

            _FillLicenseData(SelectedLicenseInfo);

            if (!SelectedLicenseInfo.IsLicenseExpired())
            {
                MessageBox.Show($"Selected license haven't expired yet!!, it will expire in {SelectedLicenseInfo.ExpirationDate.ToShortDateString()}",
                    "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNotes.Enabled = false;
                btnRenewLocalDrivingLicense.Enabled = false;
                return;
            }

            if (!SelectedLicenseInfo.IsActive)
            {
                MessageBox.Show($"Selected license is not active!!, we can't renew non active license",
                    "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNotes.Enabled = false;
                btnRenewLocalDrivingLicense.Enabled = false;
                return;
            }

            if (SelectedLicenseInfo.IsDetained)
            {
                MessageBox.Show("Selected license is detained, you can't renew detained license!!",
                    "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNotes.Enabled = false;
                btnRenewLocalDrivingLicense.Enabled = false;
                return;
            }

            txtNotes.Enabled = true;
            btnRenewLocalDrivingLicense.Enabled = true;
        }

        private void frmRenewLocalDrivingLicense_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();
        }

        private void llShowLicensesHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo == null)
                return;

            frmPersonLicenseHistory frm = new frmPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.ApplicationInfo.ApplicantPersonID);
            frm.ShowDialog();
        }

        private void llShowNewLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_NewLicenseID == -1)
                return;

            frmLicenseInfo frm = new frmLicenseInfo(_NewLicenseID);
            frm.ShowDialog();
        }

        private void btnRenewLocalDrivingLicense_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to renew this license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            _RenewLicense();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}