using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                    Customer customer = db.Customers.FirstOrDefault(m => m.UserId == order.CustomerUserId);

                    if (customer != null)
                    {
                        result.IsGuest = false;
                        result.OrderPersonName = customer.AspNetUser.FullName;
                        result.OrderPersonAddress = customer.CustomerAddress;
                        result.OrderPersonPhoneNumber = customer.CustomerPhoneNumber;
                        result.OrderPersonId = customer.CustomerId;
                        result.OrderPersonTaxCode = customer.TaxCode;
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
                List<ProductMaterial> productMaterials = new List<ProductMaterial>();
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
            #region Update MaterialInOrderItem
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
                        m => m.ProductMaterialId == materialViewModel.ProductMaterialId && m.IsActive).OrderBy(m => m.ImportDate).ToList();
                    //Compare each input material with material ViewModel and merge each material of orderItem to input material
                    foreach (InputMaterial inputMaterial in tempList)
                    {
                        if (materialViewModel.NeedQuantity > 0)
                        {
                            if (inputMaterial.RemainQuantity >= materialViewModel.NeedQuantity)
                            {
                                materialViewModel.NeedQuantity = 0;
                            }
                            else
                            {
                                materialViewModel.NeedQuantity -= inputMaterial.RemainQuantity;
                            }
                            InputBill inputBill = inputMaterial.InputBill;
                            outputMaterial.InputBills.Add(inputBill);
                        }
                    }
                    db.OutputMaterials.Add(outputMaterial);
                    db.SaveChanges();
                }
            }
            #endregion
            #region Update InputMaterial and ProductMaterial

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
            order.StaffApproveUserId = "User002";
            db.SaveChanges();
            contextTransaction.Commit();
            #endregion
            return true;
        }

        public static OrderViewModel MakeOrderViewModel(List<CartViewModel> cartList, string customerId)
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
                result.OrderPersonName = customer.AspNetUser.FullName;
                result.OrderPersonAddress = customer.CustomerAddress;
                result.OrderPersonPhoneNumber = customer.CustomerPhoneNumber;
                result.OrderPersonTaxCode = customer.TaxCode;
                result.IsGuest = false;
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
                    OrderItemViewModel orderItemViewModel = new OrderItemViewModel();
                    //Create OrderItemViewModel and add to list
                    OrderItem orderItem = new OrderItem();
                    orderItem.ProductId = cartViewModel.ProductId;
                    orderItem.Quantity = cartViewModel.Quantity;
                    orderItem.RealPrice = cartViewModel.RealPrice;
                    orderItem.Amount = cartViewModel.Quantity * cartViewModel.RealPrice;
                    totalAmount += cartViewModel.Quantity * cartViewModel.RealPrice;
                    orderItemViewModel.OrderItem = orderItem;
                    orderItemViewModel.ProductName = cartViewModel.ProductName;
                    orderItemViewModelList.Add(orderItemViewModel);

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

                        }
                    }

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

        public static bool AddOrderForCustomer(List<CartViewModel> cartList, string customerId, int deposit, DateTime deliveryDate)
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
            // Get current identity of Order table
            int currentOrderId = db.Database.ExecuteSqlCommand("IDENT_CURRENT( 'Orders' )");
            String orderCode = "0" + now.ToString("yyyyMMdd") + ((currentOrderId+1) % 10000);
            order.OrderCode = orderCode;

            return true;
        }
    }

}