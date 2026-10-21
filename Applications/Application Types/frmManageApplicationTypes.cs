using DVLD_Business;
using System;
using System.Windows.Forms;

namespace DVLD
{
    public partial class frmManageApplicationTypes : Form
    {
        public frmManageApplicationTypes()
        {
            InitializeComponent();
        }

        private void _LoadData()
        {
            dgvApplicationTypes.DataSource = clsApplicationType.GetAllApplicationTypes();
            lblRecords.Text = dgvApplicationTypes.Rows.Count.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvApplicationTypes.CurrentRow == null || dgvApplicationTypes.CurrentRow.Index < 0)
                return;

            int ApplicationTypeID = Convert.ToInt32(dgvApplicationTypes.CurrentRow.Cells[0].Value);
            frmUpdateApplicationType frm = new frmUpdateApplicationType(ApplicationTypeID);
            frm.ShowDialog();
            frmManageApplicationTypes_Load(null, null);
        }

        private void frmManageApplicationTypes_Load(object sender, EventArgs e)
        {
            _LoadData();
        }
    }
}
