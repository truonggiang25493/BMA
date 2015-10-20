using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Business
{
    public class OrderBusiness
    {
        public static OrderViewModel GetOrderViewModel(int orderId)
        {
            OrderViewModel result = new OrderViewModel();
            BMAEntities db = new BMAEntities();
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
            if (order != null)
            {
                //Get Basic Info
                result.Order = order;
                int taxAmount = 0;
                int totalAmount = 0;
                //Get Order item info, TaxAmount, TotalAmount
                List<OrderItemViewModel> orderItemViewModelList = new List<OrderItemViewModel>();
                List<OrderItem> orderItemList = db.OrderItems.Where(m => m.OrderId == orderId).ToList();
                foreach (OrderItem orderItem in orderItemList)
                {
                    OrderItemViewModel orderItemViewModel = new OrderItemViewModel();
                    orderItemViewModel.OrderItem = orderItem;
                    orderItemViewModel.MaterialList = GetMaterialListForOrderItem(orderItem.OrderItemId);
                    taxAmount += orderItem.TaxAmount;
                    totalAmount += orderItem.Amount;
                    Product productTemp = db.Products.FirstOrDefault(m => m.ProductId == orderItem.ProductId);
                    if (productTemp != null)
                    {
                        orderItemViewModel.ProductName = productTemp.ProductName;
                    }
                    orderItemViewModelList.Add(orderItemViewModel);
                    if (order.DeliveryTime != null) ((DateTime)order.DeliveryTime).ToString("yyyy-MM-dd");
                }
                result.TaxAmount = taxAmount;
                result.TotalAmount = totalAmount;
                result.OrderItemList = orderItemViewModelList;
                //Get Material info
                List<MaterialViewModel> materialViewModelListTemp = GetMaterialListForOrder(orderId);
                // Check the order is enough material or not
                result.IsEnoughMaterial = true;
                foreach (MaterialViewModel materialViewModel in materialViewModelListTemp)
                {
                    if (!materialViewModel.IsEnough)
                    {
                        result.IsEnoughMaterial = false;
                    }
                }

                result.MaterialList = materialViewModelListTemp;
                List<InputMaterialViewModel> inputMaterialViewModelListTemp = GetInputMaterialList(result.MaterialList);
                // Get Material Cost
                if (inputMaterialViewModelListTemp.Count != 0)
                {
                    result.MaterialCost = GetMaterialCost(inputMaterialViewModelListTemp);
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
        public static List<MaterialViewModel> GetMaterialListForOrder(int orderId)
        {
            BMAEntities db = new BMAEntities();
            List<MaterialViewModel> resultList = new List<MaterialViewModel>();
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
            if (order != null)
            {
                // Create ProductMaterial List
                // Scan all of Order Item List to calc the amount of all material and add into resultList
                foreach (var orderItem in order.OrderItems)
                {
                    List<MaterialViewModel> materialListForOrderItem = GetMaterialListForOrderItem(orderItem.OrderItemId);
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

        public static List<MaterialViewModel> GetMaterialListForOrderItem(int orderItemId)
        {
            List<MaterialViewModel> resultList = new List<MaterialViewModel>();
            BMAEntities db = new BMAEntities();
            OrderItem orderItem = db.OrderItems.FirstOrDefault(m => m.OrderItemId == orderItemId);
            if (orderItem != null)
            {
                foreach (Recipe recipe in orderItem.Product.Recipes)
                {
                    MaterialViewModel materialViewModel = new MaterialViewModel();
                    materialViewModel.ProductMaterialId = recipe.ProductMaterialId;
                    materialViewModel.NeedQuantity = (int)Math.Floor(recipe.RecipeQuantity * orderItem.Quantity);
                    materialViewModel.Unit = recipe.ProductMaterial.ProductMaterialUnit;
                    materialViewModel.ProductMaterialName = recipe.ProductMaterial.ProductMaterialName;
                    resultList.Add(materialViewModel);
                }
            }
            return resultList;
        }
        public static List<InputMaterialViewModel> GetInputMaterialList(List<MaterialViewModel> inputList)
        {
            List<MaterialViewModel> materialList = new List<MaterialViewModel>();
            foreach (MaterialViewModel materialViewModelInput in inputList)
            {
                MaterialViewModel materialViewModel = new MaterialViewModel();
                materialViewModel.IsEnough = materialViewModelInput.IsEnough;
                materialViewModel.NeedQuantity = materialViewModelInput.NeedQuantity;
                materialViewModel.ProductMaterialId = materialViewModelInput.ProductMaterialId;
                materialViewModel.ProductMaterialName = materialViewModelInput.ProductMaterialName;
                materialViewModel.StorageQuantity = materialViewModelInput.StorageQuantity;
                materialViewModel.Unit = materialViewModelInput.Unit;
                materialList.Add(materialViewModel);
            }

            List<InputMaterialViewModel> resultList = new List<InputMaterialViewModel>();
            if (materialList.Count != 0)
            {
                BMAEntities db = new BMAEntities();
                foreach (MaterialViewModel materialViewModel in materialList)
                {

                    List<InputMaterial> tempList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive).OrderBy(m => m.ImportDate).ToList();
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

        public static int GetMaterialCost(List<InputMaterialViewModel> inputList)
        {
            int result = 0;

            foreach (InputMaterialViewModel inputMaterialViewModel in inputList)
            {
                result += (int)Math.Floor(inputMaterialViewModel.Price * inputMaterialViewModel.Quantity);
            }

            return result;
        }

        public static bool ApproveOrder(int orderId, int deposit, DateTime deliveryTime)
        {
            OrderViewModel orderViewModel = GetOrderViewModel(orderId);
            if (orderViewModel.Order.Flag)
            {
                return false;
            }
            if (orderViewModel == null)
            {
                return false;
            }
            if (orderViewModel.Order.OrderStatus != 0)
            {
                return false;
            }
            if (!orderViewModel.IsEnoughMaterial)
            {
                return false;
            }
            BMAEntities db = new BMAEntities();
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
            #region Update OutputMaterial; ExportFrom and InputMaterial
            foreach (OrderItemViewModel orderItemViewModel in orderViewModel.OrderItemList)
            {
                foreach (MaterialViewModel materialViewModel in orderItemViewModel.MaterialList)
                {
                    OutputMaterial outputMaterial = new OutputMaterial();
                    outputMaterial.ExportQuantity = materialViewModel.NeedQuantity;
                    outputMaterial.ProductMaterialId = materialViewModel.ProductMaterialId;
                    outputMaterial.OrderItemId = orderItemViewModel.OrderItem.OrderItemId;
                    //Get list of InputMaterial available order by importTime descending
                    List<InputMaterial> tempList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive && m.RemainQuantity > 0).OrderBy(m => m.ImportDate).ToList();
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
                    return false;
                }
                productMaterial.CurrentQuantity -= materialViewModel.NeedQuantity;
                db.SaveChanges();
            }
            #endregion
            #region UpdateOrder

            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderViewModel.Order.OrderId);
            if (order == null)
            {
                contextTransaction.Rollback();
                return false;
            }
            order.OrderStatus = 2;
            order.DeliveryTime = deliveryTime;
            order.ApproveTime = DateTime.Now;
            order.DepositAmount = deposit;
            //Bug
            order.StaffApproveUserId = 2;
            db.SaveChanges();
            #endregion

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

        public static OrderViewModel MakeOrderViewModel(List<CartViewModel> cartList, int customerId)
        {
            OrderViewModel result = new OrderViewModel();
            BMAEntities db = new BMAEntities();
            // Create new order
            Order order = new Order();
            order.CustomerUserId = customerId;
            result.Order = order;
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
            // Temp var for total amount
            int totalAmount = 0;
            // Creat OrderItemViewModel List and MaterialViewModelList
            List<OrderItemViewModel> orderItemViewModelList = new List<OrderItemViewModel>();
            List<MaterialViewModel> materialViewModelList = new List<MaterialViewModel>();

            foreach (CartViewModel cartViewModel in cartList)
            {
                if (cartViewModel.Quantity != 0)
                {
                    List<MaterialViewModel> materialViewModelListForOrderItem = new List<MaterialViewModel>();
                    OrderItemViewModel orderItemViewModel = new OrderItemViewModel();
                    //Create OrderItemViewModel and add to list
                    OrderItem orderItem = new OrderItem();
                    orderItem.ProductId = cartViewModel.ProductId;
                    orderItem.Quantity = cartViewModel.Quantity;
                    orderItem.RealPrice = cartViewModel.RealPrice;
                    orderItem.Amount = cartViewModel.Quantity * cartViewModel.RealPrice;
                    var firstOrDefault = db.TaxRates.FirstOrDefault(m => m.TaxTypeId == 1 && m.EndDate == null);
                    if (firstOrDefault != null)
                    {
                        int taxRateTemp = firstOrDefault.TaxRateValue;
                        orderItem.TaxAmount = (cartViewModel.Quantity * cartViewModel.RealPrice * taxRateTemp) / 100;
                    }
                    totalAmount += cartViewModel.Quantity * cartViewModel.RealPrice;
                    orderItemViewModel.OrderItem = orderItem;
                    orderItemViewModel.ProductName = cartViewModel.ProductName;


                    //Create MaterialViewModel and add to list
                    // Get Product in DB
                    Product product = db.Products.FirstOrDefault(m => m.ProductId == cartViewModel.ProductId);
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
                            //Create MaterialViewModel for OrderItem
                            MaterialViewModel materialViewModelForOrderItem = new MaterialViewModel();
                            materialViewModelForOrderItem.ProductMaterialId = recipe.ProductMaterialId;
                            materialViewModelForOrderItem.NeedQuantity = (int)Math.Floor(recipe.RecipeQuantity * cartViewModel.Quantity);
                            materialViewModelListForOrderItem.Add(materialViewModelForOrderItem);
                        }
                    }
                    orderItemViewModel.MaterialList = materialViewModelListForOrderItem;
                    orderItemViewModelList.Add(orderItemViewModel);
                }
            }

            // Complete attribute in order
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
            result.TotalAmount = totalAmount;
            TaxRate taxRate = db.TaxRates.FirstOrDefault(m => m.TaxTypeId == 1);
            if (taxRate != null)
            {
                result.TaxAmount = totalAmount * taxRate.TaxRateValue / 100;
            }
            result.OrderItemList = orderItemViewModelList;
            result.MaterialList = materialViewModelList;
            // Calculate the Material cost
            List<InputMaterialViewModel> inputMaterialList = GetInputMaterialList(materialViewModelList);
            result.MaterialCost = GetMaterialCost(inputMaterialList);
            return result;
        }

        public static bool AddOrderForCustomer(List<CartViewModel> cartList, int customerId, int deposit, DateTime deliveryDate)
        {
            OrderViewModel orderViewModel = MakeOrderViewModel(cartList, customerId);
            if (orderViewModel == null)
            {
                return false;
            }
            BMAEntities db = new BMAEntities();
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
            // Add order
            Order order = new Order();
            DateTime now = DateTime.Now;
            order.ApproveTime = now;
            order.CreateTime = now;
            order.OrderStatus = 2;
            order.DeliveryTime = deliveryDate;
            order.DepositAmount = deposit;
            // Temp Bug
            order.StaffApproveUserId = 2;
            order.TotalValue = orderViewModel.TaxAmount + orderViewModel.TotalAmount;
            //Temp
            order.StaffApproveUserId = 2;
            // Get current identity of Order table
            var currentOrderId = db.Database.SqlQuery<decimal>("SELECT IDENT_CURRENT('Orders')").FirstOrDefault();
            String orderCode = "O" + now.ToString("yyyyMMdd") + (((currentOrderId + 1) % 10000)).ToString(new string('0', 4));
            order.OrderCode = orderCode;

            //Add customer
            Customer customer = db.Customers.FirstOrDefault(m => m.UserId == customerId && m.IsActive);
            if (customer != null)
            {
                order.CustomerUserId = customer.UserId;
            }

            #region Add OrderItem, OutputMaterial, ExportFrom, InputMaterial
            int currentOrderItemId = (int)db.Database.SqlQuery<decimal>("SELECT IDENT_CURRENT('OrderItem')").FirstOrDefault();
            List<OrderItem> orderItemList = new List<OrderItem>();
            foreach (OrderItemViewModel orderItemViewModel in orderViewModel.OrderItemList)
            {
                OrderItem orderItem = orderItemViewModel.OrderItem;
                currentOrderItemId++;
                List<OutputMaterial> outputMaterialList = new List<OutputMaterial>();

                foreach (MaterialViewModel materialViewModel in orderItemViewModel.MaterialList)
                {
                    OutputMaterial outputMaterial = new OutputMaterial();
                    outputMaterial.ExportQuantity = materialViewModel.NeedQuantity;
                    outputMaterial.ProductMaterialId = materialViewModel.ProductMaterialId;
                    //Get list of InputMaterial available order by importTime descending
                    List<InputMaterial> tempList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive && m.RemainQuantity > 0).OrderBy(m => m.ImportDate).ToList();
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

        public static OrderViewModel MakeOrderViewForNewCustomerModel(List<CartViewModel> cartList, User customerUser)
        {
            OrderViewModel result = new OrderViewModel();
            BMAEntities db = new BMAEntities();
            // Create new order
            Order order = new Order();
            order.CustomerUserId = 0;
            result.Order = order;
            //Add new customer info for OrderView
            //Customer info

            result.OrderPersonName = customerUser.Fullname;
            result.OrderPersonAddress = customerUser.Customers.ElementAt(0).CustomerAddress;
            result.OrderPersonPhoneNumber = customerUser.Customers.ElementAt(0).CustomerPhoneNumber;
            result.OrderPersonTaxCode = customerUser.Customers.ElementAt(0).TaxCode;
            result.IsGuest = true;
            result.IsLoyal = false;

            // Temp var for total amount
            int totalAmount = 0;
            // Creat OrderItemViewModel List and MaterialViewModelList
            List<OrderItemViewModel> orderItemViewModelList = new List<OrderItemViewModel>();
            List<MaterialViewModel> materialViewModelList = new List<MaterialViewModel>();

            foreach (CartViewModel cartViewModel in cartList)
            {
                if (cartViewModel.Quantity != 0)
                {
                    List<MaterialViewModel> materialViewModelListForOrderItem = new List<MaterialViewModel>();
                    OrderItemViewModel orderItemViewModel = new OrderItemViewModel();
                    //Create OrderItemViewModel and add to list
                    OrderItem orderItem = new OrderItem();
                    orderItem.ProductId = cartViewModel.ProductId;
                    orderItem.Quantity = cartViewModel.Quantity;
                    orderItem.RealPrice = cartViewModel.RealPrice;
                    orderItem.Amount = cartViewModel.Quantity * cartViewModel.RealPrice;
                    var firstOrDefault = db.TaxRates.FirstOrDefault(m => m.TaxTypeId == 1 && m.EndDate == null);
                    if (firstOrDefault != null)
                    {
                        int taxRateTemp = firstOrDefault.TaxRateValue;
                        orderItem.TaxAmount = (cartViewModel.Quantity * cartViewModel.RealPrice * taxRateTemp) / 100;
                    }
                    totalAmount += cartViewModel.Quantity * cartViewModel.RealPrice;
                    orderItemViewModel.OrderItem = orderItem;
                    orderItemViewModel.ProductName = cartViewModel.ProductName;


                    //Create MaterialViewModel and add to list
                    // Get Product in DB
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
                            //Create MaterialViewModel for OrderItem
                            MaterialViewModel materialViewModelForOrderItem = new MaterialViewModel();
                            materialViewModelForOrderItem.ProductMaterialId = recipe.ProductMaterialId;
                            materialViewModelForOrderItem.NeedQuantity = (int)Math.Floor(recipe.RecipeQuantity * cartViewModel.Quantity);
                            materialViewModelListForOrderItem.Add(materialViewModelForOrderItem);
                        }
                    }
                    orderItemViewModel.MaterialList = materialViewModelListForOrderItem;
                    orderItemViewModelList.Add(orderItemViewModel);
                }
            }

            // Complete attribute in order
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
            result.TotalAmount = totalAmount;
            TaxRate taxRate = db.TaxRates.FirstOrDefault(m => m.TaxTypeId == 1);
            if (taxRate != null)
            {
                result.TaxAmount = totalAmount * taxRate.TaxRateValue / 100;
            }
            result.OrderItemList = orderItemViewModelList;
            result.MaterialList = materialViewModelList;
            // Calculate the Material cost
            List<InputMaterialViewModel> inputMaterialList = GetInputMaterialList(materialViewModelList);
            result.MaterialCost = GetMaterialCost(inputMaterialList);
            return result;
        }
        public static bool AddOrderForNewCustomer(List<CartViewModel> cartList, User inputCustomer, int deposit, DateTime deliveryDate)
        {
            OrderViewModel orderViewModel = MakeOrderViewForNewCustomerModel(cartList, inputCustomer);
            if (orderViewModel == null)
            {
                return false;
            }
            BMAEntities db = new BMAEntities();
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
            // Add order
            Order order = new Order();
            DateTime now = DateTime.Now;
            order.ApproveTime = now;
            order.CreateTime = now;
            order.OrderStatus = 2;
            order.DeliveryTime = deliveryDate;
            order.DepositAmount = deposit;
            order.TotalValue = orderViewModel.TaxAmount + orderViewModel.TotalAmount;
            //Temp
            order.StaffApproveUserId = 2;
            // Get current identity of Order table
            var currentOrderId = db.Database.SqlQuery<decimal>("SELECT IDENT_CURRENT('Orders')").FirstOrDefault();
            String orderCode = "0" + now.ToString("yyyyMMdd") + (((currentOrderId + 1) % 10000)).ToString(new string('0', 4));
            order.OrderCode = orderCode;

            //Add customer
            order.User = inputCustomer;

            //Add OrderItem and OutputMaterial 
            List<OrderItem> orderItemList = new List<OrderItem>();
            foreach (OrderItemViewModel orderItemViewModel in orderViewModel.OrderItemList)
            {
                OrderItem orderItem = orderItemViewModel.OrderItem;
                List<OutputMaterial> outputMaterialList = new List<OutputMaterial>();

                foreach (MaterialViewModel materialViewModel in orderItemViewModel.MaterialList)
                {
                    OutputMaterial outputMaterial = new OutputMaterial();
                    outputMaterial.ExportQuantity = materialViewModel.NeedQuantity;
                    outputMaterial.ProductMaterialId = materialViewModel.ProductMaterialId;
                    //Get list of InputMaterial available order by importTime descending
                    List<InputMaterial> tempList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive).OrderBy(m => m.ImportDate).ToList();
                    //Compare each input material with material ViewModel and merge each material of orderItem to input material
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
                orderItemList.Add(orderItem);
            }
            order.OrderItems = orderItemList;


            #region Update InputMaterial and ProductMaterial

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

                //Compare each input material with material ViewModel and update the remainQuantity in input material
                List<InputMaterial> tempList = db.InputMaterials.Where(
                      m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive).OrderBy(m => m.ImportDate).ToList();

                foreach (InputMaterial inputMaterial in tempList)
                {
                    if (materialViewModel.NeedQuantity > 0)
                    {
                        if (inputMaterial.RemainQuantity >= materialViewModel.NeedQuantity)
                        {
                            inputMaterial.RemainQuantity -= materialViewModel.NeedQuantity;
                            db.SaveChanges();
                            materialViewModel.NeedQuantity = 0;
                        }
                        else
                        {
                            materialViewModel.NeedQuantity -= inputMaterial.RemainQuantity;
                            inputMaterial.RemainQuantity = 0;
                            db.SaveChanges();
                        }
                    }
                }

            }
            #endregion
            // Add order to db
            db.Orders.Add(order);
            db.SaveChanges();
            contextTransaction.Commit();
            return true;
        }

        public static bool Cancel(int orderId, int returnDeposit, int isReturnDeposit)
        {
            BMAEntities db = new BMAEntities();
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
            if (order != null)
            {
                #region If status is "Chờ xử lý) (0) just remove from DB
                if (order.OrderStatus == 0)
                {
                    db.OrderItems.RemoveRange(order.OrderItems);
                    db.Orders.Remove(order);
                    db.SaveChanges();
                }
                #endregion
                #region If status is "Chờ xác nhận" (1), "Đã duyệt" (2), return deposit that staff input and return material; status become "Hủy"
                if (order.OrderStatus == 1 || order.OrderStatus == 2)
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
                    //Temp Bug
                    order.CancelUserId = 2;

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
                if (order.OrderStatus == 3 || order.OrderStatus == 4)
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
                    db.SaveChanges();
                }
                #endregion
                #region If status is "Đã hoàn thành" (5), "Hủy" (6), can not cancel
                if (order.OrderStatus == 5 || order.OrderStatus == 6)
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

        public static MaterialInfoForProductListViewModel GetMaterialListForOrder(List<CartViewModel> cartList)
        {
            BMAEntities db = new BMAEntities();
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
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive).OrderBy(m => m.ImportDate).ToList();
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

        public static bool UpdateOrder(List<CartViewModel> cartList, int orderId, int depositAmount,
            DateTime deliveryDate)
        {
            BMAEntities db = new BMAEntities();
            User customerUser = new User();
            Order previousOrder = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
            if (previousOrder != null)
            {
                customerUser = db.Customers.FirstOrDefault(m => m.UserId == previousOrder.CustomerUserId && m.IsActive).User;
            }
            OrderViewModel orderViewModel = MakeOrderViewForNewCustomerModel(cartList, customerUser);
            if (orderViewModel == null)
            {
                return false;
            }
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
            // Add order
            Order order = new Order();
            order.CreateTime = previousOrder.CreateTime;
            order.OrderStatus = 1;
            order.DeliveryTime = deliveryDate;
            order.DepositAmount = depositAmount;
            order.TotalValue = orderViewModel.TaxAmount + orderViewModel.TotalAmount;
            //Temp Bug temp staff approve user id
            order.StaffApproveUserId = 2;
            // Get current identity of Order table
            order.OrderCode = previousOrder.OrderCode;

            //Add customer
            order.User = customerUser;

            // Update prevoius order
            previousOrder.IsStaffEdit = true;
            order.PreviousOrderId = previousOrder.OrderId;
            // Bug ask when customer confirm
            order.IsStaffEdit = false;

            //Add OrderItem and OutputMaterial 
            #region Add OrderItem, OutputMaterial, ExportFrom, InputMaterial
            int currentOrderItemId = (int)db.Database.SqlQuery<decimal>("SELECT IDENT_CURRENT('OrderItem')").FirstOrDefault();
            List<ExportFrom> exportFromList = new List<ExportFrom>();
            List<OrderItem> orderItemList = new List<OrderItem>();
            foreach (OrderItemViewModel orderItemViewModel in orderViewModel.OrderItemList)
            {
                OrderItem orderItem = orderItemViewModel.OrderItem;
                currentOrderItemId++;
                List<OutputMaterial> outputMaterialList = new List<OutputMaterial>();

                foreach (MaterialViewModel materialViewModel in orderItemViewModel.MaterialList)
                {
                    OutputMaterial outputMaterial = new OutputMaterial();
                    outputMaterial.ExportQuantity = materialViewModel.NeedQuantity;
                    outputMaterial.ProductMaterialId = materialViewModel.ProductMaterialId;
                    //Get list of InputMaterial available order by importTime descending
                    List<InputMaterial> tempList = db.InputMaterials.Where(
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive && m.RemainQuantity > 0).OrderBy(m => m.ImportDate).ToList();
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
    }

}