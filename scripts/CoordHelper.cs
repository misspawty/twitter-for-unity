//Standard Namespaces
using UnityEngine;
using System;

namespace Utils
{
    /// <summary>
    /// Coordinate Conversions according to page 38 of the Ordnance Survey's guide to crs in Britain.
    ///
    /// </summary> 
    /// <revisionHistory visible="true">
    ///  <revision date = "2016-04-12" version="0.6.1.0" author="Jascha Gruebel" visible="true" >
    ///    - Rename functions to correspond to C# style as well as to offer more functional naming.
    ///  </revision>
    ///  <revision date = "2016-04-11" version="0.6.0.1"  author="Jascha Gruebel" visible="true">
    ///    - Added comments about classes and functions.
    ///    - Renamed to CoordHelper (previously CoordConv).
    ///  </revision>
    public static class CoordHelper
    {
        /// <summary>
        /// Converts a WGS84 coordinates into a Cartesian system.
        /// </summary>
        /// <param name="coords">WGS84 coordinates</param>
        /// <returns>Cartesian coordinates</returns>
        public static Vector3 WGS84toCartesian(Vector3 coords)
        {
            // doing calculations in double precision for greater accuracy along the way
            double lon = coords.x;
            double ht = coords.y;
            double lat = coords.z;

            // semi major axis = 6378137
            // first eccentricity squared = 0.006694380004260827
            double v = 6378137 / (Math.Sqrt(1 - (0.006694380004260827 * Math.Sin(lat) * Math.Sin(lat))));
            double x = (v + ht) * Math.Cos(lat) * Math.Cos(lon);
            double y = (v + ht) * Math.Cos(lat) * Math.Sin(lon);
            double z = ((1 - 0.006694380004260827) * v + ht) * Math.Sin(lat);
            return new Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Converts a WGS84 coordinates into WebMercator coordinates.
        /// 
        /// http://www.gal-systems.com/2011/07/convert-coordinates-between-web.html
        /// which in turn is based on ESRI formula
        /// </summary>
        /// <remarks>
        /// Height remains the same in both formats.
        /// </remarks>
        /// <param name="coords">WGS84 coordinates</param>
        /// <returns>WebMercator coordiantes</returns>
        public static Vector3 WGS84toWebMercator(Vector3 coords)
        {
            // doing calculations in double precision for greater accuracy along the way
            double lonWGS84 = (double)(coords.x);
            double latWGS84 = (double)(coords.z);

            double num = lonWGS84 * 0.017453292519943295;
            double x = 6378137.0 * num;
            double a = latWGS84 * 0.017453292519943295;
            double lon = x;
            double lat = 3189068.5 * Math.Log((1.0 + Math.Sin(a)) / (1.0 - Math.Sin(a)));
            return new Vector3((float)lon, coords.y, (float)lat);
        }

        /// <summary>
        /// Converts a WebMercator coordinates into WGS84 coordinates.
        /// </summary>
        /// <remarks>
        /// Height remains the same in both formats.
        /// </remarks>
        /// <param name="coords">WebMercator coordinates</param>
        /// <returns>WGS84 coordinates</returns>
        public static Vector3 WebMercatorToWGS84(Vector3 coords)
        {
            // doing calculations in double precision for greater accuracy along the way...?
            double lon = (double)(coords.x);
            double lat = (double)(coords.z);

            double num3 = lon / 6378137.0;
            double num4 = num3 * 57.295779513082323;
            double num5 = Math.Floor((double)((num4 + 180.0) / 360.0));
            double num6 = num4 - (num5 * 360.0);
            double num7 = 1.5707963267948966 - (2.0 * Math.Atan(Math.Exp((-1.0 * lat) / 6378137.0)));
            double lonWGS84 = num6;
            double latWGS84 = num7 * 57.295779513082323;

            return new Vector3((float)lonWGS84, coords.y, (float)latWGS84);
        }
    }
}