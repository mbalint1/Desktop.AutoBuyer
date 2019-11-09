using System;
using System.Collections.Generic;
using System.Linq;
using AutoBuyer.Data.DTO;
using AutoBuyer.Data.Interfaces;

namespace AutoBuyer.Data.Utilities
{
    public class FutwizParser : IFutParser
    {
        public const string FutwizGoldBase = "https://www.futwiz.com/en/fifa20/players?minrating=75&maxrating=99&release=nonrare&page=";
        public const string FutwizGoldRareBase = "https://www.futwiz.com/en/fifa20/players?minrating=75&maxrating=99&release=rare&page=";
        public const int FutwizGoldPageLimit = 48;
        public const int FutwizGoldRarePageLimit = 32;

        public IWebUtility WebUtility { get; }

        public FutwizParser(IWebUtility webUtility)
        {
            WebUtility = webUtility;
        }

        public List<Player> GetAllFutPlayers()
        {
            throw new NotImplementedException();
        }

        public void CreateFutWizCsv()
        {
            try
            {
                for (int i = 0; i <= FutwizGoldPageLimit; i++)
                {
                    var pageResults = WebUtility.GetRestResponse(FutwizGoldBase + i);
                    var players = ParseFutwizPage(pageResults);
                }
                for (int i = 0; i <= FutwizGoldRarePageLimit; i++)
                {
                    var pageResults = WebUtility.GetRestResponse(FutwizGoldRareBase + i);
                    var players = ParseFutwizPage(pageResults);
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
    }
}