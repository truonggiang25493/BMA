using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using BMA.Business;
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
            List<Order> orderList = db.Orders.Where(m => !m.IsStaffEdit).ToList();
            // Custom sort
            orderList.Sort(
                delegate(Order o1, Order o2)
                {
                    if (o1.OrderStatus != o2.OrderStatus)
                    {
                        return o1.OrderStatus.CompareTo(o2.OrderStatus);
                    }
                    return o1.CreateTime.CompareTo(o2.CreateTime);

                });
            ViewBag.Title = "Danh sách đơn hàng";
            return View(orderList);
        }

        // GET: Detail
        public ActionResult Detail(int id)
        {
            OrderViewModel orderViewModel = OrderBusiness.GetOrderViewModel(id);
            if (orderViewModel != null)
            {
                return View(orderViewModel);
            }
            return RedirectToAction("Index");
        }



        // GET: Edit
        public ActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa đơn hàng";
            OrderViewModel orderViewModel = OrderBusiness.GetOrderViewModel(id);
            TaxRate taxRate = db.TaxRates.FirstOrDefault(m => m.TaxTypeId == 1 && m.EndDate == null);
            if (taxRate != null)
            {
                orderViewModel.TaxRate = taxRate.TaxRateValue;
            }
            if (orderViewModel != null)
            {
                if (orderViewModel.Order.OrderStatus == 0 && !orderViewModel.Order.IsStaffEdit)
                {
                    InitiateProductList(orderViewModel.Order.OrderId);
                    return View(orderViewModel);
                }

            }
            // Return error and redirect to url string Bug
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            string orderIdString = form["orderId"];
            string[] productIdString = Regex.Split(form["productId"], ",");
            string[] priceString = Regex.Split(form["productPrice"], ",");
            string[] quantityString = Regex.Split(form["productQuantity"], ",");
            string depositAmountString = form["depositAmount"];
            string deliveryDateString = form["deliveryDate"];

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
                int orderId = Convert.ToInt32(orderIdString);
                int depositAmount = Convert.ToInt32(depositAmountString);
                DateTime deliveryDate = Convert.ToDateTime(deliveryDateString);
                bool rs = true;
                rs = OrderBusiness.UpdateOrder(cartList, orderId, depositAmount, deliveryDate);
                if (rs)
                {
                    return RedirectToAction("Index");
                }

            }
            // Error page Bug Do not have error page
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ApproveOrder(int deposit, DateTime deliveryDate, int orderId)
        {
            bool rs = OrderBusiness.ApproveOrder(orderId, deposit, deliveryDate);
            if (!rs)
            {
                return RedirectToAction("Detail", orderId);
            }
            return RedirectToAction("Index");
        }

        private List<CartViewModel> GetCart()
        {
            List<CartViewModel> cartList = Session["Cart"] as List<CartViewModel>;
            if (cartList == null)
            {
                cartList = new List<CartViewModel>();
                Session["Cart"] = cartList;
            }
            return cartList;
        }
        public ActionResult Add()
        {
            List<CartViewModel> inputList = GetCart();
            if (inputList.Count == 0)
            {
                List<Product> productList = db.Products.Where(m => m.IsActive).ToList();
                foreach (Product product in productList)
                {
                    CartViewModel cart = new CartViewModel();
                    cart.ProductId = product.ProductId;
                    cart.ProductName = product.ProductName;
                    cart.Quantity = 0;
                    cart.StandardPrice = product.ProductStandardPrice;
                    inputList.Add(cart);
                }
            }
            List<CartViewModel> cartList = inputList.OrderByDescending(m => m.Quantity).ToList();
            Session["Cart"] = cartList;
            return View(cartList);
        }

        [HttpPost]
        public ActionResult AddProductToOrder(int productId, int price, int quantity)
        {
            List<CartViewModel> cartList = GetCart();
            bool check = false;
            if (cartList.Count != 0)
            {
                foreach (CartViewModel cartViewModel in cartList)
                {
                    if (cartViewModel.ProductId == productId)
                    {
                        cartViewModel.Quantity = quantity;
                        cartViewModel.RealPrice = price;
                        check = true;
                    }
                }
            }
            ViewBag.IsAdded = check;
            Session["Cart"] = cartList;
            return RedirectToAction("Add");
        }

        public ActionResult AddCustomerToOrder()
        {
            return View("AddCustomerToOrder");
        }

        [HttpPost]
        public ActionResult CheckoutWithCustomer(int customerId)
        {
            List<CartViewModel> inputCartList = GetCart();
            OrderViewModel order = OrderBusiness.MakeOrderViewModel(inputCartList, customerId);
            return View(order);
        }

        [HttpPost]
        public ActionResult AddOrderForCustomer(int deposit, DateTime deliveryDate, int customerId)
        {
            bool rs = false;
            List<CartViewModel> cart = Session["Cart"] as List<CartViewModel>;
            if (customerId.Equals("0"))
            {
                User customer = Session["NewCustomer"] as User;
                rs = OrderBusiness.AddOrderForNewCustomer(cart, customer, deposit, deliveryDate);
            }
            else
            {
                rs = OrderBusiness.AddOrderForCustomer(cart, customerId, deposit, deliveryDate);
            }
            if (rs)
            {
                Session["Cart"] = null;
                return RedirectToAction("Index");

            }
            return RedirectToAction("CheckoutWithCustomer", customerId);
        }

        public ActionResult CancelAddOrder()
        {
            Session["Cart"] = null;
            ViewBag.IsAdded = null;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CreateCustomerForOrder(FormCollection form)
        {
            string customerNameString = form["customerName"];
            string customerEmailString = form["customerEmail"];
            string customerAddressString = form["customerAddress"];
            string customerPhoneNumberString = form["customerPhoneNumber"];
            string customerTaxCodeString = form["customerTaxCode"];
            string usernameString = form["username"];
            if (
                !(customerNameString.IsEmpty() || customerAddressString.IsEmpty() || customerPhoneNumberString.IsEmpty() ||
                  customerTaxCodeString.IsEmpty() || customerEmailString.IsEmpty()))
            {
                Customer customer = new Customer();
                customer.CustomerAddress = customerAddressString;
                customer.CustomerPhoneNumber = customerPhoneNumberString;
                customer.TaxCode = customerTaxCodeString;

                User aspNetUser = new User();
                aspNetUser.Username = usernameString;
                aspNetUser.Email = customerEmailString;
                aspNetUser.Fullname = customerNameString;
                // Password Bug

                aspNetUser.Customers.Add(customer);
                Session["NewCustomer"] = aspNetUser;
                return RedirectToAction("CheckoutWithNewCustomer");

            }
            return RedirectToAction("AddCustomerToOrder");
        }

        public ActionResult CheckoutWithNewCustomer()
        {
            List<CartViewModel> inputCartList = GetCart();
            var customerUser = Session["NewCustomer"];
            OrderViewModel order = OrderBusiness.MakeOrderViewForNewCustomerModel(inputCartList, (User)customerUser);
            return View("CheckoutWithCustomer", order);
        }
        [HttpPost]
        public ActionResult Cancel(int orderId, int isReturnDeposit, int returnDeposit, string url)
        {
            bool rs = OrderBusiness.Cancel(orderId, returnDeposit, isReturnDeposit);
            // Call cancel order from OrderBusiness

            if (rs)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // Error message

                // Ridirect to url
                if (url == null || url.IsEmpty())
                {
                    url = "Index";
                }
                return RedirectToAction(url);
            }
        }
        private List<Product> InitiateProductList(int orderId)
        {
            List<Product> productList = db.Products.Where(m => m.IsActive).ToList();
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
            return resultList;
        }
        [HttpPost]
        public ActionResult GetListOfProductToAdd(int orderId)
        {
            List<Product> productList = Session["ProductList"] as List<Product>;
            return PartialView(productList);
        }

        [HttpPost]
        public int RemoveProductInProductList(int productId)
        {
            List<Product> productList = Session["ProductList"] as List<Product>;
            List<Product> resultList = new List<Product>();
            if (productList != null && productList.Count > 0)
            {
                resultList.AddRange(productList.Where(product => product.ProductId != productId));
                Session["ProductList"] = resultList;
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
                MaterialInfoForProductListViewModel material = OrderBusiness.GetMaterialListForOrder(cartList);
                material.OrderId = Convert.ToInt32(orderIdString);
                return PartialView("MaterialListForOrder", material);
            }
            return PartialView("MaterialListForOrder", null);
        }
    }
}