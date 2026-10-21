using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD
{
    public partial class frmTakeTest : Form
    {
        private int _TestAppointmentID = -1;
        private clsTestType.enTestType _TestTypeID = clsTestType.enTestType.VisionTest;
        private clsTest _TestInfo;

        public frmTakeTest(clsTestType.enTestType TestTypeID, int TestAppointmentID)
        {
            _TestTypeID = TestTypeID;
            _TestAppointmentID = TestAppointmentID;
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmTakeTest_Load(object sender, EventArgs e)
        {
            ctrlScheduledTest1.TestTypeID = _TestTypeID;
            ctrlScheduledTest1.LoadData(_TestAppointmentID);

            int TestID = ctrlScheduledTest1.TestID;

            if (TestID != -1)
            {
                //قفل عناصر الإدخال ثم عرض معلومات الإختبار القديم
                btnSave.Enabled = false;
                rbPass.Enabled = false;
                rbFail.Enabled = false;
                txtNotes.Enabled = false;

                _TestInfo = clsTest.Find(TestID);

                if(_TestInfo != null)
                {
                    
                    if (_TestInfo.TestResult == true)
                        rbPass.Checked = true;
                    else
                        rbFail.Checked = true;

                    txtNotes.Text = _TestInfo.Notes;

                    ctrlScheduledTest1.TestID = _TestInfo.TestID;
                }
            }
            else
            {
                _TestInfo = new clsTest();
                rbPass.Checked = true;
            }
        }   

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save? After that you can not change test result!!",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            _TestInfo.Notes = txtNotes.Text.Trim();
            _TestInfo.TestResult = rbPass.Checked;
            _TestInfo.CreatedByUserID = clsGlobal.CurrentUser.UserID;
            _TestInfo.TestAppointmentID = _TestAppointmentID;

            if (_TestInfo.Save())
            {
                MessageBox.Show("Data saved successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ctrlScheduledTest1.TestID = _TestInfo.TestID;

                // قفل عناصر الواجهة بعد نجاح الحفظ
                btnSave.Enabled = false;
                rbPass.Enabled = false;
                rbFail.Enabled = false;
                txtNotes.Enabled = false;
            }
            else
                MessageBox.Show("Failed to save data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
