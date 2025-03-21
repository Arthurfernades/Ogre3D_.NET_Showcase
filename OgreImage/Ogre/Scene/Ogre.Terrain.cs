using org.ogre;

namespace OgreImage
{
    public partial class Ogre
    {
        private TerrainGlobalOptions mTerrainGlobals;

        public TerrainGroup mTerrainGroup;
        
        public bool isTerrainLoaded = false;

        private Vector3 mTerrainPos = new Vector3();

        public void CreateTerrain()
        {
            #region CamNode position

            pivotNode.setPosition(mTerrainPos + new Vector3(0, 100, 0));

            #endregion

            #region Terrain Light

            Light light = TerrainLight();

            #endregion

            #region Terrain

            mTerrainGroup = new TerrainGroup(scnMgr, Terrain.Alignment.ALIGN_X_Z, 513, 12000);

            if(mTerrainGlobals == null)
                mTerrainGlobals = new TerrainGlobalOptions();

            mTerrainGroup.setOrigin(new Vector3(0, 0, 0));

            ConfigureTerrainDefaults(light);

            SetDefaultMaterial();

            mTerrainGroup.defineTerrain(0, 0, 0);

            mTerrainGroup.loadAllTerrains(true);

            mTerrainGroup.freeTemporaryResources();

            #endregion

            isTerrainLoaded = true;
        }

        private void ConfigureTerrainDefaults(Light light)
        {
            // Configuração global do terreno
            mTerrainGlobals.setMaxPixelError(0);
            mTerrainGlobals.setCompositeMapDistance(3000);

            // Configura iluminação global do terreno
            mTerrainGlobals.setLightMapDirection(light.getDerivedDirection());
            mTerrainGlobals.setCompositeMapAmbient(scnMgr.getAmbientLight());
            mTerrainGlobals.setCompositeMapDiffuse(light.getDiffuseColour());

            //Terrain box height
            mTerrainGlobals.setSkirtSize(100);

            mTerrainGlobals.setUseRayBoxDistanceCalculation(true);
        }

        private void SetDefaultMaterial()
        {
            Terrain.ImportData defaultimp = mTerrainGroup.getDefaultImportSettings();

            defaultimp.worldSize = 12000;
            defaultimp.minBatchSize = 33;
            defaultimp.maxBatchSize = 65;
        }
    }
}
