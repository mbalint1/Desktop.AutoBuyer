using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using AutoBuyer.Core.Data;
using AutoBuyer.Core.Models;
using AutoBuyer.Data.DTO;
using Newtonsoft.Json;
using RestSharp;
using Player = AutoBuyer.Core.Models.Player;

namespace AutoBuyer.Core.API
{
    public class ApiProvider
    {
        public RestClient ApiClient { get; }

        public ApiProvider()
        {
            //TODO: move this url somewhere else
            ApiClient = new RestClient(ConfigurationManager.AppSettings["baseApiUrl"]);
        }

        public void InsertTransactionLog(TransactionLog log, string token)
        {
            var request = new RestRequest("/api/transactions/", Method.POST);
            var json = JsonConvert.SerializeObject(log);
            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.AddParameter("Authorization", "Bearer " + token, ParameterType.HttpHeader);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var response = ApiClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var authResponse = JsonConvert.DeserializeObject<TransactionLog>(response.Content);
                }
            }
            catch (Exception e)
            {
            }
        }

        public string TokenAuthenticate(string user, string password)
        {
            var token = string.Empty;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Must provide user and password");
            }

            var request = new RestRequest("/auth/token/", Method.POST);

            var authBody = new AuthRequest
            {
                Username = user,
                Password = password,
                AppName = "Desktop.AutoBuyer"
            };

            var json = JsonConvert.SerializeObject(authBody);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var response = ApiClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(response.Content);
                    token = authResponse.AccessToken;
                }
            }
            catch (Exception e)
            {
            }

            return token;
        }

        public List<AutoBuyer.Data.DTO.Player> GetAllPlayers(string token)
        {
            var players = new List<AutoBuyer.Data.DTO.Player>();

            var request = new RestRequest("/api/Players/", Method.GET);
            request.AddParameter("Authorization", "Bearer " + token, ParameterType.HttpHeader);

            try
            {
                var response = ApiClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var data = response.Content;

                    var dbPlayers = JsonConvert.DeserializeObject<List<AutoBuyer.Data.DTO.Player>>(data);

                    players.AddRange(dbPlayers);
                }
            }
            catch (Exception e)
            {
            }

            return players;
        }

        public void EndSession()
        {
            var request = new RestRequest("/api/sessions/", Method.PUT);
            var json = JsonConvert.SerializeObject(CurrentSession.Current);
            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.AddParameter("Authorization", "Bearer " + CurrentSession.Current?.AccessToken, ParameterType.HttpHeader);
            request.RequestFormat = DataFormat.Json;

            try
            {
                ApiClient.Execute(request);
            }
            catch (Exception ex)
            {
                //TODO: log? throw?
            }
        }

        public bool TryLockPlayerForSearch()
        {
            bool gotPlayer = false;

            var request = new RestRequest("/api/sessions/", Method.POST);
            var json = JsonConvert.SerializeObject(CurrentSession.Current);
            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.AddParameter("Authorization", "Bearer " + CurrentSession.Current.AccessToken, ParameterType.HttpHeader);
            request.RequestFormat = DataFormat.Json;

            try
            {
                var response = ApiClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var authResponse = JsonConvert.DeserializeObject<SessionDTO>(response.Content);

                    if (int.TryParse(authResponse.SessionId, out var result))
                    {
                        CurrentSession.Current.SessionId = authResponse.SessionId;
                        gotPlayer = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: log? throw?
            }

            return gotPlayer;
        }

        public void SendMessage(string subject, string message)
        {
            //TODO: Currently this endpoint does not work because Azure blocks all SMTP traffic by default.
            //TODO: Commenting out this code because I would like to come back to it if we can find a workaround in Azure. 
            //var request = new RestRequest("/message", Method.POST);

            //var messageBody = new Message
            //{
            //    Subject = subject,
            //    Body = message
            //};

            //var json = JsonConvert.SerializeObject(messageBody);

            //request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            //request.AddParameter("Authorization", "Bearer " + CurrentSession.Current.AccessToken, ParameterType.HttpHeader);
            //request.RequestFormat = DataFormat.Json;

            //try
            //{
            //    var response = ApiClient.Execute(request);

            //    if (response.StatusCode == HttpStatusCode.OK)
            //    {
            //        // return a bool maybe?
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //TODO: log? throw?
            //}

            //TODO: We are going to directly send the message from here. This needs to be a temporary fix

            var request = new RestRequest("/message/data", Method.GET);
            request.AddParameter("Authorization", "Bearer " + CurrentSession.Current.AccessToken, ParameterType.HttpHeader);

            try
            {
                var response = ApiClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var data = response.Content;
                    var cleaned = data.Replace("\"", "");

                    var stuffs = cleaned.Split(' ');

                    try
                    {
                        var client = new SmtpClient("smtp.gmail.com", 587)
                        {
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(stuffs[1], stuffs[0]),
                            EnableSsl = true
                        };

                        client.Send(stuffs[1], stuffs[3], subject, message);
                    }
                    catch (Exception ex)
                    {
                        //TODO: Logging
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}