using System;
using System.Collections.Generic;
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
        public ActionResult Index()
        {
            ViewBag.TreeView = "order";
            ViewBag.TreeViewMenu = "orderList";
            ViewBag.Title = "Danh sách đơn hàng";

            OrderBusiness orderBusiness = new OrderBusiness();
            List<OrderViewModel> orderViewModelList = orderBusiness.GetSortedOrderViewModelList();

            // Bug if orderViewModelList is null return error
            ViewBag.Title = "Danh sách đơn hàng";
            return View(orderViewModelList);
        }

        // GET: Detail
        public ActionResult Detail(int id)
        {
            OrderBusiness orderBusiness = new OrderBusiness();
            OrderViewModel orderViewModel = orderBusiness.GetOrderViewModel(id);
            if (orderViewModel != null)
            {
                ViewBag.TreeView = "order";
                ViewBag.TaxRate = orderBusiness.GetVatRateAtTime(orderViewModel.Order.CreateTime);
                if (!orderViewModel.IsEnoughMaterial)
                {
                    ViewBag.ShortageOfMaterial = true;
                }
                return View(orderViewModel);
            }
            // Bug return error page when orderViewModel is null
            return RedirectToAction("Index");
        }



        // GET: Edit
        public ActionResult Edit(int id)
        {
            ViewBag.Title = "Chỉnh sửa đơn hàng";

            Order order = db.Orders.FirstOrDefault(m => m.OrderId == id);
            if (order != null)
            {
                order.CustomerEditingFlag = true;
                db.SaveChanges();
            }
            OrderBusiness orderBusiness = new OrderBusiness();
            OrderViewModel orderViewModel = orderBusiness.GetOrderViewModel(id);
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
                OrderBusiness orderBusiness = new OrderBusiness();
                rs = orderBusiness.UpdateOrder(cartList, orderId, depositAmount, deliveryDate);
                if (rs)
                {
                    return RedirectToAction("Index");
                }

            }
            // Error page Bug Do not have error page
            return RedirectToAction("Index");
        }

        [HttpPost]
        public int ApproveOrder(int deposit, DateTime deliveryDate, int orderId)
        {
            OrderBusiness orderBusiness = new OrderBusiness();
            bool rs = orderBusiness.ApproveOrder(orderId, deposit, deliveryDate);
            return rs ? 1 : 0;
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
            if (cartList.Count != 0)
            {
                foreach (CartViewModel cartViewModel in cartList)
                {
                    if (cartViewModel.ProductId == productId)
                    {
                        cartViewModel.Quantity = quantity;
                        cartViewModel.RealPrice = price;
                    }
                }
            }
            Session["Cart"] = cartList;
            return RedirectToAction("Add");
        }

        [HttpPost]
        public ActionResult RemoveProductFromOrder(int productId)
        {
            List<CartViewModel> cartList = GetCart();
            if (cartList.Count != 0)
            {
                foreach (CartViewModel cartViewModel in cartList)
                {
                    if (cartViewModel.ProductId == productId)
                    {
                        cartViewModel.Quantity = 0;
                    }
                }
            }
            Session["Cart"] = cartList;
            return RedirectToAction("Add");
        }

        [HttpPost]
        public int CheckCart()
        {
            List<CartViewModel> cartList = GetCart();
            foreach (CartViewModel cart in cartList)
            {
                if (cart.Quantity > 0)
                {
                    return 1;
                }
            }
            return 0;
        }

        public ActionResult AddCustomerToOrder()
        {
            return View("AddCustomerToOrder");
        }

        [HttpPost]
        public ActionResult CheckoutWithCustomer(int customerId)
        {
            List<CartViewModel> inputCartList = GetCart();
            OrderBusiness orderBusiness = new OrderBusiness();
            OrderViewModel order = orderBusiness.MakeOrderViewModel(inputCartList, customerId);
            return View(order);
        }

        [HttpPost]
        public int AddOrderForCustomer(int deposit, DateTime deliveryDate, int customerId)
        {
            int rs = 0;
            List<CartViewModel> cart = Session["Cart"] as List<CartViewModel>;
            if (customerId == 0)
            {
                CustomerViewModel customer = Session["NewCustomer"] as CustomerViewModel;
                OrderBusiness orderBusiness = new OrderBusiness();
                bool check = orderBusiness.AddOrderForNewCustomer(cart, customer, deposit, deliveryDate);
                if (check)
                {
                    rs = 1;
                }
            }
            else
            {
                OrderBusiness orderBusiness = new OrderBusiness();
                bool check = orderBusiness.AddOrderForCustomer(cart, customerId, deposit, deliveryDate);
                if (check)
                {
                    rs = 1;
                }
            }
            if (rs == 1)
            {
                Session["Cart"] = null;
            }
            return rs;
        }

        public ActionResult CancelAddOrder()
        {
            Session["Cart"] = null;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CheckoutWithNewCustomer(FormCollection form)
        {
            List<CartViewModel> inputCartList = GetCart();

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
                Session["NewCustomer"] = customer;
                OrderBusiness orderBusiness = new OrderBusiness();
                OrderViewModel order = orderBusiness.MakeOrderViewForNewCustomerModel(inputCartList, customer);
                return View("CheckoutWithCustomer", order);
            }
            return RedirectToAction("AddCustomerToOrder");
        }
        [HttpPost]
        public ActionResult Cancel(int orderId, int isReturnDeposit, int returnDeposit, string url)
        {
            OrderBusiness orderBusiness = new OrderBusiness();
            bool rs = orderBusiness.Cancel(orderId, returnDeposit, isReturnDeposit);
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
            OrderBusiness orderBusiness = new OrderBusiness();
            Order order = orderBusiness.GetOrder(orderId);
            // Get tax rate
            TaxRate taxRate = db.TaxRates.FirstOrDefault(m => m.EndDate >= order.CreateTime && m.BeginDate <= order.CreateTime);
            if (taxRate != null)
            {
                ViewBag.TaxRate = taxRate.TaxRateValue;
            }
            // Bug check order is null or not.
            return View(order);
        }
    }
}