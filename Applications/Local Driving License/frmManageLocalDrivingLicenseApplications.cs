using DVLD_Business;
using System;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;

namespace DVLD
{
    public partial class frmManageLocalDrivingLicenseApplications : Form
    {
        public frmManageLocalDrivingLicenseApplications()
        {
            InitializeComponent();
        }

        private DataTable _LocalDrivingLicenseApplications;
        private clsLocalDrivingLicenseApplication _LocalDrivingLicenseApplicationInfo;

        private void _LoadData()
        {
            _LocalDrivingLicenseApplications = clsLocalDrivingLicenseApplication.GetAllLocalDrivingLicenseApplications();
            dgvLocalDrivingLicenseApplications.DataSource = _LocalDrivingLicenseApplications;
            lblRecords.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
            cbFilter.SelectedIndex = 0;
        }

        private void _ResetContextMenuItemsToDefault()
        {
            showApplicationDetailsToolStripMenuItem.Enabled = true;
            editApplicationToolStripMenuItem.Enabled = true;
            deleteApplicationToolStripMenuItem.Enabled = true;
            cancelApplicationToolStripMenuItem.Enabled = true;
            scheduleTestsToolStripMenuItem.Enabled = true;
            issueDrivingLicenseToolStripMenuItem.Enabled = true;
            showLicenseToolStripMenuItem.Enabled = true;
            showPersonLicenseHistoryToolStripMenuItem.Enabled = true;
        }

        private int _GetSelectedLocalDrivingLicenseApplicationID() 
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow == null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0)
                return -1;

            return Convert.ToInt32(dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value);
        }

        private clsLocalDrivingLicenseApplication _GetLocalDrivingLicenseApplicationInfo()
        {
            int ID = _GetSelectedLocalDrivingLicenseApplicationID();

            return (ID == -1) ? null : clsLocalDrivingLicenseApplication.FindByLocalDrivingLicenseApplicationID(ID);
        }

        private void btnAddNewLDLApplication_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicenseApplication frm = new frmAddUpdateLocalDrivingLicenseApplication();
            frm.ShowDialog();
            _LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmManageLDLApplications_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilter.SelectedIndex != 0);
            txtFilterValue.Text = "";
            txtFilterValue.Focus();

            // التأكد إن الداتا تيبل تم تحميلها الأول
            if (_LocalDrivingLicenseApplications != null)
            {
                _LocalDrivingLicenseApplications.DefaultView.RowFilter = "";
            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            // الحماية من خيار None أو النص الفاضي
            if (cbFilter.SelectedIndex == 0 || string.IsNullOrWhiteSpace(txtFilterValue.Text) || _LocalDrivingLicenseApplications == null)
            {
                if (_LocalDrivingLicenseApplications != null)
                    _LocalDrivingLicenseApplications.DefaultView.RowFilter = "";

                dgvLocalDrivingLicenseApplications.DataSource = _LocalDrivingLicenseApplications;
                lblRecords.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
                return;
            }

            string FilterColumn = "";

            switch (cbFilter.SelectedIndex)
            {
                case 1: FilterColumn = "[L.D.L. App. ID]"; break;
                case 2: FilterColumn = "[National No.]"; break;
                case 3: FilterColumn = "[Full Name]"; break;
                case 4: FilterColumn = "Status"; break;
            }

            // تنظيف النص وتفادي مشكلة الـ Single Quote
            string FilterValue = txtFilterValue.Text.Trim().Replace("'", "''");

            if (FilterColumn == "[L.D.L. App. ID]")
            {
                //الحماية من الحروف التي تم أخذها Copy ثم الضغط على Save داخل الفلتر
                if (int.TryParse(FilterValue, out int ID))
                    _LocalDrivingLicenseApplications.DefaultView.RowFilter = $"{FilterColumn} = {ID}";
                else
                    _LocalDrivingLicenseApplications.DefaultView.RowFilter = $"{FilterColumn} = -1";
            }
            else
                _LocalDrivingLicenseApplications.DefaultView.RowFilter = $"{FilterColumn} LIKE '{FilterValue}%'";

            dgvLocalDrivingLicenseApplications.DataSource = _LocalDrivingLicenseApplications;
            lblRecords.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(cbFilter.SelectedIndex == 1)
            {
                if(!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            }

            else if(cbFilter.SelectedIndex == 2)
            {
                if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            }

            else
            {
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                    e.Handled = true;
            }
        }

        private void cancelApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _LocalDrivingLicenseApplicationInfo = _GetLocalDrivingLicenseApplicationInfo();

            if (_LocalDrivingLicenseApplicationInfo != null)
            {
                if (MessageBox.Show("Are you sure you want to cancel this application?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_LocalDrivingLicenseApplicationInfo.Cancel())
                        MessageBox.Show("Application cancelled successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Failed to cancel application.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            _LoadData();
        }

        private void deleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _LocalDrivingLicenseApplicationInfo = _GetLocalDrivingLicenseApplicationInfo();

            if (_LocalDrivingLicenseApplicationInfo != null)
            {
                if (MessageBox.Show("Are you sure you want to delete this application?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_LocalDrivingLicenseApplicationInfo.Delete())
                        MessageBox.Show("Application deleted successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Can't delete this application because there are data linked to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            _LoadData();
        }

        private void editApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicenseApplication frm =
                new frmAddUpdateLocalDrivingLicenseApplication(_GetSelectedLocalDrivingLicenseApplicationID());
            frm.ShowDialog();
            _LoadData();
        }

        private void showApplicationDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmLocalDrivingLicenseApplicationDetails frm = new frmLocalDrivingLicenseApplicationDetails(_GetSelectedLocalDrivingLicenseApplicationID());
            frm.ShowDialog();
            _LoadData();
        }

        private void cmsManageLocalDrivingLicenseApplications_Opening(object sender, CancelEventArgs e)
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow == null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0)
            {
                e.Cancel = true;
                return;
            }

            _ResetContextMenuItemsToDefault();

            _LocalDrivingLicenseApplicationInfo = _GetLocalDrivingLicenseApplicationInfo();

            if(_LocalDrivingLicenseApplicationInfo == null)
            {
                e.Cancel = true;
                return;
            }

            int TotalPassedTests = Convert.ToInt32(dgvLocalDrivingLicenseApplications.CurrentRow.Cells["Passed Tests"].Value);
  
            scheduleTestsToolStripMenuItem.Enabled = (TotalPassedTests >= 0 && TotalPassedTests < 3)
                && (_LocalDrivingLicenseApplicationInfo.ApplicationStatus == clsLocalDrivingLicenseApplication.enApplicationStatus.New)
                && (!_LocalDrivingLicenseApplicationInfo.IsLicenseIssued());

            if (scheduleTestsToolStripMenuItem.Enabled)
            {
                scheduleVisionTestToolStripMenuItem.Enabled = (TotalPassedTests == 0);
                scheduleWrittenTestToolStripMenuItem.Enabled = (TotalPassedTests == 1);
                scheduleStreetTestToolStripMenuItem.Enabled = (TotalPassedTests == 2);
            }
            
            issueDrivingLicenseToolStripMenuItem.Enabled = (TotalPassedTests == 3)
                && (!_LocalDrivingLicenseApplicationInfo.IsLicenseIssued())
                && (_LocalDrivingLicenseApplicationInfo.ApplicationStatus == clsLocalDrivingLicenseApplication.enApplicationStatus.New);

            showLicenseToolStripMenuItem.Enabled = (_LocalDrivingLicenseApplicationInfo.IsLicenseIssued());

            deleteApplicationToolStripMenuItem.Enabled = (_LocalDrivingLicenseApplicationInfo.ApplicationStatus == clsLocalDrivingLicenseApplication.enApplicationStatus.New);
            editApplicationToolStripMenuItem.Enabled = deleteApplicationToolStripMenuItem.Enabled;
            cancelApplicationToolStripMenuItem.Enabled = deleteApplicationToolStripMenuItem.Enabled;
        }

        private void dgvLocalDrivingLicenseApplications_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // نتأكد إنه داس كليك يمين وإنه داس على صف حقيقي (مش الـ Header مثلاً)
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                // بنخلي الخلية الحالية هي أول خلية في الصف اللي اتداس عليه عشان نحدث الـ CurrentRow
                dgvLocalDrivingLicenseApplications.CurrentCell = dgvLocalDrivingLicenseApplications.Rows[e.RowIndex].Cells[0];
            }
        }

        private void scheduleVisionTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTestAppointments frm = new frmTestAppointments(_GetSelectedLocalDrivingLicenseApplicationID(), clsTestType.enTestType.VisionTest);
            frm.ShowDialog();
            _LoadData();
        }

        private void scheduleWrittenTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTestAppointments frm = new frmTestAppointments(_GetSelectedLocalDrivingLicenseApplicationID(), clsTestType.enTestType.WrittenTest);
            frm.ShowDialog();
            _LoadData();
        }

        private void scheduleStreetTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTestAppointments frm = new frmTestAppointments(_GetSelectedLocalDrivingLicenseApplicationID(), clsTestType.enTestType.StreetTest);
            frm.ShowDialog();
            _LoadData();
        }

        private void issueDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmIssueDrivingLicenseForFirstTime frm = new frmIssueDrivingLicenseForFirstTime(_GetSelectedLocalDrivingLicenseApplicationID());
            frm.ShowDialog();
            _LoadData();
        }

        private void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LicenseID = _LocalDrivingLicenseApplicationInfo.GetActiveLicenseID();

            if(LicenseID == -1)
            {
                MessageBox.Show($"License with ID = {LicenseID} is not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            frmLicenseInfo frm = new frmLicenseInfo(LicenseID);
            frm.ShowDialog();
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPersonLicenseHistory frm = new frmPersonLicenseHistory(_LocalDrivingLicenseApplicationInfo.ApplicantPersonID);
            frm.ShowDialog();
        }
    }
}