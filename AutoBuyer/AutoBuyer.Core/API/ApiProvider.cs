using System;
using System.Configuration;
using System.Net;
using AutoBuyer.Data.DTO;
using Newtonsoft.Json;
using RestSharp;

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
                //ApiClient.ExecuteAsync(request, response =>
                //{
                //    if (response.StatusCode == HttpStatusCode.OK)
                //    {
                //        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(response.Content);
                //        token = authResponse.AccessToken;
                //    }
                //});

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
    }
}