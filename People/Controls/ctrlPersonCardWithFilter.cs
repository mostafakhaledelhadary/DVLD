using DVLD_Business;
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace DVLD
{
    public partial class ctrlPersonCardWithFilter : UserControl
    {
        public ctrlPersonCardWithFilter()
        {
            InitializeComponent();   
        }

        public int PersonID 
        {
            get { return ctrlPersonCard1.PersonID; }
        }

        public clsPerson SelectedPersonInfo
        {
            get { return ctrlPersonCard1.SelectedPersonInfo; }
        }

        public event Action OnPersonNotFound;

        private bool _FilterEnabled = true;
        public bool FilterEnabled
        {
            get { return _FilterEnabled; }
            set
            {
                _FilterEnabled = value;
                gbFilter.Enabled = _FilterEnabled;
            }
        }

        public void LoadPersonInfo(int PersonID)
        {
            cbFilter.SelectedIndex = 1;
            txtFilterValue.Text = PersonID.ToString();
            _FindNow();
        }

        private void _FindNow()
        {
            switch (cbFilter.Text)
            {
                case "Person ID":
                    {
                        if (int.TryParse(txtFilterValue.Text, out int personID))
                            ctrlPersonCard1.LoadPersonInfo(personID);
                        break;
                    }


                case "National No.":
                    {
                        ctrlPersonCard1.LoadPersonInfo(txtFilterValue.Text);
                        break;
                    }
            }

            if (PersonID == -1)
            {
                OnPersonNotFound?.Invoke();
                FilterFocus();
            }
        }

        private void _LoadNewPersonData(int PersonID)
        {
            ctrlPersonCard1.LoadPersonInfo(PersonID);
            cbFilter.SelectedIndex = 1;
            txtFilterValue.Text = PersonID.ToString();
        }

        private void btnFindPerson_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valid!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FindNow();
        }

        public void FilterFocus()
        {
            // لو الفلتر مقفول (زي حالة الـ Update)، اخرج وما تمسحش حاجة
            if (!FilterEnabled)
                return;

            txtFilterValue.Text = "";
            errorProvider1.SetError(txtFilterValue, null);
            txtFilterValue.Focus();
        }
        
        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
                btnFindPerson.PerformClick();

            if (cbFilter.Text == "National No.")
            {
                if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

            if (cbFilter.Text == "Person ID")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }  
        }

        private void btnAddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.OnSaveComplete += _LoadNewPersonData;
            frm.ShowDialog();
        }

        private void ctrlPersonCardWithFilter_Load(object sender, EventArgs e)
        {
            cbFilter.SelectedIndex = 0;
            txtFilterValue.Focus();
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterFocus();
        }

        private void txtFilterValue_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
            {
                errorProvider1.SetError(txtFilterValue, "This field is required.");
            }
            else
                errorProvider1.SetError(txtFilterValue, null);
        }

        public void ResetPersonInfo()
        {
            ctrlPersonCard1.ResetPersonInfo();
        }
    }
}
