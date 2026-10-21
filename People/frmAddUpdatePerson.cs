using DVLD.Properties;
using DVLD_Business;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace DVLD
{
    public partial class frmAddUpdatePerson : Form
    {
        private enum enMode : byte { AddNew = 0, Update = 1 }
        private enum enGender : byte { Male = 0, Female = 1 }

        private enMode _Mode;
        private clsPerson _Person;
        private int _PersonID = -1;
        public event Action<int> OnSaveComplete;

        public frmAddUpdatePerson()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        public frmAddUpdatePerson(int PersonID)
        {
            InitializeComponent();
            _PersonID = PersonID;
            _Mode = enMode.Update;
        }

        private void _SetDefaultValues()
        {
            _FillCountriesInComboBox();
            dtpDateOfBirth.MaxDate = DateTime.Now.AddYears(-18);
            dtpDateOfBirth.Value = dtpDateOfBirth.MaxDate;
            dtpDateOfBirth.MinDate = DateTime.Now.AddYears(-100);
            cbCountry.SelectedIndex = cbCountry.FindString("Egypt");
        }

        private void _LoadData()
        {
            _SetDefaultValues();

            //Add new person.
            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Person";
                this.Text = "Add New Person";
                _Person = new clsPerson();
                return;
            }

            //Edit person.
            lblTitle.Text = "Update Person";
            this.Text = "Update Person";

            _Person = clsPerson.Find(_PersonID);

            if (_Person == null)
            {
                MessageBox.Show("Person not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            lblPersonID.Text = _PersonID.ToString();
            txtFirstName.Text = _Person.FirstName;
            txtSecondName.Text = _Person.SecondName;
            txtThirdName.Text = _Person.ThirdName;
            txtLastName.Text = _Person.LastName;
            txtNationalNo.Text = _Person.NationalNo;

            if (_Person.Gender == (byte)enGender.Male)
                rbMale.Checked = true;
            else
                rbFemale.Checked = true;

            txtEmail.Text = _Person.Email;
            txtPhone.Text = _Person.Phone;
            txtAddress.Text = _Person.Address;
            dtpDateOfBirth.Value = _Person.DateOfBirth;
            cbCountry.SelectedIndex = cbCountry.FindString(_Person.CountryInfo.Name);

            if (!string.IsNullOrEmpty(_Person.ImagePath))
            {
                pbImage.ImageLocation = _Person.ImagePath;
            }
            else
                pbImage.ImageLocation = "";

            llRemoveImage.Visible = (!string.IsNullOrEmpty(_Person.ImagePath));
        }

        private void frmAddUpdatePerson_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //private bool _HandlePersonImage()
        //{
        //    /* there are 3 probabilities in changing:
        //       Person: 1- changed his image to a new image (if he was having an old image) (this means that !string.IsNullOrEmpty(pbImage.ImageLocation)).
        //               2- or deleted that old image and didn't set any image (this means that _Person.ImagePath = "").
        //               3- or setting an image (if he wasn't have any images) (this means that !string.IsNullOrEmpty(pbImage.ImageLocation)).
        //     */
        //    if (_Person.ImagePath != pbImage.ImageLocation)
        //    {
        //        /* Check if person was having an old image
        //           if true -> delete this old image.
        //           if false -> this means that this Person did that change by setting an image.
        //           otherwise, this means that this Person didn't make any changes and will not enter to this condition: (_Person.ImagePath != pbImage.ImageLocation)
        //        */
        //        if (!string.IsNullOrEmpty(_Person.ImagePath))
        //        {
        //            try
        //            {
        //                if (File.Exists(_Person.ImagePath))
        //                {
        //                    File.Delete(_Person.ImagePath);
        //                }
        //            }

        //            catch(IOException e)
        //            {
        //                MessageBox.Show(e.Message);
        //                return false;
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(pbImage.ImageLocation))
        //        {
        //            string UniqueName = Guid.NewGuid().ToString();
        //            FileInfo sourceFileName = new FileInfo(pbImage.ImageLocation);
        //            string GUIDName = UniqueName + sourceFileName.Extension;
        //            string destinationFolder = @"D:\DVLD-People-Images\";

        //            if (!Directory.Exists(destinationFolder))
        //            {
        //                Directory.CreateDirectory(destinationFolder);
        //            }

        //            string destinationFileName = destinationFolder + GUIDName;
        //            File.Copy(sourceFileName.FullName, destinationFileName, true);
        //            _Person.ImagePath = destinationFileName;
        //        }
        //        else
        //            _Person.ImagePath = "";

        //        return true;
        //    }

        //    return true;
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fileds are not valid!, put the mouse over the red icon(s) to see the error.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            _Person.NationalNo = txtNationalNo.Text.Trim();
            _Person.FirstName = txtFirstName.Text.Trim();
            _Person.SecondName = txtSecondName.Text.Trim();
            _Person.ThirdName = txtThirdName.Text.Trim();
            _Person.LastName = txtLastName.Text.Trim();

            if (rbMale.Checked)
                _Person.Gender = (byte)enGender.Male;
            else
                _Person.Gender = (byte)enGender.Female;

            _Person.Email = txtEmail.Text.Trim();
            _Person.Phone = txtPhone.Text.Trim();
            _Person.Address = txtAddress.Text.Trim();
            _Person.DateOfBirth = dtpDateOfBirth.Value;
            _Person.NationalityCountryID = clsCountry.Find(cbCountry.Text).ID;

            bool IsImageChanged = (_Person.ImagePath != pbImage.ImageLocation);      
            string destinationFileName = "";
            string OldImagePath = _Person.ImagePath;
            FileInfo sourceFileName = null;

            if (IsImageChanged)
            {
                if (!string.IsNullOrEmpty(pbImage.ImageLocation))
                {
                    string UniqueName = Guid.NewGuid().ToString();
                    sourceFileName = new FileInfo(pbImage.ImageLocation);
                    string GUIDName = UniqueName + sourceFileName.Extension;
                    string destinationFolder = @"D:\DVLD-People-Images\";

                    if (!Directory.Exists(destinationFolder))
                    {
                        Directory.CreateDirectory(destinationFolder);
                    }

                    destinationFileName = destinationFolder + GUIDName;
                    _Person.ImagePath = destinationFileName;
                }
                else
                    _Person.ImagePath = "";
            }


            if (_Person.Save())
            {
                if (IsImageChanged)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(OldImagePath) && File.Exists(OldImagePath))
                            File.Delete(OldImagePath);

                        if (sourceFileName != null)
                            File.Copy(sourceFileName.FullName, destinationFileName, true);
                    }

                    catch(Exception ex)
                    {
                        MessageBox.Show($"Data saved, but there was an issue handling the image file: {ex.Message}");
                    }
                }

                _Mode = enMode.Update;
                lblPersonID.Text = _Person.PersonID.ToString();
                lblTitle.Text = "Update Person";
                this.Text = "Update Person";
                MessageBox.Show("Data saved successfully.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OnSaveComplete?.Invoke(_Person.PersonID);
            }
            else
                MessageBox.Show("Failed to save data.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void _FillCountriesInComboBox()
        {
            DataTable Countries = clsCountry.GetAllCountries();

            foreach (DataRow Row in Countries.Rows)
            {
                cbCountry.Items.Add(Row["CountryName"]);
            }
        }

        private void txtNationalNo_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNationalNo.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtNationalNo, "This field is required!");
                return;
            }

            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtNationalNo, null);
            }

            /* txtNationalNo.Text.Trim() != _Person.NationalNo: This condition means that you are in update mode and you changed _Person.NationalNo.
             
               clsPerson.IsPersonExists(txtNationalNo.Text.Trim()): This condition is to check if txtNationalNo.Text you entered
               belongs to another person in database (while you are changing _Person.NationalNo in update mode).
            */
            if (txtNationalNo.Text.Trim() != _Person.NationalNo && clsPerson.IsPersonExists(txtNationalNo.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtNationalNo, "This National No. is already assigned to another person, please enter another one.");
            }

            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtNationalNo, null);
            }
        }

        private void RequiredFields_Validating(object sender, CancelEventArgs e)
        {
            TextBox Field = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(Field.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(Field, "This field is required.");
            }

            else
            {
                e.Cancel = false;
                errorProvider1.SetError(Field, null);
            }
        }

        private void llSetImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.DefaultExt = "jpg";
            openFileDialog1.Title = "Open";
            openFileDialog1.Multiselect = false;
            openFileDialog1.InitialDirectory = @"C:/";
            openFileDialog1.Filter = @"Image files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pbImage.ImageLocation = openFileDialog1.FileName;
                llRemoveImage.Visible = true;
            }
        }

        private void llRemoveImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            pbImage.ImageLocation = "";

            if (rbMale.Checked)
                pbImage.Image = Resources.Male_512;
            else
                pbImage.Image = Resources.Female_512;

            llRemoveImage.Visible = false;
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                e.Cancel = false;
                errorProvider1.SetError(txtEmail, null);
                return;
            }

            if (!clsValidation.IsValidEmail(txtEmail.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, "Invalid email format!");
            }

            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtEmail, null);
            }
        }

        private void rbGender_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked) return;

            if (pbImage.Image != null && string.IsNullOrEmpty(pbImage.ImageLocation))
            {
                if (rbMale.Checked)
                    pbImage.Image = Resources.Male_512;
                else
                    pbImage.Image = Resources.Female_512;
            }
        }

        private void Validation_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(sender == txtFirstName || sender == txtSecondName || sender == txtThirdName || sender == txtLastName)
            {
                if(!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

            if (sender == txtNationalNo)
            {
                if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

            if (sender == txtPhone)
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
        }
    }
}