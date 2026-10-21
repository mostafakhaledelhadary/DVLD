using DVLD.Properties;
using DVLD_Business;
using System;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace DVLD
{
    public partial class frmManagePeople : Form
    {
        public frmManagePeople()
        {
            InitializeComponent();
        }

        private DataTable _People;

        private void _RefreshData()
        {
            _People = clsPerson.GetAllPeople();
            dgvPeople.DataSource = _People;
            lblRecords.Text = dgvPeople.Rows.Count.ToString();
            cbFilter.SelectedIndex = 0;
        }

        private void _ResetData()
        {
            txtFilterValue.Text = "";
            txtFilterValue.Focus();
            _People.DefaultView.RowFilter = "";
        }

        private void frmManagePeople_Load(object sender, EventArgs e)
        {
            _RefreshData();
        }

        private void btnAddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.ShowDialog();
            _RefreshData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPeople.CurrentRow == null || dgvPeople.CurrentRow.Index < 0)
                return;

            int PersonID = (int)dgvPeople.CurrentRow.Cells["PersonID"].Value;
            frmPersonDetails frm = new frmPersonDetails(PersonID);
            frm.ShowDialog();
            _RefreshData();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPeople.CurrentRow == null || dgvPeople.CurrentRow.Index < 0)
                return;

            int PersonID = (int)dgvPeople.CurrentRow.Cells["PersonID"].Value;
            frmAddUpdatePerson frm = new frmAddUpdatePerson(PersonID);
            frm.ShowDialog();
            _RefreshData();
        }

        private void addNewPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddNewPerson_Click(sender, e);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPeople.CurrentRow == null || dgvPeople.CurrentRow.Index < 0)
                return;

            if (MessageBox.Show("Are you sure you want to delete this person?", "Delete person",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                int PersonID = (int)dgvPeople.CurrentRow.Cells["PersonID"].Value;
                string ImagePathToDeleteFromFile = dgvPeople.CurrentRow.Cells["ImagePath"].Value?.ToString();

                if (clsPerson.DeletePerson(PersonID))
                {
                    if (!string.IsNullOrEmpty(ImagePathToDeleteFromFile) && File.Exists(ImagePathToDeleteFromFile))
                    {
                        try
                        {
                            File.Delete(ImagePathToDeleteFromFile);
                        }

                        catch
                        {

                        }
                    }

                    MessageBox.Show("Person deleted successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshData();
                }
                else
                    MessageBox.Show("Person can't be deleted because there is data linked to it.",
                        "Deletion failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not implemented yet.", "Not ready", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not implemented yet.", "Not ready", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilterValue.Text.Trim()))
            {
                _People.DefaultView.RowFilter = "";
                dgvPeople.DataSource = _People;
                lblRecords.Text = dgvPeople.Rows.Count.ToString();
                return;
            }


            string FilterColumn = "";

            switch (cbFilter.SelectedIndex)
            {
                case 1: FilterColumn = "PersonID"; break;
                case 2: FilterColumn = "NationalNo"; break;
                case 3: FilterColumn = "FirstName"; break;
                case 4: FilterColumn = "SecondName"; break;
                case 5: FilterColumn = "ThirdName"; break;
                case 6: FilterColumn = "LastName"; break;
                case 7: FilterColumn = "Nationality"; break;
                case 8: FilterColumn = "Gender"; break;
                case 9: FilterColumn = "Phone"; break;
                case 10: FilterColumn = "Email"; break;
            }
            
            if (FilterColumn == "PersonID")
            {
                if (int.TryParse(txtFilterValue.Text.Trim(), out int personID))
                {
                    _People.DefaultView.RowFilter = $"{FilterColumn} = {personID}";
                }
            }
            else
                _People.DefaultView.RowFilter = $"{FilterColumn} LIKE '{txtFilterValue.Text.Trim()}%'";

            dgvPeople.DataSource = _People;
            lblRecords.Text = dgvPeople.Rows.Count.ToString();
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilter.Text != "None");
            _ResetData();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilter.Text == "Person ID" || cbFilter.Text == "Phone")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

            if (cbFilter.Text == "National No.")
            {
                if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

            if (cbFilter.Text == "First Name" || cbFilter.Text == "Second Name" || cbFilter.Text == "Third Name" ||
                cbFilter.Text == "Last Name" || cbFilter.Text == "Nationality" || cbFilter.Text == "Gender")
            {
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)Keys.Space)
                {
                    e.Handled = true;
                }
            }
        }

        private void dgvPeople_DoubleClick(object sender, EventArgs e)
        {
            if (dgvPeople.CurrentRow == null || dgvPeople.CurrentRow.Index < 0)
                return;

            int PersonID = (int)dgvPeople.CurrentRow.Cells["PersonID"].Value;
            frmPersonDetails frm = new frmPersonDetails(PersonID);
            frm.ShowDialog();
            _RefreshData();
        }
    }
}