using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BMA.Models;
using BMA.Models.ViewModel;
using Microsoft.Owin;

namespace BMA.Business
{
    public class SupplierBusiness
    {
        private static BMAEntities db = new BMAEntities();

        #region Get Supplier List

        public static List<Supplier> GetSupplierList()
        {
            List<Supplier> supplierList = db.Suppliers.OrderBy(n => n.IsActive).ToList();
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
        public static bool EditSupplier(int supplierId,String supplierName, String supplierAddress, String supplierPhoneNumber, String supplierEmail, String supplierTaxCode)
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

    }
}