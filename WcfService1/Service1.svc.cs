using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WcfService1
{
    public class Service1 : IService1
    {
        public string WebDownload(string url)
        {
            try
            {
                WebClient channel = new WebClient();
                string content = channel.DownloadString(url);
                return content;
            }
            catch (Exception ex)
            {
                return "Error downloading content: " + ex.Message;
            }
        }
    }
}
