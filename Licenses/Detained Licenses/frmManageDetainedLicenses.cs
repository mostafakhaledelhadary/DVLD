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
    public partial class frmManageDetainedLicenses : Form
    {
        private DataTable _AllDetainedLicenses;

        public frmManageDetainedLicenses()
        {
            InitializeComponent();
        }

        private void _LoadData()
        {
            _AllDetainedLicenses = clsDetainedLicense.GetAllDetainedLicenses();
            dgvDetainedLicenses.DataSource = _AllDetainedLicenses;
            lblRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();
            cbFilter.SelectedIndex = 0;
        }

        private void frmManageDetainedLicenses_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private int _GetSelectedLicenseID()
        {
            if (dgvDetainedLicenses.CurrentRow == null || dgvDetainedLicenses.CurrentRow.Index < 0)
                return -1;

            return Convert.ToInt32(dgvDetainedLicenses.CurrentRow.Cells["License ID"].Value);
        }

        private int _GetSelectedPersonID()
        {
            if (dgvDetainedLicenses.CurrentRow == null || dgvDetainedLicenses.CurrentRow.Index < 0)
                return -1;

            clsPerson PersonInfo = clsPerson.Find(dgvDetainedLicenses.CurrentRow.Cells["National No."].Value.ToString());

            return (PersonInfo != null) ? PersonInfo.PersonID : -1;
        }

        private void _ApplyFilter()
        {
            string FilterColumn = "";

            switch (cbFilter.Text)
            {
                case "Detain ID": FilterColumn = "[Detain ID]"; break;
                case "Is Released": FilterColumn = "[Is Released?]"; break;
                case "National No.": FilterColumn = "[National No.]"; break;
                case "Full Name": FilterColumn = "[Full Name]"; break;
                case "Release Application ID": FilterColumn = "[Release Application ID]"; break;
                default: FilterColumn = "None"; break;
            }

            if(FilterColumn == "None")
            {
                _AllDetainedLicenses.DefaultView.RowFilter = "";
                lblRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();
                return;
            }

            if (FilterColumn != "[Is Released?]" && string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
            {
                _AllDetainedLicenses.DefaultView.RowFilter = "";
                lblRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();
                return;
            }

            if (FilterColumn == "[Detain ID]" || FilterColumn == "[Release Application ID]")
            {
                if (int.TryParse(txtFilterValue.Text.Trim(), out int ID))
                {
                    _AllDetainedLicenses.DefaultView.RowFilter = $"{FilterColumn} = {ID}";
                }
                else
                    _AllDetainedLicenses.DefaultView.RowFilter = $"{FilterColumn} = -1";
            }

            else if (FilterColumn == "[Is Released?]")
            {
                if (cbIsReleased.Text == "Yes")
                    _AllDetainedLicenses.DefaultView.RowFilter = $"{FilterColumn} = 1";

                else if (cbIsReleased.Text == "No")
                    _AllDetainedLicenses.DefaultView.RowFilter = $"{FilterColumn} = 0";

                else
                    _AllDetainedLicenses.DefaultView.RowFilter = "";
            }

            else
                _AllDetainedLicenses.DefaultView.RowFilter = $"{FilterColumn} LIKE '{txtFilterValue.Text.Trim()}%'";

            lblRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            _ApplyFilter();
        }

        private void cbIsReleased_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ApplyFilter();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(cbFilter.Text == "Detain ID" || cbFilter.Text == "Release Application ID")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            }

            if (cbFilter.Text == "National No.")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar))
                    e.Handled = true;
            }

            if (cbFilter.Text == "Full Name")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                    e.Handled = true;
            }
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilter.Text != "None" && cbFilter.Text != "Is Released");
            cbIsReleased.Visible = (cbFilter.Text == "Is Released");

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }

            if (cbIsReleased.Visible)
            {
                cbIsReleased.SelectedIndex = 0;
            }

            _ApplyFilter();
        }

        private void btnReleaseLicense_Click(object sender, EventArgs e)
        {
            frmReleaseDetainedLicense frm = new frmReleaseDetainedLicense();
            frm.ShowDialog();
            _LoadData();
        }

        private void btnDetainLicense_Click(object sender, EventArgs e)
        {
            frmDetainLicense frm = new frmDetainLicense();
            frm.ShowDialog();
            _LoadData();
        }

        private void cmsManageDetainedLicenses_Opening(object sender, CancelEventArgs e)
        {
            if (dgvDetainedLicenses.CurrentRow == null || dgvDetainedLicenses.CurrentRow.Index < 0)
            {
                e.Cancel = true;
                return;
            }

            releaseDetainedLicenseToolStripMenuItem.Enabled =
                !Convert.ToBoolean(dgvDetainedLicenses.CurrentRow.Cells["Is Released?"].Value);
        }

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPersonDetails frm = new frmPersonDetails(_GetSelectedPersonID());
            frm.ShowDialog();
            _LoadData();
        }

        private void showLicenseDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmLicenseInfo frm = new frmLicenseInfo(_GetSelectedLicenseID());
            frm.ShowDialog();
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPersonLicenseHistory frm = new frmPersonLicenseHistory(_GetSelectedPersonID());
            frm.ShowDialog();
            _LoadData();
        }

        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReleaseDetainedLicense frm = new frmReleaseDetainedLicense(_GetSelectedLicenseID());
            frm.ShowDialog();
            _LoadData();
        }
    }
}