using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.WebPages;
using BMA.Business;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class OrderController : Controller
    {
        private readonly BMAEntities db = new BMAEntities();


        // GET: Order
        public ActionResult Index(string fromDate, string toDate, string customerName, int? orderStatus)
        {
            try
            {
                // Check autherization
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
                {
                    return RedirectToAction("Index", "Home");
                }
                Session["Cart"] = null;
                ViewBag.TreeView = "order";
                ViewBag.TreeViewMenu = "orderList";
                ViewBag.Title = "Danh sách đơn hàng";

                OrderBusiness orderBusiness = new OrderBusiness();
                List<OrderViewModel> orderViewModelList;
                if (fromDate != null && !fromDate.Trim().IsEmpty() && toDate != null && !toDate.Trim().IsEmpty())
                {
                    DateTime FromDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime ToDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    orderViewModelList = orderBusiness.GetSortedOrderViewModelList(FromDate, ToDate, customerName, orderStatus);
                }
                else
                {
                    orderViewModelList = orderBusiness.GetSortedOrderViewModelList(null, null, customerName, orderStatus);
                }
                
                if (orderViewModelList == null)
                {
                    return RedirectToAction("ManageError", "Error");
                }
                ViewBag.Title = "Danh sách đơn hàng";
                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                ViewBag.CustomerName = customerName;
                ViewBag.OrderStatus = orderStatus;
                return View(orderViewModelList);
               
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }

        }

        // GET: Detail
        public ActionResult Detail(int id)
        {
            try
            {
                // Check autherization
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
                {
                    return RedirectToAction("Index", "Home");
                }
                OrderBusiness orderBusiness = new OrderBusiness();
                OrderViewModel orderViewModel = orderBusiness.GetOrderViewModel(id);
                if (orderViewModel == null)
                {
                    return RedirectToAction("ManageError", "Error");
                }
                ViewBag.TreeView = "order";
                ViewBag.TaxRate = orderBusiness.GetVatRateAtTime(orderViewModel.Order.CreateTime);
                if (!orderViewModel.IsEnoughMaterial)
                {
                    ViewBag.ShortageOfMaterial = true;
                }
                return View(orderViewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }
        }



        // GET: Edit
        public ActionResult Edit(int id)
        {
            try
            {
                // Check autherization
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
                {
                    return RedirectToAction("Index", "Home");
                }

                OrderBusiness orderBusiness = new OrderBusiness();
                int minQuantity = orderBusiness.GetMinQuantity();
                ViewBag.MinQuantity = minQuantity;

                List<DiscountByQuantity> discountByQuantityList = orderBusiness.GetDiscountByQuantityList();
                ViewBag.DiscountByQuantityList = discountByQuantityList;
                OrderViewModel orderViewModel = orderBusiness.GetOrderViewModel(id);
                TaxRate taxRate = db.TaxRates.FirstOrDefault(m => m.TaxTypeId == 1 && m.EndDate >= DateTime.Now && m.BeginDate <= DateTime.Now);
                if (taxRate != null)
                {
                    orderViewModel.TaxRate = taxRate.TaxRateValue;
                }
                if (orderViewModel == null)
                {
                    return RedirectToAction("ManageError", "Error");
                }
                if (orderViewModel.Order.OrderStatus == 0 && !orderViewModel.Order.CustomerEditingFlag)
                {
                    if (!orderViewModel.IsEnoughMaterial)
                    {
                        ViewBag.ShortageOfMaterial = true;
                    }
                    ViewBag.Title = "Chỉnh sửa đơn hàng";
                    ViewBag.TreeView = "order";
                    InitiateProductList(orderViewModel.Order.OrderId);
                    return View(orderViewModel);
                }
                return RedirectToAction("ManageError", "Error");
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }
        }

        [HttpPost]
        public int Edit(FormCollection form)
        {
            // Check autherization
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return -7;
            }
            else
            {
                string orderIdString = form["orderId"];
                string[] productIdString = Regex.Split(form["productId"], ",");
                string[] priceString = Regex.Split(form["productPrice"], ",");
                string[] quantityString = Regex.Split(form["productQuantity"], ",");
                string depositAmountString = form["depositAmount"];
                string deliveryDateString = form["deliveryDate"];

                List<CartViewModel> cartList = new List<CartViewModel>();
                int totalQuantity = 0;
                if (productIdString.Length == priceString.Length && priceString.Length == quantityString.Length)
                {
                    for (int i = 0; i < productIdString.Length; i++)
                    {
                        CartViewModel cartViewModel = new CartViewModel();
                        cartViewModel.ProductId = Convert.ToInt32(productIdString[i]);
                        cartViewModel.RealPrice = Convert.ToInt32(priceString[i]);
                        int quantity = Convert.ToInt32(quantityString[i]);
                        cartViewModel.Quantity = quantity;
                        totalQuantity += quantity;
                        cartList.Add(cartViewModel);
                    }
                }
                if (totalQuantity == 0)
                {
                    return -1;
                }
                OrderBusiness orderBusiness = new OrderBusiness(); ;
                int minQuantity = orderBusiness.GetMinQuantity();
                if (totalQuantity < minQuantity)
                {
                    ViewBag.MinQuantity = minQuantity;
                    return -2;
                }
                int orderId = Convert.ToInt32(orderIdString);
                int depositAmount = Convert.ToInt32(depositAmountString);
                DateTime deliveryDate = DateTime.ParseExact(deliveryDateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                bool rs = true;
                rs = orderBusiness.UpdateOrder(cartList, orderId, depositAmount, deliveryDate, staffUser.UserId);
                if (rs)
                {
                    Session["ProductList"] = null;
                    return 1;
                }

                return 0;
            }



        }

        [HttpPost]
        public int ApproveOrder(FormCollection form)
        {
            //Check autherization
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return -7;
            }
            else
            {
                String customerName = form["customerName"];

                if (customerName == null)
                {

                    int orderId = Convert.ToInt32(form["orderId"]);
                    int deposit = Convert.ToInt32(form["deposit"].Replace(".", ""));
                    string deliveryDateString = form["deliveryDate"];
                    DateTime deliveryDate = DateTime.ParseExact(deliveryDateString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                    OrderBusiness orderBusiness = new OrderBusiness();

                    return orderBusiness.ApproveOrder(orderId, deposit, deliveryDate, staffUser.UserId, null);
                }
                else
                {
                    string orderIdString = form["orderId"];
                    string username = form["username"];
                    string email = form["customerEmail"];
                    string customerAddress = form["customerAddress"];
                    string customerPhoneNumber = form["customerPhoneNumber"];
                    string customerTaxCode = form["customerTaxCode"];
                    if (!(customerName.IsEmpty() || orderIdString.IsEmpty() || username.IsEmpty() || email.IsEmpty() ||
                          customerAddress.IsEmpty() || customerPhoneNumber.IsEmpty() || customerTaxCode.IsEmpty()))
                    {
                        OrderBusiness orderBusiness = new OrderBusiness();
                        int check = orderBusiness.CheckCustomerField(email, customerAddress, customerPhoneNumber,
                            customerTaxCode, username);
                        if(check != 0)
                        {
                            return check;
                        }

                        int orderId = Convert.ToInt32(orderIdString);
                        int deposit = Convert.ToInt32(form["deposit"].Replace(".", ""));

                        CustomerViewModel customer = new CustomerViewModel
                        {
                            Username = username,
                            CustomerEmail = email,
                            CustomerAddress = customerAddress,
                            CustomerPhoneNumber = customerPhoneNumber,
                            CustomerName = customerName,
                            CustomerTaxCode = customerTaxCode
                        };


                        DateTime deliveryDate = DateTime.ParseExact(form["deliveryDate"], "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
                        
                        int rs = orderBusiness.ApproveOrder(orderId, deposit, deliveryDate, staffUser.UserId, customer);
                        return rs;
                    }
                    return 0;
                }
            }
        }

        [HttpPost]
        public int AddToCart(int[] productId, int[] productQuantity, int[] productPrice)
        {
            List<CartViewModel> cartList = new List<CartViewModel>();
            if (productId != null && productQuantity != null && productPrice != null && productId.Length > 0 &&
                productQuantity.Length > 0 && productPrice.Length > 0 && productId.Length == productQuantity.Length && productQuantity.Length == productPrice.Length)
            {
                for (int i = 0; i < productId.Length; i++)
                {
                    OrderBusiness orderBusiness = new OrderBusiness();
                    Product product = orderBusiness.GetProductById(productId[i]);
                    if (productQuantity[i] > 0)
                    {
                        CartViewModel cartViewModel = new CartViewModel
                        {
                            ProductId = productId[i],
                            ProductName = product.ProductName,
                            Quantity = productQuantity[i],
                            RealPrice = productPrice[i],
                            StandardPrice = product.ProductStandardPrice
                        };
                        cartList.Add(cartViewModel);
                    }
                }
            }
            if (cartList.Count > 0)
            {
                Session["Cart"] = cartList;
                return 1;
            }
            return 0;
        }


        public ActionResult Add()
        {
            try
            {
                // Check autherization
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
                {
                    return RedirectToAction("Index", "Home");
                }

                OrderBusiness orderBusiness = new OrderBusiness();
                InitiateProductList(null);
                ViewBag.MinQuantity = orderBusiness.GetMinQuantity();
                ViewBag.TreeView = "order";
                ViewBag.TreeViewMenu = "addOrder";
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }
        }

        public ActionResult AddCustomerToOrder()
        {
            try
            {
                // Check autherization
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
                {
                    Session["Cart"] = null;
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.TreeView = "order";
                ViewBag.TreeViewMenu = "addOrder";
                return View("AddCustomerToOrder");
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }


        }

        [HttpPost]
        public ActionResult CheckoutWithCustomer(int customerId)
        {
            try
            {
                // Check autherization
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
                {
                    Session["Cart"] = null;
                    return RedirectToAction("Index", "Home");
                }

                List<CartViewModel> inputCartList = Session["Cart"] as List<CartViewModel>;
                OrderBusiness orderBusiness = new OrderBusiness();
                OrderViewModel order = orderBusiness.MakeOrderViewModel(inputCartList, customerId, null);
                if (!order.IsEnoughMaterial)
                {
                    ViewBag.ShortageOfMaterial = true;
                }
                ViewBag.TaxRate = orderBusiness.GetVatRateAtTime(DateTime.Now);
                Session["CustomerId"] = customerId;
                ViewBag.TreeView = "order";
                ViewBag.TreeViewMenu = "addOrder";
                return View(order);
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }

        }

        [HttpPost]
        public int AddOrderForCustomer(FormCollection form)
        {
            int deposit = Convert.ToInt32(form["deposit"].Replace(".", ""));
            string deliveryDate = form["deliveryDate"];
            int customerId = Convert.ToInt32(form["customerId"]);

            //Check autherization
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return -7;
            }
            else
            {
                int rs = 0;
                List<CartViewModel> cart = Session["Cart"] as List<CartViewModel>;

                DateTime deliveryDate1 = DateTime.ParseExact(deliveryDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                if (customerId == 0)
                {
                    CustomerViewModel customer = Session["NewCustomer"] as CustomerViewModel;
                    OrderBusiness orderBusiness = new OrderBusiness();
                    bool check = orderBusiness.AddOrder(cart, null, customer, staffUser.UserId, deposit, deliveryDate1);
                    if (check)
                    {
                        rs = 1;
                    }
                }
                else
                {
                    OrderBusiness orderBusiness = new OrderBusiness();
                    bool check = orderBusiness.AddOrder(cart, customerId, null, staffUser.UserId, deposit, deliveryDate1);
                    if (check)
                    {
                        rs = 1;
                    }
                }
                if (rs == 1)
                {
                    Session["CustomerId"] = null;
                    Session["NewCustomer"] = null;
                    Session["Cart"] = null;
                }
                return rs;
            }
        }

        public ActionResult CancelAddOrder()
        {
            // Check autherization
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                Session["Cart"] = null;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Session["Cart"] = null;
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult CheckoutWithNewCustomer(FormCollection form)
        {
            try
            {
                // Check autherization
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
                {
                    Session["Cart"] = null;
                    return RedirectToAction("Index", "Home");
                }

                List<CartViewModel> inputCartList = Session["Cart"] as List<CartViewModel>;

                string customerNameString = form["customerName"];
                string customerEmailString = form["customerEmail"];
                string customerAddressString = form["customerAddress"];
                string customerPhoneNumberString = form["customerPhoneNumber"];
                string customerTaxCodeString = form["customerTaxCode"];
                string usernameString = form["username"];
                if (
                    !(customerNameString.IsEmpty() || customerAddressString.IsEmpty() || customerPhoneNumberString.IsEmpty() ||
                      customerTaxCodeString.IsEmpty() || customerEmailString.IsEmpty() || usernameString.IsEmpty()))
                {
                    CustomerViewModel customer = new CustomerViewModel();
                    customer.CustomerName = customerNameString;
                    customer.CustomerAddress = customerAddressString;
                    customer.CustomerPhoneNumber = customerPhoneNumberString;
                    customer.CustomerTaxCode = customerTaxCodeString;
                    customer.Username = usernameString;
                    customer.CustomerEmail = customerEmailString;
                    Session["CustomerId"] = 0;
                    Session["NewCustomer"] = customer;
                    OrderBusiness orderBusiness = new OrderBusiness();
                    OrderViewModel order = orderBusiness.MakeOrderViewModel(inputCartList, null, customer);
                    ViewBag.TreeView = "order";
                    ViewBag.TreeViewMenu = "addOrder";
                    return View("CheckoutWithCustomer", order);
                }
                return RedirectToAction("AddCustomerToOrder");
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }
        }

        [HttpPost]
        public int Cancel(int orderId, int isReturnDeposit, int returnDeposit, string url)
        {
            try
            {
                // Check autherization
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
                {
                    return -7;
                }

                OrderBusiness orderBusiness = new OrderBusiness();
                bool rs = orderBusiness.Cancel(orderId, returnDeposit, isReturnDeposit, staffUser.UserId);
                return rs ? 1 : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        private void InitiateProductList(int? orderId)
        {
            List<Product> productList = db.Products.Where(m => m.IsActive).ToList();
            if (orderId == null)
            {
                List<CartViewModel> cartList = Session["Cart"] as List<CartViewModel>;
                if (cartList != null)
                {
                    foreach (CartViewModel cartViewModel in cartList)
                    {
                        productList.Remove(productList.FirstOrDefault(m => m.ProductId == cartViewModel.ProductId));
                    }
                }
                Session["ProductListForAdd"] = productList;
            }
            else
            {
                List<Product> resultList = new List<Product>();
                Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
                if (productList.Count > 0 && order != null)
                {
                    foreach (Product product in productList)
                    {
                        bool check = true;
                        foreach (OrderItem orderItem in order.OrderItems)
                        {
                            if (orderItem.ProductId == product.ProductId)
                            {
                                check = false;
                            }
                        }
                        if (check)
                        {
                            resultList.Add(product);
                        }
                    }
                }
                Session["ProductList"] = resultList;
            }
        }
        [HttpPost]
        public ActionResult GetListOfProductToAdd(int? orderId)
        {
            List<Product> productList;
            if (orderId != null)
            {
                productList = Session["ProductList"] as List<Product>;
            }
            else
            {
                productList = Session["ProductListForAdd"] as List<Product>;
            }
            return PartialView(productList);
        }

        [HttpPost]
        public int RemoveProductInProductList(int[] productId)
        {
            List<Product> productList = Session["ProductList"] as List<Product>;
            if (productId.Length > 0)
            {
                foreach (int index in productId)
                {
                    if (productList != null && productList.Count > 0)
                    {
                        productList.Remove(productList.FirstOrDefault(m => m.ProductId == index));
                    }
                }
                Session["ProductList"] = productList;
                return 1;
            }
            return 0;
        }

        [HttpPost]
        public int RemoveProductInProductListForAdd(int[] productId)
        {
            List<Product> productList = Session["ProductListForAdd"] as List<Product>;
            if (productId!=null && productId.Length > 0)
            {
                foreach (int index in productId)
                {
                    if (productList != null && productList.Count > 0)
                    {
                        productList.Remove(productList.FirstOrDefault(m => m.ProductId == index));
                    }
                }
                Session["ProductListForAdd"] = productList;
                return 1;
            }
            return 0;
        }

        [HttpPost]
        public int AddProductInProductList(int productId)
        {
            List<Product> productList = Session["ProductList"] as List<Product>;
            Product product = db.Products.FirstOrDefault(m => m.ProductId == productId && m.IsActive);
            if (product != null)
            {
                if (productList != null)
                {
                    bool check = true;
                    foreach (Product product1 in productList)
                    {
                        if (product1.ProductId == productId)
                        {
                            check = false;
                        }
                    }
                    if (check)
                    {
                        productList.Add(product);
                    }
                    return 1;
                }
                return 0;
            }


            return 0;
        }

        [HttpPost]
        public int AddProductInProductListForAdd(int productId)
        {
            List<Product> productList = Session["ProductListForAdd"] as List<Product>;
            Product product = db.Products.FirstOrDefault(m => m.ProductId == productId && m.IsActive);
            if (product != null)
            {
                if (productList != null && productList.Count > 0)
                {
                    bool check = true;
                    foreach (Product product1 in productList)
                    {
                        if (product1.ProductId == productId)
                        {
                            check = false;
                        }
                    }
                    if (check)
                    {
                        productList.Add(product);
                    }
                    return 1;
                }
                else if (productList != null && productList.Count == 0)
                {
                    productList.Add(product);
                    return 1;
                }
                return 0;
            }
            return 0;
        }

        [HttpPost]
        public ActionResult UpdateMaterialList(FormCollection form)
        {
            string[] productIdString = Regex.Split(form["productId"], ",");
            string[] priceString = Regex.Split(form["productPrice"], ",");
            string[] quantityString = Regex.Split(form["productQuantity"], ",");
            string orderIdString = form["orderId"];
            List<CartViewModel> cartList = new List<CartViewModel>();
            if (productIdString.Length == priceString.Length && priceString.Length == quantityString.Length)
            {
                for (int i = 0; i < productIdString.Length; i++)
                {
                    CartViewModel cartViewModel = new CartViewModel();
                    cartViewModel.ProductId = Convert.ToInt32(productIdString[i]);
                    cartViewModel.RealPrice = Convert.ToInt32(priceString[i]);
                    cartViewModel.Quantity = Convert.ToInt32(quantityString[i]);
                    cartList.Add(cartViewModel);
                }
            }
            if (cartList.Count > 0)
            {
                OrderBusiness orderBusiness = new OrderBusiness();
                MaterialInfoForProductListViewModel material = orderBusiness.GetMaterialListForOrder(cartList);
                material.OrderId = Convert.ToInt32(orderIdString);
                return PartialView("MaterialListForOrder", material);
            }
            return PartialView("MaterialListForOrder", null);
        }

        [HttpPost]
        public ActionResult GetShortageMaterialList()
        {
            OrderBusiness orderBusiness = new OrderBusiness();
            List<ShortageMaterialViewModel> shortageMaterialList =
                orderBusiness.GetShortageMaterialList();
            return PartialView(shortageMaterialList);
        }

        [HttpPost]
        public int ChangeOrderState(int orderId)
        {
            OrderBusiness orderBusiness = new OrderBusiness();
            if (orderBusiness.ChangeOrderState(orderId))
            {
                return 1;
            }
            return 0;
        }


        public ActionResult ExportBill(int orderId)
        {
            // Check autherization
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                OrderBusiness orderBusiness = new OrderBusiness();
                Order order = orderBusiness.GetOrder(orderId);
                // Get tax rate
                TaxRate taxRate = db.TaxRates.FirstOrDefault(m => m.EndDate >= order.CreateTime && m.BeginDate <= order.CreateTime);
                if (taxRate != null)
                {
                    ViewBag.TaxRate = taxRate.TaxRateValue;
                }

                // Get Manager Name
                User user = db.Users.FirstOrDefault(m => m.Role.Name.Contains("Manager"));
                if (user != null)
                {
                    ViewBag.ManagerName = user.Fullname;
                }

                // Bug check order is null or not.
                return View(order);
            }

        }



        #region Check customer field

        [HttpPost]
        public int CheckCustomerField(FormCollection form)
        {
            string customerEmailString = form["customerEmail"];
            string customerAddressString = form["customerAddress"];
            string customerPhoneNumberString = form["customerPhoneNumber"];
            string customerTaxCodeString = form["customerTaxCode"];
            string usernameString = form["username"];

            OrderBusiness orderBusiness = new OrderBusiness();
            return orderBusiness.CheckCustomerField(customerEmailString, customerAddressString, customerPhoneNumberString, customerTaxCodeString, usernameString);
        }


        #endregion

        [HttpPost]
        public int ExportMaterial(FormCollection form)
        {
            string productMaterialIdString = form["productMaterialId"];
            string outputMaterialIdString = form["outputMaterialId"];
            string exportQuantityString = form["exportQuantity"];

            try
            {
                int productMaterialId = Convert.ToInt32(productMaterialIdString);
                int outputMaterialId = Convert.ToInt32(outputMaterialIdString);
                int exportQuantity = Convert.ToInt32(exportQuantityString);

                OrderBusiness business = new OrderBusiness();

                return business.ExportMaterial(productMaterialId, outputMaterialId, exportQuantity) ? 1 : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}