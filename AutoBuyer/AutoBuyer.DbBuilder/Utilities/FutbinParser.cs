﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoBuyer.Data.DTO;
using AutoBuyer.Data.Interfaces;

namespace AutoBuyer.Data.Utilities
{
    public class FutbinParser : IFutParser
    {
        public const string FutbinPlayerBase = "https://www.futbin.com/20/player/";
        public const string FutbinGoldBase = "https://www.futbin.com/20/players?version=gold_nr&page=";
        public const string FutbinRareGoldBase = "https://www.futbin.com/20/players?version=gold_rare&page=";
        public const int FutbinGoldPageLimit = 41;
        public const int FutbinGoldRarePageLimit = 27;

        private IWebUtility WebUtility { get; }

        private Dictionary<string, int> PlayerData { get; set; } = new Dictionary<string, int>();

        public FutbinParser(IWebUtility webUtility)
        {
            WebUtility = webUtility;
        }

        public List<Player> GetAllFutPlayers()
        {
            var players = new List<Player>();

            try
            {
                for (int i = 1; i <= FutbinGoldRarePageLimit; i++)
                {
                    var pageResults = WebUtility.GetRestResponse(FutbinRareGoldBase + i);
                    players.AddRange(ParseFutPage(pageResults, PlayerType.RareGold));
                }

                for (int i = 1; i <= FutbinGoldPageLimit; i++)
                {
                    var pageResults = WebUtility.GetRestResponse(FutbinGoldBase + i);
                    players.AddRange(ParseFutPage(pageResults, PlayerType.Gold));
                }
            }
            catch (Exception ex)
            {
                //TODO: Logging
            }

            return players;
        }

        private List<Player> ParseFutPage(string rawPageData, PlayerType playerType)
        {
            var players = new List<Player>();

            const string cardIdentifier = "<a href=\"/20/player/";

            var split = rawPageData.Split('\n').ToList();

            foreach (var line in split)
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith(cardIdentifier))
                {
                    var lastIndex = trimmed.IndexOf(cardIdentifier, StringComparison.Ordinal);
                    var idAndRest = trimmed.Substring(lastIndex + cardIdentifier.Length);

                    var futbinId = idAndRest.Substring(0, idAndRest.IndexOf('/'));

                    var rawPlayerData = WebUtility.GetRestResponse(FutbinPlayerBase + futbinId);

                    var player = PlayerFromFutbinData(rawPlayerData, futbinId, playerType);

                    if (player.Versions.Any())
                    {
                        players.Add(player);
                    }
                }
            }

            return players;
        }

        private Player PlayerFromFutbinData(string rawPlayerData, string futbinId, PlayerType playerType)
        {
            var player = new Player
            {
                CreatedBy = "AutoBuyer.DbBuilder",
                ModifiedBy = "AutoBuyer.DbBuilder",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            var version = new PlayerVersion
            {
                ThirdPartyId = futbinId,
                Type = playerType,
                CreatedBy = "AutoBuyer.DbBuilder",
                ModifiedBy = "AutoBuyer.DbBuilder",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };


            const string ratingIdentifier = "<div style=\"color:;\" class=\"pcdisplay-rat\">";
            const string positionIdentifier = "<div style=\"color:;\" class=\"pcdisplay-pos\">";
            const string nameIdentifier = "<div class=\"hidden\" id=\"baseurl\" data-url=\"player/";

            var lines = rawPlayerData.Split('\n').ToList();

            bool hasRating = false;
            bool hasPosition = false;
            bool hasName = false;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (!hasRating && trimmed.StartsWith(ratingIdentifier))
                {
                    var rating = Convert.ToInt32(trimmed.Substring(ratingIdentifier.Length, 2));

                    version.Rating = rating;
                    hasRating = true;
                }
                else if (!hasPosition && trimmed.StartsWith(positionIdentifier))
                {
                    var length = trimmed.LastIndexOf('<') - positionIdentifier.Length;

                    version.Position = trimmed.Substring(positionIdentifier.Length, length);
                    hasPosition = true;
                }
                else if (trimmed.StartsWith(nameIdentifier))
                {
                    var completeIdentifier = $"{nameIdentifier}{futbinId}/";
                    var length = trimmed.LastIndexOf('"') - completeIdentifier.Length;

                    var name = trimmed.Substring(completeIdentifier.Length, length).Trim();

                    player.Name = name;
                    hasName = true;
                }

                //Doing this because many pages have previous year's info and we don't want those to overwrite
                if (hasRating && hasPosition && hasName)
                {
                    break;
                }
            }

            var ratingTemp = version.Rating;
            var alreadyExists = PlayerData.TryGetValue(player.Name, out ratingTemp);

            if (!alreadyExists)
            {
                PlayerData.Add(player.Name, version.Rating);
                player.Versions.Add(version);
            }

            return player;
        }

        private string RemoveDiacritics(string text)
        {
            return System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding(1251).GetBytes(text));
        }
    }
}