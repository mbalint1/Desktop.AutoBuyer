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
            var futParser = new FutParsers();

            futParser.CreateFutbinDatabase();
        }
    }
}
