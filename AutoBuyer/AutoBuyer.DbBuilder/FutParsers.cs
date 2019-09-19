using System;
using System.Collections.Generic;
using System.Linq;
using AutoBuyer.DbBuilder.DTO;

namespace AutoBuyer.DbBuilder
{
    public class FutParsers
    {
        public const string FutbinPlayerBase = "https://www.futbin.com/20/player/";
        public const string FutbinGoldBase = "https://www.futbin.com/20/players?version=gold_nr&page=";
        public const string FutbinRareGoldBase = "https://www.futbin.com/20/players?version=gold_rare&page=";
        public const int FutbinGoldPageLimit = 41;
        public const int FutbinGoldRarePageLimit = 27;

        public const string FutwizGoldBase = "https://www.futwiz.com/en/fifa20/players?minrating=75&maxrating=99&release=nonrare&page=";
        public const string FutwizGoldRareBase = "https://www.futwiz.com/en/fifa20/players?minrating=75&maxrating=99&release=rare&page=";
        public const int FutwizGoldPageLimit = 48;
        public const int FutwizGoldRarePageLimit = 32;

        public WebUtilities WebUtility { get; }

        public DbUtilities DbUtility { get; }

        public FutParsers()
        {
            WebUtility = new WebUtilities();
            DbUtility = new DbUtilities();
        }

        public void CreateFutbinDatabase()
        {
            try
            {
                for (int i = 0; i <= FutbinGoldPageLimit; i++)
                {
                    var pageResults = WebUtility.GetRestResponse(FutbinGoldBase + i);
                    var players = ParseFutbinPage(pageResults, PlayerType.Gold);
                    DbUtility.SavePlayers(players);
                }
            }
            catch (Exception ex)
            {
                //TODO: Logging
            }
        }

        private List<Player> ParseFutbinPage(string rawPageData, PlayerType playerType)
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

                    players.Add(player);
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
            const string nameIdentifier = "<title>FIFA 20 Player - ";

            var lines = rawPlayerData.Split('\n').ToList();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith(ratingIdentifier))
                {
                    version.Rating = Convert.ToInt32(trimmed.Substring(ratingIdentifier.Length, 2));
                }
                else if (trimmed.StartsWith(positionIdentifier))
                {
                    var length = trimmed.LastIndexOf('<') - positionIdentifier.Length;

                    version.Position = trimmed.Substring(positionIdentifier.Length, length);
                }
                else if (trimmed.StartsWith(nameIdentifier))
                {
                    var length = trimmed.LastIndexOf('|') - nameIdentifier.Length;

                    var rawName = trimmed.Substring(nameIdentifier.Length, length);

                    player.Name = RemoveDiacritics(rawName.Trim());
                }
            }

            player.Versions.Add(version);

            return player;
        }

        private string RemoveDiacritics(string text)
        {
            return System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding(1251).GetBytes(text));
        }

        #region Unused Futwiz

        public void CreateFutWizCsv()
        {
            try
            {
                for (int i = 0; i <= FutwizGoldPageLimit; i++)
                {
                    var pageResults = WebUtility.GetRestResponse(FutwizGoldBase + i);
                    var players = ParseFutwizPage(pageResults);
                    DbUtility.WritePlayerFile(players);
                }
                for (int i = 0; i <= FutwizGoldRarePageLimit; i++)
                {
                    var pageResults = WebUtility.GetRestResponse(FutwizGoldRareBase + i);
                    var players = ParseFutwizPage(pageResults);
                    DbUtility.WritePlayerFile(players);
                }
            }
            catch (Exception ex)
            {
                //TODO: Logging
            }
        }

        private List<string> ParseFutwizPage(string rawPageData)
        {
            //<div class="card-20-pack-face"><div class="card-20-pack-face-inner"><img src="/assets/img/fifa20/faces/202126.png" alt="Harry Kane 89 Rated" /></div></div>

            const string cardIdentifier = "<div class=\"card-20-pack-face\">";
            const string apostrophe = "&#039;";

            var split = rawPageData.Split('\n').ToList();

            var players = new List<string>();

            for (int i = split.Count - 1; i >= 0; i--)
            {
                if (!split[i].StartsWith(cardIdentifier))
                {
                    split.RemoveAt(i);
                }
            }

            foreach (var nameRow in split)
            {
                var startIndex = nameRow.LastIndexOf("alt=", StringComparison.Ordinal) + 5;
                var endIndex = nameRow.LastIndexOf("Rated", StringComparison.Ordinal) - 1;

                var nameAndRating = nameRow.Substring(startIndex, endIndex - startIndex);

                var words = nameAndRating.Split(' ');
                var rating = words[words.Length - 1];
                var name = string.Join(" ", words.Reverse().Skip(1).Reverse());

                if (name.Contains(apostrophe))
                {
                    name = name.Replace(apostrophe, "'");
                }

                players.Add($"{name},{rating}");
            }

            return players;
        }

        #endregion Unused Futwiz
    }
}