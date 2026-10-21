using DVLD_Business;
using System;
using System.Data;
using System.Windows.Forms;

namespace DVLD
{
    public partial class frmManageUsers : Form
    {
        public frmManageUsers()
        {
            InitializeComponent();
        }

        private DataTable _Users;

        private void _LoadData()
        {
            _Users = clsUser.GetAllUsers();
            dgvUsers.DataSource = _Users;
            lblRecords.Text = dgvUsers.Rows.Count.ToString();
            cbFilter.SelectedIndex = 0;
        }

        private void btnAddNewUser_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser frm = new frmAddUpdateUser();
            frm.ShowDialog();
            _LoadData();
        }

        private void frmManageUsers_Load(object sender, EventArgs e)
        {
            _LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _ResetData()
        {
            txtFilterValue.Text = "";
            txtFilterValue.Focus();
            _Users.DefaultView.RowFilter = "";
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilter.Text != "None" && cbFilter.Text != "Is Active?");
            cbIsActive.Visible = (cbFilter.Text == "Is Active?");

            if(cbFilter.Text == "Is Active?")
            {
                cbIsActive.SelectedIndex = 0;
                cbIsActive.Focus();
            }

            _ResetData();
        }

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilter.Text == "User ID" || cbFilter.Text == "Person ID")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

            if (cbFilter.Text == "Username")
            {
                if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

            if (cbFilter.Text == "Full Name")
            {
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)Keys.Space)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
            {
                _Users.DefaultView.RowFilter = "";
                dgvUsers.DataSource = _Users;
                lblRecords.Text = dgvUsers.Rows.Count.ToString();
                return;
            }

            string FilterColumn = "";

            switch (cbFilter.SelectedIndex)
            {
                case 1: FilterColumn = "[User ID]"; break;
                case 2: FilterColumn = "Username"; break;
                case 3: FilterColumn = "[Person ID]"; break;
                case 4: FilterColumn = "[Full Name]"; break;
            }

            if (FilterColumn == "[User ID]" || FilterColumn == "[Person ID]")
            {
                if (int.TryParse(txtFilterValue.Text.Trim(), out int ID))
                {
                    _Users.DefaultView.RowFilter = $"{FilterColumn} = {ID}";
                }
            }
            else if(FilterColumn == "Username" || FilterColumn == "[Full Name]")
                _Users.DefaultView.RowFilter = $"{FilterColumn} LIKE '{txtFilterValue.Text.Trim()}%'";

            dgvUsers.DataSource = _Users;
            lblRecords.Text = dgvUsers.Rows.Count.ToString();
        }

        private void cbIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbIsActive.SelectedIndex)
            {
                case 0: _Users.DefaultView.RowFilter = ""; break;
                case 1: _Users.DefaultView.RowFilter = "[Is Active?] = 1"; break;
                case 2: _Users.DefaultView.RowFilter = "[Is Active?] = 0"; break;
            }

            dgvUsers.DataSource = _Users;
            lblRecords.Text = dgvUsers.Rows.Count.ToString();
        }

        private void addNewPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddNewUser_Click(sender, e);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null || dgvUsers.CurrentRow.Index < 0)
                return;

            frmAddUpdateUser frm = new frmAddUpdateUser((int)dgvUsers.CurrentRow.Cells["User ID"].Value);
            frm.ShowDialog();
            _LoadData();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null || dgvUsers.CurrentRow.Index < 0)
                return;

            frmUserInfo frm = new frmUserInfo((int)dgvUsers.CurrentRow.Cells["User ID"].Value);
            frm.ShowDialog();
            _LoadData();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null || dgvUsers.CurrentRow.Index < 0)
                return;

            if (MessageBox.Show("Are you sure you want to delete this user?", "Delete User",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (clsUser.DeleteUser((int)dgvUsers.CurrentRow.Cells["User ID"].Value))
                    MessageBox.Show("User deleted successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);

                else
                    MessageBox.Show("Can't delete this user because there is data linked to it.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _LoadData();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null || dgvUsers.CurrentRow.Index < 0)
                return;

            frmChangePassword frm = new frmChangePassword((int)dgvUsers.CurrentRow.Cells["User ID"].Value);
            frm.ShowDialog();
            _LoadData();
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet.", "Soon", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet.", "Soon", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void dgvUsers_DoubleClick(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null || dgvUsers.CurrentRow.Index < 0)
                return;

            frmUserInfo frm = new frmUserInfo((int)dgvUsers.CurrentRow.Cells["User ID"].Value);
            frm.ShowDialog();
            _LoadData();
        }
    }
}
