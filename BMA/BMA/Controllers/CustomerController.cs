﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BMA.Business;
using BMA.Models;

namespace BMA.Controllers
{
    public class CustomerController : Controller
    {
        private BMAEntities db = new BMAEntities();
        private CustomerBusiness customerBusiness = new CustomerBusiness();
        // GET: Customer

        public ActionResult GetCustomerPartialView(int? customerId)
        {
            try
            {
                if (customerId != null)
                {
                    ViewBag.CustomerId = customerId;
                }
                List<Customer> customerList = customerBusiness.GetCustomerList();
                return PartialView(customerList);
            }
            catch (Exception)
            {
                return null;
            }

        }

        [HttpPost]
        public ActionResult Create(FormCollection form, string returnUrl)
        {
            return RedirectToAction(returnUrl);
        }

        [HttpPost]
        public int AddCustomerForOrder(FormCollection form)
        {
            try
            {
                string customerName = form["customerName"];
                string orderIdString = form["orderId"];
                string username = form["username"];
                string email = form["customerEmail"];
                string customerAddress = form["customerAddress"];
                string customerPhoneNumber = form["customerPhoneNumber"];
                string customerTaxCode = form["customerTaxCode"];
                if (
                    !(customerName.IsEmpty() || orderIdString.IsEmpty() || username.IsEmpty() || email.IsEmpty() ||
                      customerAddress.IsEmpty() || customerPhoneNumber.IsEmpty() || customerTaxCode.IsEmpty()))
                {
                    int orderId = Convert.ToInt32(orderIdString);
                    bool rs = customerBusiness.AddCustomerForOrder(username, email, customerName, customerAddress,
                        customerPhoneNumber, customerTaxCode, orderId);
                    return rs ? 1 : 0;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        #region Get Customer Index

        public ActionResult CustomerIndex()
        {
            try
            {
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] == 3)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    try
                    {
                        ViewBag.TreeView = "customerIndex";

                        var stafflList = CustomerBusiness.GetCustomerIndex();
                        if (stafflList == null)
                        {
                            RedirectToAction("CustomerIndex", "Customer");
                        }
                        return View(stafflList);
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("ManageError", "Error");
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }
            
        }

        #endregion

        #region Get customer detail
        public ActionResult CustomerDetail(int id)
        {
            try
            {
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] == 3)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    try
                    {
                        ViewBag.TreeView = "customerIndex";
                        Customer customerDetail = customerBusiness.GetCustomerDetail(id);
                        if (customerDetail == null)
                        {
                            return RedirectToAction("CustomerIndex", "Customer");

                        }
                        return View(customerDetail);
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("ManageError", "Error");
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }
            
        }

        #endregion

        #region Change Customer Status
        [HttpPost]
        public int ChangeCustomerStatus(int id)
        {
            try
            {
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
                {
                    return -7;
                }
                else
                {
                    Boolean result = CustomerBusiness.ChangeCustomerStatus(id);
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
            catch (Exception)
            {
                return 0;
            }
            
        }
        #endregion

        #region Change Customer Loyal

        [HttpPost]
        public int ChangeCustomerLoyal(int id)
        {
            try
            {
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
                {
                    return -7;
                }
                else
                {
                    Boolean result = CustomerBusiness.ChangeCustomerLoyal(id);
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
            catch (Exception)
            {
                return 0;
            }
           
        }

        #endregion

        #region Get order by customer

        public ActionResult GetOrderByCustomerTable(int id)
        {
            try
            {
                List<Order> orderByCustomerList = db.Orders.Where(n => n.CustomerUserId == id).OrderByDescending(n => n.DeliveryTime).ToList();
                return PartialView("OrderedByCustomerPartialView", orderByCustomerList);
            }
            catch (Exception)
            {

                return null;
            }
           
        }

        #endregion
    }
}