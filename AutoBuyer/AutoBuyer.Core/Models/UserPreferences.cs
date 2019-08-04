using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBuyer.Core.Models
{
    public class UserPreferences
    {
        public bool FirstTimeUser { get; set; } //TODO: Make setters private

        public bool ValidButtonCoordinates { get; set; } //TODO: Make setters private
    }
}