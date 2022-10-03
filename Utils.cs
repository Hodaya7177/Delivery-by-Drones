using System;
namespace Tools
{
    public static class Utils
    {
        /// <summary>
        /// the function receive two locations and calculates the difference between them
        /// https://stackoverflow.com/questions/18883601/function-to-calculate-distance-between-two-coordinates
        /// </summary>
        /// <param name="currentLocation"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static double DistanceCalculation(double lat1, double lon1, double lat2, double lon2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515 * 1.609344;
            return dist;
        }
    }
}
