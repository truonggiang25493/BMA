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
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.TreeView = "supplier";
                ViewBag.TreeViewMenu = "listSupplier";
                var supplierslList = SupplierBusiness.GetSupplierList();
                if (supplierslList == null)
                {
                    RedirectToAction("SupplierIndex", "Supplier");
                }
                return View(supplierslList);
            }
        }

        #endregion

        #region Get Supplier Detail
        public ActionResult SupplierDetail(int id)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.TreeView = "supplier";
                Supplier supplierDetail = supplierBusiness.GetSupplier(id);
                if (supplierDetail == null)
                {
                    RedirectToAction("SupplierIndex", "Supplier");

                }
                return View(supplierDetail);
            }
        }

        #endregion

        #region Change Supplier Status
        [HttpPost]
        public int ChangeSupplierStatus(int id)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return -7;
            }
            else
            {
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
        }
        #endregion

        #region Get input bill by supplier

        public ActionResult GetInputBillBySupplierTable(int id)
        {

            List<InputBill> inputBillBySupplierList = db.InputBills.Where(n => n.SupplierId == id).ToList();
            return PartialView("InputBillBySupplier", inputBillBySupplierList);
        }


        #endregion

        #region Edit Supplier View

        public ActionResult EditSupplier(int id)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int) Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    ViewBag.TreeView = "supplier";
                    Supplier supplier = db.Suppliers.SingleOrDefault(m => m.SupplierId == id);
                    if (supplier == null)
                    {
                        return RedirectToAction("SupplierIndex", "Supplier");
                    }
                    else
                    {
                        return View(supplier);
                    }
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "StoreInfor");
                }
            }
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
                  supplierTaxCode.IsEmpty() || supplierIdString.IsEmpty()))
            {
                int supplierId = Convert.ToInt32(supplierIdString);

                bool result = SupplierBusiness.EditSupplier(supplierId, supplierName, supplierAddress,
                    supplierPhoneNumber, supplierEmail, supplierTaxCode);
                return result ? 1 : 0;
            }
            return 0;

        }

        #endregion

        #region Add New Supplier View

        public ActionResult AddSupplier()
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.TreeView = "supplier";
                ViewBag.TreeViewMenu = "addSupplier";
                return View("AddSupplier");
            }
        }

        #endregion

        #region Add New Supplier
        [HttpPost]
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

        #region Check supplier information is exsited in database

        [HttpPost]
        public int CheckSupplierInfo(FormCollection form)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return -7;
            }
            else
            {
                string supplierName = form["txtSupplierName"];
                string supplierAddress = form["txtSupplierAddress"];
                string supplierPhoneNumber = form["txtSupplierPhoneNumber"];
                string supplierEmail = form["txtSupplierEmail"];
                string supplierTaxCode = form["txtSupplierTaxCode"];

                return supplierBusiness.CheckSupplierInfo(supplierName, supplierAddress, supplierPhoneNumber,
                    supplierEmail, supplierTaxCode);
            }
        }

        #endregion

        #region Check supplier information is exsited in database when edit

        [HttpPost]
        public int CheckSupplierInfoInEdit(FormCollection form)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return -7;
            }
            else
            {
                SupplierBusiness supplierBusiness = new SupplierBusiness();
                string supplierName = form["txtSupplierName"];
                string supplierAddress = form["txtSupplierAddress"];
                string supplierPhoneNumber = form["txtSupplierPhoneNumber"];
                string supplierEmail = form["txtSupplierEmail"];
                string supplierTaxCode = form["txtSupplierTaxCode"];
                int supplierId = Convert.ToInt32(form["SupplierId"]);

                return supplierBusiness.CheckSupplierInfoInEdit(supplierName, supplierAddress, supplierPhoneNumber,
                    supplierEmail, supplierTaxCode, supplierId);
            }
        }

        #endregion


    }
}