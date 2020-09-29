using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AutoBuyer.Data.DTO;
using AutoBuyer.Data.Interfaces;

namespace AutoBuyer.Data.Utilities
{
    public class FutbinParser : IFutParser
    {
        public const string FutbinPlayerBase = "https://www.futbin.com/21/player/";
        public const string FutbinGoldBase = "https://www.futbin.com/21/players?version=gold_nr&page=";
        public const string FutbinRareGoldBase = "https://www.futbin.com/21/players?version=gold_rare&page=";
        public const int FutbinGoldPageLimit = 37;
        public const int FutbinGoldRarePageLimit = 25;

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

            const string cardIdentifier = "<a href=\"/21/player/";

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


        public static string RemoveDiacritics(string stIn)
        {
            var stFormD = stIn.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var t in stFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string RemoveNonASCII(string stIn)
        {
            // Equivilent of the first 255 characters in utf-8 ascii characters.
            var result = Regex.Replace(stIn, @"[^\u0000-\u007F]", " ");
            return result;
        }
    }
}