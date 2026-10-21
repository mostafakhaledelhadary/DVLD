using DVLD_Business;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DVLD
{
    public partial class frmAddUpdateUser : Form
    {
        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;
        private clsUser _User;
        private int _UserID = -1;
        
        public frmAddUpdateUser()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
            ctrlPersonCardWithFilter1.OnPersonNotFound += DisablebtnSaveAndtpLoginInfo;
        }

        public frmAddUpdateUser(int UserID)
        {
            InitializeComponent();
            _UserID = UserID;
            _Mode = enMode.Update;
        }

        private void DisablebtnSaveAndtpLoginInfo()
        {
            btnSave.Enabled = false;
            tpLoginInfo.Enabled = false;
        }

        private void _LoadData()
        {
            if (_Mode == enMode.AddNew)
            {
                ctrlPersonCardWithFilter1.FilterFocus();
                lblAddUpdateUser.Text = "Add New User";
                this.Text = "Add New User";
                _User = new clsUser();
                tpLoginInfo.Enabled = false;
                return;
            }

            lblAddUpdateUser.Text = "Update User";
            this.Text = "Update User";

            _User = clsUser.FindByUserID(_UserID);
            ctrlPersonCardWithFilter1.FilterEnabled = false;
            btnSave.Enabled = true;

            if (_User == null)
            {
                MessageBox.Show("User not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            ctrlPersonCardWithFilter1.LoadPersonInfo(_User.PersonID);
            lblUserID.Text = _UserID.ToString();
            txtUsername.Text = _User.Username;
            txtPassword.Text = _User.Password;
            txtConfirmPassword.Text = _User.Password;

            if (_User.IsActive)
                chkIsActive.Checked = true;
            else
                chkIsActive.Checked = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_Mode == enMode.Update)
            {
                tcUserInfo.SelectedTab = tcUserInfo.TabPages["tpLoginInfo"];
                return;
            }

            //In case of add new mode.
            if (ctrlPersonCardWithFilter1.PersonID != -1)
            {
                if (clsUser.IsPersonIsAUser(ctrlPersonCardWithFilter1.PersonID))
                {
                    MessageBox.Show("Selected Person is already a user, choose another one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ctrlPersonCardWithFilter1.FilterFocus();
                    ctrlPersonCardWithFilter1.ResetPersonInfo();
                }
                else
                {
                    btnSave.Enabled = true;
                    tpLoginInfo.Enabled = true;
                    tcUserInfo.SelectedTab = tcUserInfo.TabPages["tpLoginInfo"];
                }
            }
            else
            {
                ctrlPersonCardWithFilter1.FilterFocus();
                btnSave.Enabled = false;
                tpLoginInfo.Enabled = false;
                MessageBox.Show("Person not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtUsername_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtUsername, "Username cannot be blank!");
                return;
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtUsername, null);
            }

            if(_User.Username != txtUsername.Text.Trim() && clsUser.IsUserExists(txtUsername.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtUsername, "This Username is already assigned to another user, please enter another one.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtUsername, null);
            }
        }

        private void txtConfirmPassword_Validating(object sender, CancelEventArgs e)
        {
            if(txtConfirmPassword.Text.Trim() != txtPassword.Text.Trim())
            {
                e.Cancel = true;
                errorProvider1.SetError(txtConfirmPassword, "Password should be matched.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtConfirmPassword, null);
            }
        }

        private void frmAddUpdateUser_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fileds are not valid!, put the mouse over the red icon(s) to see the error.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }           

            _User.PersonID = ctrlPersonCardWithFilter1.PersonID;
            _User.Username = txtUsername.Text.Trim();
            _User.Password = txtPassword.Text.Trim();

            if(chkIsActive.Checked)
                _User.IsActive = true;
            else
                _User.IsActive = false;

            if (_User.Save())
            {
                MessageBox.Show("Data saved successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblAddUpdateUser.Text = "Update User";
                this.Text = "Update User";
                lblUserID.Text = _User.UserID.ToString();
                _Mode = enMode.Update;
            }
            else
                MessageBox.Show("Failed to save data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void frmAddUpdateUser_Activated(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.FilterFocus();
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtPassword, "Password cannot be blank!");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtPassword, null);
            }
        }
    }
}
