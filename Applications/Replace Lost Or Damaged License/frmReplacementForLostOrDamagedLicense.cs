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
    public partial class frmReplacementForLostOrDamagedLicense : Form
    {
        private int _NewLicenseID = -1;

        public frmReplacementForLostOrDamagedLicense()
        {
            InitializeComponent();
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += _Validation;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _ResetDefaultValues()
        {
            _NewLicenseID = -1;
            lblTitle.Text = "Replacement For Damaged License";
            this.Text = "Replacement For Damaged License";

            clsApplicationType AppType = clsApplicationType.Find((int)clsApplication.enApplicationType.ReplaceDamagedDrivingLicense);
            lblApplicationFees.Text = (AppType != null) ? AppType.Fees.ToString() : "0";

            lblLicenseReplacementApplicationID.Text = "N/A";
            lblApplicationDate.Text = DateTime.Now.ToShortDateString();
            lblReplacedLicenseID.Text = "N/A";
            lblOldLicenseID.Text = "N/A";
            lblUsername.Text = clsGlobal.CurrentUser.Username;
            btnIssueReplacement.Enabled = false;
            llShowLicensesHistory.Enabled = false;
            llShowNewLicenseInfo.Enabled = false;
        }

        private void _FillLicenseData(clsLicense SelectedLicenseInfo)
        {
            lblOldLicenseID.Text = SelectedLicenseInfo.LicenseID.ToString();
            llShowLicensesHistory.Enabled = true;
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

            if (!SelectedLicenseInfo.IsActive)
            {
                MessageBox.Show("Selected license is not active, choose an active license.", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnIssueReplacement.Enabled = false;
                return;
            }

            if(SelectedLicenseInfo.IsLicenseExpired())
            {
                MessageBox.Show("Person's license is expired", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnIssueReplacement.Enabled = false;
                return;
            }

            if (SelectedLicenseInfo.IsDetained)
            {
                MessageBox.Show("Selected license is detained, you can't replace detained license!!",
                    "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnIssueReplacement.Enabled = false;
                return;
            }

            btnIssueReplacement.Enabled = true;
        }

        private clsLicense.enIssueReason _GetIssueReason()
        {
            if (rbDamagedLicense.Checked) return clsLicense.enIssueReason.ReplacementForDamaged;
            else return clsLicense.enIssueReason.ReplacementForLost;
        }

        private void _ReplaceDamagedOrLostLicense()
        {
            clsLicense NewLicense = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.Replace(_GetIssueReason(), clsGlobal.CurrentUser.UserID);

            if (NewLicense == null)
            {
                MessageBox.Show("Failed to replace license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show($"License replaced successfully with ID = {NewLicense.LicenseID}",
                    "License Replaced", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _NewLicenseID = NewLicense.LicenseID;
                lblLicenseReplacementApplicationID.Text = NewLicense.ApplicationID.ToString();
                lblReplacedLicenseID.Text = NewLicense.LicenseID.ToString();
                gbReplacementFor.Enabled = false;
                ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
                btnIssueReplacement.Enabled = false;
                llShowLicensesHistory.Enabled = true;
                llShowNewLicenseInfo.Enabled = true;
            }
        }

        private void btnIssueReplacement_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to issue a replacement for license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            _ReplaceDamagedOrLostLicense();
        }

        private void llShowNewLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_NewLicenseID == -1)
                return;

            frmLicenseInfo frm = new frmLicenseInfo(_NewLicenseID);
            frm.ShowDialog();
        }

        private void frmReplacedForLostOrDamagedLicense_Load(object sender, EventArgs e)
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

        private void rbReplacementFor_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked) return;

            if (rbDamagedLicense.Checked)
            {
                lblTitle.Text = "Replacement For Damaged License";
                this.Text = "Replacement For Damaged License";
                lblApplicationFees.Text = clsApplicationType.Find((int)clsApplication.enApplicationType.ReplaceDamagedDrivingLicense).Fees.ToString();
            }
            else
            {
                lblTitle.Text = "Replacement For Lost License";
                this.Text = "Replacement For Lost License";
                lblApplicationFees.Text = clsApplicationType.Find((int)clsApplication.enApplicationType.ReplaceLostDrivingLicense).Fees.ToString();
            }
        }
    }
}