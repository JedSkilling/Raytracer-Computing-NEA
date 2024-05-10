using System.Numerics;

namespace TestingWithConsole_Raytracer
{
    internal class Program
    {
        static Random rnd = new Random();
        static int pointCount = 60;
        struct test { public int x; public int y; 
        
        
        }
        static void Main(string[] args)
        {
            


            Vec3 normal = new(0, 0, 1);
            string allPoints = "";
            for (int i = 0; i < pointCount; i++)
            {
                Vec3 currPoint = FindingNewRayDirection_RejectionMethodUniform(normal);
                string vecInfo = currPoint.x.ToString() + "," + currPoint.y.ToString() + "," + currPoint.z.ToString();
                allPoints += vecInfo + "\n";
            }
            Console.WriteLine(allPoints);
        }

        public static double modulus(double a, double b)
        {
            double k = Math.Floor(a / b);
            double modVal = a - k * b;
            return modVal;
        }
        public static Vec3 FindingNewRayDirection_TrigMethodNonUniform(Vec3 normal)
        {
            double theta = rnd.NextDouble() * 2 * Math.PI;  //   Get a random number for the trig functions
            double phi = rnd.NextDouble() * 2 * Math.PI;    //  Could decrease precision, 2 or 3 dp is likely to be enough

            double cosTheta = Math.Cos(theta);
            double sinTheta = Math.Sin(theta);

            double cosPhi = Math.Cos(phi);
            double sinPhi = Math.Sin(phi);

            //  This method will give us an already normalised value
            //  newDir will be a random point lying on a unit sphere
            Vec3 newDir = new(cosPhi * cosTheta, cosPhi * sinTheta, sinPhi);
            //  If newDir faces back in towards the centre of the object we flip it so it points out
            if (newDir * normal < 0)
            {
                newDir *= -1;
            }
            return newDir;
        }
        /*
        public static Vec3 FindingNewRayDirection_NormalMethodUniform(Vec3 normal)
        {
            rnd.
            Vec3 newDir = new(normal.x, normal.y, normal.z);
        }*/

        public static Vec3 FindingNewRayDirection_RejectionMethodUniform(Vec3 normal)
        {
            
            Vec3 newDir = new(rnd.NextDouble()*2 - 1, rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1);
            double magnitude = newDir.Magnitude();
            while (magnitude > 1|| magnitude < 0.00001)
            {
                newDir = new(rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1);
                magnitude = newDir.Magnitude();
            }
            if (newDir * normal < 0)
            {
                newDir *= -1;
            }
            
            return newDir * (1/magnitude);
        }

        public static Vec3 FindingNewRayDirection_TEMPDEBUG(Vec3 normal)
        {

            Vec3 newDir = new(rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1);
            /*
            double magnitude = newDir.Magnitude();
            while (magnitude > 1 || magnitude < 0.00001)
            {
                newDir = new(rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1);
                magnitude = newDir.Magnitude();
            }
            if (newDir * normal < 0)
            {
                newDir *= -1;
            }
            */
            return newDir;
        }
        public static Vec3 FindingNewRayDirection_CubeMethodNonUniform(Vec3 normal)
        {

            Vec3 newDir = new(rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1);
            double magnitude = newDir.Magnitude();
            while (magnitude < 0.00001)
            {
                newDir = new(rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1, rnd.NextDouble() * 2 - 1);
                magnitude = newDir.Magnitude();
            }
            if (newDir * normal < 0)
            {
                newDir *= -1;
            }

            return newDir * (1 / magnitude);
        }
    }

}