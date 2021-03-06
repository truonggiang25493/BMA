﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class OrderViewModel
    {
        // Basic info of Order
        public Order Order { get; set; }
        public int MaterialCost { get; set; }
        public int TaxAmount { get; set; }
        public int TotalAmount { get; set; }
        public bool IsEnoughMaterial { get; set; }
        public int TaxRate { get; set; }
        // Customer or Guest info
        public int OrderPersonId { get; set; }
        public String OrderPersonName { get; set; }
        public String OrderPersonAddress { get; set; }
        public String OrderPersonPhoneNumber { get; set; }
        public String OrderPersonTaxCode { get; set; }
        public bool IsGuest { get; set; }
        public bool IsLoyal { get; set; }
        // Material Info
        public List<MaterialViewModel> MaterialList { get; set; }


    }
}