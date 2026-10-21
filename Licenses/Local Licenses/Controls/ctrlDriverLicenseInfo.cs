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
    public partial class ctrlDriverLicenseInfo : UserControl
    {
        private int _LicenseID = -1;
        private clsLicense _LicenseInfo;
        public clsLicense SelectedLicenseInfo => _LicenseInfo;
        public int LicenseID => _LicenseID;

        public ctrlDriverLicenseInfo()
        {
            InitializeComponent();
        }

        private void _ResetValues()
        {
            lblLicenseClass.Text = "????";
            lblDriverFullName.Text = "????";
            lblLicenseID.Text = "N/A";
            lblNationalNo.Text = "????";
            lblGender.Text = "????";
            lblIssueDate.Text = "00/00/0000";
            lblIssueReason.Text = "????";
            lblNotes.Text = "????";
            lblIsActive.Text = "????";
            lblDateOfBirth.Text = "00/00/0000";
            lblDriverID.Text = "N/A";
            lblExpirationDate.Text = "00/00/0000";
            lblIsDetained.Text = "????";
            pbDriverImage.Image = Resources.Male_512;
        }

        private void _LoadPersonImage()
        {
            if (_LicenseInfo.ApplicationInfo.PersonInfo.Gender == 0)
                pbDriverImage.Image = Resources.Male_512;
            else
                pbDriverImage.Image = Resources.Female_512;

            string ImagePath = _LicenseInfo.ApplicationInfo.PersonInfo.ImagePath;

            if (!string.IsNullOrEmpty(ImagePath))
            {
                if (File.Exists(ImagePath))
                    pbDriverImage.ImageLocation = ImagePath;
                else
                    MessageBox.Show("Could not find the person image on the desk, default image will be set.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadData(int LicenseID)
        {
            _LicenseID = LicenseID;

            _LicenseInfo = clsLicense.FindByLicenseID(LicenseID);

            if (_LicenseInfo == null)
            {
                _ResetValues();
                return;
            }

            lblLicenseClass.Text = _LicenseInfo.LicenseClassInfo.ClassName;
            lblDriverFullName.Text = _LicenseInfo.ApplicationInfo.ApplicantFullName;
            lblLicenseID.Text = _LicenseInfo.LicenseID.ToString();
            lblNationalNo.Text = _LicenseInfo.ApplicationInfo.PersonInfo.NationalNo;
            lblGender.Text = (_LicenseInfo.ApplicationInfo.PersonInfo.Gender == 0) ?  "Male" : "Female";
            lblIssueDate.Text = _LicenseInfo.IssueDate.ToShortDateString();
            lblIssueReason.Text = _LicenseInfo.IssueReasonText;
            lblNotes.Text = (!string.IsNullOrEmpty(_LicenseInfo.Notes)) ? _LicenseInfo.Notes : "No notes";
            lblIsActive.Text = (_LicenseInfo.IsActive) ? "Yes" : "No";
            lblDateOfBirth.Text = _LicenseInfo.ApplicationInfo.PersonInfo.DateOfBirth.ToShortDateString();
            lblDriverID.Text = _LicenseInfo.DriverID.ToString();
            lblExpirationDate.Text = _LicenseInfo.ExpirationDate.ToShortDateString();
            lblIsDetained.Text = (_LicenseInfo.IsDetained) ? "Yes" : "No";
            _LoadPersonImage();
        }
    }
}
