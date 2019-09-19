using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoBuyer.DbBuilder.DTO;
using Npgsql;

namespace AutoBuyer.DbBuilder
{
    public class PostgresProvider
    {
        public void InsertPlayers(List<Player> players)
        {
            var connString = "Host=;Username=;Password=;Database=;SslMode=Require";

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    for (int i = 0; i < players.Count; i++)
                    {
                        var player = players[i];

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = Queries.InsertPlayerVersion;
                            Queries.AddVersionParameters(cmd, player.Versions.First());
                            if (i == 0)
                            {
                                cmd.Prepare();
                            }

                            player.Versions.First().Id = cmd.ExecuteScalar()?.ToString();
                        }

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = Queries.InsertPlayers;
                            Queries.AddPlayerParameters(cmd, player);
                            if (i == 0)
                            {
                                cmd.Prepare();
                            }

                            player.Id = cmd.ExecuteScalar()?.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Log
            }
        }
    }
}