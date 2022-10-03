using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public override string ToString()
        {
            return string.Format("\n{0}\n{1}\n", longSexagesimal(Longitude), latSexagesimal(Latitude));
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
