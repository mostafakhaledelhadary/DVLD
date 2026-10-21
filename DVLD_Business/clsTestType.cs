using System;
using System.Data;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsTestType
    {
        private enum enMode {AddNew = 1, Update = 2}
        private enMode _Mode = enMode.AddNew;

        public enum enTestType {VisionTest = 1, WrittenTest = 2, StreetTest = 3}

        public enTestType ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public float Fees { get; set; }

        public clsTestType()
        {
            ID = enTestType.VisionTest;
            Title = "";
            Description = "";
            Fees = 0;
            _Mode = enMode.AddNew;
        }

        private clsTestType(enTestType ID, string Title, string Description, float Fees)
        {
            this.ID = ID;
            this.Title = Title;
            this.Description = Description;
            this.Fees = Fees;
            _Mode = enMode.Update;
        }

        public static clsTestType Find(enTestType ID)
        {
            string Title = "", Description = "";
            float Fees = 0;

            if (clsTestTypeData.GetTestTypeInfoByID((int)ID, ref Title, ref Description, ref Fees))
                return new clsTestType(ID, Title, Description, Fees);
            else
                return null;
        }

        public static DataTable GetAllTestTypes()
        {
            return clsTestTypeData.GetAllTestTypes();
        }

        private bool _AddNewTestType()
        {
            ID = (enTestType)clsTestTypeData.AddNewTestType(Title, Description, Fees);

            return ((int)ID != -1);
        }

        private bool _UpdateTestType()
        {
            return clsTestTypeData.UpdateTestType((int)ID, Title, Description, Fees);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    {
                        if (_AddNewTestType())
                        {
                            _Mode = enMode.Update;
                            return true;
                        }
                        else
                            return false;
                    }

                case enMode.Update:
                    return _UpdateTestType();

                default:
                    return false;
            }
        }
    }
}
