using System;
using System.Collections.Generic;
using System.Linq;
using BMA.Models;

namespace BMA.Business
{
    public class SupplierBusiness
    {
        private static BMAEntities db = new BMAEntities();

        #region Get Supplier List

        public static List<Supplier> GetSupplierList()
        {
            List<Supplier> supplierList = db.Suppliers.OrderBy(n => n.IsActive).ThenByDescending(n=>n.SupplierId).ToList();
            return supplierList;
        }
        #endregion

        #region View supplier detail
        public Supplier GetSupplier(int id)
        {
            Supplier supplierDetail = db.Suppliers.SingleOrDefault(n => n.SupplierId == id);
            return supplierDetail;
        }
        #endregion

        #region Change supplier status
        public static bool ChangeSupplierStatus(int id)
        {
            var supplierDetail = db.Suppliers.SingleOrDefault(n => n.SupplierId == id);
            if (supplierDetail != null)
            {
                if (supplierDetail.IsActive)
                {
                    supplierDetail.IsActive = false;

                }
                else
                {
                    supplierDetail.IsActive = true;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    return false;
                }
                return true;
            }

            else
            {
                return false;
            }
        }
        #endregion

        #region Edit supplier
        public static bool EditSupplier(int supplierId, String supplierName, String supplierAddress, String supplierPhoneNumber, String supplierEmail, String supplierTaxCode)
        {
            var supplierDetail = db.Suppliers.SingleOrDefault(n => n.SupplierId == supplierId);
            if (supplierDetail != null)
            {
                try
                {
                    supplierDetail.SupplierId = supplierId;
                    supplierDetail.SupplierName = supplierName;
                    supplierDetail.SupplierAddress = supplierAddress;
                    supplierDetail.SupplierPhoneNumber = supplierPhoneNumber;
                    supplierDetail.SupplierEmail = supplierEmail;
                    supplierDetail.SupplierTaxCode = supplierTaxCode;

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                    return false;
                }
                return true;
            }

            else
            {
                return false;
            }
        }
        #endregion

        #region Add new supplier
        public static bool AddSupplier(Supplier supplier)
        {
            if (supplier == null)
            {
                return false;
            }
            try
            {
                db.Suppliers.Add(supplier);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                string s = e.ToString();
                return false;
            }
            return true;
        }
        #endregion

        #region Check Supplier Infomation
        public int CheckSupplierInfo(string supplierName, string supplierAddress, string supplierPhoneNumber, string supplierEmail, string supplierTaxCode)
        {
            Supplier checkName = db.Suppliers.FirstOrDefault(m => m.SupplierName.Equals(supplierName.Trim()));
            if (checkName != null)
            {
                return 1;
            }
            Supplier checkAddress = db.Suppliers.FirstOrDefault(m => m.SupplierAddress.Equals(supplierAddress.Trim()));
            if (checkAddress != null)
            {
                return 2;
            }
            Supplier checkPhoneNumber = db.Suppliers.FirstOrDefault(m => m.SupplierPhoneNumber.Equals(supplierPhoneNumber.Trim()));
            if (checkPhoneNumber != null)
            {
                return 3;
            }
            Supplier checkEmail = db.Suppliers.FirstOrDefault(m => m.SupplierEmail.Equals(supplierEmail.Trim()));
            if (checkEmail != null)
            {
                return 4;
            }
            Supplier checkTaxCode = db.Suppliers.FirstOrDefault(m => m.SupplierTaxCode.Equals(supplierTaxCode.Trim()));
            if (checkTaxCode != null)
            {
                return 5;
            }
            return 0;
        }

        #endregion

        #region Check Supplier Infomation When Editting
        public int CheckSupplierInfoInEdit(string supplierName, string supplierAddress, string supplierPhoneNumber, string supplierEmail, string supplierTaxCode, int id)
        {
            Supplier checkName = db.Suppliers.FirstOrDefault(m => m.SupplierName.Equals(supplierName.Trim()) && m.SupplierId != id);
            if (checkName != null)
            {
                return 1;
            }
            Supplier checkAddress = db.Suppliers.FirstOrDefault(m => m.SupplierAddress.Equals(supplierAddress.Trim()) && m.SupplierId != id);
            if (checkAddress != null)
            {
                return 2;
            }
            Supplier checkPhoneNumber = db.Suppliers.FirstOrDefault(m => m.SupplierPhoneNumber.Equals(supplierPhoneNumber.Trim()) && m.SupplierId != id);
            if (checkPhoneNumber != null)
            {
                return 3;
            }
            Supplier checkEmail = db.Suppliers.FirstOrDefault(m => m.SupplierEmail.Equals(supplierEmail.Trim()) && m.SupplierId != id);
            if (checkEmail != null)
            {
                return 4;
            }
            Supplier checkTaxCode = db.Suppliers.FirstOrDefault(m => m.SupplierTaxCode.Equals(supplierTaxCode.Trim()) && m.SupplierId != id);
            if (checkTaxCode != null)
            {
                return 5;
            }
            return 0;
        }

        #endregion

    }
}