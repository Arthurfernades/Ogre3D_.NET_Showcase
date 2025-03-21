using org.ogre;

namespace OgreImage
{
    public partial class Ogre
    {
        private float realRedValue, realGreenValue, realBlueValue;

        public bool isOrbitLightOn = false;

        /// <summary>
        /// Adjusts the 3 ambient light values ​​simultaneously.
        /// </summary>
        /// <param name="value"></param>
        public void UpdateBrightness(float value)
        {
            realRedValue = realGreenValue = realBlueValue = value / 100;

            scnMgr.setAmbientLight(new ColourValue(realRedValue, realGreenValue, realBlueValue));
        }
        public Light TerrainLight()
        {
            realRedValue = realGreenValue = realBlueValue = 0.1f;

            scnMgr.setAmbientLight(new ColourValue(realRedValue, realGreenValue, realBlueValue));

            Light light = scnMgr.createLight("TerrainLight");
            light.setType(Light.LightTypes.LT_DIRECTIONAL);
            light.setDiffuseColour(new ColourValue(1, 1, 1));
            light.setSpecularColour(new ColourValue(0.4f, 0.4f, 0.4f));
            SceneNode lightNode = scnMgr.getRootSceneNode().createChildSceneNode();
            lightNode.setDirection(new Vector3(0.5f, -0.75f, -0.8f));
            lightNode.attachObject(light);

            return light;
        }

        public Light SimpleLight()
        {
            realRedValue = realGreenValue = realBlueValue = 0.1f;

            scnMgr.setAmbientLight(new ColourValue(realRedValue, realGreenValue, realBlueValue));

            Light light = scnMgr.createLight("SimpleLight");
            SceneNode lightnode = scnMgr.getRootSceneNode().createChildSceneNode();
            lightnode.setPosition(0f, 30f, 45f);
            lightnode.attachObject(light);

            return light;
        }

        public void CreateOrbitLight()
        {
            Light lightCam = scnMgr.createLight("OrbitLight");

            lightCam.setVisible(false);

            lightCam.setType(Light.LightTypes.LT_POINT);
            lightCam.setDiffuseColour(1f, 1f, 1f);

            SceneNode lightCamNode = scnMgr.getRootSceneNode().createChildSceneNode();
            lightCamNode.attachObject(lightCam);

            CameraMan lightCamMan = new CameraMan(lightCamNode);
            lightCamMan.setStyle(CameraStyle.CS_ORBIT);
            lightCamMan.setYawPitchDist(new Radian(camManYaw), new Radian(camManPitch), camManDist);

            isOrbitLightOn = true;
        }
    }
}
