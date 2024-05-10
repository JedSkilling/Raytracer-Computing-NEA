namespace RayTracer___Raymarching__Computing_NEA_
{
    public class Ray
    {
        public Vec3 position;
        public Vec3? direction;

        //  Previous shape is what shape the ray just hit
        //  After a collision calculation, previous shape will be set to what has just been collided with
        public Shape? previousShape;

        public bool hasHitLight = false;

        public Vec3 productOfReflectance = new(1, 1, 1);

        public Ray(Vec3 position, Vec3 direction)
        {
            this.position = position;
            this.direction = direction;
            this.previousShape = null;
        }
    }
}
