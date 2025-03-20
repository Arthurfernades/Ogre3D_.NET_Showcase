using org.ogre;

namespace OgreImage
{
    public partial class Ogre
    {
        private TerrainGlobalOptions mTerrainGlobals;

        public TerrainGroup mTerrainGroup;

        private Vector3 mTerrainPos = new Vector3();

        public void CreateTerrain()
        {
            #region CamNode position

            pivotNode.setPosition(mTerrainPos + new Vector3(0, 100, 0));

            #endregion

            #region Terrain Light

            Light light = scnMgr.createLight("DirectionalLight");
            light.setType(Light.LightTypes.LT_DIRECTIONAL);
            light.setDiffuseColour(new ColourValue(1, 1, 1));
            light.setSpecularColour(new ColourValue(0.4f, 0.4f, 0.4f));
            SceneNode lightNode = scnMgr.getRootSceneNode().createChildSceneNode();
            lightNode.setDirection(new Vector3(0.5f, -0.75f, -0.8f));
            lightNode.attachObject(light);

            #endregion

            #region Terrain

            mTerrainGroup = new TerrainGroup(scnMgr, Terrain.Alignment.ALIGN_X_Z, 513, 12000);
            mTerrainGlobals = new TerrainGlobalOptions();

            mTerrainGroup.setOrigin(new Vector3(0, 0, 0));

            ConfigureTerrainDefaults(light);

            // Define o material padrão antes de carregar o terreno
            SetDefaultMaterial();

            mTerrainGroup.defineTerrain(0, 0, 0);

            mTerrainGroup.loadAllTerrains(false);

            mTerrainGroup.freeTemporaryResources();

            #endregion
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

            //Altura da caixa do terreno
            mTerrainGlobals.setSkirtSize(2);

            mTerrainGlobals.setUseRayBoxDistanceCalculation(true);
        }

        private void SetDefaultMaterial()
        {
            Terrain.ImportData defaultimp = mTerrainGroup.getDefaultImportSettings();

            // Configuração do terreno
            defaultimp.worldSize = 12000;
            defaultimp.minBatchSize = 33;
            defaultimp.maxBatchSize = 65;

            // Adicionando camada de textura ao terreno
            defaultimp.layerList.Add(new Terrain.LayerInstance());

            Image combined = new Image();
            combined.loadTwoImagesAsRGBA("Ground23_col.jpg", "Ground23_spec.png", "General");
            TextureManager.getSingleton().loadImage("Ground23_diffspec", "General", combined);

            defaultimp.layerList[0].worldSize = 200;
            defaultimp.layerList[0].textureNames.Add("Ground23_diffspec.dds");
            defaultimp.layerList[0].textureNames.Add("Ground23_normheight.dds");
        }
    }
}
