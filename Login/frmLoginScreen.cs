using DVLD_Business;
using System;
using System.Windows.Forms;

namespace DVLD
{
    public partial class frmLoginScreen : Form
    {
        public frmLoginScreen()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
           Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            clsUser User = clsUser.FindByUsernameAndPassword(txtUsername.Text.Trim(), txtPassword.Text.Trim());

            if (User != null)
            {
                if (User.IsActive)
                {
                    if (chkRememberMe.Checked)
                    {
                        clsGlobal.RememberUsernameAndPassword(txtUsername.Text.Trim(), txtPassword.Text.Trim());
                    }
                    else
                    {
                        clsGlobal.DeleteStoredCredentials();
                    }

                    clsGlobal.CurrentUser = User;
                    this.Hide();
                    frmMain frm = new frmMain();
                    frm.ShowDialog();
                    _LoadCredentials();
                    this.Show();
                }
                else
                {
                    txtUsername.Focus();
                    MessageBox.Show("Your account isn't active, please contact your admin.");
                }
            }
            else
            {
                txtUsername.Focus();
                MessageBox.Show("Invalid Username or Password.", "Wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _LoadCredentials()
        {
            string Username = "", Password = "";

            if (clsGlobal.GetStoredCredentials(ref Username, ref Password))
            {
                txtUsername.Text = Username;
                txtPassword.Text = Password;
                chkRememberMe.Checked = true;
            }
            else
            {
                chkRememberMe.Checked = false;
                txtUsername.Text = "";
                txtPassword.Text = "";
            }
        }

        private void frmLoginScreen_Load(object sender, EventArgs e)
        {
            _LoadCredentials();
        }
    }
}