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
    public partial class ctrlDriverLicenseInfoWithFilter : UserControl
    {
        public ctrlDriverLicenseInfoWithFilter()
        {
            InitializeComponent();
        }

        public event Action OnLicenseSelected;

        //Expression-bodied member
        //Read-Only Property
        public clsLicense SelectedLicenseInfo => ctrlDriverLicenseInfo1.SelectedLicenseInfo;
        private bool _FilterEnabled = true;
        public bool FilterEnabled
        {
            get { return _FilterEnabled; }
            set 
            {
                _FilterEnabled = value;
                gbFilter.Enabled = _FilterEnabled;
            }
        }

        public void LoadLicenseInfo(int LicenseID)
        {
            txtFilterValue.Text = LicenseID.ToString();
            gbFilter.Enabled = false;
            ctrlDriverLicenseInfo1.LoadData(LicenseID);
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                btnFindLicense.PerformClick();
                return;
            }

            e.Handled = (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar));
        }

        private void btnFindLicense_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid!, put the mouse over the red icon(s) to see the error.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
            {
                MessageBox.Show("Text is empty, please enter a number!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FilterFocus();
                return;
            }

            if (!int.TryParse(txtFilterValue.Text, out int licenseID))
                return;

            ctrlDriverLicenseInfo1.LoadData(licenseID);

            if (ctrlDriverLicenseInfo1.SelectedLicenseInfo == null)
            {
                MessageBox.Show($"License with ID = {licenseID} is not found!",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FilterFocus();
                return;
            }

            OnLicenseSelected?.Invoke();
        }

        private void txtFilterValue_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
            {
                errorProvider1.SetError(txtFilterValue, "This field is required.");
            }
            else
            {
                errorProvider1.SetError(txtFilterValue, null);
            }
        }

        public void FilterFocus()
        {
            txtFilterValue.Text = "";
            txtFilterValue.Focus();
        }
    }
}
