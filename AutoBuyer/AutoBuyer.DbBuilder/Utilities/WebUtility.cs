using System;
using System.IO;
using System.Net;
using System.Threading;
using AutoBuyer.Data.Interfaces;

namespace AutoBuyer.Data.Utilities
{
    public class WebUtilities : IWebUtility
    {
        public string GetRestResponse(string requestUrl)
        {
            Thread.Sleep(1500);

            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            var content = string.Empty;
            request.Method = "GET";
            try
            {
                using (var response = request.GetResponse())
                {
                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream);
                        content = reader.ReadToEnd();
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Log
            }
            return content;
        }
    }
}