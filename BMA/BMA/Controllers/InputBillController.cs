using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;

namespace BMA.Controllers
{
    public class InputBillController : Controller
    {
        private BMAEntities db = new BMAEntities();
        //private InputMaterialBusiness inputMaterialBusiness = new InputMaterialBusiness();
        //
        // GET: /InputBill/
        public ActionResult InputBillIndex()
        {
            return View();
        }

	}
}