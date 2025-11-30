using System;
using System.Net;
using System.ServiceModel.Activation;

namespace WcfService1
{
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
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
