using org.ogre;

namespace OgreImage
{
    public partial class Ogre
    {
        private float realRedValue, realGreenValue, realBlueValue;

        private float lightCamManYaw, lightCamManPitch, lightCamManDist;

        private Camera spotLightCam;

        private Light lightCam;

        private CameraMan lightCamMan;

        /// <summary>
        /// Adjusts the 3 ambient light values ​​simultaneously.
        /// </summary>
        /// <param name="value"></param>
        public void UpdateBrightness(float value)
        {
            realRedValue = realGreenValue = realBlueValue = value / 100;

            scnMgr.setAmbientLight(new ColourValue(realRedValue, realGreenValue, realBlueValue));
        }
    }
}
