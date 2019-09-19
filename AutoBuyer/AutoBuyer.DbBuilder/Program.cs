﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBuyer.DbBuilder.DTO;

namespace AutoBuyer.DbBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var futParser = new FutParsers();

                futParser.CreateFutbinDatabase();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
