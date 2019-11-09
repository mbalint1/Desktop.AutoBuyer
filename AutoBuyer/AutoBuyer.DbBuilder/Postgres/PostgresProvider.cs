using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using AutoBuyer.Data.DTO;
using AutoBuyer.Data.Interfaces;
using Npgsql;

namespace AutoBuyer.Data.Postgres
{
    public class PostgresProvider : IDbProvider
    {
        private string _connString;

        public PostgresProvider()
        {
            var stuffs = File.ReadAllText(ConfigurationManager.AppSettings["pFile"]).Trim().Split(',');

            if (stuffs.Length < 4)
            {
                throw new ConfigurationErrorsException("Configuration missing required elements");
            }
            else
            {
                _connString = stuffs[3];
            }
        }

        public void InsertPlayers(List<Player> players)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    for (int i = 0; i < players.Count; i++)
                    {
                        var player = players[i];

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = Queries.InsertPlayers;
                            Queries.AddPlayerParameters(cmd, player);
                            if (i == 0)
                            {
                                cmd.Prepare();
                            }

                            var playerId = cmd.ExecuteScalar()?.ToString();

                            player.Id = playerId;
                        }

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = Queries.InsertPlayerVersion;
                            Queries.AddVersionParameters(cmd, player);
                            if (i == 0)
                            {
                                cmd.Prepare();
                            }

                            player.Versions.First().VersionId = cmd.ExecuteScalar()?.ToString();
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Log
                throw;
            }
        }

        public void InsertTransactionLogs(List<TransactionLog> logs)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connString))
                {
                    conn.Open();

                    for (int i = 0; i < logs.Count; i++)
                    {
                        var log = logs[i];

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = Queries.InsertTransactionLogs;
                            Queries.AddTransactionLogParams(cmd, log);
                            if (i == 0)
                            {
                                cmd.Prepare();
                            }

                            var transactionId = cmd.ExecuteScalar()?.ToString();

                            log.TransactionId = transactionId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Log
                throw;
            }
        }
    }
}