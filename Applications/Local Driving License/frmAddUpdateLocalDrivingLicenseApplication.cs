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
    public partial class frmAddUpdateLocalDrivingLicenseApplication : Form
    {
        private enum enMode : byte { AddNew = 0, Update = 1 }
        private enMode _Mode;
        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplication;
        private int _LocalDrivingLicenseApplicationID;

        public frmAddUpdateLocalDrivingLicenseApplication()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
            ctrlPersonCardWithFilter1.OnPersonNotFound += DisablebtnSaveAndtpApplicationInfo;
        }

        public frmAddUpdateLocalDrivingLicenseApplication(int LocalDrivingLicenseApplication)
        {
            InitializeComponent();
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplication;
            _Mode = enMode.Update;
        }

        private void DisablebtnSaveAndtpApplicationInfo()
        {
            btnSave.Enabled = false;
            tpApplicationInfo.Enabled = false;
        }

        private void _FillLicenseClassesInComboBox()
        {
            cbLicenseClass.DataSource = clsLicenseClass.GetAllLicenseClasses();
            cbLicenseClass.DisplayMember = "ClassName";
            cbLicenseClass.ValueMember = "LicenseClassID";
        }

        private void _LoadData()
        {
            _FillLicenseClassesInComboBox();

            if (_Mode == enMode.AddNew)
            {
                ctrlPersonCardWithFilter1.FilterFocus();
                tpApplicationInfo.Enabled = false;
                lblAddUpdateLDLApplication.Text = "New Local Driving License Application";
                this.Text = "New Local Driving License Application";
                _LocalDrivingLicenseApplication = new clsLocalDrivingLicenseApplication();
                lblFees.Text = clsApplicationType.Find((int)clsApplication.enApplicationType.NewDrivingLicense).Fees.ToString();
                cbLicenseClass.SelectedIndex = 2;
                lblUser.Text = clsGlobal.CurrentUser.Username;
                lblApplicationDate.Text = DateTime.Now.ToShortDateString();
                return;
            }

            lblAddUpdateLDLApplication.Text = "Update Local Driving License Application";
            this.Text = "Update Local Driving License Application";

            _LocalDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingLicenseApplicationID(_LocalDrivingLicenseApplicationID);

            if (_LocalDrivingLicenseApplication == null)
            {
                MessageBox.Show("Application not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            ctrlPersonCardWithFilter1.LoadPersonInfo(_LocalDrivingLicenseApplication.ApplicantPersonID);
            ctrlPersonCardWithFilter1.FilterEnabled = false;
            btnSave.Enabled = true;

            lblLocalDrivingLicenseApplicationID.Text = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
            lblApplicationDate.Text = _LocalDrivingLicenseApplication.ApplicationDate.ToShortDateString();
            cbLicenseClass.SelectedIndex =
                cbLicenseClass.FindString(clsLicenseClass.Find(_LocalDrivingLicenseApplication.LicenseClassID).ClassName);
            lblFees.Text = _LocalDrivingLicenseApplication.PaidFees.ToString();
            lblUser.Text = _LocalDrivingLicenseApplication.UserInfo.Username;
        }

        private void frmAddUpdateLocalDrivingLicenseApplication_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if(_Mode == enMode.Update)
            {
                tcLDLApplicationInfo.SelectedTab = tcLDLApplicationInfo.TabPages["tpApplicationInfo"];
                return;
            }

            //Add new.
            if (ctrlPersonCardWithFilter1.PersonID != -1)
            {
                tcLDLApplicationInfo.SelectedTab = tcLDLApplicationInfo.TabPages["tpApplicationInfo"];
                tpApplicationInfo.Enabled = true;
                btnSave.Enabled = true;
            }
            else
            {
                MessageBox.Show("Person not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.FilterFocus();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int ActiveApplicationID = clsApplication.GetActiveApplicationIDForLicenseClass(ctrlPersonCardWithFilter1.PersonID,
                clsApplication.enApplicationType.NewDrivingLicense, Convert.ToInt32(cbLicenseClass.SelectedValue));

            if(ActiveApplicationID != -1 && ActiveApplicationID != _LocalDrivingLicenseApplication.ApplicationID)
            {
                MessageBox.Show("Choose another License Class, the selected Person already has an active application for the selected class.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbLicenseClass.Focus();
                return;
            }

            int LicenseID = clsLicense.GetActiveLicenseIDByPersonID(ctrlPersonCardWithFilter1.PersonID, Convert.ToInt32(cbLicenseClass.SelectedValue));

            if (LicenseID != -1)
            {
                MessageBox.Show("The selected Person already has a license in this license class", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbLicenseClass.Focus();
                return;
            }

            if(_Mode == enMode.AddNew)
            {
                _LocalDrivingLicenseApplication.ApplicantPersonID = ctrlPersonCardWithFilter1.PersonID;
                _LocalDrivingLicenseApplication.ApplicationDate = DateTime.Now;
                _LocalDrivingLicenseApplication.ApplicationTypeID = (int)clsApplication.enApplicationType.NewDrivingLicense;
                _LocalDrivingLicenseApplication.ApplicationStatus = clsApplication.enApplicationStatus.New;
                _LocalDrivingLicenseApplication.LastStatusDate = DateTime.Now;
                _LocalDrivingLicenseApplication.PaidFees = Convert.ToSingle(lblFees.Text);
                _LocalDrivingLicenseApplication.CreatedByUserID = clsGlobal.CurrentUser.UserID;
            }

            _LocalDrivingLicenseApplication.LicenseClassID = Convert.ToInt32(cbLicenseClass.SelectedValue);

            if (_LocalDrivingLicenseApplication.Save())
            {
                this.Text = "Update Local Driving License Application";
                lblAddUpdateLDLApplication.Text = "Update Local Driving License Application";
                lblLocalDrivingLicenseApplicationID.Text = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
                _Mode = enMode.Update;
                _LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID;
                ctrlPersonCardWithFilter1.FilterEnabled = false;
                MessageBox.Show("Data saved successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Failed to save data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
