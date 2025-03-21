using org.ogre;
using System.Diagnostics;
using System.Windows;
using System;
using Math = org.ogre.Math;
using System.Collections.Generic;

namespace OgreImage
{
    public partial class Ogre
    {
        public SceneNode lastEntityNode;

        private Dictionary<string, Entity> entityDictionary = new Dictionary<string, Entity>();

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

                if (isTerrainLoaded)
                    ClampToTerrain(entity, entityNode);

                ZoomAll(); //Reposiociona a câmera toda a vez que for adiciona uma entidade, mas só vai mexer na câmera se alterar o bounding box global.
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao criar a entidade: " + ex.Message);
            }
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

            if (isTerrainLoaded)
                ClampToTerrain(groundEntity, groundEntityNode);

            ZoomAll();
        }

        public void AddStaticGeometryToScene()
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

            //TODO: Implement ClampToTerrain function.
        }

        private void ClampToTerrain(Entity entity, SceneNode entityNode)
        {
            AxisAlignedBox entityAabb = entity.getBoundingBox();

            float entityYAxis = entityAabb.getMinimum().y;

            float terrainYAxis = mTerrainGroup.getHeightAtWorldPosition(entityNode.getPosition());

            entityNode.setPosition(0, -entityYAxis + terrainYAxis, 0);
        }

        public Vector3 EntitySize()
        {
            AxisAlignedBox boundingBox = lastEntityNode._getWorldAABB();

            Vector3 min = boundingBox.getMinimum();

            Vector3 max = boundingBox.getMaximum();

            Vector3 size = max - min;

            return size;
        }

        public void MoveObject(int deltaX, int deltaY, int deltaZ)
        {
            if (lastEntityNode == null) return;

            float sense = lastEntityNode.getPosition().distance(camNode.getPosition()) * 0.0012f;

            Vector3 camPos = camNode.getPosition();
            Vector3 entityPos = lastEntityNode.getPosition();

            Vector3 look = camPos - entityPos;
            look.normalise();

            Vector3 globalUp = new Vector3(0, 1, 0);

            Vector3 right, up, back;

            float overheadThreshold = 0.9f;

            if (System.Math.Abs(look.dotProduct(globalUp)) > overheadThreshold)
            {
                right = new Vector3(1, 0, 0);
                up = new Vector3(0, 0, -1);
                back = new Vector3(0, -1, 0);
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

                back = right.crossProduct(up);
            }

            Vector3 movement = right.__mul__(deltaX * sense) + up.__mul__(-deltaY * sense) + back.__mul__(deltaZ * sense);

            lastEntityNode.translate(movement, Node.TransformSpace.TS_WORLD);
        }

        /// <summary>
        /// Selects an Entity in the scene, if one is already selected it will change the selection.
        /// </summary>
        /// <param name="entityName"></param>
        public void SelectEntity(string entityName)
        {
            SceneNode currentEntityNode = scnMgr.getEntity(entityName).getParentSceneNode();

            if (lastEntityNode == null)
            {
                currentEntityNode.showBoundingBox(true);
            }
            else if (currentEntityNode != lastEntityNode)
            {
                lastEntityNode.showBoundingBox(false);

                currentEntityNode.showBoundingBox(true);
            }

            lastEntityNode = currentEntityNode;
        }

        public void UnselectEntity()
        {
            lastEntityNode?.showBoundingBox(false);
            lastEntityNode = null;
        }

        public void DeleteEntity()
        {
            scnMgr.destroyEntity(lastEntityNode.getAttachedObject(0));

            entityDictionary.Remove(lastEntityNode.getName());

            lastEntityNode = null;
        }
    }
}
