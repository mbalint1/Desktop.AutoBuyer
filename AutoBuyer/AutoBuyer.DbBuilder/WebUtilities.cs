using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace AutoBuyer.DbBuilder
{
    public class WebUtilities
    {
        public string GetRestResponse(string requestUrl)
        {
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            var content = string.Empty;
            request.Method = "GET";
            try
            {
                using (var response = request.GetResponse())
                {
                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream);
                        content = reader.ReadToEnd();
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Log
            }
            return content;
        }

        public List<string> PageToPlayerCsvList(string pageData)
        {
            return FutwizParser(pageData);
        }

        private List<string> FutwizParser(string rawPageData)
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