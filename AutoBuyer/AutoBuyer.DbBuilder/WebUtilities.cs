using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using AutoBuyer.DbBuilder.DTO;

namespace AutoBuyer.DbBuilder
{
    public class WebUtilities
    {
        public string GetRestResponse(string requestUrl)
        {
            Thread.Sleep(1000);

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