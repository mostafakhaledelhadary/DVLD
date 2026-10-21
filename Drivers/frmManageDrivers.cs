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
    public partial class frmManageDrivers : Form
    {
        private DataTable _AllDriversInfo;

        public frmManageDrivers()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _LoadData()
        {
            _AllDriversInfo = clsDriver.GetAllDrivers();
            dgvDrivers.DataSource = _AllDriversInfo;
            lblRecords.Text = dgvDrivers.Rows.Count.ToString();
            _AllDriversInfo.DefaultView.RowFilter = "";
            cbFilter.SelectedIndex = 0;
        }

        private int _GetPersonID()
        {
            if (dgvDrivers.CurrentRow == null || dgvDrivers.CurrentRow.Index < 0)
                return -1;

            return Convert.ToInt32(dgvDrivers.CurrentRow.Cells["Person ID"].Value);
        }

        private void frmManageDrivers_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilterValue.Text.Trim()) || cbFilter.Text == "None" || _AllDriversInfo == null)
            {
                if(_AllDriversInfo != null)
                    _AllDriversInfo.DefaultView.RowFilter = "";

                lblRecords.Text = dgvDrivers.Rows.Count.ToString();
                return;
            }

            string FilterColumn = "";

            switch (cbFilter.Text)
            {
                case "Driver ID": FilterColumn = "[Driver ID]"; break;

                case "Person ID": FilterColumn = "[Person ID]"; break;

                case "National No.": FilterColumn = "[National No.]"; break;

                case "Full Name": FilterColumn = "[Full Name]"; break;
            }

            string FilterValue = txtFilterValue.Text.Trim().Replace("'", "''");

            if (FilterColumn == "[Driver ID]" || FilterColumn == "[Person ID]")
            {
                //الحماية من الحروف التي تم أخذها Copy ثم الضغط على Save داخل الفلتر
                if(int.TryParse(FilterValue, out int ID))
                    _AllDriversInfo.DefaultView.RowFilter = $"{FilterColumn} = {ID}";
                else
                    _AllDriversInfo.DefaultView.RowFilter = $"{FilterColumn} = -1";
            }
            else
                _AllDriversInfo.DefaultView.RowFilter = $"{FilterColumn} LIKE '{FilterValue}%'";

            lblRecords.Text = dgvDrivers.Rows.Count.ToString();
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilter.Text != "None");
            txtFilterValue.Text = "";
            txtFilterValue.Focus();

            if(_AllDriversInfo != null)
                _AllDriversInfo.DefaultView.RowFilter = "";
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(cbFilter.Text == "Driver ID" || cbFilter.Text == "Person ID")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            }

            if (cbFilter.Text == "National No.")
            {
                if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            }

            if (cbFilter.Text == "Full Name")
            {
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                    e.Handled = true;
            }
        }

        private void showPersonInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPersonDetails frm = new frmPersonDetails(_GetPersonID());
            frm.ShowDialog();
            _LoadData();
        }

        private void issueInternationalLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddNewInternationalLicenseApplication frm = new frmAddNewInternationalLicenseApplication();
            frm.ShowDialog();
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPersonLicenseHistory frm = new frmPersonLicenseHistory(_GetPersonID());
            frm.ShowDialog();
            _LoadData();
        }
    }
}