using DVLD.Properties;
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
using System.IO;

namespace DVLD
{
    public partial class ctrlDriverInternationalLicenseInfo : UserControl
    {
        public ctrlDriverInternationalLicenseInfo()
        {
            InitializeComponent();
        }

        public void LoadData(int InternationalLicenseID)
        {
            clsInternationalLicense InternationalLicenseInfo = clsInternationalLicense.FindByInternationalLicenseID(InternationalLicenseID);

            if(InternationalLicenseInfo == null)
            {
                MessageBox.Show($"International License with ID = {InternationalLicenseID} is not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblDriverFullName.Text = InternationalLicenseInfo.ApplicationInfo.ApplicantFullName;
            lblInternationalLicenseID.Text = InternationalLicenseID.ToString();
            lblLicenseID.Text = InternationalLicenseInfo.IssuedUsingLocalLicenseID.ToString();
            lblNationalNo.Text = InternationalLicenseInfo.ApplicationInfo.PersonInfo.NationalNo;
            lblGender.Text = (InternationalLicenseInfo.ApplicationInfo.PersonInfo.Gender == 0) ? "Male" : "Female";
            lblIssueDate.Text = InternationalLicenseInfo.IssueDate.ToShortDateString();
            lblInternationalLicenseApplicationID.Text = InternationalLicenseInfo.ApplicationID.ToString();
            lblIsActive.Text = InternationalLicenseInfo.IsActive ? "Yes" : "No";
            lblDateOfBirth.Text = InternationalLicenseInfo.ApplicationInfo.PersonInfo.DateOfBirth.ToShortDateString();
            lblDriverID.Text = InternationalLicenseInfo.DriverID.ToString();
            lblExpirationDate.Text = InternationalLicenseInfo.ExpirationDate.ToShortDateString();

            string imagePath = InternationalLicenseInfo.ApplicationInfo.PersonInfo.ImagePath;

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                pbDriverImage.ImageLocation = imagePath;
            }
            else
            { 
                if (InternationalLicenseInfo.ApplicationInfo.PersonInfo.Gender == 0)
                    pbDriverImage.Image = Resources.Male_512;
                else
                    pbDriverImage.Image = Resources.Female_512;
            }
        }
    }
}