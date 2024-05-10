using System;
using System.Numerics;

namespace RayTracer___Raymarching__Computing_NEA_
{
    public enum comboType
    {
        Union,
        Intersection
    }

    //  Complex class structure is used, with inheritance and polymorphism
    abstract public class Shape
    {
        public abstract double SDF(Vec3 rayLocation);
        public abstract Vec3 FindNormal(Vec3 rayLocation);
        public Vec3 position;

        public Vec3 k_s;
        public Vec3 k_d;
        public double alpha;

        public Vec3 lightStrength = new Vec3(0, 0, 0);

        //  Some are kept as null as we may not want to define them every case
        public Shape(Vec3 position, Vec3 diffuseComponent, double alpha, Vec3 specularComponent = null, Vec3 lightStrength = null) //  Need to change
        {
            //  General info all shapes will need
            this.position = position;

            if (specularComponent != null || diffuseComponent == null)
            {
                this.k_s = specularComponent;
            }
            else
            {
                this.k_s = new Vec3(1, 1, 1) - diffuseComponent;
            }
            this.k_d = diffuseComponent;
            this.alpha = alpha;

            if (lightStrength == null)
            {
                this.lightStrength = new(0, 0, 0);
            }
            else
            {
                this.lightStrength = lightStrength;
            }
        }

        public Vec3 BRDF_phong(Vec3 omega_i, Vec3 omega_o, Vec3 normal)
        {
            
            omega_o *= -1;  //  Added line, accounts for that we want the direction to heading outwards
            omega_i.Normalise();    //  Incoming light (From a physics perspective)
            omega_o.Normalise();    //  Outgoing from a physics perspective, so these are the opposite of the order we got our rays
            normal.Normalise();

            //  Max is in place in case the dot product is negative
            Vec3 reflectedRay = 2 * Math.Max(omega_i * normal, 0) * normal - omega_i;

            //  Reflectance is the sum of the seperate components of specular and diffuse reflection
            Vec3 specularComponent = this.k_s * Math.Pow(Math.Max(omega_o * reflectedRay, 0), this.alpha);
            Vec3 reflectanceComponent = this.k_d * Math.Max(omega_o * normal, 0);

            //  The dot product term accounts for the shallow angles, which have a lower contribution
            Vec3 reflectance = (specularComponent + reflectanceComponent) * Math.Max(normal * omega_i, 0);

            //  Reflectance is in the form (R, G, B)
            //  Each component is how much of the colour is reflected
            return reflectance;
        }

        public Vec3 FindNormalNumerically(Vec3 rayLocation)
        {
            //  Hardcoded precision
            double epsilon = 0.0001;

            //  Numeric differentiation (The explicit equation can usually be found for optimising time taken per calculation, but this is still fast enough in our case, and faster to implement)
            Vec3 normal = 1 / (2 * epsilon) * new Vec3(
                this.SDF(new Vec3(rayLocation.x + epsilon, rayLocation.y, rayLocation.z)) - this.SDF(new Vec3(rayLocation.x - epsilon, rayLocation.y, rayLocation.z)),
                this.SDF(new Vec3(rayLocation.x, rayLocation.y + epsilon, rayLocation.z)) - this.SDF(new Vec3(rayLocation.x, rayLocation.y - epsilon, rayLocation.z)),
                this.SDF(new Vec3(rayLocation.x, rayLocation.y, rayLocation.z + epsilon)) - this.SDF(new Vec3(rayLocation.x, rayLocation.y, rayLocation.z - epsilon)));
            normal.Normalise();
            return normal;
        }

    }

    class Sphere : Shape
    {
        double radius { get; set; }
        public Sphere(Vec3 position, Vec3 diffuseComponent, double alpha, double radius, Vec3 specularComponent = null, Vec3 lightStrength = null) : base(position, diffuseComponent, alpha, specularComponent, lightStrength)
        {
            //  Only extra info that a sphere needs is the radius
            this.radius = radius;

        }

        public override double SDF(Vec3 rayLocation)
        {
            double signedDistance = Math.Sqrt(Math.Pow((rayLocation.x - position.x), 2) + Math.Pow((rayLocation.y - position.y), 2) + Math.Pow((rayLocation.z - position.z), 2)) - radius;

            return signedDistance;
        }



        public override Vec3 FindNormal(Vec3 rayLocation)
        {
            Vec3 normal = rayLocation - this.position;
            normal.Normalise();
            return normal;
        }
    }

    class Plane : Shape
    {
        Vec3 pointOnPlane { get; set; }
        Vec3 normal { get; set; }
        public Plane(Vec3 diffuseComponent, double alpha, Vec3 position, Vec3 normal, Vec3 specularComponent = null, Vec3 lightStrength = null) : base(position, diffuseComponent, alpha, specularComponent, lightStrength)
        {
            //  A plane can be definined by a point on the plane and it's normal
            this.pointOnPlane = position;
            this.normal = Vec3.Normalise(normal);

        }

        public override double SDF(Vec3 rayLocation)
        {
            double signedDistance = rayLocation * this.normal - this.pointOnPlane * this.normal;
            return signedDistance;
        }

        public override Vec3 FindNormal(Vec3 rayLocation)
        {
            return this.normal;
        }
    }

    class Line : Shape
    {
        //  Radius is how much area around the line is taken up
        double radius { get; set; }
        double segmentDist;

        Vec3 pointA;
        Vec3 pointB;

        double LB = double.MinValue;    //   Upper bound and Lower bound for lambda values, either -infinity or 0 and 1 or +infinity
        double UB = double.MaxValue;    //  Technically not actually an infinite line because of this, but will have no actual effect
        public Line(Vec3 diffuseComponent, double alpha, Vec3 pointA, Vec3 pointB, double radius, bool haltA = true, bool haltB = true, Vec3 specularComponent = null, Vec3 lightStrength = null, Vec3 position = null) : base(position, diffuseComponent, alpha, specularComponent, lightStrength)
        {

            this.radius = radius;
            this.pointA = pointA;
            this.pointB = pointB;
            this.segmentDist = (pointB - pointA) * (pointB - pointA);
            if (haltA)
            {
                this.LB = 0;
            }
            if (haltB)
            {
                this.UB = 1;
            }

        }

        public override double SDF(Vec3 rayLocation)
        {
            double lambdaInfinite = (rayLocation - pointA) * (pointB - pointA) / segmentDist;  //  Doesn't account for end points
            double lambdaActual = Math.Min(Math.Max(lambdaInfinite, LB), UB);

            //  Use how far along the line we are to get a specific point
            Vec3 closestPointOnLine = pointA + lambdaActual * (pointB - pointA);

            //  Now we take dist from the raylocation to closestPoint lying on the line
            double signedDistance = Math.Sqrt((rayLocation - closestPointOnLine) * (rayLocation - closestPointOnLine)) - radius;


            return signedDistance;
        }

        public override Vec3 FindNormal(Vec3 rayLocation)
        {
            double lambdaInfinite = (rayLocation - pointA) * (pointB - pointA) / segmentDist;  //  Doesn't account for end points

            double lambdaActual = Math.Min(Math.Max(lambdaInfinite, LB), UB);

            //  Use how far along the line we are to get a specific point
            Vec3 closestPointOnLine = pointA + lambdaActual * (pointB - pointA);

            //  Now we take dist from the raylocation to closestPoint lying on the line
            Vec3 normal = Vec3.Normalise(rayLocation - closestPointOnLine);

            return normal;
        }
    }

    class Cuboid : Shape
    {
        double cornerSmoothing { get; set; }
        Vec3 centerPosition { get; set; }
        Vec3 cornerPosition { get; set; }
        Quaternion rotation { get; set; }
        public Cuboid(Vec3 position, Vec3 cornerPosition, Vec3 diffuseComponent, double alpha, double[] fullRotationInfo = null, double cornerSmoothing = 0, Vec3 specularComponent = null, Vec3 lightStrength = null) : base(position, diffuseComponent, alpha, specularComponent, lightStrength)
        {
            //  We input the position of the center of the cuboid, and the position of a corner relative to that
            //  Along with the usual info for the BRDF and a parameter for smoothing the corner
            if (fullRotationInfo == null)
            {
                fullRotationInfo = new double[] { 0, 0, 0 };
            }
            this.cornerSmoothing = cornerSmoothing;
            this.centerPosition = position;
            this.cornerPosition = cornerPosition;
            newRotation(fullRotationInfo);

        }

        public override double SDF(Vec3 rayLocation)
        {
            Vec3 transformedPoint = InverseRigidTransformation(rayLocation);
            transformedPoint = new Vec3(Math.Abs(transformedPoint.x), Math.Abs(transformedPoint.y), Math.Abs(transformedPoint.z));
            Vec3 relOuterCourner = transformedPoint - this.cornerPosition;
            Vec3 outerCase = new Vec3(Math.Max(relOuterCourner.x, 0), Math.Max(relOuterCourner.y, 0), Math.Max(relOuterCourner.z, 0));
            double signedDistance = Math.Sqrt(outerCase * outerCase) + Math.Min(Math.Max(Math.Max(relOuterCourner.x, relOuterCourner.y), relOuterCourner.z), 0) - cornerSmoothing;

            return signedDistance;
        }



        public override Vec3 FindNormal(Vec3 rayLocation)
        {
            Vec3 normal = FindNormalNumerically(rayLocation);

            return normal;
        }

        public Vec3 InverseRigidTransformation(Vec3 startPoint) //  Allows us to use any possible rotation or translation of an axis aligned cuboid (What our SDF actually works with)
        {
            Vec3 endPoint = Vec3.RotatePoint(startPoint - this.position, Quaternion.Conjugate(this.rotation));

            return endPoint;
        }



        public void newRotation(double[] fullRotationInfo)
        {
            double xyPlaneRot = fullRotationInfo[0];
            double yzPlaneRot = fullRotationInfo[1];
            double xzPlaneRot = fullRotationInfo[2];

            //  Generates corresponding quaternions for xy, yz and xy plane rotations, then combines them
            Quaternion xyQuat = new Quaternion(0, 0, MathF.Sin((float)(xyPlaneRot / 2) * MathF.PI / 180), MathF.Cos((float)(xyPlaneRot / 2) * MathF.PI / 180));
            Quaternion yzQuat = new Quaternion(MathF.Sin((float)(yzPlaneRot / 2) * MathF.PI / 180), 0, 0, MathF.Cos((float)(yzPlaneRot / 2) * MathF.PI / 180));

            //  We take negative of the xzPlaneRot for the xzQuat as for quaternions as positive xz rotation points downwards
            //  This can be checked on the linked interactive quaternion page by Ben Eater
            Quaternion xzQuat = new Quaternion(0, -MathF.Sin((float)(xzPlaneRot / 2) * MathF.PI / 180), 0, MathF.Cos((float)(xzPlaneRot / 2) * MathF.PI / 180));

            this.rotation = yzQuat * xyQuat * xzQuat;
        }
    }

    class Combination : Shape
    {
        public Shape shape1;
        public Shape shape2;

        public double sdfMergeStrength;    //  Weighting for how smooth the combination is
        public double colourMergeStrength;
        public comboType type;
        public Combination(double alpha, Shape shape1, Shape shape2, double sdfWeighting, comboType type, Vec3 diffuseComponent = null, double colourMergeStrength = 15, Vec3 specularComponent = null, Vec3 lightStrength = null, Vec3 position = null) : base(position, diffuseComponent, alpha, specularComponent, lightStrength)
        {

            this.shape1 = shape1;
            this.shape2 = shape2;
            this.sdfMergeStrength = sdfWeighting;
            this.colourMergeStrength = colourMergeStrength;
            this.type = type;


        }

        public override double SDF(Vec3 rayLocation)
        {
            double signedDistance;
            double shape1Dist = shape1.SDF(rayLocation);
            double shape2Dist = shape2.SDF(rayLocation);
            if (type == comboType.Union)
            {
                signedDistance = -Math.Log(Math.Exp(-sdfMergeStrength * shape1Dist) + Math.Exp(-sdfMergeStrength * shape2Dist)) / sdfMergeStrength;
            }
            else
            {
                //  Only union is implemented so far
                throw new Exception();
            }




            return signedDistance;
        }

        private Vec3 lerp(Vec3 lP0, Vec3 lP1, double lt)
        {
            double newX = (1 - lt) * lP0.x + lt * lP1.x;
            double newY = (1 - lt) * lP0.y + lt * lP1.y;
            double newZ = (1 - lt) * lP0.z + lt * lP1.z;

            Vec3 lP2 = new Vec3(newX, newY, newZ);
            return lP2;
        }
        public override Vec3 FindNormal(Vec3 rayLocation)   //  Also calculates new shape colour at the point
        {
            Vec3 normal = FindNormalNumerically(rayLocation);

            double shape1Dist = shape1.SDF(rayLocation);
            double shape2Dist = shape2.SDF(rayLocation);
            double t = Math.Exp(shape1Dist / colourMergeStrength) / (Math.Exp(shape1Dist / colourMergeStrength) + Math.Exp(shape2Dist / colourMergeStrength));
            this.k_s = lerp(shape1.k_s, shape2.k_s, t);
            this.k_d = lerp(shape1.k_d, shape2.k_d, t);
            return normal;
        }

    }

    class InfiniteSphere : Shape
    {
        double radius { get; set; }
        Vec3 repetitionDistancesVector;
        public InfiniteSphere(Vec3 position, Vec3 diffuseComponent, double alpha, double radius, Vec3 repetitionVector, Vec3 specularComponent = null, Vec3 lightStrength = null) : base(position, diffuseComponent, alpha, specularComponent, lightStrength)
        {
            //  The edge case where an object steps outside of it's repetition boundary will currently cause artifacts
            this.radius = radius;

            this.repetitionDistancesVector = repetitionVector;
            this.position = VecModulus(position, this.repetitionDistancesVector);

        }

        public override double SDF(Vec3 rayLocation)
        {
            rayLocation = VecModulus(rayLocation, repetitionDistancesVector);
            double signedDistance = Math.Sqrt(Math.Pow((rayLocation.x - position.x), 2) + Math.Pow((rayLocation.y - position.y), 2) + Math.Pow((rayLocation.z - position.z), 2)) - radius;
            return signedDistance;
        }



        public override Vec3 FindNormal(Vec3 rayLocation)
        {
            rayLocation = VecModulus(rayLocation, repetitionDistancesVector);
            Vec3 normal = rayLocation - this.position;
            normal.Normalise();
            return normal;
        }

        public double Modulus(double a, double b)   //  Used in infite repetitions
        {
            double k = Math.Floor(a / b);
            double modVal = a - k * b;
            return modVal;
        }

        public Vec3 VecModulus(Vec3 a, Vec3 b)
        {
            Vec3 modVec = new Vec3(Modulus(a.x, b.x), Modulus(a.y, b.y), Modulus(a.z, b.z));
            return modVec;
        }
    }
}
