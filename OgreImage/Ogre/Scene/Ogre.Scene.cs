using org.ogre;

namespace OgreImage
{
    public partial class Ogre
    {
        private AxisAlignedBox globalBoundingBox;

        public void CreateScene(int i)
        {
            switch (i)
            {
                case 0:
                    EmptyScene();
                    break;
            }
        }

        public void ShowGlobalAxis()
        {
            float farDistance = cam.getFarClipDistance() / 2;

            ManualObject axis = scnMgr.createManualObject("Axis");
            axis.begin("BaseWhiteNoLighting", RenderOperation.OperationType.OT_LINE_LIST);

            // Eixo X (Vermelho)
            axis.position(-farDistance, 0, 0);
            axis.colour(new ColourValue(100, 0, 0));
            axis.position(farDistance, 0, 0);

            // Eixo Y (Verde)
            axis.position(0, -farDistance, 0);
            axis.colour(new ColourValue(0, 100, 0));
            axis.position(0, farDistance, 0);

            // Eixo Z (Azul)
            axis.position(0, 0, -farDistance);
            axis.colour(new ColourValue(0, 0, 100));
            axis.position(0, 0, farDistance);

            axis.end();

            SceneNode axisNode = scnMgr.getRootSceneNode().createChildSceneNode("AxisNode");
            axisNode.attachObject(axis);

            ManualObject grid = scnMgr.createManualObject("Grid");
            grid.begin("BaseWhiteNoLighting", RenderOperation.OperationType.OT_LINE_LIST);

            int gridSize = (int)farDistance;  // Define o tamanho da grade (1000x1000 unidades)
            int step = 10;        // Distância entre as linhas

            // Linhas no eixo Z (Movendo-se ao longo do X)
            for (int x = -gridSize / 2; x <= gridSize / 2; x += step)
            {
                grid.position(x, 0, -gridSize / 2);
                grid.colour(0.5f, 0.5f, 0.5f);  // Cor cinza
                grid.position(x, 0, gridSize / 2);
            }

            // Linhas no eixo X (Movendo-se ao longo do Z)
            for (int z = -gridSize / 2; z <= gridSize / 2; z += step)
            {
                grid.position(-gridSize / 2, 0, z);
                grid.colour(0.5f, 0.5f, 0.5f);
                grid.position(gridSize / 2, 0, z);
            }

            grid.end();

            SceneNode gridNode = scnMgr.getRootSceneNode().createChildSceneNode("GridNode");
            gridNode.attachObject(grid);
        }

        public void HideGlobalAxis()
        {
            scnMgr.destroyManualObject("Axis");
            scnMgr.destroyManualObject("Grid");
            scnMgr.destroySceneNode("AxisNode");
            scnMgr.destroySceneNode("GridNode");
        }

        public void ShowFog()
        {
            ColourValue fadeColour = new ColourValue(0.93f, 0.86f, 0.76f);
            scnMgr.setFog(FogMode.FOG_LINEAR, fadeColour, .001f, 500, 1000);
            renderTarget.getViewport(0).setBackgroundColour(fadeColour);
        }

        public void HideFog()
        {
            scnMgr.setFog(FogMode.FOG_NONE);
        }

        private void InitShadows()
        {
            //scnMgr.setShadowTechnique(ShadowTechnique.SHADOWTYPE_STENCIL_ADDITIVE);
            scnMgr.setShadowTechnique(ShadowTechnique.SHADOWTYPE_TEXTURE_ADDITIVE_INTEGRATED);
            scnMgr.setShadowTextureCountPerLightType(Light.LightTypes.LT_DIRECTIONAL, 3);
            scnMgr.setShadowDirLightTextureOffset(0);
            scnMgr.setShadowTextureCount(3);
            scnMgr.setShadowTextureSettings(1024, 3, PixelFormat.PF_FLOAT32_R);
            MaterialPtr mat = MaterialManager.getSingleton().getByName("PSSM/shadow_caster");
            scnMgr.setShadowTextureCasterMaterial(mat);

            ShadowCameraSetupPtr pssmSetup = PSSMShadowCameraSetup.create();
            scnMgr.setShadowCameraSetup(pssmSetup);

            scnMgr.setShadowFarDistance(1000);
            scnMgr.setShadowCasterRenderBackFaces(false);
            scnMgr.setShadowTextureSelfShadow(true);
        }

        /// <summary>
        /// Constrói a cena vazia para que possa adicionar uma Entity posteriormente.
        /// </summary>
        private void EmptyScene()
        {
            #region Ambient Light

            if (!isTerrainLoaded)
            {
                SimpleLight();
            }

            #endregion            

            pivotNode = scnMgr.getRootSceneNode().createChildSceneNode("PivotNode");

            float farDistance = 5;

            ManualObject axis = scnMgr.createManualObject("CenterAxisPivot");

            axis.begin("BaseWhiteNoLighting", RenderOperation.OperationType.OT_LINE_LIST);

            // Eixo X (Vermelho)
            axis.position(-farDistance, 0, 0);
            axis.colour(new ColourValue(1, 0, 0));
            axis.position(farDistance, 0, 0);

            // Eixo Y (Verde)
            axis.position(0, -farDistance, 0);
            axis.colour(new ColourValue(0, 1, 0));
            axis.position(0, farDistance, 0);

            // Eixo Z (Azul)
            axis.position(0, 0, -farDistance);
            axis.colour(new ColourValue(0, 0, 1));
            axis.position(0, 0, farDistance);

            axis.end();

            pivotNode.attachObject(axis);

            #region Camera

            camNode = scnMgr.getRootSceneNode().createChildSceneNode("CameraNode");

            cam = scnMgr.createCamera("OrbitCamera");
            cam.setProjectionType(ProjectionType.PT_PERSPECTIVE);
            cam.setFOVy(new Radian(new Degree(45)));
            cam.setNearClipDistance(5);
            cam.setFarClipDistance(24000);
            cam.setAutoAspectRatio(true);
            cam.setPolygonMode(PolygonMode.PM_SOLID);
            camNode.attachObject(cam);

            #endregion

            #region Camera Man
            camMan = new CameraMan(camNode);
            camMan.setStyle(CameraStyle.CS_ORBIT);
            camMan.setYawPitchDist(new Radian(camManYaw), new Radian(camManPitch), camManDist);
            camMan.setTarget(pivotNode);

            #endregion            
        }

        public void AddSkyBox(string option)
        {
            if (root == null) return;

            switch (option)
            {
                case "Nenhum":
                    if (scnMgr.isSkyBoxEnabled())
                        scnMgr.setSkyBoxEnabled(false);
                    else if (scnMgr.isSkyDomeEnabled())
                        scnMgr.setSkyDomeEnabled(false);
                    else if (scnMgr.isSkyPlaneEnabled())
                        scnMgr.setSkyPlaneEnabled(false);
                    break;

                case "Padrão":
                    scnMgr.setSkyBox(true, "AuE/SkyBox", 1000);
                    break;

                case "Noite":
                    scnMgr.setSkyBox(true, "AuE/NightSkyBox", 1000);
                    break;

                case "IA":
                    scnMgr.setSkyBox(true, "AuE/IASkyBox", 1000);
                    break;

                case "Domo":
                    scnMgr.setSkyDome(true, "AuE/PixPlantCloudySkyDome", 5, 2);
                    break;

                case "Plano":
                    Plane skyPlane = new Plane
                    {
                        d = 1000,
                        normal = new Vector3(0, -1, 0)
                    };
                    scnMgr.setSkyPlane(true, skyPlane, "Examples/SpaceSkyPlane", 1500, 75);
                    break;
            }
        }

        public void AddTerrain(string option)
        {
            if (root == null) return;

            if (option == "Nenhum")
            {
                if (scnMgr.hasLight("TerrainLight"))
                {
                    scnMgr.destroyLight("TerrainLight");
                    SimpleLight();
                }

                if (isTerrainLoaded)
                {
                    mTerrainGroup.removeAllTerrains();
                    mTerrainGroup.update();
                    isTerrainLoaded = false;
                }

                return;
            }

            if (isTerrainLoaded)
            {
                byte numLayer = mTerrainGroup.getTerrain(0, 0).getLayerCount();

                if (numLayer > 0)
                {
                    for (byte i = 0; i < numLayer; i++)
                    {
                        mTerrainGroup.getTerrain(0, 0).removeLayer(i);
                    }
                }
            }


            if (!isTerrainLoaded)
            {
                if (scnMgr.hasLight("SimpleLight"))
                    scnMgr.destroyLight("SimpleLight");

                CreateTerrain();
            }

            mTerrainGroup.getTerrain(0, 0).addLayer(0);

            switch (option)
            {
                case "Grama":
                    mTerrainGroup.getTerrain(0, 0).setLayerTextureName(0, 0, "grass_green-01_diffusespecular.dds");
                    mTerrainGroup.getTerrain(0, 0).setLayerTextureName(0, 1, "grass_green-01_normalheight.dds");
                    break;

                case "Grama Desgastada":
                    mTerrainGroup.getTerrain(0, 0).setLayerTextureName(0, 0, "Ground37_diffspec.dds");
                    mTerrainGroup.getTerrain(0, 0).setLayerTextureName(0, 1, "Ground37_normheight.dds");
                    break;

                case "Pedra":
                    mTerrainGroup.getTerrain(0, 0).setLayerTextureName(0, 0, "Rock20_diffspec.dds");
                    mTerrainGroup.getTerrain(0, 0).setLayerTextureName(0, 1, "Rock20_normheight.dds");
                    break;

                case "Tijolo":
                    mTerrainGroup.getTerrain(0, 0).setLayerTextureName(0, 0, "Bricks076C_diffspec.dds");
                    mTerrainGroup.getTerrain(0, 0).setLayerTextureName(0, 1, "Bricks076C_normheight.dds");
                    break;
            }

            mTerrainGroup.getTerrain(0, 0).setLayerWorldSize(0, 200);
        }

        public void CleanScene()
        {
            if (scnMgr == null) return;

            foreach (var entity in entityDictionary.Values)
            {
                scnMgr.destroyEntity(entity);
            }

            entityDictionary.Clear();

            MeshManager.getSingleton().removeAll();
        }
    }
}