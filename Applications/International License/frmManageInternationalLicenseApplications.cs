using DVLD.Applications.International_License;
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
    public partial class frmManageInternationalLicenseApplications : Form
    {
        private DataTable _InternationalLicenses;

        public frmManageInternationalLicenseApplications()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _LoadData()
        {
            _InternationalLicenses = clsInternationalLicense.GetAllInternationalLicenses();
            dgvInternationalLicenseApplications.DataSource = _InternationalLicenses;
            lblRecords.Text = dgvInternationalLicenseApplications.Rows.Count.ToString();
        }

        private void frmManageInternationalLicenseApplications_Load(object sender, EventArgs e)
        {
            _LoadData();
            cbFilter.SelectedIndex = 0;
        }

        private void btnAddNewInternationalLicenseApplication_Click(object sender, EventArgs e)
        {
            frmAddNewInternationalLicenseApplication frm = new frmAddNewInternationalLicenseApplication();
            frm.ShowDialog();
            _LoadData();
        }

        private void _ApplyFilter()
        {
            string FilterColumn = "";

            switch (cbFilter.Text)
            {
                case "International License ID": FilterColumn = "[International License ID]"; break;
                case "Application ID": FilterColumn = "[Application ID]"; break;
                case "Driver ID": FilterColumn = "[Driver ID]"; break;
                case "Local License ID": FilterColumn = "[Local License ID]"; break;
                case "Is Active": FilterColumn = "[Is Active?]"; break;
                default: FilterColumn = "None"; break;
            }

            if (FilterColumn == "None")
            {
                _InternationalLicenses.DefaultView.RowFilter = "";
                lblRecords.Text = dgvInternationalLicenseApplications.Rows.Count.ToString();
                return;
            }

            if (FilterColumn == "[Is Active?]")
            {
                if (cbIsActive.Text == "Yes")
                    _InternationalLicenses.DefaultView.RowFilter = $"{FilterColumn} = 1";

                else if (cbIsActive.Text == "No")
                    _InternationalLicenses.DefaultView.RowFilter = $"{FilterColumn} = 0";

                else
                    _InternationalLicenses.DefaultView.RowFilter = "";
            }
            else
            {
                if(string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
                    _InternationalLicenses.DefaultView.RowFilter = "";

                else if (int.TryParse(txtFilterValue.Text.Trim(), out int ID))
                    _InternationalLicenses.DefaultView.RowFilter = $"{FilterColumn} = {ID}";

                else
                    _InternationalLicenses.DefaultView.RowFilter = $"{FilterColumn} = -1";
            }

            lblRecords.Text = dgvInternationalLicenseApplications.Rows.Count.ToString();
        }

        private int _GetSelectedPersonID()
        {
            if (dgvInternationalLicenseApplications.CurrentRow == null || dgvInternationalLicenseApplications.CurrentRow.Index < 0)
                return -1;

            int DriverID = Convert.ToInt32(dgvInternationalLicenseApplications.CurrentRow.Cells["Driver ID"].Value);
            clsDriver DriverInfo = clsDriver.FindByDriverID(DriverID);

            return (DriverInfo != null) ? DriverInfo.PersonID : -1;
        }

        private int _GetSelectedInternationalLicenseID()
        {
            if (dgvInternationalLicenseApplications.CurrentRow == null || dgvInternationalLicenseApplications.CurrentRow.Index < 0)
                return -1;

            int InternationalLicenseID = Convert.ToInt32(dgvInternationalLicenseApplications.CurrentRow.Cells["International License ID"].Value);

            return InternationalLicenseID;
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            _ApplyFilter();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(cbFilter.Text == "International License ID" || cbFilter.Text == "Application ID"
                || cbFilter.Text == "Driver ID" || cbFilter.Text == "Local License ID")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            }
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilter.Text != "None" && cbFilter.Text != "Is Active");
            cbIsActive.Visible = (cbFilter.Text == "Is Active");

            if (cbIsActive.Visible)
            {
                cbIsActive.SelectedIndex = 0;
            }

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }

            _ApplyFilter();
        }

        private void cbIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ApplyFilter();
        }

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = _GetSelectedPersonID();

            if (PersonID == -1)
            {
                MessageBox.Show("No Person Selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            frmPersonDetails frm = new frmPersonDetails(PersonID);
            frm.ShowDialog();
        }

        private void showLicenseDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int InternationalLicenseID = _GetSelectedInternationalLicenseID();

            if (InternationalLicenseID == -1)
            {
                MessageBox.Show("No License Selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            frmInternationalLicenseInfo frm = new frmInternationalLicenseInfo(InternationalLicenseID);
            frm.ShowDialog();
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = _GetSelectedPersonID();

            if (PersonID == -1)
            {
                MessageBox.Show("No Person Selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            frmPersonLicenseHistory frm = new frmPersonLicenseHistory(PersonID);
            frm.ShowDialog();
        }
    }
}