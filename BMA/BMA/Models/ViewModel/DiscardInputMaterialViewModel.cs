using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class DiscardInputMaterialViewModel
    {
        public int DiscardId { get; set; }
        public int InputMaterialId { get; set; }
        public int DiscardQuantity { get; set; }
        public DateTime DiscardDate { get; set; }
        public string DiscardNote { get; set; }
    }
}