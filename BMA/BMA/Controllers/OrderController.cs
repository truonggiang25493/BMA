using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class OrderController : Controller
    {
        private BMAEntities db = new BMAEntities();
        // GET: Order
        public ActionResult Index()
        {
            List<Order> orderList = db.Orders.ToList();
            // Custom sort
            orderList.Sort(
                delegate(Order o1, Order o2)
                {
                    if (o1.OrderStatus == o2.OrderStatus)
                    {
                        return o1.CreateTime.CompareTo(o2.CreateTime);
                    }
                    return o1.OrderStatus.CompareTo(o2.OrderStatus);
                });
            return View(orderList);
        }

        // GET: Detail
        public ActionResult Detail(int id)
        {
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == id);
            if (order != null)
            {
                // Get and transfer total amount and total tax of order
                int totalAmount = 0;
                int totalTax = 0;
                foreach (var orderItem in order.OrderItems)
                {
                    totalAmount += orderItem.Amount;
                    totalTax += orderItem.TaxAmount;
                }
                ViewBag.TotalAmount = totalAmount;
                ViewBag.TotalTax = totalTax;
                //  if customer is not null, get and transfer
                //  else get and transfer guest info
                if (order.CustomerUserId != null)
                {
                    Customer customer = db.Customers.FirstOrDefault(m => m.UserId == order.CustomerUserId);
                    ViewBag.Customer = customer;
                }
                else
                {
                    GuestInfo guestInfo = db.GuestInfoes.FirstOrDefault(m => m.GuestInfoId == order.GuestInfoId);
                    ViewBag.GuestInfo = guestInfo;
                }
                ViewBag.IsEnough = 1;
                // Get Material list in order
                // Create ProductMaterial List
                List<ProductMaterial> productMaterials = new List<ProductMaterial>();
                // Scan all of Order Item List to calc the amount of all material
                foreach (var orderItem in order.OrderItems)
                {
                    foreach (Recipe recipe in orderItem.Product.Recipes)
                    {
                        if (productMaterials.Count != 0)
                        {
                            bool check = false;
                            foreach (ProductMaterial productMaterial in productMaterials)
                            {
                                if (productMaterial.ProductMaterialId == recipe.ProductMaterialId)
                                {
                                    check = true;
                                    productMaterial.CurrentQuantity += recipe.RecipeQuantity * orderItem.Quantity;
                                }
                            }
                            if (!check)
                            {
                                ProductMaterial productMaterial = new ProductMaterial();
                                productMaterial.ProductMaterialId = recipe.ProductMaterialId;
                                productMaterial.CurrentQuantity = recipe.RecipeQuantity * orderItem.Quantity;
                                productMaterials.Add(productMaterial);
                            }
                        }
                        else
                        {
                            ProductMaterial productMaterial = new ProductMaterial();
                            productMaterial.ProductMaterialId = recipe.ProductMaterialId;
                            productMaterial.CurrentQuantity = recipe.RecipeQuantity * orderItem.Quantity;
                            productMaterials.Add(productMaterial);
                        }
                    }
                }
                List<MaterialInOrderViewModel> materialInOrderViewModels =
                    new List<MaterialInOrderViewModel>();
                // Compare all product material with current quantity of product material in Db
                if (productMaterials.Count != 0)
                {
                    foreach (ProductMaterial productMaterial in productMaterials)
                    {
                        ProductMaterial productMaterialInDb =
                            db.ProductMaterials.FirstOrDefault(
                                m => m.ProductMaterialId == productMaterial.ProductMaterialId);
                        if (productMaterialInDb != null)
                        {
                            MaterialInOrderViewModel materialListInOrderViewModel =
                                new MaterialInOrderViewModel();
                            materialListInOrderViewModel.ProductMaterialName = productMaterialInDb.ProductMaterialName;
                            materialListInOrderViewModel.StorageQuantity = productMaterial.CurrentQuantity;
                            if (productMaterialInDb.CurrentQuantity <= productMaterial.CurrentQuantity)
                            {
                                materialListInOrderViewModel.IsEnough = false;
                                ViewBag.IsEnough = 0;
                            }
                            else
                            {
                                materialListInOrderViewModel.IsEnough = true;
                            }
                            materialInOrderViewModels.Add(materialListInOrderViewModel);
                        }

                    }

                }
                if (materialInOrderViewModels.Count != 0)
                {
                    ViewBag.MaterialList = materialInOrderViewModels;
                }
            }

            return View(order);
        }

        // GET: Delete
        public ActionResult Delete(int id)
        {
            return View();
        }

        // GET: Edit
        public ActionResult Edit(int id)
        {
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == id);
            if (order != null)
            {
                // Get and transfer total amount and total tax of order
                int totalAmount = 0;
                int totalTax = 0;
                foreach (var orderItem in order.OrderItems)
                {
                    totalAmount += orderItem.Amount;
                    totalTax += orderItem.TaxAmount;
                }
                ViewBag.TotalAmount = totalAmount;
                ViewBag.TotalTax = totalTax;
                //  if customer is not null, get and transfer
                //  else get and transfer guest info
                if (order.CustomerUserId != null)
                {
                    Customer customer = db.Customers.FirstOrDefault(m => m.UserId == order.CustomerUserId);
                    ViewBag.Customer = customer;
                }
                else
                {
                    GuestInfo guestInfo = db.GuestInfoes.FirstOrDefault(m => m.GuestInfoId == order.GuestInfoId);
                    ViewBag.GuestInfo = guestInfo;
                }
            }


            return View(order);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            return RedirectToAction("Index", "Order");
        }


    }
}