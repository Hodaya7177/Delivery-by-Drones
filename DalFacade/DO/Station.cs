using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public struct Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int AvailableChargeSlots { get; set; }
        public bool isActive { get; set; }

        public override string ToString()
        {
            return string.Format("Id is: {0}\nName of station: {1}\nLongitude is: {2}\nLatitude is: {3}\nNum of charge slots: {4}\n", Id, Name, longSexagesimal(Longitude), latSexagesimal(Latitude), AvailableChargeSlots);
        }


        // BONUS:
        // the functions below convert coordinates to base 60

        public string longSexagesimal(double longitude)
        {
            double absValOfDegree = Math.Abs(longitude);
            double minute = (absValOfDegree - (int)absValOfDegree) * 60;
            return string.Format("{0}°{1}\' {2}\"{3}", (int)longitude, (int)(minute), Math.Round((minute - (int)minute) * 60), longitude < 0 ? "S" : "N");
        }

        public string latSexagesimal(double latitude)
        {
            double absValOfDegree = Math.Abs(latitude);
            double minute = (absValOfDegree - (int)absValOfDegree) * 60;
            return string.Format("{0}°{1}\' {2}\"{3}", (int)latitude, (int)(minute), Math.Round((minute - (int)minute) * 60), latitude < 0 ? "W" : "E");
        }
    }
}
