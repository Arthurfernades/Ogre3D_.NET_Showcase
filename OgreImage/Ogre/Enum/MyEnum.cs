namespace OgreImage
{
    public class MyEnum
    {
        public enum eVPCamera
        {
            eVPTop = 1,
            eVPSW = 2,
            eVPSE = 3,
            eVPNE = 4,
            eVPNW = 5,
            eVPFRONT = 6,
            eVPBACK = 7,
            eVPLEFT = 8,
            eVPRIGHT = 9
        }

        public enum eVPProjectionStyle
        {
            eVPPerspective = 1,
            eVPOrthographic = 2
        }

        public enum eCameraView
        {
            eOrbitCamera = 1,
            eFreeLookCamera = 2
        }        
    }
}
