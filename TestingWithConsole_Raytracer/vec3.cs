using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestingWithConsole_Raytracer
{
    internal class Vec3 : IEquatable<Vec3>  //  NOT IN ACTUAL CODE AND SHOULDN'T BE 
    {
        public double x;
        public double y;
        public double z;

        public Vec3(double x_in, double y_in, double z_in)
        {
            x = x_in;
            y = y_in;
            z = z_in;
        }

        public Vec3(Quaternion input)
        {
            x = input.X;
            y = input.Y;
            z = input.Z;
            if(input.W > 0.0001)    //  Debug and could be removed
            {
                //MessageBox.Show("Error with quaternion to vector, real component is non zero");
            }
        }

        public Quaternion AsQuaternion()
        {
            Quaternion quat = new Quaternion((float)x, (float)y, (float)z, 0);
            return quat;
        }

        public void Normalise()
        {
            double normFraction = 1 / Math.Sqrt(Math.Pow(this.x, 2) + Math.Pow(this.y, 2) + Math.Pow(this.z, 2));
            x *= normFraction;
            y *= normFraction;
            z *= normFraction;
        }

        public bool Equals(Vec3 other)  //  NOT IN ACTUAL CODE AND  SHOULDN'T BE 
        {
            return this.x == other.x && this.y == other.y && this.z == other.z;
        }
        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }
        public static Vec3 Cross(Vec3 a, Vec3 b)
        {
            double s_1 = a.y * b.z - a.z * b.y;
            double s_2 = a.z * b.x - a.x * b.z;
            double s_3 = a.x * b.y - a.y * b.x;
            return new Vec3(s_1, s_2, s_3);

        }

        static public Vec3 Normalise(Vec3 a)
        {
            double normFraction = 1 / Math.Sqrt(Math.Pow(a.x, 2) + Math.Pow(a.y, 2) + Math.Pow(a.z, 2));
            return a * normFraction;
        }

        //  Should colour be its own class?
        static public Vec3 ColorCombination(Vec3 a, Vec3 b) //  For combining reflectance, we treat this as an array
        {
            Vec3 c = new(a.x * b.x, a.y * b.y, a.z * b.z);
            return c;
        }

        static public double operator *(Vec3 a, Vec3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        static public Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        static public Vec3 operator *(double a, Vec3 b)
        {
            return new Vec3(a * b.x, a * b.y, a * b.z);
        }

        static public Vec3 operator *(Vec3 b, double a)
        {
            return new Vec3(a * b.x, a * b.y, a * b.z);
        }

        static public Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

    }
}
