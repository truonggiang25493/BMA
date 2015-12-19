using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web.WebPages;
using BMA.Controllers;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Business
{
    public class OrderBusiness
    {
        private BMAEntities db;

        public OrderBusiness()
        {
            db = new BMAEntities();
        }

        public OrderViewModel GetOrderViewModel(int orderId)
        {
            OrderViewModel result = new OrderViewModel();
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
            if (order != null)
            {
                //Get Basic Info
                result.Order = order;

                // Check the order is enough material or not
                result.IsEnoughMaterial = true;
                //Get Material info
                if (order.OrderStatus == 0)
                {
                    List<MaterialViewModel> materialViewModelList = GetMaterialListForOrder(orderId);
                    if (result.Order.OrderStatus == 0)
                    {
                        foreach (MaterialViewModel materialViewModel in materialViewModelList)
                        {
                            if (!materialViewModel.IsEnough)
                            {
                                result.IsEnoughMaterial = false;
                            }
                        }
                    }

                    result.MaterialList = materialViewModelList;

                    List<InputMaterialViewModel> inputMaterialViewModelListTemp = GetInputMaterialList(result.MaterialList);
                    // Get Material Cost
                    if (inputMaterialViewModelListTemp.Count != 0)
                    {
                        result.MaterialCost = GetMaterialCost(inputMaterialViewModelListTemp);
                    }
                }


                // Get Customer or Guest info
                if (order.CustomerUserId != null)
                {
                    Customer customer = db.Customers.SingleOrDefault(m => m.UserId == order.CustomerUserId);
                    if (customer != null)
                    {
                        result.IsGuest = false;
                        result.OrderPersonName = customer.User.Fullname;
                        result.OrderPersonAddress = customer.CustomerAddress;
                        result.OrderPersonPhoneNumber = customer.CustomerPhoneNumber;
                        result.OrderPersonId = customer.CustomerId;
                        result.OrderPersonTaxCode = customer.TaxCode;
                        result.IsLoyal = customer.IsLoyal;
                    }
                }
                else
                {
                    GuestInfo guestInfo = db.GuestInfoes.FirstOrDefault(m => m.GuestInfoId == order.GuestInfoId);
                    if (guestInfo != null)
                    {
                        result.IsGuest = true;
                        result.OrderPersonName = guestInfo.GuestInfoName;
                        result.OrderPersonAddress = guestInfo.GuestInfoAddress;
                        result.OrderPersonPhoneNumber = guestInfo.GuestInfoPhone;
                        result.OrderPersonId = guestInfo.GuestInfoId;
                        result.OrderPersonTaxCode = null;
                        result.IsLoyal = false;
                    }
                }
            }
            return result;
        }
        private List<MaterialViewModel> GetMaterialListForOrder(int orderId)
        {
            List<MaterialViewModel> resultList = new List<MaterialViewModel>();
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
            if (order != null)
            {
                // Create ProductMaterial List
                // Scan all of Order Item List to calc the amount of all material and add into resultList
                foreach (var orderItem in order.OrderItems)
                {
                    List<MaterialViewModel> materialListForOrderItem = GetMaterialListForOrderItem(orderItem.Product.ProductId, orderItem.Quantity);
                    if (materialListForOrderItem != null)
                    {
                        foreach (MaterialViewModel materialViewModelForOrderItem in materialListForOrderItem)
                        {
                            if (resultList.Count == 0)
                            {
                                resultList.Add(materialViewModelForOrderItem);

                            }
                            else
                            {
                                bool check = false;
                                foreach (MaterialViewModel materialViewModel in resultList)
                                {
                                    if (materialViewModel.ProductMaterialId ==
                                        materialViewModelForOrderItem.ProductMaterialId)
                                    {
                                        materialViewModel.NeedQuantity += materialViewModelForOrderItem.NeedQuantity;
                                        check = true;
                                    }
                                }
                                if (!check)
                                {
                                    resultList.Add(materialViewModelForOrderItem);
                                }
                            }
                        }

                    }

                }
                // Check each material isEnough
                if (resultList.Count != 0)
                {
                    foreach (MaterialViewModel materialViewModel in resultList)
                    {
                        ProductMaterial productMaterial =
                            db.ProductMaterials.FirstOrDefault(
                                m => m.ProductMaterialId == materialViewModel.ProductMaterialId);
                        if (productMaterial != null)
                        {
                            materialViewModel.StorageQuantity = productMaterial.CurrentQuantity;
                            if (materialViewModel.NeedQuantity <= productMaterial.CurrentQuantity)
                            {
                                materialViewModel.IsEnough = true;
                            }
                            else
                            {
                                materialViewModel.IsEnough = false;
                            }
                        }
                    }
                }
            }
            return resultList;
        }
        /// <summary>
        /// Get Material list for order item (Product with quantity
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>List of Material View Model</returns>
        private List<MaterialViewModel> GetMaterialListForOrderItem(int productId, int quantity)
        {
            List<MaterialViewModel> resultList = new List<MaterialViewModel>();
            Product product = db.Products.FirstOrDefault(m => m.ProductId == productId);
            if (product != null)
            {
                foreach (Recipe recipe in product.Recipes)
                {
                    MaterialViewModel materialViewModel = new MaterialViewModel();
                    materialViewModel.ProductMaterialId = recipe.ProductMaterialId;
                    materialViewModel.NeedQuantity = (int)Math.Floor(recipe.RecipeQuantity * quantity);
                    materialViewModel.StorageQuantity = recipe.ProductMaterial.CurrentQuantity;
                    materialViewModel.Unit = recipe.ProductMaterial.ProductMaterialUnit;
                    materialViewModel.ProductMaterialName = recipe.ProductMaterial.ProductMaterialName;
                    resultList.Add(materialViewModel);
                }
            }
            return resultList;
        }

        public List<InputMaterialViewModel> GetInputMaterialList(List<MaterialViewModel> inputList)
        {
            List<MaterialViewModel> materialList = new List<MaterialViewModel>();
            foreach (MaterialViewModel materialViewModelInput in inputList)
            {
                MaterialViewModel materialViewModel = new MaterialViewModel
                {
                    IsEnough = materialViewModelInput.IsEnough,
                    NeedQuantity = materialViewModelInput.NeedQuantity,
                    ProductMaterialId = materialViewModelInput.ProductMaterialId,
                    ProductMaterialName = materialViewModelInput.ProductMaterialName,
                    StorageQuantity = materialViewModelInput.StorageQuantity,
                    Unit = materialViewModelInput.Unit
                };
                materialList.Add(materialViewModel);
            }

            List<InputMaterialViewModel> resultList = new List<InputMaterialViewModel>();
            if (materialList.Count != 0)
            {
                foreach (MaterialViewModel materialViewModel in materialList)
                {

                    List<InputMaterial> tempList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive && m.RemainQuantity > 0).OrderByDescending(m => m.InputMaterialExpiryDate).ToList();
                    foreach (InputMaterial inputMaterial in tempList)
                    {

                        if (materialViewModel.NeedQuantity > 0 && inputMaterial.RemainQuantity >= materialViewModel.NeedQuantity)
                        {
                            InputMaterialViewModel inputMaterialViewModel = new InputMaterialViewModel();
                            inputMaterialViewModel.InputMaterialId = inputMaterial.InputMaterialId;
                            inputMaterialViewModel.Quantity = materialViewModel.NeedQuantity;
                            inputMaterialViewModel.Price = inputMaterial.InputMaterialPrice;
                            materialViewModel.NeedQuantity = 0;
                            resultList.Add(inputMaterialViewModel);
                        }
                        else if ((materialViewModel.NeedQuantity > 0 && inputMaterial.RemainQuantity < materialViewModel.NeedQuantity))
                        {
                            InputMaterialViewModel inputMaterialViewModel = new InputMaterialViewModel();
                            inputMaterialViewModel.InputMaterialId = inputMaterial.InputMaterialId;
                            inputMaterialViewModel.Price = inputMaterial.InputMaterialPrice;
                            inputMaterialViewModel.Quantity = inputMaterial.RemainQuantity;
                            materialViewModel.NeedQuantity -= inputMaterial.RemainQuantity;
                            resultList.Add(inputMaterialViewModel);
                        }
                    }
                }
            }

            return resultList;
        }

        public int GetMaterialCost(List<InputMaterialViewModel> inputList)
        {
            int result = 0;

            foreach (InputMaterialViewModel inputMaterialViewModel in inputList)
            {
                result += (int)Math.Floor(inputMaterialViewModel.Price * inputMaterialViewModel.Quantity);
            }

            return result;
        }

        public int ApproveOrder(int orderId, int deposit, DateTime deliveryTime, int staffUserId, CustomerViewModel newCustomer)
        {
            OrderViewModel orderViewModel = GetOrderViewModel(orderId);
            if (orderViewModel.Order.CustomerEditingFlag)
            {
                return -1;
            }
            if (orderViewModel == null)
            {
                return -2;
            }
            if (orderViewModel.Order.OrderStatus != 0)
            {
                return -3;
            }
            if (!orderViewModel.IsEnoughMaterial)
            {
                return -4;
            }
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
            DateTime now = DateTime.Now;
            #region Update OutputMaterial; ExportFrom and InputMaterial
            foreach (OrderItem orderItem in orderViewModel.Order.OrderItems)
            {
                List<MaterialViewModel> materialListForOrderItem =
                    GetMaterialListForOrderItem(orderItem.Product.ProductId, orderItem.Quantity);
                foreach (MaterialViewModel materialViewModel in materialListForOrderItem)
                {
                    OutputMaterial outputMaterial = new OutputMaterial();
                    outputMaterial.ExportQuantity = materialViewModel.NeedQuantity;
                    outputMaterial.ProductMaterialId = materialViewModel.ProductMaterialId;
                    outputMaterial.ExportTime = now;
                    outputMaterial.OrderItemId = orderItem.OrderItemId;
                    //Get list of InputMaterial available order by ExpireDate descending
                    List<InputMaterial> tempList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive && m.RemainQuantity > 0).OrderByDescending(m => m.InputMaterialExpiryDate).ToList();
                    //Compare each input material with material ViewModel and merge each material of orderItem to input material
                    foreach (InputMaterial inputMaterial in tempList)
                    {
                        if (materialViewModel.NeedQuantity > 0)
                        {
                            ExportFrom exportFrom = new ExportFrom();
                            if (inputMaterial.RemainQuantity >= materialViewModel.NeedQuantity)
                            {
                                exportFrom.ExportFromQuantity = materialViewModel.NeedQuantity;
                                inputMaterial.RemainQuantity -= materialViewModel.NeedQuantity;
                                materialViewModel.NeedQuantity = 0;

                            }
                            else
                            {
                                materialViewModel.NeedQuantity -= inputMaterial.RemainQuantity;
                                exportFrom.ExportFromQuantity = inputMaterial.RemainQuantity;
                                inputMaterial.RemainQuantity = 0;
                            }
                            InputBill inputBill = inputMaterial.InputBill;
                            // Get info for ExportFrom
                            exportFrom.InputBill = inputBill;
                            // Add input bill to output material
                            outputMaterial.ExportFroms.Add(exportFrom);
                        }
                    }
                    if (materialViewModel.NeedQuantity > 0)
                    {
                        contextTransaction.Rollback();
                        return -6;
                    }
                    db.OutputMaterials.Add(outputMaterial);
                    db.SaveChanges();
                }
            }
            #endregion
            #region Update ProductMaterial

            foreach (MaterialViewModel materialViewModel in orderViewModel.MaterialList)
            {
                //Update currentQuantity of product material
                ProductMaterial productMaterial =
                    db.ProductMaterials.FirstOrDefault(m => m.ProductMaterialId == materialViewModel.ProductMaterialId);
                if (productMaterial == null)
                {
                    contextTransaction.Rollback();
                    return 0;
                }
                productMaterial.CurrentQuantity -= materialViewModel.NeedQuantity;
                if (productMaterial.CurrentQuantity < 0)
                {
                    contextTransaction.Rollback();
                    return -6;
                }
                db.SaveChanges();
            }
            #endregion
            #region UpdateOrder

            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderViewModel.Order.OrderId);
            if (order == null)
            {
                contextTransaction.Rollback();
                return -2;
            }
            order.OrderStatus = 2;
            order.PlanDeliveryTime = deliveryTime;
            order.ApproveTime = now;
            order.DepositAmount = deposit;
            order.StaffApproveUserId = staffUserId;

            if (newCustomer != null)
            {
                Customer customer = new Customer
                {
                    CustomerAddress = newCustomer.CustomerAddress,
                    CustomerPhoneNumber = newCustomer.CustomerPhoneNumber,
                    TaxCode = newCustomer.CustomerTaxCode,
                    IsActive = true,
                    IsLoyal = false
                };
                List<Customer> customers = new List<Customer> { customer };
                AccountBusiness accountBusiness = new AccountBusiness();
                // Generate random password
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[6];
                var random = new Random();
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                string password = new String(stringChars);

                User user = new User
                {
                    Fullname = newCustomer.CustomerName,
                    Email = newCustomer.CustomerEmail,
                    Username = newCustomer.Username,

                    Password = accountBusiness.CreatePassword(password)
                };

                Role role = db.Roles.FirstOrDefault(m => m.Name.Equals("Customer"));
                user.Role = role;
                user.Customers = customers;

                order.User = user;

                // Remove Guest Info
                GuestInfo guestInfo = order.GuestInfo;
                if (guestInfo != null)
                {
                    order.GuestInfo = null;
                    db.GuestInfoes.Remove(guestInfo);
                }

                // Send Email
                string passwordStore = "Tiembanhdautay";
                string from = "tiembanh.dautaybma@gmail.com";
                string to = user.Email;

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.To.Add(to);
                mail.From = new MailAddress(from);
                mail.Subject = string.Format("{0}{1}", "Tạo tài khoản cho khách hàng ", user.Fullname);
                mail.Body += "<html lang='vi'>";
                mail.Body += "<head>";
                mail.Body += "<meta charset='utf-8'>";
                mail.Body += "</head>";
                mail.Body += "<body>";
                mail.Body += "<div> Bạn vừa được tạo tài khoản tại Tiệm Bánh Dâu Tây</div>";
                mail.Body += string.Format("{0}{1}", "Tên tài khoản: ", user.Username);
                mail.Body += "<div></div>";
                mail.Body += string.Format("{0}{1}", "Mật khẩu: ", password);
                mail.Body += "</body>";
                mail.Body += "</html>";
                var mailBody = mail.Body;
                var htmlBody = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/html");
                mail.AlternateViews.Add(htmlBody);

                mail.Priority = MailPriority.High;
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(from, passwordStore);
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                try
                {
                    smtp.Send(mail);
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    return -5;
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return 0;
            }

            #endregion

            try
            {
                contextTransaction.Commit();
            }
            catch (Exception)
            {
                contextTransaction.Rollback();
                return 0;
            }
            finally
            {
                contextTransaction.Dispose();
            }


            return 1;
        }
        /// <summary>
        /// Make OrderViewModel
        /// </summary>
        /// <param name="cartList">List of product</param>
        /// <param name="customerId">The ID of the customer in DB</param>
        /// <param name="customerInput">The new customer info</param>
        /// <returns></returns>
        public OrderViewModel MakeOrderViewModel(List<CartViewModel> cartList, int? customerId, CustomerViewModel customerInput)
        {
            OrderViewModel result = new OrderViewModel();
            // Create new order
            Order order = new Order();
            if (customerId != null && customerInput == null)
            {
                order.CustomerUserId = customerId;
                //Add info for OrderView
                //Customer info
                Customer customer = db.Customers.FirstOrDefault(m => m.UserId == customerId && m.IsActive);
                if (customer != null)
                {
                    result.OrderPersonName = customer.User.Fullname;
                    result.OrderPersonAddress = customer.CustomerAddress;
                    result.OrderPersonPhoneNumber = customer.CustomerPhoneNumber;
                    result.OrderPersonTaxCode = customer.TaxCode;
                    result.IsGuest = false;
                    result.IsLoyal = customer.IsLoyal;
                }
            }
            else if (customerId == null && customerInput != null)
            {
                order.CustomerUserId = 0;
                //Add new customer info for OrderView
                //Customer info

                result.OrderPersonName = customerInput.CustomerName;
                result.OrderPersonAddress = customerInput.CustomerAddress;
                result.OrderPersonPhoneNumber = customerInput.CustomerPhoneNumber;
                result.OrderPersonTaxCode = customerInput.CustomerTaxCode;
                result.IsGuest = true;
                result.IsLoyal = false;
            }

            // Temp var for total amount
            int totalAmount = 0;
            // Taemp var for total quantity
            int totalQuantity = 0;
            // Creat OrderItemViewModel List and MaterialViewModelList
            List<OrderItem> orderItemList = new List<OrderItem>();
            List<MaterialViewModel> materialViewModelList = new List<MaterialViewModel>();

            foreach (CartViewModel cartViewModel in cartList)
            {
                if (cartViewModel.Quantity != 0)
                {
                    //Create OrderItem and add to list
                    OrderItem orderItem = new OrderItem();
                    Product product = db.Products.FirstOrDefault(m => m.ProductId == cartViewModel.ProductId);
                    orderItem.Product = product;
                    orderItem.Quantity = cartViewModel.Quantity;
                    orderItem.RealPrice = cartViewModel.RealPrice;
                    orderItem.Amount = cartViewModel.Quantity * cartViewModel.RealPrice;
                    orderItemList.Add(orderItem);

                    totalAmount += cartViewModel.Quantity * cartViewModel.RealPrice;
                    totalQuantity += cartViewModel.Quantity;

                    //Create MaterialViewModel and add to list
                    // Get Product in DB
                    if (product != null)
                    {
                        foreach (Recipe recipe in product.Recipes)
                        {
                            if (materialViewModelList.Count == 0)
                            {
                                MaterialViewModel materialViewModel = new MaterialViewModel();
                                // Get each product material and multiply with quantity
                                materialViewModel.ProductMaterialId = recipe.ProductMaterialId;
                                materialViewModel.ProductMaterialName = recipe.ProductMaterial.ProductMaterialName;
                                materialViewModel.NeedQuantity =
                                    (int)Math.Floor(recipe.RecipeQuantity * cartViewModel.Quantity);
                                materialViewModel.StorageQuantity = recipe.ProductMaterial.CurrentQuantity;
                                // Add into MaterialViewModelList
                                materialViewModelList.Add(materialViewModel);
                            }
                            else
                            {
                                bool check = true;
                                foreach (MaterialViewModel materialViewModel in materialViewModelList)
                                {
                                    if (materialViewModel.ProductMaterialId == recipe.ProductMaterialId)
                                    {
                                        materialViewModel.NeedQuantity +=
                                            (int)Math.Floor(recipe.RecipeQuantity * cartViewModel.Quantity);
                                        check = false;
                                    }
                                }
                                if (check)
                                {
                                    MaterialViewModel materialViewModel = new MaterialViewModel();
                                    // Get each product material and multiply with quantity
                                    materialViewModel.ProductMaterialId = recipe.ProductMaterialId;
                                    materialViewModel.ProductMaterialName = recipe.ProductMaterial.ProductMaterialName;
                                    materialViewModel.NeedQuantity =
                                        (int)Math.Floor(recipe.RecipeQuantity * cartViewModel.Quantity);
                                    materialViewModel.StorageQuantity = recipe.ProductMaterial.CurrentQuantity;
                                    // Add into MaterialViewModelList
                                    materialViewModelList.Add(materialViewModel);
                                }
                            }

                        }
                    }
                }
            }

            // Complete attribute in order
            //Check enough material for order
            result.IsEnoughMaterial = true;
            foreach (MaterialViewModel materialViewModel in materialViewModelList)
            {
                if (materialViewModel.NeedQuantity > materialViewModel.StorageQuantity)
                {
                    materialViewModel.IsEnough = false;
                    result.IsEnoughMaterial = false;
                }
                else
                {
                    materialViewModel.IsEnough = true;
                }
            }


            int discountAmount = 0;
            DiscountByQuantity discountByQuantity =
                db.DiscountByQuantities.FirstOrDefault(
                    m => m.QuantityFrom <= totalQuantity && m.QuantityTo >= totalQuantity);

            if (discountByQuantity != null)
            {
                discountAmount = totalAmount * discountByQuantity.DiscountValue / 100;
            }

            int totalTax = 0;
            TaxRate taxRate =
                db.TaxRates.FirstOrDefault(
                    m =>
                        m.TaxType.Abbreviation.Equals("GTGT") && m.BeginDate <= DateTime.Now &&
                        m.EndDate >= DateTime.Now);
            if (taxRate != null)
            {
                totalTax = (totalAmount - discountAmount) * taxRate.TaxRateValue / 100;
            }

            order.Amount = totalAmount;
            order.DiscountAmount = discountAmount;
            order.TaxAmount = totalTax;
            order.OrderItems = orderItemList;
            result.Order = order;

            result.MaterialList = materialViewModelList;
            // Calculate the Material cost
            List<InputMaterialViewModel> inputMaterialList = GetInputMaterialList(materialViewModelList);
            result.MaterialCost = GetMaterialCost(inputMaterialList);
            return result;
        }


        /// <summary>
        /// Add new order
        /// </summary>
        /// <param name="cartList">List of product</param>
        /// <param name="customerUserId">If add for customer in DB, the ID of user of customer</param>
        /// <param name="inputCustomer">If add for new customer</param>
        /// <param name="staffUserId">Adding statff user id</param>
        /// <param name="deposit">Deposit</param>
        /// <param name="deliveryDate">Delivery Date</param>
        /// <returns></returns>
        public bool AddOrder(List<CartViewModel> cartList, int? customerUserId, CustomerViewModel inputCustomer, int staffUserId, int deposit, DateTime deliveryDate, string orderNote)
        {
            OrderViewModel orderViewModel = MakeOrderViewModel(cartList, customerUserId, null);
            if (orderViewModel == null)
            {
                return false;
            }
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
            // Add order
            Order order = new Order();
            order = orderViewModel.Order;
            DateTime now = DateTime.Now;
            order.ApproveTime = now;
            order.CreateTime = now;
            order.OrderStatus = 2;
            order.PlanDeliveryTime = deliveryDate;
            order.DepositAmount = deposit;
            order.StaffApproveUserId = staffUserId;
            order.OrderNote = orderNote;

            // Get current identity of Order table
            var currentOrderId = db.Database.SqlQuery<decimal>("SELECT IDENT_CURRENT('Orders')").FirstOrDefault();
            String orderCode = "O" + now.ToString("yyyyMMdd") + (((currentOrderId + 1) % 10000)).ToString(new string('0', 4));
            order.OrderCode = orderCode;

            // Customer
            // Exist User
            if (customerUserId != null && inputCustomer == null)
            {
                order.CustomerUserId = customerUserId;
            }
            // New User
            else if (customerUserId == null && inputCustomer != null)
            {
                //Add customer
                User checkUser = db.Users.FirstOrDefault(m => m.Username == inputCustomer.Username ||
                    m.Email == inputCustomer.CustomerEmail);
                Customer checkCustomer =
                    db.Customers.FirstOrDefault(
                        m =>
                            m.CustomerAddress == inputCustomer.CustomerAddress ||
                            m.CustomerPhoneNumber == inputCustomer.CustomerPhoneNumber ||
                            m.TaxCode == inputCustomer.CustomerTaxCode);
                if (checkUser == null && checkCustomer == null)
                {
                    AccountBusiness accountBusiness = new AccountBusiness();

                    // Create user
                    Role role = db.Roles.FirstOrDefault(m => m.Name.Equals("Customer"));
                    // Generate password

                    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[6];
                    var random = new Random();
                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }
                    string password = new String(stringChars);


                    User user = new User
                    {
                        Username = inputCustomer.Username,
                        Email = inputCustomer.CustomerEmail,
                        Password = accountBusiness.CreatePassword(password),
                        Role = role,
                        Fullname = inputCustomer.CustomerName

                    };


                    // Creat customer
                    Customer customer = new Customer
                    {
                        CustomerAddress = inputCustomer.CustomerAddress,
                        CustomerPhoneNumber = inputCustomer.CustomerPhoneNumber,
                        TaxCode = inputCustomer.CustomerTaxCode,
                        IsActive = true
                    };

                    user.Customers.Add(customer);
                    order.User = user;

                    string passwordStore = "Tiembanhdautay";
                    string from = "tiembanh.dautaybma@gmail.com";
                    string to = user.Email;

                    MailMessage mail = new MailMessage();
                    mail.IsBodyHtml = true;
                    mail.To.Add(to);
                    mail.From = new MailAddress(from);
                    mail.Subject = string.Format("{0}{1}", "Tạo tài khoản cho khách hàng ", user.Fullname);
                    mail.Body += "<html lang='vi'>";
                    mail.Body += "<head>";
                    mail.Body += "<meta charset='utf-8'>";
                    mail.Body += "</head>";
                    mail.Body += "<body>";
                    mail.Body += "<div> Bạn vừa được tạo tài khoản tại Tiệm Bánh Dâu Tây</div>";
                    mail.Body += string.Format("{0}{1}", "Tên tài khoản: ", user.Username);
                    mail.Body += "<div></div>";
                    mail.Body += string.Format("{0}{1}", "Mật khẩu: ", password);
                    mail.Body += "</body>";
                    mail.Body += "</html>";
                    var mailBody = mail.Body;
                    var htmlBody = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/html");
                    mail.AlternateViews.Add(htmlBody);

                    mail.Priority = MailPriority.High;
                    SmtpClient smtp = new SmtpClient();
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(from, passwordStore);
                    smtp.Port = 587;
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;

                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }

                }

            }

            #region Add OrderItem, OutputMaterial, ExportFrom, InputMaterial
            foreach (OrderItem orderItem in orderViewModel.Order.OrderItems)
            {
                List<OutputMaterial> outputMaterialList = new List<OutputMaterial>();
                List<MaterialViewModel> materialListForOrderItem = GetMaterialListForOrderItem(orderItem.Product.ProductId,
                    orderItem.Quantity);
                foreach (MaterialViewModel materialViewModel in materialListForOrderItem)
                {
                    OutputMaterial outputMaterial = new OutputMaterial();
                    outputMaterial.ExportQuantity = materialViewModel.NeedQuantity;
                    outputMaterial.ProductMaterialId = materialViewModel.ProductMaterialId;
                    //Get list of InputMaterial available order by expire date descending
                    List<InputMaterial> tempList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive && m.RemainQuantity > 0).OrderByDescending(m => m.InputMaterialExpiryDate).ToList();
                    //Compare each input material with material ViewModel and merge each material of orderItem to input material
                    foreach (InputMaterial inputMaterial in tempList)
                    {
                        if (materialViewModel.NeedQuantity > 0)
                        {
                            ExportFrom exportFrom = new ExportFrom();
                            if (inputMaterial.RemainQuantity >= materialViewModel.NeedQuantity)
                            {
                                exportFrom.ExportFromQuantity = materialViewModel.NeedQuantity;
                                inputMaterial.RemainQuantity -= materialViewModel.NeedQuantity;
                                materialViewModel.NeedQuantity = 0;
                            }
                            else
                            {
                                materialViewModel.NeedQuantity -= inputMaterial.RemainQuantity;
                                exportFrom.ExportFromQuantity = inputMaterial.RemainQuantity;
                                inputMaterial.RemainQuantity = 0;
                            }
                            InputBill inputBill = inputMaterial.InputBill;
                            // Get info for ExportFrom
                            exportFrom.InputBill = inputBill;
                            // Add input bill to output material
                            outputMaterial.ExportFroms.Add(exportFrom);
                        }
                    }
                    outputMaterialList.Add(outputMaterial);
                }
                orderItem.OutputMaterials = outputMaterialList;
            }
            #endregion




            #region Update ProductMaterial

            foreach (MaterialViewModel materialViewModel in orderViewModel.MaterialList)
            {
                //Update currentQuantity of product material
                ProductMaterial productMaterial =
                    db.ProductMaterials.FirstOrDefault(m => m.ProductMaterialId == materialViewModel.ProductMaterialId);
                if (productMaterial == null)
                {
                    return false;
                }
                productMaterial.CurrentQuantity -= materialViewModel.NeedQuantity;
                db.SaveChanges();
            }
            #endregion
            // Add order to db
            db.Orders.Add(order);
            db.SaveChanges();
            try
            {
                contextTransaction.Commit();
            }
            catch (Exception)
            {
                contextTransaction.Rollback();
            }
            finally
            {
                contextTransaction.Dispose();
            }



            return true;
        }


        public bool Cancel(int orderId, int returnDeposit, int isReturnDeposit, int userId)
        {
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
            if (order != null)
            {
                #region If status is "Chờ xử lý) (0) remove order from DB. If GuestInfo is exist, remove GuestInfo. Remove Previous OrderId
                if (order.OrderStatus == 0)
                {
                    db.OrderItems.RemoveRange(order.OrderItems);
                    if (order.GuestInfo != null)
                    {
                        GuestInfo guestInfo = order.GuestInfo;

                        order.GuestInfo = null;
                        db.GuestInfoes.Remove(guestInfo);
                    }
                    Order orderCheck = db.Orders.FirstOrDefault(m => m.PreviousOrderId == orderId);
                    if (orderCheck != null)
                    {
                        orderCheck.PreviousOrderId = null;
                    }
                    db.Orders.Remove(order);
                    db.SaveChanges();
                }
                #endregion
                #region If status is "Chờ xác nhận" (1), "Đã duyệt" (2), return deposit that staff input and return material; status become "Hủy"
                else if (order.OrderStatus == 1 || order.OrderStatus == 2)
                {
                    DbContextTransaction contextTransaction = db.Database.BeginTransaction();
                    foreach (OrderItem orderItem in order.OrderItems)
                    {
                        foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                        {
                            foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                            {

                                // Return InputMaterial
                                int productMaterialId = outputMaterial.ProductMaterialId;
                                InputMaterial inputMaterial = db.InputMaterials.FirstOrDefault(
                                    m => m.ProductMaterialId == productMaterialId && m.InputBillId == exportFrom.InputbillId);
                                if (inputMaterial != null)
                                {
                                    inputMaterial.RemainQuantity += exportFrom.ExportFromQuantity;
                                }
                                // Return ProductMaterial
                                ProductMaterial productMaterial =
                                    db.ProductMaterials.FirstOrDefault(m => m.ProductMaterialId == productMaterialId);
                                if (productMaterial != null)
                                {
                                    productMaterial.CurrentQuantity += exportFrom.ExportFromQuantity;
                                }
                            }
                            db.SaveChanges();
                            // Remove ExportFrom
                            db.ExportFroms.RemoveRange(outputMaterial.ExportFroms);

                        }
                        // Remove OutputMaterial
                        db.OutputMaterials.RemoveRange(orderItem.OutputMaterials);
                    }

                    // Return Deposit
                    if (isReturnDeposit == 0)
                    {
                        order.ReturnDeposit = 0;
                    }
                    else
                    {
                        order.ReturnDeposit = returnDeposit;
                    }
                    // Remove the previous order
                    if (order.OrderStatus == 1)
                    {
                        Order previousOrder = db.Orders.FirstOrDefault(m => m.OrderId == order.PreviousOrderId);
                        if (previousOrder != null)
                        {
                            db.OrderItems.RemoveRange(previousOrder.OrderItems);
                            db.Orders.Remove(previousOrder);
                        }
                    }

                    // Change order status become "Hủy"
                    order.OrderStatus = 6;
                    order.CancelTime = DateTime.Now;
                    order.CancelUserId = userId;

                    db.SaveChanges();
                    // Commit transaction
                    try
                    {
                        contextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        contextTransaction.Rollback();
                    }
                    finally
                    {
                        contextTransaction.Dispose();
                    }
                }
                #endregion
                #region If status is "Đang sản xuất" (3), "Đang giao hàng" (4), return deposit that staff input; status become "Hủy"
                else if (order.OrderStatus == 3 || order.OrderStatus == 4)
                {
                    // Return Deposit
                    if (isReturnDeposit == 0)
                    {
                        order.ReturnDeposit = 0;
                    }
                    else
                    {
                        order.ReturnDeposit = returnDeposit;
                    }
                    // Update order status
                    order.OrderStatus = 6;
                    order.CancelTime = DateTime.Now;
                    order.CancelUserId = userId;
                    db.SaveChanges();
                }
                #endregion
                #region If status is "Đã hoàn thành" (5), "Hủy" (6), can not cancel
                else if (order.OrderStatus == 5 || order.OrderStatus == 6)
                {
                    return false;
                }
                #endregion
            }
            else
            {
                return false;
            }
            return true;
        }

        public MaterialInfoForProductListViewModel GetMaterialListForOrder(List<CartViewModel> cartList)
        {
            MaterialInfoForProductListViewModel result = new MaterialInfoForProductListViewModel();
            List<MaterialViewModel> materialViewModelList = new List<MaterialViewModel>();
            if (cartList != null && cartList.Count > 0)
            {

                foreach (CartViewModel cartViewModel in cartList)
                {
                    Product product = db.Products.FirstOrDefault(m => m.ProductId == cartViewModel.ProductId && m.IsActive);
                    if (product != null)
                    {
                        foreach (Recipe recipe in product.Recipes)
                        {
                            if (materialViewModelList.Count == 0)
                            {
                                MaterialViewModel materialViewModel = new MaterialViewModel();
                                // Get each product material and multiply with quantity
                                materialViewModel.ProductMaterialId = recipe.ProductMaterialId;
                                materialViewModel.ProductMaterialName = recipe.ProductMaterial.ProductMaterialName;
                                materialViewModel.NeedQuantity =
                                    (int)Math.Floor(recipe.RecipeQuantity * cartViewModel.Quantity);
                                materialViewModel.StorageQuantity = recipe.ProductMaterial.CurrentQuantity;
                                // Add into MaterialViewModelList
                                materialViewModelList.Add(materialViewModel);
                            }
                            else
                            {
                                bool check = true;
                                foreach (MaterialViewModel materialViewModel in materialViewModelList)
                                {
                                    if (materialViewModel.ProductMaterialId == recipe.ProductMaterialId)
                                    {
                                        materialViewModel.NeedQuantity +=
                                            (int)Math.Floor(recipe.RecipeQuantity * cartViewModel.Quantity);
                                        check = false;
                                    }
                                }
                                if (check)
                                {
                                    MaterialViewModel materialViewModel = new MaterialViewModel();
                                    // Get each product material and multiply with quantity
                                    materialViewModel.ProductMaterialId = recipe.ProductMaterialId;
                                    materialViewModel.ProductMaterialName = recipe.ProductMaterial.ProductMaterialName;
                                    materialViewModel.NeedQuantity =
                                        (int)Math.Floor(recipe.RecipeQuantity * cartViewModel.Quantity);
                                    materialViewModel.StorageQuantity = recipe.ProductMaterial.CurrentQuantity;
                                    // Add into MaterialViewModelList
                                    materialViewModelList.Add(materialViewModel);
                                }
                            }
                        }
                    }

                }

                // Check enough material
                bool isEnoughMaterial = true;
                foreach (MaterialViewModel materialViewModel in materialViewModelList)
                {
                    if (materialViewModel.NeedQuantity > materialViewModel.StorageQuantity)
                    {
                        materialViewModel.IsEnough = false;
                        isEnoughMaterial = false;
                    }
                    else
                    {
                        materialViewModel.IsEnough = true;
                    }
                }
                result.MaterialList = materialViewModelList;
                result.IsEnough = isEnoughMaterial;
                // Get material cost
                int materialCost = 0;
                foreach (MaterialViewModel materialViewModel in materialViewModelList)
                {
                    int needQuantity = materialViewModel.NeedQuantity;
                    List<InputMaterial> inputMaterialList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive && m.RemainQuantity > 0).OrderBy(m => m.ImportDate).ToList();
                    foreach (InputMaterial inputMaterial in inputMaterialList)
                    {
                        if (needQuantity > 0)
                        {
                            if (inputMaterial.RemainQuantity >= needQuantity)
                            {
                                materialCost += (int)Math.Floor(inputMaterial.InputMaterialPrice * needQuantity);
                                needQuantity = 0;
                            }
                            else
                            {
                                materialCost +=
                                    (int)Math.Floor(inputMaterial.InputMaterialPrice * inputMaterial.RemainQuantity);
                                needQuantity -= inputMaterial.RemainQuantity;
                            }
                        }
                    }
                }
                result.MaterialCost = materialCost;
            }

            return result;
        }

        public int UpdateOrder(List<CartViewModel> cartList, int orderId, int depositAmount,
            DateTime deliveryDate, int staffUserId, string orderNote)
        {

            Order previousOrder = db.Orders.FirstOrDefault(m => m.OrderId == orderId);

            if (previousOrder == null)
            {
                return 0;
            }

            int check = 0;

            foreach (OrderItem orderItem in previousOrder.OrderItems)
            {

                foreach (CartViewModel cartViewModel in cartList)
                {
                    if (cartViewModel.ProductId == orderItem.ProductId &&
                              cartViewModel.Quantity == orderItem.Quantity &&
                              cartViewModel.RealPrice == orderItem.RealPrice)
                    {
                        check++;
                    }
                }
            }

            if (previousOrder.OrderItems.Count == check && cartList.Count == check)
            {
                return 2;
            }

            OrderViewModel orderViewModel = MakeOrderViewModel(cartList, previousOrder.User.UserId, null);

            

            if (orderViewModel == null)
            {
                return 0;
            }
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
            // Add order
            Order order = new Order();
            order.CreateTime = previousOrder.CreateTime;
            order.OrderStatus = 1;
            order.PlanDeliveryTime = deliveryDate;
            order.DepositAmount = depositAmount;
            order.Amount = orderViewModel.Order.Amount;
            order.TaxAmount = orderViewModel.Order.TaxAmount;
            order.StaffApproveUserId = staffUserId;
            order.OrderNote = orderNote;
            // Get current identity of Order table
            order.OrderCode = previousOrder.OrderCode;

            //Add customer
            order.User = previousOrder.User;

            // Update prevoius order
            previousOrder.IsStaffEdit = true;
            order.PreviousOrderId = previousOrder.OrderId;
            order.IsStaffEdit = false;
            order.StaffEditTime = DateTime.Now;

            //Add OrderItem and OutputMaterial 
            #region Add OrderItem, OutputMaterial, ExportFrom, InputMaterial
            List<OrderItem> orderItemList = new List<OrderItem>();
            foreach (OrderItem orderItem in orderViewModel.Order.OrderItems)
            {
                TaxRate taxRate =
                    db.TaxRates.FirstOrDefault(
                        m =>
                            m.TaxTypeId == 1 && previousOrder.CreateTime <= m.EndDate &&
                            previousOrder.CreateTime >= m.BeginDate);
                List<OutputMaterial> outputMaterialList = new List<OutputMaterial>();
                List<MaterialViewModel> materialListForOrderItem = GetMaterialListForOrderItem(orderItem.Product.ProductId,
                    orderItem.Quantity);
                foreach (MaterialViewModel materialViewModel in materialListForOrderItem)
                {
                    OutputMaterial outputMaterial = new OutputMaterial();
                    outputMaterial.ExportQuantity = materialViewModel.NeedQuantity;
                    outputMaterial.ProductMaterialId = materialViewModel.ProductMaterialId;
                    //Get list of InputMaterial available order by expire date descending
                    List<InputMaterial> tempList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive && m.RemainQuantity > 0).OrderByDescending(m => m.InputMaterialExpiryDate).ToList();
                    //Compare each input material with material ViewModel and merge each material of orderItem to input material
                    foreach (InputMaterial inputMaterial in tempList)
                    {
                        if (materialViewModel.NeedQuantity > 0)
                        {
                            ExportFrom exportFrom = new ExportFrom();
                            if (inputMaterial.RemainQuantity >= materialViewModel.NeedQuantity)
                            {
                                exportFrom.ExportFromQuantity = materialViewModel.NeedQuantity;
                                inputMaterial.RemainQuantity -= materialViewModel.NeedQuantity;
                                materialViewModel.NeedQuantity = 0;
                            }
                            else
                            {
                                materialViewModel.NeedQuantity -= inputMaterial.RemainQuantity;
                                exportFrom.ExportFromQuantity = inputMaterial.RemainQuantity;
                                inputMaterial.RemainQuantity = 0;
                            }
                            InputBill inputBill = inputMaterial.InputBill;
                            // Get info for ExportFrom
                            exportFrom.InputBill = inputBill;
                            outputMaterial.ExportFroms.Add(exportFrom);
                        }
                    }
                    outputMaterialList.Add(outputMaterial);
                }
                orderItem.OutputMaterials = outputMaterialList;
                orderItemList.Add(orderItem);
            }
            order.OrderItems = orderItemList;
            #endregion




            #region Update ProductMaterial

            foreach (MaterialViewModel materialViewModel in orderViewModel.MaterialList)
            {
                //Update currentQuantity of product material
                ProductMaterial productMaterial =
                    db.ProductMaterials.FirstOrDefault(m => m.ProductMaterialId == materialViewModel.ProductMaterialId);
                if (productMaterial == null)
                {
                    return 0;
                }
                productMaterial.CurrentQuantity -= materialViewModel.NeedQuantity;
                db.SaveChanges();
            }
            #endregion
            // Add order to db
            db.Orders.Add(order);
            db.SaveChanges();
            try
            {
                contextTransaction.Commit();
            }
            catch (Exception)
            {
                contextTransaction.Rollback();
                return 0;
            }
            finally
            {
                contextTransaction.Dispose();
            }



            return 1;
        }

        public bool IsEnoughMaterialForOrder(Order order)
        {
            List<MaterialViewModel> materialViewModelList = new List<MaterialViewModel>();
            foreach (OrderItem orderItem in order.OrderItems)
            {
                // Get material list for each order item
                List<MaterialViewModel> materialListForOrderItem = GetMaterialListForOrderItem(orderItem.Product.ProductId, orderItem.Quantity);

                // Check each material in materialListForOrderItem, if NeedQuantity > Storage Quantity, return false
                // else if materialViewModelList does not have any member, add into it
                // else check each member in materialViewModel depends on ProductMaterialId, if NeedQuantity + this.NeedQuantity > Storage Quantity, return false
                // else plus into NeedQuantity
                // else add this materialListForOrderItem into materialViewModelList
                foreach (MaterialViewModel materialForOrderItemViewModel in materialListForOrderItem)
                {
                    if (materialForOrderItemViewModel.NeedQuantity > materialForOrderItemViewModel.StorageQuantity)
                    {
                        return false;
                    }
                    if (materialViewModelList.Count == 0)
                    {
                        materialViewModelList.Add(materialForOrderItemViewModel);
                    }
                    else
                    {
                        bool check = true;
                        foreach (MaterialViewModel materialViewModel in materialViewModelList)
                        {
                            if (materialViewModel.ProductMaterialId == materialForOrderItemViewModel.ProductMaterialId)
                            {
                                check = false;
                                if ((materialViewModel.NeedQuantity + materialForOrderItemViewModel.NeedQuantity) >
                                    materialViewModel.StorageQuantity)
                                {
                                    return false;
                                }
                                else
                                {
                                    materialViewModel.NeedQuantity += materialForOrderItemViewModel.NeedQuantity;
                                }
                            }
                        }
                        if (check)
                        {
                            materialViewModelList.Add(materialForOrderItemViewModel);
                        }
                    }
                }
            }
            return true;
        }

        public List<ShortageMaterialViewModel> GetShortageMaterialList()
        {
            List<ShortageMaterialViewModel> result = new List<ShortageMaterialViewModel>();
            List<ShortageMaterialViewModel> processList = new List<ShortageMaterialViewModel>();
            List<Order> orderList = db.Orders.Where(m => m.OrderStatus == 0 && !m.IsStaffEdit).ToList();
            if (orderList.Count > 0)
            {
                foreach (Order order in orderList)
                {
                    foreach (OrderItem orderItem in order.OrderItems)
                    {
                        // Get material list for each order item
                        List<MaterialViewModel> materialListForOrderItem = GetMaterialListForOrderItem(orderItem.Product.ProductId, orderItem.Quantity);
                        // If processList does not have any member, add new ShortageMaterialInOrderViewModel
                        // Else check each member of processList depends on ProductMaterialId, if has plus NeedQuantity and add 
                        //  If if check the ShortageMaterialInOrderViewModel, does not have add new
                        //  Else plus Need Quantity
                        foreach (MaterialViewModel materialViewModel in materialListForOrderItem)
                        {
                            if (processList.Count == 0)
                            {
                                ShortageMaterialViewModel shortageMaterial = new ShortageMaterialViewModel
                                {
                                    NeedQuantity = materialViewModel.NeedQuantity,
                                    CurrentQuantity = materialViewModel.StorageQuantity,
                                    ProductMaterialId = materialViewModel.ProductMaterialId,
                                    ProductMaterialName = materialViewModel.ProductMaterialName,
                                    ProductMaterialUnit = materialViewModel.Unit
                                };
                                ShortageMaterialInOrderViewModel shortageMaterialInOrder =
                                    new ShortageMaterialInOrderViewModel
                                    {
                                        NeedQuantity = materialViewModel.NeedQuantity,
                                        OrderCode = order.OrderCode,
                                        OrderId = order.OrderId
                                    };
                                if (shortageMaterial.ShortageMaterialInOrderList == null)
                                {
                                    List<ShortageMaterialInOrderViewModel> shortageMaterialInOrderList =
                                        new List<ShortageMaterialInOrderViewModel>();
                                    shortageMaterialInOrderList.Add(shortageMaterialInOrder);
                                    shortageMaterial.ShortageMaterialInOrderList = shortageMaterialInOrderList;
                                }
                                else
                                {
                                    shortageMaterial.ShortageMaterialInOrderList.Add(shortageMaterialInOrder);
                                }
                                processList.Add(shortageMaterial);
                            }
                            else
                            {
                                bool check = true;
                                foreach (ShortageMaterialViewModel shortageMaterial in processList)
                                {
                                    if (shortageMaterial.ProductMaterialId ==
                                        materialViewModel.ProductMaterialId)
                                    {
                                        check = false;
                                        shortageMaterial.NeedQuantity += materialViewModel.NeedQuantity;
                                        bool orderCheck = true;
                                        foreach (ShortageMaterialInOrderViewModel shortageMaterialInOrder in shortageMaterial.ShortageMaterialInOrderList)
                                        {
                                            if (shortageMaterialInOrder.OrderId == order.OrderId)
                                            {
                                                orderCheck = false;
                                                shortageMaterialInOrder.NeedQuantity += materialViewModel.NeedQuantity;
                                            }
                                        }
                                        if (orderCheck)
                                        {
                                            ShortageMaterialInOrderViewModel shortageMaterialInOrder = new ShortageMaterialInOrderViewModel();
                                            shortageMaterialInOrder.NeedQuantity = materialViewModel.NeedQuantity;
                                            shortageMaterialInOrder.OrderCode = order.OrderCode;
                                            shortageMaterialInOrder.OrderId = order.OrderId;
                                            if (shortageMaterial.ShortageMaterialInOrderList == null)
                                            {
                                                List<ShortageMaterialInOrderViewModel> shortageMaterialInOrderList =
                                                    new List<ShortageMaterialInOrderViewModel>();
                                                shortageMaterialInOrderList.Add(shortageMaterialInOrder);
                                                shortageMaterial.ShortageMaterialInOrderList = shortageMaterialInOrderList;
                                            }
                                            else
                                            {
                                                shortageMaterial.ShortageMaterialInOrderList.Add(shortageMaterialInOrder);
                                            }
                                        }
                                    }
                                }
                                if (check)
                                {
                                    ShortageMaterialViewModel shortageMaterial = new ShortageMaterialViewModel
                                    {
                                        NeedQuantity = materialViewModel.NeedQuantity,
                                        CurrentQuantity = materialViewModel.StorageQuantity,
                                        ProductMaterialId = materialViewModel.ProductMaterialId,
                                        ProductMaterialName = materialViewModel.ProductMaterialName,
                                        ProductMaterialUnit = materialViewModel.Unit
                                    };
                                    ShortageMaterialInOrderViewModel shortageMaterialInOrder =
                                        new ShortageMaterialInOrderViewModel
                                        {
                                            NeedQuantity = materialViewModel.NeedQuantity,
                                            OrderCode = order.OrderCode,
                                            OrderId = order.OrderId
                                        };
                                    if (shortageMaterial.ShortageMaterialInOrderList == null)
                                    {
                                        List<ShortageMaterialInOrderViewModel> shortageMaterialInOrderList =
                                            new List<ShortageMaterialInOrderViewModel>();
                                        shortageMaterialInOrderList.Add(shortageMaterialInOrder);
                                        shortageMaterial.ShortageMaterialInOrderList = shortageMaterialInOrderList;
                                    }
                                    else
                                    {
                                        shortageMaterial.ShortageMaterialInOrderList.Add(shortageMaterialInOrder);
                                    }
                                    processList.Add(shortageMaterial);
                                }
                            }
                        }

                    }
                }
            }
            if (processList.Count > 0)
            {
                foreach (ShortageMaterialViewModel shortageMaterial in processList)
                {
                    if (shortageMaterial.NeedQuantity > shortageMaterial.CurrentQuantity)
                    {
                        result.Add(shortageMaterial);
                    }
                }
            }
            return result;
        }

        #region Change Order State
        public bool ChangeOrderState(int orderId)
        {
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
            if (order == null)
            {
                return false;
            }
            if (order.OrderStatus >= 2 && order.OrderStatus <= 4)
            {
                order.OrderStatus++;
                if (order.OrderStatus == 3)
                {
                    order.StartProduceTime = DateTime.Now;
                }
                if (order.OrderStatus == 4)
                {
                    order.DeliveryTime = DateTime.Now;
                }
                if (order.OrderStatus == 5)
                {
                    order.FinishTime = DateTime.Now;
                    // Make output bill
                    OutputBill output = new OutputBill();
                    output.OutputBillAmount = order.Amount;
                    output.OutputBillTaxAmount = order.TaxAmount;
                    output.OutputBillCode = "B" + order.OrderCode.Substring(1);

                    output.OrderId = orderId;
                    db.OutputBills.Add(output);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Get Order by Id
        public Order GetOrder(int orderId)
        {
            return db.Orders.FirstOrDefault(m => m.OrderId == orderId);
        }
        #endregion

        /// <summary>
        /// Get sorted order view model list
        /// </summary>
        /// <returns></returns>
        #region Get sorted order view model list
        public List<OrderViewModel> GetSortedOrderViewModelList(DateTime? fromDate, DateTime? toDate, string customerName, int? orderStatus)
        {
            List<Order> orderList;
            if (fromDate != null && toDate != null)
            {
                DateTime toDateTemp = toDate.Value.AddDays(1);
                if (orderStatus != null && orderStatus != -1)
                {

                    orderList =
                        db.Orders.Where(
                            m =>
                                !m.IsStaffEdit && m.CreateTime >= fromDate.Value && m.CreateTime < toDateTemp &&
                                m.OrderStatus == orderStatus.Value).ToList();
                }
                else
                {
                    orderList = db.Orders.Where(
                        m =>
                            !m.IsStaffEdit && m.CreateTime >= fromDate.Value && m.CreateTime <= toDateTemp).ToList();
                }
            }
            else if (fromDate == null && toDate == null && orderStatus != null && orderStatus != -1)
            {
                orderList = db.Orders.Where(m => !m.IsStaffEdit && m.OrderStatus == orderStatus.Value).ToList();
            }
            else
            {
                orderList = db.Orders.Where(m => !m.IsStaffEdit).ToList();
            }
            List<Order> resultList;
            if (customerName != null && !customerName.Trim().IsEmpty())
            {
                resultList = new List<Order>();
                foreach (Order order in orderList)
                {
                    if (order.GuestInfo != null)
                    {
                        if (order.GuestInfo.GuestInfoName.Contains(customerName))
                        {
                            resultList.Add(order);
                        }
                    }
                    else
                    {
                        if (order.User.Fullname.Contains(customerName))
                        {
                            resultList.Add(order);
                        }
                    }
                }
            }
            else
            {
                resultList = orderList;
            }

            // Custom sort
            resultList.Sort(
                delegate(Order o1, Order o2)
                {
                    if (o1.OrderStatus != o2.OrderStatus)
                    {
                        return o1.OrderStatus.CompareTo(o2.OrderStatus);
                    }
                    return o1.CreateTime.CompareTo(o2.CreateTime);

                });
            List<OrderViewModel> orderViewModelList = new List<OrderViewModel>();
            foreach (Order order in resultList)
            {
                OrderViewModel orderViewModel = new OrderViewModel { Order = order };
                if (order.GuestInfo != null)
                {
                    orderViewModel.OrderPersonName = order.GuestInfo.GuestInfoName;
                }
                else
                {
                    orderViewModel.OrderPersonName = order.User.Fullname;
                }
                if (order.OrderStatus == 0)
                {
                    orderViewModel.IsEnoughMaterial = IsEnoughMaterialForOrder(order);
                }
                orderViewModelList.Add(orderViewModel);
            }
            return orderViewModelList;
        }
        #endregion
        /// <summary>
        /// Get VAT tax rate at the time point
        /// </summary>
        /// <param name="dateTime">Time point</param>
        /// <returns>VAT tax rate value</returns>
        #region Get VAT tax rate at the time point
        public int GetVatRateAtTime(DateTime dateTime)
        {
            TaxRate taxRate =
                db.TaxRates.FirstOrDefault(m => m.TaxRateId == 1 && dateTime >= m.BeginDate && dateTime <= m.EndDate);
            return taxRate != null ? taxRate.TaxRateValue : 0;
        }
        #endregion

        #region
        /// <summary>
        /// Get min quantity of order
        /// </summary>
        /// <returns>The min quantity of order</returns>
        public int GetMinQuantity()
        {
            return db.Policies.FirstOrDefault(m => m.PolicyId == 1).PolicyBound;
        }
        #endregion

        #region Get product list
        /// <summary>
        /// Get product list
        /// </summary>
        /// <returns>List of product in db</returns>
        public List<Product> GetProductList()
        {
            return db.Products.Where(m => m.IsActive).ToList();
        }
        #endregion

        public Product GetProductById(int productId)
        {
            return db.Products.FirstOrDefault(m => m.ProductId == productId && m.IsActive);
        }

        #region
        /// <summary>
        /// Check customer field is exist
        /// </summary>
        /// <param name="customerEmail">Email</param>
        /// <param name="customerAddress">Address</param>
        /// <param name="customerPhoneNumber">Phone</param>
        /// <param name="customerTaxCode">Tax Code</param>
        /// <param name="username">Username</param>
        /// <returns></returns>
        public int CheckCustomerField(string customerEmail, string customerAddress, string customerPhoneNumber,
            string customerTaxCode, string username)
        {
            // Check Email
            User checkEmail = db.Users.FirstOrDefault(m => m.Email.Equals(customerEmail.Trim()));
            if (checkEmail != null)
            {
                return 11;
            }

            // Check Username
            User checkUsername = db.Users.FirstOrDefault(m => m.Username.Equals(username.Trim()));
            if (checkUsername != null)
            {
                return 12;
            }

            // Check Address
            Customer checkAddress = db.Customers.FirstOrDefault(m => m.CustomerAddress.Equals(customerAddress.Trim()));
            if (checkAddress != null)
            {
                return 13;
            }

            // Check Phone Number
            Customer checkPhoneCustomer = db.Customers.FirstOrDefault(m => m.CustomerPhoneNumber.Equals(customerPhoneNumber.Trim()));
            if (checkPhoneCustomer != null)
            {
                return 14;
            }

            Staff checkPhoneStaff =
                db.Staffs.FirstOrDefault(m => m.StaffPhoneNumber.Equals(customerPhoneNumber.Trim()) && m.IsActive);
            if (checkPhoneStaff != null)
            {
                return 14;
            }

            // Check Tax Code
            Customer checkTaxCode = db.Customers.FirstOrDefault(m => m.TaxCode.Equals(customerTaxCode.Trim()));
            if (checkTaxCode != null)
            {
                return 15;
            }

            return 0;
        }
        #endregion

        public List<DiscountByQuantity> GetDiscountByQuantityList()
        {
            return db.DiscountByQuantities.ToList();
        }

        public bool ExportMaterial(int productMaterialId, int outputMaterialId, int exportQuantity)
        {
            OutputMaterial outputMaterial =
                db.OutputMaterials.FirstOrDefault(m => m.OutputMaterialId == outputMaterialId);

            ProductMaterial productMaterial =
                db.ProductMaterials.FirstOrDefault(m => m.ProductMaterialId == productMaterialId);

            if (outputMaterial == null || productMaterial == null || productMaterial.CurrentQuantity < exportQuantity)
            {
                return false;
            }

            outputMaterial.ExportQuantity += exportQuantity;

            productMaterial.CurrentQuantity -= exportQuantity;

            //Get list of InputMaterial available order by expire date descending
            List<InputMaterial> tempList = db.InputMaterials.Where(m => m.ProductMaterialId == productMaterialId && m.IsActive && m.RemainQuantity > 0).OrderByDescending(m => m.InputMaterialExpiryDate).ToList();

            //Compare each input material with material ViewModel and merge each material of orderItem to input material
            foreach (InputMaterial inputMaterial in tempList)
            {
                if (exportQuantity > 0)
                {
                    ExportFrom exportFrom = new ExportFrom();
                    if (inputMaterial.RemainQuantity >= exportQuantity)
                    {
                        exportFrom.ExportFromQuantity = exportQuantity;
                        inputMaterial.RemainQuantity -= exportQuantity;
                        exportQuantity = 0;

                    }
                    else
                    {
                        exportQuantity -= inputMaterial.RemainQuantity;
                        exportFrom.ExportFromQuantity = inputMaterial.RemainQuantity;
                        inputMaterial.RemainQuantity = 0;
                    }

                    ExportFrom exportFromCheck =
                        db.ExportFroms.FirstOrDefault(
                            m => m.InputbillId == inputMaterial.InputBillId && m.OutputMaterialId == outputMaterialId);
                    if (exportFromCheck != null)
                    {
                        exportFromCheck.ExportFromQuantity += exportFrom.ExportFromQuantity;
                    }
                    else
                    {
                        // Get info for ExportFrom
                        exportFrom.InputbillId = inputMaterial.InputBillId;
                        // Add output material to input bill
                        exportFrom.OutputMaterialId = outputMaterialId;
                        db.ExportFroms.Add(exportFrom);
                    }
                }
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
        /// <summary>
        /// Remove Order with OrderStatus = 0 and PlanDeliveryDate > DateTime.Now
        /// </summary>
        public void AutoRemoveWaitingOrder()
        {
            List<Order> waitingOrder = db.Orders.Where(m => m.OrderStatus == 0).ToList();
            if (waitingOrder.Count > 0)
            {
                List<Order> removeOrderList = new List<Order>();
                foreach (Order order in waitingOrder)
                {
                    if (order.OrderStatus == 0 && (order.PlanDeliveryTime - DateTime.Now).TotalDays < 0)
                    {
                        removeOrderList.Add(order);
                    }
                }
                if (removeOrderList.Count > 0)
                {
                    db.Orders.RemoveRange(removeOrderList);
                    db.SaveChanges();
                }
            }
        }
    }

}