using System;

namespace OgreEngine
{
    public static class Variaveis
    {
        public static Ogre myOgre;

        public static OgreImage myImage;

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

        public const String MSN_DEFAULT = "Default";
        public const String MSN_SHADERGEN = "ShaderGeneratorDefaultScheme";
        public const String SRS_NORMALMAP = "NormalMap";
    }
}
