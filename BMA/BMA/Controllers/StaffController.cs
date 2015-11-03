using System;
using System.Web.Mvc;
using BMA.Business;
using BMA.Models;

namespace BMA.Controllers
{
    public class StaffController : Controller
    {
        private BMAEntities db = new BMAEntities();
        private StaffBusiness staffBusiness = new StaffBusiness();

        #region Get supplier list
        public ActionResult StaffIndex()
        {
            ViewBag.TreeView = "staff";
            ViewBag.TreeViewMenu = "listStaff";
            var stafflList = staffBusiness.GetStaffList();
            if (stafflList == null)
            {
                RedirectToAction("StaffIndex", "Staff");
            }
            return View(stafflList);
        }
        #endregion

        #region Get Input Material Detail
        public ActionResult StaffDetail(int id)
        {
            ViewBag.TreeView = "staff";
            Staff staffDetail = staffBusiness.GetStaffDetail(id);
            if (staffDetail == null)
            {
                RedirectToAction("StaffIndex", "Staff");

            }
            return View(staffDetail);

        }

        #endregion

        #region Change Staff Status
        [HttpPost]
        public int ChangeStaffStatus(int id)
        {
            StaffBusiness staffBusiness = new StaffBusiness();
            Boolean result = StaffBusiness.ChangeStaffStatus(id);
            if (result)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion
	}
}