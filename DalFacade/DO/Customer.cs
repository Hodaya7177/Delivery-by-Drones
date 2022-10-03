using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public struct Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool isActive { get; set; }
        //public string Password { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Id is: {0}\n" +
                "Name of customer: {1}\n" +
                "Phone number: {2}\n" +
                "longitude is: {3}\n" +
                "latitude: {4}\n",
                Id, Name, Phone, longSexagesimal(Longitude), latSexagesimal(Latitude));
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
