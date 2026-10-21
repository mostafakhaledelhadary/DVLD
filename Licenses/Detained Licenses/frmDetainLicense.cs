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
    public partial class frmDetainLicense : Form
    {
        public frmDetainLicense()
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
            lblDetainID.Text = "N/A";
            lblDetainDate.Text = DateTime.Now.ToShortDateString();
            txtFineFees.Text = "";
            txtFineFees.Enabled = false;
            lblLicenseID.Text = "N/A";
            lblUsername.Text = clsGlobal.CurrentUser.Username;
            llShowLicenseInfo.Enabled = false;
            llShowLicensesHistory.Enabled = false;
            btnDetainLicense.Enabled = false;
        }

        private void _FillLicenseData(clsLicense SelectedLicenseInfo)
        {
            llShowLicensesHistory.Enabled = true;
            lblLicenseID.Text = SelectedLicenseInfo.LicenseID.ToString();
        }

        private void _Validation()
        {
            if(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo == null)
            {
                _ResetDefaultValues();
                return;
            }

            clsLicense SelectedLicenseInfo = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo;

            _FillLicenseData(SelectedLicenseInfo);

            if (!SelectedLicenseInfo.IsActive)
            {
                MessageBox.Show("Selected license is not active, we can't detain non active license!!", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnDetainLicense.Enabled = false;
                txtFineFees.Enabled = false;
                return;
            }

            if (SelectedLicenseInfo.IsDetained)
            {
                MessageBox.Show("Selected license is already detained, please choose another one", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnDetainLicense.Enabled = false;
                txtFineFees.Enabled = false;
                txtFineFees.Text = SelectedLicenseInfo.DetainedLicenseInfo.FineFees.ToString();
                lblDetainID.Text = SelectedLicenseInfo.DetainedLicenseInfo.DetainID.ToString();
                return;
            }

            txtFineFees.Enabled = true;
            txtFineFees.Focus();
            btnDetainLicense.Enabled = true;
        }

        private void _DetainLicense(float fineFees)
        {
            int DetainID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.Detain(fineFees, clsGlobal.CurrentUser.UserID);

            if(DetainID == -1)
            {
                MessageBox.Show("Failed to detain license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show($"License detained successfully with ID = {DetainID}", "License detained", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
                btnDetainLicense.Enabled = false;
                llShowLicenseInfo.Enabled = true;
                lblDetainID.Text = DetainID.ToString();
                txtFineFees.Enabled = false;
            }
        }

        private void frmDetainLicense_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();
        }

        private void btnDetainLicense_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFineFees.Text.Trim()))
            {
                MessageBox.Show("You should enter fine fees", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFineFees.Focus();
                return;
            }

            if (!float.TryParse(txtFineFees.Text.Trim(), out float fineFees) || fineFees < 0)
            {
                MessageBox.Show("Please enter a valid fine fee amount", "Invalid Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFineFees.Focus();
                return;
            }

            if (MessageBox.Show("Are you sure you want to detain this license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            _DetainLicense(fineFees);
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

        private void txtFineFees_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // منع كتابة أكثر من نقطة عشرية واحدة
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}