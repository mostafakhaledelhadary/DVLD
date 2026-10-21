using DVLD_Business;
using System.Windows.Forms;

namespace DVLD.Applications.Controls
{
    public partial class ctrlBasicApplicationInfo : UserControl
    {
        private clsApplication _ApplicationInfo;
        private int _ApplicationID = -1;

        public int ApplicationID
        {
            get { return _ApplicationID; }
        }

        public ctrlBasicApplicationInfo()
        {
            InitializeComponent();
            
        }

        public void ResetApplicationInfo()
        {
            _ApplicationID = -1;
            lblApplicationID.Text = "N/A";
            lblStatus.Text = "????";
            lblFees.Text = "0";
            lblApplicationType.Text = "????";
            lblApplicantPerson.Text = "????";
            lblApplicationDate.Text = "00/00/0000";
            lblLastStatusDate.Text = "00/00/0000";
            lblUser.Text = "????";
            llViewPersonInfo.Enabled = false;
        }

        private void _FillApplicationInfo()
        {
            _ApplicationID = _ApplicationInfo.ApplicationID;
            lblApplicationID.Text = _ApplicationInfo.ApplicationID.ToString();
            lblStatus.Text = _ApplicationInfo.StatusText;
            lblFees.Text = _ApplicationInfo.PaidFees.ToString();
            lblApplicationType.Text = _ApplicationInfo.ApplicationTypeInfo.Title;
            lblApplicantPerson.Text = _ApplicationInfo.ApplicantFullName;
            lblApplicationDate.Text = _ApplicationInfo.ApplicationDate.ToShortDateString();
            lblLastStatusDate.Text = _ApplicationInfo.LastStatusDate.ToShortDateString();
            lblUser.Text = _ApplicationInfo.UserInfo.Username;
            llViewPersonInfo.Enabled = true;
        }

        public void LoadData(int ApplicationID)
        {
            _ApplicationInfo = clsApplication.FindByApplicationID(ApplicationID);

            if( _ApplicationInfo == null)
            {
                ResetApplicationInfo();
                MessageBox.Show("Application is not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillApplicationInfo();
        }

        private void llViewPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_ApplicationInfo == null)
                return;

            frmPersonDetails frm = new frmPersonDetails(_ApplicationInfo.ApplicantPersonID);
            frm.ShowDialog();
            LoadData(_ApplicationID);
        }

    }
}
