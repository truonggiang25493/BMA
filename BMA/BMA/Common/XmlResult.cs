using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace BMA.Common
{
    public class XmlResult : ActionResult
    {
        private object _objectToSerialize;

        public XmlResult(object objectToSerialize)
        {
            _objectToSerialize = objectToSerialize;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (_objectToSerialize != null)
            {
                HttpContextBase httpContextBase = context.HttpContext;
                httpContextBase.Response.Buffer = true;
                httpContextBase.Response.Clear();

                string filename = DateTime.Now.ToString("ddMMyyyyhhss") + ".xml";
                httpContextBase.Response.AddHeader("context-disposition", "attachment; filename" + filename);
                httpContextBase.Response.ContentType = "application/xml";

                using (StringWriter writer = new StringWriter())
                {
                    var xs = new XmlSerializer(_objectToSerialize.GetType());
                    xs.Serialize(writer, _objectToSerialize);
                    httpContextBase.Response.Write(writer);
                }
            }
        }
    }
}