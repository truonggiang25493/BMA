using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class OtherExpenseBusiness
    {
        private BMAEntities db;
        public OtherExpenseBusiness()
        {
            db = new BMAEntities();
        }

        #region Get list of expense

        public List<OtherExpense> GetOtherExpenseList()
        {
            return db.OtherExpenses.OrderBy(m => m.OtherExpenseYearTime).ThenBy(m => m.OtherExpenseMonthTime).ToList();
        }


        #endregion
    }
}