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

namespace DVLD.Applications.International_License
{
    public partial class frmAddNewInternationalLicenseApplication : Form
    {
        private int _InternationalLicenseID = -1;

        public frmAddNewInternationalLicenseApplication()
        {
            InitializeComponent();
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += _Validation;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _FillInternationalLicenseData(clsInternationalLicense InternationalLicense)
        {
            _InternationalLicenseID = InternationalLicense.InternationalLicenseID;
            lblInternationalLicenseApplicationID.Text = InternationalLicense.ApplicationID.ToString();
            lblInternationalLicenseID.Text = InternationalLicense.InternationalLicenseID.ToString();

            llShowInternationalLicenseInfo.Enabled = true;
            llShowLicensesHistory.Enabled = true;
            btnIssueInternationalLicense.Enabled = false;
        }

        private void _ResetDefaultValues()
        {
            _InternationalLicenseID = -1;
            lblInternationalLicenseApplicationID.Text = "N/A";
            lblInternationalLicenseID.Text = "N/A";
            lblLocalLicenseID.Text = "N/A";

            lblApplicationDate.Text = DateTime.Now.ToShortDateString();
            lblIssueDate.Text = DateTime.Now.ToShortDateString();
            lblExpirationDate.Text = DateTime.Now.AddYears(1).ToShortDateString();
            lblFees.Text = clsApplicationType.Find((int)clsApplication.enApplicationType.NewInternationalLicense).Fees.ToString();
            lblUsername.Text = clsGlobal.CurrentUser.Username;

            btnIssueInternationalLicense.Enabled = false;
            llShowInternationalLicenseInfo.Enabled = false;
            llShowLicensesHistory.Enabled = false;
        }

        private void _Validation()
        {
            // 1. إعادة ضبط البيانات لتفادي بقاء بيانات السائق السابق
            _ResetDefaultValues();

            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo == null)
                return;

            clsLicense SelectedLicenseInfo = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo;

            lblLocalLicenseID.Text = SelectedLicenseInfo.LicenseID.ToString();
            llShowLicensesHistory.Enabled = true;

            // التحقق من صلاحية الرخصة المحلية (فئة 3، سارية، مش منتهية)
            if (!SelectedLicenseInfo.IsActive)
            {
                MessageBox.Show($"Person is not qualified to issue an International License beacuse Selected License is not active",
                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnIssueInternationalLicense.Enabled = false;
                return;
            }

            if (SelectedLicenseInfo.LicenseClassInfo.LicenseClassID != 3)
            {
                MessageBox.Show($"Selected License should be from class 3 to issue an International License",
                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnIssueInternationalLicense.Enabled = false;
                return;
            }

            if (SelectedLicenseInfo.IsLicenseExpired())
            {
                MessageBox.Show($"Selected License is expired!!",
                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnIssueInternationalLicense.Enabled = false;
                return;
            }

            // 3. التحقق من وجود رخصة دولية سارية
            int ActiveInternationalLicenseID =
                clsInternationalLicense.GetActiveInternationalLicenseIDByDriverID(SelectedLicenseInfo.DriverID);

            if (ActiveInternationalLicenseID != -1)
            {
                MessageBox.Show($"Person already has an active international license with ID = {ActiveInternationalLicenseID}",
                                "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // جاهز للإصدار
            btnIssueInternationalLicense.Enabled = true;
        }

        private void _IssueNewInternationalLicense()
        {
            clsApplication NewApplication = new clsApplication
            {
                ApplicationTypeID = Convert.ToInt32(clsApplication.enApplicationType.NewInternationalLicense),
                ApplicationStatus = clsApplication.enApplicationStatus.New,
                ApplicantPersonID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.ApplicationInfo.PersonInfo.PersonID,
                ApplicationDate = DateTime.Now,
                LastStatusDate = DateTime.Now,
                PaidFees = clsApplicationType.Find((int)clsApplication.enApplicationType.NewInternationalLicense).Fees,
                CreatedByUserID = clsGlobal.CurrentUser.UserID
            };

            if (NewApplication.Save())
            {
                clsInternationalLicense NewInternationalLicenseInfo = new clsInternationalLicense
                {
                    DriverID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverID,
                    ApplicationID = NewApplication.ApplicationID,
                    IssuedUsingLocalLicenseID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.LicenseID,
                    IssueDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddYears(1),
                    IsActive = true,
                    CreatedByUserID = clsGlobal.CurrentUser.UserID
                };

                if (NewInternationalLicenseInfo.Save())
                {
                    NewApplication.SetComplete();
                    _FillInternationalLicenseData(NewInternationalLicenseInfo);
                    ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;

                    MessageBox.Show($"International license issued successfully with ID = {NewInternationalLicenseInfo.InternationalLicenseID}",
                        "License issued", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Failed to issue an international license", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("Failed to create a new application", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void llShowInternationalLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmInternationalLicenseInfo frm = new frmInternationalLicenseInfo(_InternationalLicenseID);
            frm.ShowDialog();
        }

        private void llShowLicensesHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmPersonLicenseHistory frm = new frmPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.ApplicationInfo.PersonInfo.PersonID);
            frm.ShowDialog();
        }

        private void btnIssueInternationalLicense_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to issue the license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            _IssueNewInternationalLicense();
        }

        private void frmAddNewInternationalLicenseApplication_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();
        }
    }
}