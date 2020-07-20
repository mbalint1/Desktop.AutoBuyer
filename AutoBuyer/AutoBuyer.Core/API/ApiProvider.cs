using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
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

        public void UpdateSession(SessionInfo sessionInfo, string token)
        {
        }

        public bool TryLockPlayerForSearch(SessionInfo sessionInfo, string token)
        {
            CurrentSession.PlayerVersionId = sessionInfo.PlayerVersionId;
            CurrentSession.Token = token;

            return false;
        }
    }
}