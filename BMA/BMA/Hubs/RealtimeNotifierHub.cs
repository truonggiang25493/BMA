using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;
using BMA.Business;
using System.Configuration;
using BMA.Models;


namespace BMA.Hubs
{
    public class RealtimeNotifierHub : Hub
    {
        BMAEntities db = new BMAEntities();
        public void OnChange(Int32 info, Int32 source, Int32 type)
        {
            this.Clients.All.onChange(info, source, type);
        }

        public void OnChange2(Int32 info, Int32 source, Int32 type)
        {
            List<ProductMaterial> lstProductMaterial = db.ProductMaterials.Where(n => n.CurrentQuantity >= n.StandardQuantity && n.IsActive).ToList();
            if (lstProductMaterial != null)
            {
                this.Clients.All.onChange2(info, source, type);
            }
        }

        public void OnChange3(Int32 info, Int32 source, Int32 type)
        {
            this.Clients.All.onChange3(info, source, type);
        }
    }
}