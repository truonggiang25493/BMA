using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;
using BMA.Business;
using System.Configuration;

namespace BMA.Hubs
{
    public class RealtimeNotifierHub : Hub
    {
        public int recordsToBeProcessed = 100000;

        public void DoLongOperation()
        {
            for (int record = 0; record <= recordsToBeProcessed; record++)
            {
                if (ShouldNotifyClient(record))
                {
                    Clients.Caller.sendMessage(string.Format
                    ("Processing item {0} of {1}", record, recordsToBeProcessed));
                    Thread.Sleep(10);
                }
            }
        }

        private static bool ShouldNotifyClient(int record)
        {
            return record % 10 == 0;
        }

        public void OnChange(Int32 info, Int32 source, Int32 type)
        {
            this.Clients.All.onChange(info, source, type);
        }
    }
}