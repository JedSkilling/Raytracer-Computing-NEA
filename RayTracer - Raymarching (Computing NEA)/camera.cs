using System;
using System.Numerics;

namespace RayTracer___Raymarching__Computing_NEA_
{
    public class Camera
    {
        public Vec3 position;
        public Quaternion rotation;
        public Quaternion xyQuat;
        //  the xyQuat is diffferent from the other quaternions, as we need it for movement
        //  If you try to move forwards it should be in the direction you face, but without moving you up or down
        //  Other quaternions for rotation are only needed in newRotation()

        public Camera(Vec3 startPos, double[] fullRotationInfo)
        {
            position = startPos;

            //  Quaternions are used extensively in controlling camera rotations
            //  (Complex mathematical Structure)
            newRotation(fullRotationInfo);
        }

        public void newRotation(double[] fullRotationInfo)
        {
            double xyPlaneRot = fullRotationInfo[0];
            double yzPlaneRot = fullRotationInfo[1];
            double xzPlaneRot = fullRotationInfo[2];

            //  Generates corresponding quaternions for xy, yz and xy plane rotations, then combines them
            this.xyQuat = new Quaternion(0, 0, MathF.Sin((float)(xyPlaneRot / 2) * MathF.PI / 180), MathF.Cos((float)(xyPlaneRot / 2) * MathF.PI / 180));
            Quaternion yzQuat = new Quaternion(MathF.Sin((float)(yzPlaneRot / 2) * MathF.PI / 180), 0, 0, MathF.Cos((float)(yzPlaneRot / 2) * MathF.PI / 180));

            //  We take negative of the xzPlaneRot for the xzQuat as for quaternions as positive xz rotation points downwards
            //  This can be checked on the linked interactive quaternion page by Ben Eater
            Quaternion xzQuat = new Quaternion(0, -MathF.Sin((float)(xzPlaneRot / 2) * MathF.PI / 180), 0, MathF.Cos((float)(xzPlaneRot / 2) * MathF.PI / 180));

            this.rotation = yzQuat * xyQuat * xzQuat;
        }

        public void newDirection(Vec3 newMovement)
        {
            Vec3 deltaXY = new Vec3(newMovement.x, newMovement.y, 0);
            Vec3 deltaZ = new Vec3(0, 0, newMovement.z);

            //  We rotate only by the xyQuat as if you try to move forwards you should move on the horizontal plane, not upwards
            //  Multiplying only by the xyQuat ensures we still move along that plane correctly
            this.position += Vec3.RotatePoint(deltaXY, this.xyQuat);

            //  Change in height is unaffected by this so is fine
            this.position += deltaZ;
        }

        public Vec3 camSpaceToWorldSpace(Vec3 point_relCam)
        {
            //  Converts a point relative to the camera into a point relative to the world center
            Vec3 point_relWorld = this.position + Vec3.RotatePoint(point_relCam, this.rotation);
            return point_relWorld;
        }
    }
}
