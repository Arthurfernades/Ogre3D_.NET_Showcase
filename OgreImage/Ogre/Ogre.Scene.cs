using org.ogre;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Math = org.ogre.Math;

namespace OgreEngine
{
    public partial class Ogre
    {
        public bool ShowSkybox = true;
        public bool ShowShadows = true;
        public bool ShowTerrain = true;

        private Camera cam;

        private Light lightCam;

        private SceneNode camNode, lightCamNode, pivotNode;

        private CameraMan camMan;

        private CameraMan lightCamMan;

        private TerrainGlobalOptions mTerrainGlobals;

        public TerrainGroup mTerrainGroup;

        private Vector3 mTerrainPos = new Vector3();

        private float camManYaw, camManPitch, camManDist;

        private float camNodeXAxis, camNodeYAxis, camNodeZAxis;

        private float pivotNodeXAxis, pivotNodeYAxis, pivotNodeZAxis;

        private Dictionary<string, Entity> entityDictionary = new Dictionary<string, Entity>();

        public void CreateScene(int i)
        {
            switch (i)
            {
                case 0:
                    EmptyScene();
                    break;
                case 1:
                    SinabdScene();
                    break;
                case 2:
                    OgreHeadScene();
                    break;
                case 3:
                    NinjaScene();
                    break;
                case 4:
                    StaticGeometryScene();
                    break;
            }
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

            if (!ShowTerrain)
            {
                realRedValue = realGreenValue = realBlueValue = 0.1f;

                scnMgr.setAmbientLight(new ColourValue(realRedValue, realGreenValue, realBlueValue));

                var light = scnMgr.createLight("MainLight");
                var lightnode = scnMgr.getRootSceneNode().createChildSceneNode();
                lightnode.setPosition(0f, 30f, 45f);
                lightnode.attachObject(light);
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

            #region Light CamMan Node           

            lightCam = scnMgr.createLight("LightCam");

            lightCam.setVisible(false);

            /*if (!MostraTerreno)
            {
                lightCam.setType(Light.LightTypes.LT_POINT);
                lightCam.setDiffuseColour(1f, 1f, 1f);
            }*/

            lightCamNode = scnMgr.getRootSceneNode().createChildSceneNode();
            lightCamNode.attachObject(lightCam);

            lightCamMan = new CameraMan(lightCamNode);
            lightCamMan.setStyle(CameraStyle.CS_ORBIT);
            lightCamMan.setYawPitchDist(new Radian(camManYaw), new Radian(camManPitch), camManDist);

            #endregion            
        }

        #region Cenas prontas

        private void SinabdScene()
        {
            scnMgr.setAmbientLight(new ColourValue(.1f, .1f, .1f));

            var light = scnMgr.createLight("MainLight");
            var lightnode = scnMgr.getRootSceneNode().createChildSceneNode();
            lightnode.setPosition(0f, 30f, 45f);
            lightnode.attachObject(light);

            pivotNode = scnMgr.getRootSceneNode().createChildSceneNode("PivotNode");
            camNode = scnMgr.getRootSceneNode().createChildSceneNode("CameraNode");

            cam = scnMgr.createCamera("myCam");
            cam.setAutoAspectRatio(true);
            cam.setNearClipDistance(5);
            camNode.attachObject(cam);

            camMan = new CameraMan(camNode);
            camMan.setStyle(CameraStyle.CS_ORBIT);
            camMan.setYawPitchDist(new Radian(0), new Radian(0.3f), 15f);
            camMan.setTarget(pivotNode);

            lightCam = scnMgr.createLight("LightCam");

            lightCamNode = scnMgr.getRootSceneNode().createChildSceneNode();
            lightCamNode.attachObject(lightCam);

            lightCamMan = new CameraMan(lightCamNode);
            lightCamMan.setStyle(CameraStyle.CS_ORBIT);
            lightCamMan.setYawPitchDist(new Radian(camManYaw), new Radian(camManPitch), camManDist);

            /*var vp = getRenderWindow().addViewport(cam);
            vp.setBackgroundColour(new ColourValue(.3f, .3f, .3f));*/

            //---------------------------------
            scnMgr.setShadowTechnique(ShadowTechnique.SHADOWTYPE_TEXTURE_MODULATIVE);
            scnMgr.setShadowTextureSettings(1024, 1, PixelFormat.PF_L8);
            scnMgr.setShadowDirLightTextureOffset(0);
            //---------------------------------

            var ent = scnMgr.createEntity("Sinbad.mesh");
            var node = scnMgr.getRootSceneNode().createChildSceneNode();
            node.attachObject(ent);

            Plane plane = new Plane(new Vector3(0, 1, 0), 0);

            MeshManager.getSingleton().createPlane("ground", "General", plane, 20, 20, 20, 20, true, 1, 2, 2, new Vector3(0, 0, 1));

            Entity entPlano = scnMgr.createEntity("chao", "ground");

            SceneNode nodePlano = scnMgr.getRootSceneNode().createChildSceneNode("chaoNode");

            entPlano.setCastShadows(false);
            entPlano.setMaterialName("Examples/Rockwall");
            nodePlano.attachObject(entPlano);
            nodePlano.setPosition(0, -5, 0);
        }

        private void OgreHeadScene()
        {
            #region Ambient Light

            scnMgr.setAmbientLight(new ColourValue(0.1f, 0.1f, 0.1f));

            #endregion

            #region Camera

            camNode = scnMgr.getRootSceneNode().createChildSceneNode();
            cam = scnMgr.createCamera("myCam");
            cam.setNearClipDistance(5);
            cam.setAutoAspectRatio(true);
            camNode.setPosition(0, 0, 100);
            camNode.attachObject(cam);

            #endregion

            #region Light

            Light mainLight = scnMgr.createLight("MainLight");
            SceneNode lightNode = scnMgr.getRootSceneNode().createChildSceneNode();
            lightNode.setPosition(20, 80, 50);
            lightNode.attachObject(mainLight);

            #endregion

            #region Entity

            Entity entity = scnMgr.createEntity("ogrehead.mesh");
            SceneNode entityNode = scnMgr.getRootSceneNode().createChildSceneNode(new Vector3(0, 0, 0));
            entityNode.attachObject(entity);

            #endregion
        }

        private void NinjaScene()
        {
            #region Ambient Light

            scnMgr.setAmbientLight(new ColourValue(0f, 0f, 0f));

            #endregion

            #region Camera

            camNode = scnMgr.getRootSceneNode().createChildSceneNode();
            cam = scnMgr.createCamera("myCam");
            camNode.setPosition(200, 300, 400);
            camNode.lookAt(new Vector3(0, 0, 0), Node.TransformSpace.TS_WORLD);
            cam.setNearClipDistance(5);
            cam.setAspectRatio(1.77f);
            cam.setFOVy(new Radian(new Degree(60)));
            camNode.attachObject(cam);

            #endregion

            #region Camera Man

            camMan = new CameraMan(camNode);
            camMan.setStyle(CameraStyle.CS_ORBIT);
            camManYaw = 120;
            camManPitch = -200;
            camManDist = 800;
            camMan.setYawPitchDist(new Radian(camManYaw), new Radian(camManPitch), camManDist);

            #endregion

            #region Spotlight

            Light spotLight = scnMgr.createLight("spotLight");
            spotLight.setDiffuseColour(0, 0, 1);
            spotLight.setSpecularColour(0, 0, 1);
            spotLight.setType(Light.LightTypes.LT_SPOTLIGHT);

            SceneNode spotLightNode = scnMgr.getRootSceneNode().createChildSceneNode();
            spotLightNode.attachObject(spotLight);
            spotLightNode.setDirection(0, 0, 0);
            spotLightNode.setPosition(new Vector3(200, 200, 300));
            spotLight.setSpotlightRange(new Radian(new Degree(35)), new Radian(new Degree(50)));

            #endregion

            #region Entity            

            Entity ninjaEntity1 = scnMgr.createEntity("ninja.mesh");
            SceneNode ninjaNode1 = scnMgr.getRootSceneNode().createChildSceneNode(new Vector3(100, 0, 100));
            ninjaEntity1.setCastShadows(true);
            ninjaNode1.attachObject(ninjaEntity1);

            Entity ninjaEntity2 = scnMgr.createEntity("ninja.mesh");
            SceneNode ninjaNode2 = scnMgr.getRootSceneNode().createChildSceneNode(new Vector3(-100, 0, 100));
            ninjaEntity2.setCastShadows(true);
            ninjaNode2.attachObject(ninjaEntity2);

            Entity ninjaEntity3 = scnMgr.createEntity("ninja.mesh");
            SceneNode ninjaNode3 = scnMgr.getRootSceneNode().createChildSceneNode(new Vector3(100, 0, -100));
            ninjaEntity3.setCastShadows(true);
            ninjaNode3.attachObject(ninjaEntity3);

            Entity ninjaEntity4 = scnMgr.createEntity("ninja.mesh");
            SceneNode ninjaNode4 = scnMgr.getRootSceneNode().createChildSceneNode(new Vector3(-100, 0, -100));
            ninjaEntity4.setCastShadows(true);
            ninjaNode4.attachObject(ninjaEntity4);

            #endregion

            #region Plane

            Plane plane = new Plane(new Vector3(0, 1, 0), 0);

            MeshManager.getSingleton().createPlane("ground", "General",
                plane,
                1500, 1500, 20, 20,
                true,
                1, 5, 5,
                new Vector3(0, 0, 1));

            Entity groundEntity = scnMgr.createEntity("ground");
            SceneNode groundEntityNode = scnMgr.getRootSceneNode().createChildSceneNode();
            groundEntity.setCastShadows(false);
            groundEntity.setMaterialName("Examples/Rockwall");
            groundEntityNode.attachObject(groundEntity);

            #endregion
        }

        #endregion

        private void StaticGeometryScene()
        {
            StaticGeometry staticGeometry = scnMgr.createStaticGeometry("MyStaticGeometry");
            staticGeometry.setRegionDimensions(new Vector3(140, 140, 140));
            staticGeometry.setOrigin(new Vector3(70, 70, 70));
            staticGeometry.setCastShadows(true);

            Vector3 sgLocation = staticGeometry.getOrigin();

            Entity treeEntity = scnMgr.createEntity("ogrehead.mesh");

            for (int x = -280; x < 280; x += 20)
            {
                for (int z = -280; z < 280; z += 20)
                {
                    Vector3 pos = new Vector3(x + Math.RangeRandom(-7, 7), 0, z + Math.RangeRandom(-7, 7));

                    Quaternion ori = new Quaternion(new Radian(new Degree(Math.RangeRandom(0, 359))), new Vector3(0, 1, 0));

                    Vector3 scale = new Vector3(1, Math.RangeRandom(0.85f, 1.15f), 1);

                    staticGeometry.addEntity(treeEntity, pos, ori, scale);
                }
            }

            staticGeometry.build();
        }

        /// <summary>
        /// Adiciona uma entidade a cena pelo no do mesh.
        /// </summary>
        /// <param name="entityName"></param>
        public void AddEntityToScene(string entityName)
        {
            try
            {
                #region Entity

                Entity entity = scnMgr.createEntity(entityName + ".mesh");

                entityName += "@" + Guid.NewGuid();

                entityDictionary.Add(entityName, entity);

                float entityHeight = entity.getBoundingBox().getMaximum().y - entity.getBoundingBox().getMinimum().y / 2;

                SceneNode entityNode = scnMgr.getRootSceneNode().createChildSceneNode(entityName, new Vector3(0, 5, 0));
                entityNode.lookAt(entityNode.getPosition(), Node.TransformSpace.TS_WORLD);
                entity.setCastShadows(true);
                entityNode.attachObject(entity);

                #endregion

                Debug.Print($"Adicionada({entityName})");

                if (ShowTerrain)
                    ClampToTerrain(entity, entityNode);

                ZoomAll(); //Reposiociona a câmera toda a vez que for adiciona uma entidade, mas só vai mexer na câmera se alterar o bounding box global.
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao criar a entidade: " + ex.Message);
            }
        }

        void ClampToTerrain(Entity entity, SceneNode entityNode)
        {
            AxisAlignedBox entityAabb = entity.getBoundingBox();

            float entityYAxis = entityAabb.getMinimum().y;

            float terrainYAxis = mTerrainGroup.getHeightAtWorldPosition(entityNode.getPosition());

            entityNode.setPosition(0, -entityYAxis + terrainYAxis, 0);
        }

        public void AddPlaneToScene()
        {
            #region Plane

            Plane plane = new Plane(new Vector3(0, 1, 0), new Vector3(0, 0, 0));

            MeshManager.getSingleton().createPlane("ground", "General",
                plane,
                400, 400, 20, 20,
                true,
                1, 5, 5,
                new Vector3(0, 0, 1));

            Entity groundEntity = scnMgr.createEntity("ground");
            SceneNode groundEntityNode = scnMgr.getRootSceneNode().createChildSceneNode();
            groundEntity.setCastShadows(false);
            groundEntity.setMaterialName("Examples/Rockwall");
            groundEntityNode.attachObject(groundEntity);

            #endregion

            if (ShowTerrain)
                ClampToTerrain(groundEntity, groundEntityNode);

            ZoomAll();
        }

        public void AddSkyBox(string option)
        {
            if (!ShowSkybox) return;

            switch (option)
            {
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
            byte numLayer = mTerrainGroup.getTerrain(0, 0).getLayerCount();            

            for (byte i = 0; i < numLayer; i++)
            {
                mTerrainGroup.getTerrain(0, 0).removeLayer(i);                
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

        public Vector3 EntitySize()
        {
            AxisAlignedBox boundingBox = lastEntityNode._getWorldAABB();

            Vector3 min = boundingBox.getMinimum();

            Vector3 max = boundingBox.getMaximum();

            Vector3 size = max - min;

            return size;
        }

        public void CleanScene()
        {
            if (scnMgr == null) return;

            if (lastEntityNode != null)
            {
                scnMgr.destroyEntity(lastEntityNode.getAttachedObject(0));

                entityDictionary.Remove(lastEntityNode.getName());

                lastEntityNode = null;

                return;
            }

            foreach (var entity in entityDictionary.Values)
            {
                scnMgr.destroyEntity(entity);
            }

            entityDictionary.Clear();

            MeshManager.getSingleton().removeAll();
        }
    }
}