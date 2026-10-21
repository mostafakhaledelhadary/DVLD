using DVLD.Properties;
using DVLD_Business;
using System;
using System.Windows.Forms;

namespace DVLD
{
    public partial class ctrlPersonCard : UserControl
    {
        private clsPerson _Person;
        private int _PersonID = -1;

        public int PersonID
        {
            get { return _PersonID; }
        }

        public clsPerson SelectedPersonInfo
        {
            get { return _Person; }
        }

        public ctrlPersonCard()
        {
            InitializeComponent();
        }

        private enum enGender : byte {Male = 0, Female = 1};

        private void _LoadPersonImage()
        {
            if (!string.IsNullOrEmpty(_Person.ImagePath))
                pbImage.ImageLocation = _Person.ImagePath;
            else
            {
                if (_Person.Gender == (byte)enGender.Male)
                    pbImage.Image = Resources.Male_512;
                else
                    pbImage.Image = Resources.Female_512;
            }
        }

        private void _FillPersonInfo()
        {
            _PersonID = _Person.PersonID;

            llEditPersonInfo.Enabled = true;

            string FullName = "";

            if (!string.IsNullOrEmpty(_Person.ThirdName))
                FullName = _Person.FirstName + " " + _Person.SecondName + " " + _Person.ThirdName + " " + _Person.LastName;
            else
                FullName = _Person.FirstName + " " + _Person.SecondName + " " + _Person.LastName;


            lblPersonID.Text = _Person.PersonID.ToString();
            lblFullName.Text = FullName;
            lblNationalNo.Text = _Person.NationalNo;

            if (_Person.Gender == (byte)enGender.Male)
                lblGender.Text = "Male";
            else
                lblGender.Text = "Female";

            if (!string.IsNullOrEmpty(_Person.Email))
                lblEmail.Text = _Person.Email;
            else
                lblEmail.Text = "";

            lblPhone.Text = _Person.Phone;
            lblAddress.Text = _Person.Address;
            lblDateOfBirth.Text = _Person.DateOfBirth.ToShortDateString();
            lblCountry.Text = clsCountry.Find(_Person.NationalityCountryID).Name;
            _LoadPersonImage();
        }

        public void ResetPersonInfo()
        {
            _PersonID = -1;
            lblPersonID.Text = "N/A";
            lblFullName.Text = "?????";
            lblNationalNo.Text = "000000000000";
            lblGender.Text = "?????";
            lblEmail.Text = "example@gmail.com";
            lblAddress.Text = "?????";
            lblDateOfBirth.Text = "00/00/0000";
            lblPersonID.Text = "00000000000";
            lblCountry.Text = "?????";
            lblPhone.Text = "00000000000";
            pbImage.Image = Resources.Male_512;
            llEditPersonInfo.Enabled = false;
        }

        public void LoadPersonInfo(int PersonID)
        {
            _Person = clsPerson.Find(PersonID);

            if(_Person == null)
            {
                ResetPersonInfo();
                MessageBox.Show($"Person with ID ({PersonID}) is not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillPersonInfo();
        }

        public void LoadPersonInfo(string NationalNo)
        {
            _Person = clsPerson.Find(NationalNo);

            if (_Person == null)
            {
                ResetPersonInfo();
                MessageBox.Show($"Person with National No. ({NationalNo}) is not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillPersonInfo();
        }

        private void _RefreshData()
        {
            LoadPersonInfo(_PersonID);
        }

        private void llEditPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson(_PersonID);
            frm.ShowDialog();
            _RefreshData();
        }
    }
}
