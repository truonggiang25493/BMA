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

        public bool EditStoreInfo(string storeName, string ownerName, string address, string phoneNumber, string taxCode, string fileName)
        {
            StoreInfo store = db.StoreInfoes.SingleOrDefault();
            store.StoreName = storeName;
            store.OwnerName = ownerName;
            store.Address = address;
            store.Phonenumber = phoneNumber;
            store.TaxCode = taxCode;
            store.BakeryImage = fileName;
            db.SaveChanges();
            return true;
        }

        public bool MinQuantity(int bound)
        {
            Policy policty = db.Policies.SingleOrDefault();
            policty.PolicyBound = bound;
            db.SaveChanges();
            return true;
        }
    }
}