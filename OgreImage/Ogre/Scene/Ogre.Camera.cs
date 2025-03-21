using org.ogre;

namespace OgreImage
{
    public partial class Ogre
    {        
        private Camera cam;

        private SceneNode camNode, pivotNode;

        private CameraMan camMan;

        private float camManYaw, camManPitch, camManDist;

        private float camNodeXAxis, camNodeYAxis, camNodeZAxis;

        private float pivotNodeXAxis, pivotNodeYAxis, pivotNodeZAxis;

        public bool CameraColided()
        {
            if (mTerrainGroup == null) return false;

            Vector3 camPos = camNode.getPosition();

            float groundHeight = mTerrainGroup.getHeightAtWorldPosition(camPos);

            float minHeight = groundHeight + 20;

            if (camPos.y <= minHeight)
            {
                return true;
            }

            return false;
        }

        public void MoveOrbitCam(int deltaX, int deltaY)
        {
            float sense = 0.2f;

            if (!(CameraColided() && deltaY < 0))
                camManPitch += deltaY * sense;

            camManYaw += -deltaX * sense;

            camMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
        }

        public void MovePanCam(float deltaX, float deltaY)
        {
            float sense = camManDist * 0.0012f;

            Vector3 camPos = camNode.getPosition();
            Vector3 pivotPos = pivotNode.getPosition();

            Vector3 look = camPos - pivotPos;
            look.normalise();

            Vector3 globalUp = new Vector3(0, 1, 0);

            Vector3 right, up;

            float overheadThreshold = 0.9f;

            if (System.Math.Abs(look.dotProduct(globalUp)) > overheadThreshold)
            {
                right = new Vector3(1, 0, 0);
                up = new Vector3(0, 0, -1);
            }
            else
            {
                right = globalUp.crossProduct(look);
                if (right.length() < 0.001f)
                {
                    right = new Vector3(1, 0, 0).crossProduct(look);
                }
                right.normalise();
                up = look.crossProduct(right);
                up.normalise();
            }

            Vector3 movement = right.__mul__(-deltaX * sense) + up.__mul__(deltaY * sense);

            if (CameraColided() && deltaY < 0)
            {
                movement = right.__mul__(-deltaX * sense);
            }

            pivotNode.translate(movement, Node.TransformSpace.TS_WORLD);

            camMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
        }

        public void MovePivotNode(string axis, int value)
        {
            float sense = camManDist * 0.015f;

            Vector3 C = camNode.getPosition();
            Vector3 P = pivotNode.getPosition();
            Vector3 V = P - C;
            V.normalise();

            Vector3 movement = new Vector3();

            if (axis == "Forward" || axis == "Backward")
            {
                movement.x = V.x * value * sense;
                movement.y = V.y * value * sense;
                movement.z = V.z * value * sense;
            }
            else if (axis == "Rightward" || axis == "Leftward")
            {
                Vector3 side = new Vector3(0, 1, 0).crossProduct(V);

                side.normalise();

                movement.x = side.x * value * sense;
                movement.y = side.y * value * sense;
                movement.z = side.z * value * sense;
            }
            else if (axis == "Upward" || axis == "Downward")
            {
                Vector3 upDown = new Vector3(0, 1, 0);

                movement.x = upDown.x * value * sense;
                movement.y = upDown.y * value * sense;
                movement.z = upDown.z * value * sense;
            }

            pivotNode.translate(movement, Node.TransformSpace.TS_WORLD);

            camMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
        }

        public void PushCamera(float deltaZ)
        {
            float sense = camManDist * 0.02f;

            deltaZ *= sense;

            camManDist += deltaZ;

            camMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
        }

        public void ZoomAll()
        {
            globalBoundingBox = new AxisAlignedBox();

            if (lastEntityNode != null)
            {
                globalBoundingBox.merge(lastEntityNode.getAttachedObject(0).getBoundingBox());
            }
            else
            {
                MovableObjectMap entityList = scnMgr.getMovableObjects("Entity");

                foreach (var entity in entityList)
                {
                    if (entity.Value is MovableObject ent)
                    {
                        globalBoundingBox.merge(ent.getWorldBoundingBox(true));
                        var nodeName = entity.Value.getParentSceneNode().getName();
                        var name = entity.Value.getName();
                    }
                }

                if (globalBoundingBox.isNull())
                    return;
            }

            Vector3 max = globalBoundingBox.getMaximum();
            Vector3 min = globalBoundingBox.getMinimum();
            Vector3 center = globalBoundingBox.getCenter();

            Vector3 diagonal = max.__sub__(min);

            float radius = diagonal.length();

            float fovY = cam.getFOVy().valueRadians();

            float distanceH = radius / (float)System.Math.Tan(fovY);
            float aspectRatio = TextureWidth / TextureHeight;
            fovY *= aspectRatio;
            float distanceV = radius / (float)System.Math.Tan(fovY);

            float distance = System.Math.Max(distanceH, distanceV);

            if (distance > cam.getFarClipDistance())
                cam.setFarClipDistance(distance * 2);

            Vector3 direction = new Vector3(camNode.getPosition().x, min.y + (max.y - min.y) / 2, camNode.getPosition().z) - camNode.getPosition();

            direction.normalise();

            Vector3 vNewDirection = direction.__mul__(distance);
            var newPosition = (center - vNewDirection);

            camNodeXAxis = newPosition.x;
            camNodeYAxis = newPosition.y;
            camNodeZAxis = newPosition.z;

            camNode.setPosition(camNodeXAxis, camNodeYAxis, camNodeZAxis);

            camManDist = distance;

            pivotNodeXAxis = center.x;
            pivotNodeYAxis = center.y;
            pivotNodeZAxis = center.z;

            pivotNode.setPosition(pivotNodeXAxis, pivotNodeYAxis, pivotNodeZAxis);

            camNode.lookAt(new Vector3(pivotNodeXAxis, pivotNodeYAxis, pivotNodeZAxis), Node.TransformSpace.TS_LOCAL);

            camMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
        }        

        public void ChangeCameraPerspective(MyEnum.eVPProjectionStyle eVPPerspective)
        {
            switch (eVPPerspective)
            {
                case MyEnum.eVPProjectionStyle.eVPPerspective:
                    cam.setProjectionType(ProjectionType.PT_PERSPECTIVE);
                    cam.setFOVy(new Radian(new Degree(45)));
                    break;

                case MyEnum.eVPProjectionStyle.eVPOrthographic:
                    cam.setProjectionType(ProjectionType.PT_ORTHOGRAPHIC);
                    Vector3 min = globalBoundingBox.getMinimum();
                    Vector3 max = globalBoundingBox.getMaximum();
                    float width = max.x - min.x;
                    float height = max.y - min.y;
                    width *= 1.1f;
                    height *= 1.1f;
                    cam.setOrthoWindowWidth(width);
                    cam.setOrthoWindowHeight(height);
                    break;
            }
        }

        public void ChangeCameraView(MyEnum.eCameraView eCameraView)
        {
            switch (eCameraView)
            {
                case MyEnum.eCameraView.eOrbitCamera:
                    camMan.setStyle(CameraStyle.CS_ORBIT);
                    ZoomAll();
                    break;

                case MyEnum.eCameraView.eFreeLookCamera:
                    camMan.setStyle(CameraStyle.CS_FREELOOK);
                    break;
            }
        }
    }
}
