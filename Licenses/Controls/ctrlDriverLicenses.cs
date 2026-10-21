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
    public partial class ctrlDriverLicenses : UserControl
    {
        private clsDriver _DriverInfo;

        public ctrlDriverLicenses()
        {
            InitializeComponent();
        }

        private void _LoadDriverLicenses()
        {
            dgvLocalLicensesHistory.DataSource = clsLicense.GetDriverLicenses(_DriverInfo.DriverID);
            lblLocalLicensesRecords.Text = dgvLocalLicensesHistory.Rows.Count.ToString();

            dgvInternationalLicensesHistory.DataSource = clsInternationalLicense.GetDriverInternationalLicenses(_DriverInfo.DriverID);
            lblInternationalLicensesRecords.Text = dgvInternationalLicensesHistory.Rows.Count.ToString();
        }

        public void LoadDataByDriverID(int DriverID)
        {
            _DriverInfo = clsDriver.FindByDriverID(DriverID);

            if (_DriverInfo == null)
            {
                MessageBox.Show($"There is no Driver ID = {DriverID}.",
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _LoadDriverLicenses();
        }

        public void LoadDataByPersonID(int PersonID)
        {
            _DriverInfo = clsDriver.FindByPersonID(PersonID);

            if (_DriverInfo == null)
            {
                MessageBox.Show($"There is no Driver linked with Person ID = {PersonID}.",
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _LoadDriverLicenses();
        }

        private int _GetSelectedLicenseID()
        {
            if(tcPersonLicensesHistory.SelectedTab == tpInternationalLicenses)
            {
                if (dgvInternationalLicensesHistory.CurrentRow == null || dgvInternationalLicensesHistory.CurrentRow.Index < 0)
                    return -1;

                return Convert.ToInt32(dgvInternationalLicensesHistory.CurrentRow.Cells["International License ID"].Value);
            }
            else
            {
                if (dgvLocalLicensesHistory.CurrentRow == null || dgvLocalLicensesHistory.CurrentRow.Index < 0)
                    return -1;

                return Convert.ToInt32(dgvLocalLicensesHistory.CurrentRow.Cells["License ID"].Value);
            }
        }

        private void showLicenseInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LicenseID = _GetSelectedLicenseID();

            if (LicenseID == -1)
                return;

            if (tcPersonLicensesHistory.SelectedTab == tpInternationalLicenses)
            {
                frmInternationalLicenseInfo frm1 = new frmInternationalLicenseInfo(LicenseID);
                frm1.ShowDialog();
            }
            else
            {
                frmLicenseInfo frm2 = new frmLicenseInfo(LicenseID);
                frm2.ShowDialog();
            }
        }
    }
}