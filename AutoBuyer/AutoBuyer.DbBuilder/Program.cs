using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuyer.DbBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            const string rareBaseUrl = "https://www.futwiz.com/en/fifa20/players?minrating=75&maxrating=99&release=rare&page=";
            const string nonRareBaseUrl = "https://www.futwiz.com/en/fifa20/players?minrating=75&maxrating=99&release=nonrare&page=";

            const int rarePageLimit = 32;
            const int nonRarePageLimit = 48;

            try
            {
                var webUtility = new WebUtilities();
                var dbUtility = new DbUtilities();

                for (int i = 0; i <= rarePageLimit; i++)
                {
                    var pageResults = webUtility.GetRestResponse(rareBaseUrl + i);
                    var players = webUtility.PageToPlayerCsvList(pageResults);
                    dbUtility.WritePlayerFile(players);
                }

                for (int i = 0; i <= nonRarePageLimit; i++)
                {
                    var pageResults = webUtility.GetRestResponse(nonRareBaseUrl + i);
                    var players = webUtility.PageToPlayerCsvList(pageResults);
                    dbUtility.WritePlayerFile(players);
                }
            }
            catch (Exception ex)
            {
                //TODO: Logging
            }
        }
    }
}
