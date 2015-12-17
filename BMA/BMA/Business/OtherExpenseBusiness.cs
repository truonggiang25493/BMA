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

        #region Edit other expense

        public bool EditOtherExpense(int id, string name, int amount, int month, int year, int type)
        {
            OtherExpense otherExpense = db.OtherExpenses.FirstOrDefault(m => m.OtherExpenseId == id);
            if (otherExpense == null)
            {
                return false;
            }
            else
            {
                otherExpense.OtherExpenseName = name;
                otherExpense.OtherExpenseAmount = amount;
                otherExpense.OtherExpenseMonthTime = month;
                otherExpense.OtherExpenseYearTime = year;
                otherExpense.Type = type;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    db.Dispose();
                }
            }
            return true;
        }
        #endregion

        #region Add other expense

        public bool AddOtherExpense(string name, int amount, int timeType, int? month, int? year, DateTime? fromTime, DateTime? toTime, int type)
        {
            if (timeType == 1)
            {
                OtherExpense otherExpense = new OtherExpense
                {
                    OtherExpenseName = name,
                    OtherExpenseAmount = amount,
                    OtherExpenseMonthTime = month,
                    OtherExpenseYearTime = year,
                    Type = type
                };

                db.OtherExpenses.Add(otherExpense);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    db.Dispose();
                }
            }
            else
            {
                List<OtherExpense> otherExpenseList = new List<OtherExpense>();
                for (DateTime tempTime = fromTime.Value; tempTime <= toTime.Value; tempTime = tempTime.AddMonths(1))
                {
                    OtherExpense otherExpense = new OtherExpense
                    {
                        OtherExpenseName = name,
                        OtherExpenseMonthTime = tempTime.Month,
                        OtherExpenseYearTime = tempTime.Year,
                        Type = type
                    };
                    otherExpenseList.Add(otherExpense);
                }
                int division = otherExpenseList.Count();
                foreach (OtherExpense otherExpense in otherExpenseList)
                {
                    otherExpense.OtherExpenseAmount = amount / division;
                }
                db.OtherExpenses.AddRange(otherExpenseList);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    db.Dispose();
                }
            }
            return true;
        }
        #endregion

        #region Delete other expense

        public bool DeleteOtherExpense(int id)
        {
            OtherExpense otherExpense = db.OtherExpenses.FirstOrDefault(m => m.OtherExpenseId == id);
            if (otherExpense == null)
            {
                return false;
            }
            db.OtherExpenses.Remove(otherExpense);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                db.Dispose();
            }
            return true;

        }
        #endregion
    }
}