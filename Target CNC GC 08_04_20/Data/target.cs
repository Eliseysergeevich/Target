﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Target_CNC_GC_08_04_20
{
   
    public static class Field
    {
        public static double nmpLat;// = 86.391;
        public static double nmpLon;// = 169.818;
        public static double startLatitude;// = 55.812264;
        public static double startLongitude;// = 38.04094;
        public static int heightField; //длина (высота) поля
        public static int widthField;//ширина поля

        public static double DistanceField(double latitude, double longitude, double latitudeStart, double longitudeStart)
        {
            double sin1 = Math.Sin((latitude - latitudeStart) * Math.PI / 180.0 / 2);
            double sin2 = Math.Sin((longitude - longitudeStart) * Math.PI / 180.0 / 2);
            double cos2 = Math.Cos(latitude * Math.PI / 180.0);
            double cos1 = Math.Cos(longitude * Math.PI / 180.0);
            double sin12 = sin1 * sin1;
            double sin22 = sin2 * sin2;
            double sum = sin12 + cos1 * cos2 * sin22;
            double sumsqrt = Math.Sqrt(sum);
            double an = 2 * Math.Asin(sumsqrt);
            double s1 = 6372795 * an; ;
            return s1;

        }
        public static double DistanceCulc(double latitude, double longitude, double latitudeStart, double longitudeStart)
        {
            double lat1Rad = latitudeStart * Math.PI / 180.0;
            double long1Rad = longitudeStart * Math.PI / 180.0;
            double lat2Rad = latitude * Math.PI / 180.0;
            double long2Rad = longitude * Math.PI / 180.0;
            double sinlat1Rad = Math.Sin(lat1Rad);
            double sinlat2Rad = Math.Sin(lat2Rad);
            double coslat1Rad = Math.Cos(lat1Rad);
            double coslat2Rad = Math.Cos(lat2Rad);
            double sinDeltaLong = Math.Sin(long2Rad-long1Rad);
            double cosDeltaLong = Math.Cos(long2Rad - long1Rad);
            double temp1 = coslat2Rad * sinDeltaLong;
            temp1 = temp1 * temp1;
            double temp2 = coslat1Rad * sinlat2Rad;
            double temp3 = sinlat1Rad * coslat2Rad;
            temp3 = temp3 * cosDeltaLong;
            temp3 = temp2 - temp3;
            temp3 = temp3 * temp3;
            temp3 = temp1 + temp3;
            temp3 = Math.Sqrt(temp3);
            double temp4 = sinlat1Rad * sinlat2Rad;
            double temp5 = coslat1Rad * coslat2Rad;
            temp5 = temp5 * cosDeltaLong;
            temp5 = temp4 + temp5;
            double temp6 = temp3 / temp5;
            temp6 = Math.Atan(temp6);
            double s = 6372795 * temp6;
            return s;

        }
        public static double DistanceRadCulc(double latitude, double longitude, double latitudeStart, double longitudeStart)
        {
            double lat1Rad = latitudeStart * Math.PI / 180.0;
            double long1Rad = longitudeStart * Math.PI / 180.0;
            double lat2Rad = latitude * Math.PI / 180.0;
            double long2Rad = longitude * Math.PI / 180.0;
            double sinlat1Rad = Math.Sin(lat1Rad);
            double sinlat2Rad = Math.Sin(lat2Rad);
            double coslat1Rad = Math.Cos(lat1Rad);
            double coslat2Rad = Math.Cos(lat2Rad);
            double sinDeltaLong = Math.Sin(long2Rad - long1Rad);
            double cosDeltaLong = Math.Cos(long2Rad - long1Rad);
            double temp1 = coslat2Rad * sinDeltaLong;
            temp1 = temp1 * temp1;
            double temp2 = coslat1Rad * sinlat2Rad;
            double temp3 = sinlat1Rad * coslat2Rad;
            temp3 = temp3 * cosDeltaLong;
            temp3 = temp2 - temp3;
            temp3 = temp3 * temp3;
            temp3 = temp1 + temp3;
            temp3 = Math.Sqrt(temp3);
            double temp4 = sinlat1Rad * sinlat2Rad;
            double temp5 = coslat1Rad * coslat2Rad;
            temp5 = temp5 * cosDeltaLong;
            temp5 = temp4 + temp5;
            double temp6 = temp3 / temp5;
            double s = Math.Atan(temp6);
            return s;

        }

        public static double AngleField(double latitude, double longitude, double latitudeStart, double longitudeStart)
        {
            double lat1Rad = latitudeStart * Math.PI / 180.0;
            double long1Rad = longitudeStart * Math.PI / 180.0;
            double lat2Rad = latitude * Math.PI / 180.0;
            double long2Rad = longitude * Math.PI / 180.0;
            double sinlat1Rad = Math.Sin(lat1Rad);
            double sinlat2Rad = Math.Sin(lat2Rad);
            double coslat1Rad = Math.Cos(lat1Rad);
            double coslat2Rad = Math.Cos(lat2Rad);
            double sinDeltaLong = Math.Sin(long2Rad - long1Rad);
            double cosDeltaLong = Math.Cos(long2Rad - long1Rad);
            double temp1 = coslat1Rad * sinlat2Rad - sinlat1Rad * coslat2Rad * cosDeltaLong;
            double temp2 = sinDeltaLong * coslat2Rad;
            double temp3 = Math.Atan2(-temp2, temp1) * 180.0 / Math.PI;
            if (temp1 < 0) temp3 = temp3 + 180;
            double temp4 = -(temp3 + 180 % 360 - 180) * Math.PI / 180.0;
            double anglerad = temp4 - ((2 * Math.PI) * (Math.Floor(temp4 / (2 * Math.PI))));
            double angle = anglerad * 180.0 / Math.PI;
            return angle;
        }
        static double CalculationSMP(double latitudeStart, double longitudeStart, double latitudSMP, double longitudeSMP)
        {
            double cos2 = Math.Cos(latitudSMP * Math.PI / 180.0);
            double cos1 = Math.Cos(latitudeStart * Math.PI / 180.0);
            double sinLatS = Math.Sin(latitudeStart * Math.PI / 180.0);
            double sinLat1 = Math.Sin(latitudSMP * Math.PI / 180.0);
            double sinDeltaLong = Math.Sin((longitudeSMP - longitudeStart) * Math.PI / 180.0);
            double cosDeltaLong = Math.Cos((longitudeSMP - longitudeStart) * Math.PI / 180.0);
            double tgDegr = (cos2 * sinDeltaLong) / (cos1 * sinLat1 - sinLatS * cos2 * cosDeltaLong);
            double deg = Math.Atan(tgDegr) * 180 / Math.PI;
            return deg;
        }
    }
    public class Target
    {

        public static string[] tupeOfTargetArray = { "Гонг 200", "Гонг 250", "Гонг 300", "Гонг 350", "Гонг 400", " Мишень №5", "Мишень №5а", "Мишень №6", "Мишень №8" };
        //string nameTarget;//Номер мишени
        //int nomberSensorsBlock;
        //int nomberIndicationBlock;
        //double latitude, longitude; //Широта и долгота
        //bool north; //северная - true
        //bool east;// восточная - true
        //int typeOfTarget;
        //public string name;
        

        public string NameTarget {get;set;}
        public int NomberSensorsBlock { get; set; }
        public int NomberIndicationBlock { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TypeOfTarget { get; set; }
        public bool NorthLatitude { get; set; }
        public bool EastLongitude { get; set; }

        public double Distance(double latitude, double longitude, double latitudeStart, double longitudeStart)
        {
            double sin1 = Math.Sin((latitude - latitudeStart)*Math.PI/180.0/2);
            double sin2 = Math.Sin((longitude - longitudeStart)*Math.PI/180.0/2);
            double cos2 = Math.Cos(latitude*Math.PI/180.0);
            double cos1 = Math.Cos(longitude*Math.PI / 180.0);
            double sin12 =sin1*sin1;
            double sin22 = sin2*sin2;
            double sum = sin12 + cos1 * cos2 * sin22;
            double sumsqrt = Math.Sqrt(sum);
            double an = 2 * Math.Asin(sumsqrt);
            double s1 = 6372795 * an; ;
            return s1;

        }
        public Target(string name, int nsb, int nib, int typrOfTarget)
        {
            NameTarget = name;
            NomberSensorsBlock = nsb;
            NomberIndicationBlock = nib;
            Latitude = Field.startLatitude;
            Longitude = Field.startLongitude;
            TypeOfTarget = tupeOfTargetArray[typrOfTarget];
        }
        public Target(string name, int nsb, int nib, double latitude, double longitude, int typrOfTarget)
        {
            NameTarget = name;
            NomberSensorsBlock = nsb;
            NomberIndicationBlock = nib;
            Latitude = latitude;
            Longitude = longitude;
            TypeOfTarget = tupeOfTargetArray[typrOfTarget];
        }
    }
}
