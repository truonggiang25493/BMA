using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace BMA.DBChangesNotifer
{
    public class ConfirmToCustomerNotifier
    {
        private SqlConnection con;
        private SqlCommand cmd;
        private SqlDependency dependency;
        private String connectionString;
        private String dependencyCheckSql;

        ~ConfirmToCustomerNotifier()
        {
            this.Dispose();
        }

        public event EventHandler<ChangeEventArgs> Change;

        public Boolean Start(String connectionStringName, String dependencyCheckSql)
        {
            new SqlClientPermission(PermissionState.Unrestricted).Demand();

            this.connectionString = ConfigurationManager.ConnectionStrings["BMAChangeDB"].ConnectionString;
            this.dependencyCheckSql = dependencyCheckSql;

            var result = SqlDependency.Start(this.connectionString);

            this.con = new SqlConnection(this.connectionString);
            this.con.Open();

            this.cmd = this.con.CreateCommand();
            this.cmd.CommandText = this.dependencyCheckSql;

            this.Setup(true);

            return (result);
        }

        public Boolean Stop()
        {
            var result = false;

            if (this.cmd != null)
            {
                this.cmd.Notification = null;
                this.cmd.Dispose();
                this.cmd = null;
            }

            if (this.con != null)
            {
                try
                {
                    this.con.Close();
                    this.con = null;
                }
                catch
                {
                    this.con = null;
                }
            }

            if (this.dependency != null)
            {
                result = SqlDependency.Stop(this.connectionString);
                this.dependency.OnChange -= this.OnChange;
                this.dependency = null;
            }

            this.Change = null;

            return (result);
        }

        private void Setup(Boolean initial)
        {
            if (initial == false)
            {
                this.dependency.OnChange -= this.OnChange;
            }

            this.cmd.Notification = null;
            this.dependency = new SqlDependency(this.cmd);
            this.dependency.OnChange += this.OnChange;
            this.cmd.ExecuteScalar();
        }

        private void OnChange(Object sender, SqlNotificationEventArgs e)
        {
            this.Setup(false);

            var handler = this.Change;

            if (handler != null)
            {
                handler(sender, new ChangeEventArgs((ChangeInfo)(Int32)e.Info, (ChangeSource)(Int32)e.Source, (ChangeType)(Int32)e.Type));
            }
        }
        public void Dispose()
        {
            this.Stop();
        }
    }
}