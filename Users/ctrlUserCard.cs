using DVLD_Business;
using System;
using System.Windows.Forms;

namespace DVLD.Users
{
    public partial class ctrlUserCard : UserControl
    {
        private clsUser _User;
        private int _UserID = -1;

        public int UserID
        {
            get { return _UserID; }
        }

        public ctrlUserCard()
        {
            InitializeComponent();
        }

        public void LoadUserInfo(int UserID)
        {
            _User = clsUser.FindByUserID(UserID);

            if (_User == null)
            {
                _ResetUserInfo();
                MessageBox.Show($"User with UserID ({UserID}) is not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillUserInfo();
        }

        private void _FillUserInfo()
        {
            _UserID = _User.UserID;
            ctrlPersonCard1.LoadPersonInfo(_User.PersonID);
            lblUserID.Text = _User.UserID.ToString();
            lblUsername.Text = _User.Username;

            if (_User.IsActive)
                lblIsActive.Text = "Yes";
            else
                lblIsActive.Text = "No";
        }

        private void _ResetUserInfo()
        {
            _UserID = -1;
            lblUserID.Text = "N/A";
            lblUsername.Text = "?????";
            lblIsActive.Text = "?????";
            ctrlPersonCard1.ResetPersonInfo();
        }

    }
}
