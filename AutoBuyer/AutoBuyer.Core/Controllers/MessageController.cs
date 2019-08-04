using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace AutoBuyer.Core.Controllers
{
    public class MessageController
    {
        public void SendEmail(string sendFrom, string sendTo, string subject, string message)
        {
            try
            {
                var stuffs = File.ReadAllText(ConfigurationManager.AppSettings["pFile"]).Trim().Split(',');

                if (stuffs.Length < 2)
                {
                    //TODO: throw some sort of configuration error
                    return;
                }

                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(stuffs[1], stuffs[0]),
                    EnableSsl = true
                };

                client.Send(sendFrom, sendTo, subject, message);
            }
            catch (Exception ex)
            {
                //TODO: Logging
            }
        }
    }
}