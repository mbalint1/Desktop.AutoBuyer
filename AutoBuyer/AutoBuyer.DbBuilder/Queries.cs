using System;
using System.Collections.Generic;
using System.Linq;
using AutoBuyer.DbBuilder.DTO;
using Npgsql;

namespace AutoBuyer.DbBuilder
{
    public static class Queries
    {
        public static string InsertPlayers = @"INSERT INTO public.""Players"" (""Version_Id"", ""Name"", ""Created_By"", ""Created_Date"", ""Modified_By"", ""Modified_Date"") VALUES (@version, @name, @createdBy, @createdDate, @modifiedBy, @modifiedDate) RETURNING ""Player_Id"";";
        public static string InsertPlayerVersion = @"INSERT INTO public.""Player_Version"" (""Fut_Id"", ""Player_Type"", ""Rating"", ""Position"", ""Created_By"", ""Created_Date"", ""Modified_By"", ""Modified_Date"") VALUES (@futId, @playerType, @rating, @position, @createdBy, @createdDate, @modifiedBy, @modifiedDate) RETURNING ""Version_Id"";";

        public static void AddVersionParameters(NpgsqlCommand cmd, PlayerVersion version)
        {
            cmd.Parameters.AddWithValue("futId", Convert.ToInt32(version.ThirdPartyId));
            cmd.Parameters.AddWithValue("playerType", version.Type.ToString());
            cmd.Parameters.AddWithValue("rating", version.Rating);
            cmd.Parameters.AddWithValue("position", version.Position);
            cmd.Parameters.AddWithValue("createdBy", version.CreatedBy);
            cmd.Parameters.AddWithValue("createdDate", version.CreatedDate);
            cmd.Parameters.AddWithValue("modifiedBy", version.ModifiedBy);
            cmd.Parameters.AddWithValue("modifiedDate", version.ModifiedDate);
        }

        public static void AddPlayerParameters(NpgsqlCommand cmd, Player player)
        {
            cmd.Parameters.AddWithValue("version", Convert.ToInt32(player.Versions.First().Id));
            cmd.Parameters.AddWithValue("name", player.Name);
            cmd.Parameters.AddWithValue("createdBy", player.CreatedBy);
            cmd.Parameters.AddWithValue("createdDate", player.CreatedDate);
            cmd.Parameters.AddWithValue("modifiedBy", player.ModifiedBy);
            cmd.Parameters.AddWithValue("modifiedDate", player.ModifiedDate);
        }
    }
}