using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;
using System.Data.Entity;

namespace BMA.Business
{
    public class StoreInforBusiness
    {
        private readonly BMAEntities db;
        public StoreInforBusiness()
        {
            db = new BMAEntities();
        }

        public bool EditStoreInfo(string storeName, string ownerName, string Email, string address, string Province, string District, string phoneNumber, string Fax, string taxCode, string fileName)
        {
            StoreInfo store = db.StoreInfoes.SingleOrDefault();
            store.StoreName = storeName;
            store.OwnerName = ownerName;
            store.Email = Email;
            store.Address = address;
            store.Province = Province;
            store.District = District;
            store.Phonenumber = phoneNumber;
            store.Fax = Fax;
            store.TaxCode = taxCode;
            store.BakeryImage = fileName;
            db.SaveChanges();
            return true;
        }

        public bool MinQuantity(int bound)
        {
            Policy policty = db.Policies.SingleOrDefault(n=>n.PolicyId == 1);
            policty.PolicyBound = bound;
            db.SaveChanges();
            return true;
        }

        public bool changeDiscountQuantity(int[] quantityFrom, int[] quantityTo, int[] discountRate, bool beUsing)
        {
            List<DiscountByQuantity> discountByQuantity = db.DiscountByQuantities.ToList();
            for (int i = 0; i < quantityFrom.Length; i++)
            {
                discountByQuantity[i].QuantityFrom = quantityFrom[i];
                discountByQuantity[i].QuantityTo = quantityTo[i];
                discountByQuantity[i].DiscountValue = discountRate[i];
                discountByQuantity[i].beUsing = beUsing;
                db.SaveChanges();
            }
            return true;
        }

        public bool changeMaxPrice(int maxPrice)
        {
            Policy policy = db.Policies.SingleOrDefault(n => n.PolicyId == 2);
            policy.PolicyBound = maxPrice;
            db.SaveChanges();
            return true;
        }

        public bool changeCategory(string[] categoryName)
        {
            List<Category> category = db.Categories.Where(n => n.CategoryName != "Bánh").ToList();
            for (int i = 0; i < categoryName.Length; i++)
            {
                category[i].CategoryName = categoryName[i];
                db.SaveChanges();
            }
            return true;
        }

        public bool deleteCategory(int categoryId)
        {
            Category category = db.Categories.SingleOrDefault(n=>n.CategoryId == categoryId);
            db.Categories.Remove(category);
            db.SaveChanges();
            return true;
        }

        public bool addCategory(string categoryName)
        {
            Category category = new Category();
            category.CategoryName = categoryName;
            db.Categories.Add(category);
            db.SaveChanges();
            return true;
        }
    }
}