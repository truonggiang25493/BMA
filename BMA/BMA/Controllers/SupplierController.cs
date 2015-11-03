using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.WebPages;
using BMA.Business;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class SupplierController : Controller
    {
        private BMAEntities db = new BMAEntities();
        private SupplierBusiness supplierBusiness = new SupplierBusiness();

        #region Get supplier list
        public ActionResult SupplierIndex()
        {
            var supplierslList = SupplierBusiness.GetSupplierList();
            if (supplierslList == null)
            {
                RedirectToAction("SupplierIndex", "Supplier");
            }
            return View(supplierslList);
        }
        #endregion

        #region Get Input Material Detail
        public ActionResult SupplierDetail(int id)
        {
            Supplier supplierDetail = supplierBusiness.GetSupplier(id);
            if (supplierDetail == null)
            {
                RedirectToAction("SupplierIndex", "Supplier");

            }
            return View(supplierDetail);

        }

        #endregion

        #region Change Supplier Status
        [HttpPost]
        public int ChangeSupplierStatus(int id)
        {
            SupplierBusiness supplierBusiness = new SupplierBusiness();
            Boolean result = SupplierBusiness.ChangeSupplierStatus(id);
            if (result)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region Edit Supplier View
        public ActionResult EditSupplier(int id)
        {
            Supplier supplier = db.Suppliers.SingleOrDefault(m => m.SupplierId == id);
            var productMaterial = db.ProductMaterials.ToList();
            ViewBag.productMaterial = productMaterial;
            return View(supplier);
        }
        #endregion    
        
        #region Edit Supplier
        [HttpPost]
        public int EditSupplier(FormCollection f)
        {
            String supplierName = f["txtSupplierName"];
            String supplierAddress = f["txtSupplierAddress"];
            String supplierPhoneNumber = f["txtSupplierPhoneNumber"];
            String supplierEmail = f["txtSupplierEmail"];
            String supplierTaxCode = f["txtSupplierTaxCode"];
            String supplierIdString = f["SupplierId"];
            if (
                !(supplierName.IsEmpty() || supplierAddress.IsEmpty() ||
                  supplierPhoneNumber.IsEmpty() || supplierEmail.IsEmpty() ||
                  supplierTaxCode.IsEmpty()||supplierIdString.IsEmpty()))
            {
                int supplierId = Convert.ToInt32(supplierIdString);

                bool result = SupplierBusiness.EditSupplier(supplierId,supplierName,supplierAddress,supplierPhoneNumber,supplierEmail,supplierTaxCode);
                return result ? 1 : 0;
            }
            return 0;
        }
        #endregion

        #region Add New Supplier View
        public ActionResult AddSupplier()
        {
            return View("AddSupplier");
        }
        #endregion
        [HttpPost]
        #region Add New Supplier
        public int AddSupplier(FormCollection f)
        {

            String supplierName = f["txtSupplierName"];
            String supplierAddress = f["txtSupplierAddress"];
            String supplierPhoneNumber = f["txtSupplierPhoneNumber"];
            String supplierEmail = f["txtSupplierEmail"];
            String supplierTaxCode = f["txtSupplierTaxCode"];
            Supplier supplier = new Supplier();
            try
            {
                supplier.SupplierName = supplierName;
                supplier.SupplierAddress = supplierAddress;
                supplier.SupplierPhoneNumber = supplierPhoneNumber;
                supplier.SupplierEmail = supplierEmail;
                supplier.SupplierTaxCode = supplierTaxCode;
                supplier.IsActive = true;
            }
            catch (Exception)
            {
                return 0;

            }

            bool result = SupplierBusiness.AddSupplier(supplier);
            if (result)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }
        #endregion
    }
}