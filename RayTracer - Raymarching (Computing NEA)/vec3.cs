using System;
using System.Numerics;
using System.Windows;

namespace RayTracer___Raymarching__Computing_NEA_
{
    public class Vec3
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

        public Vec3(Quaternion input)   //  Vector from a quaternion (With a zero real component)
        {
            x = input.X;
            y = input.Y;
            z = input.Z;
            if (input.W > 0.1)
            {
                MessageBox.Show("Error with quaternion to vector, real component is non zero");
            }
        }

        public Quaternion AsQuaternion()    //  Converts vector to quaternion (Real part is kept zero)
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

        public static Vec3 RotatePoint(Vec3 initialVec, Quaternion rotation)
        {
            Quaternion initialQuat = initialVec.AsQuaternion();
            Quaternion finalQuat = rotation * initialQuat * Quaternion.Conjugate(rotation);
            return new Vec3(finalQuat);
        }

        static public Vec3 Normalise(Vec3 a)
        {
            double normFraction = 1 / Math.Sqrt(Math.Pow(a.x, 2) + Math.Pow(a.y, 2) + Math.Pow(a.z, 2));
            return a * normFraction;
        }

        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        static public Vec3 ColorCombination(Vec3 a, Vec3 b) //  For combining reflectance, we treat this as an array
        {
            //  Each element is multiplied individually
            Vec3 c = new(a.x * b.x, a.y * b.y, a.z * b.z);
            return c;
        }

        static public double operator *(Vec3 a, Vec3 b) //  Dot/Scalar product
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
