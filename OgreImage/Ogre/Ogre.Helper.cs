using org.ogre;
using System;
using System.Windows;
using System.Windows.Interop;
using Camera = org.ogre.Camera;

namespace OgreEngine
{
    public partial class Ogre
    {
        #region Variáveis

        public string compositor;

        public bool isLightCamOn = false;

        private float lightCamManYaw, lightCamManPitch, lightCamManDist;

        private float realRedValue, realGreenValue, realBlueValue;

        private Camera spotLightCam;

        private SceneNode lastEntityNode;

        AxisAlignedBox globalBoundingBox;

        #endregion

        #region Entity

        /// <summary>
        /// Seleciona uma Entity na cena, caso tenha uma já selecionada irá trocar a seleção.
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

        /// <summary>
        /// Caso tenha uma Entity selecionada irá remover a seleção.
        /// </summary>
        public void UnselectEntity()
        {
            lastEntityNode?.showBoundingBox(false);
            lastEntityNode = null;
        }

        #endregion

        #region Camera

        public void MoveOrbitCam(int deltaX, int deltaY)
        {
            float sense = 0.2f;

            if (!(CameraColided() && deltaY < 0))
                camManPitch += deltaY * sense;

            camManYaw += -deltaX * sense;

            camMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);

            lightCamMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
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
            lightCamMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
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

            lightCamMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
        }

        public void MoveObject(int deltaX, int deltaY)
        {
            if (lastEntityNode == null) return;

            float sense = camManDist * 0.0012f;

            Vector3 C = camNode.getPosition();
            Vector3 P = lastEntityNode.getPosition();

            Vector3 V = C - P;
            V.normalise();

            Vector3 movement = new Vector3();

            Vector3 side = new Vector3(0, 1, 0).crossProduct(V);

            side.normalise();

            movement.x = side.x * deltaX * sense;
            movement.y = side.y * deltaX * sense;
            movement.z = side.z * deltaX * sense;

            Vector3 upDown = new Vector3(0, -1, 0);

            movement.x += upDown.x * deltaY * sense;
            movement.y += upDown.y * deltaY * sense;
            movement.z += upDown.z * deltaY * sense;

            lastEntityNode.translate(movement, Node.TransformSpace.TS_WORLD);
        }

        /// <summary>
        /// Movimenta Camera na coordenada Z.
        /// </summary>
        /// <param name="deltaZ"></param>
        public void PushCamera(float deltaZ)
        {
            float sense = camManDist * 0.02f;

            deltaZ *= sense;

            camManDist += deltaZ;

            camMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);

            lightCamMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
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
                // Obter todos os objetos movíveis da cena
                MovableObjectMap entityList = scnMgr.getMovableObjects("Entity");

                // Calcular o bounding box global de todos os objetos
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

            // Obter as coordenadas máximas e mínimas do bounding box
            Vector3 max = globalBoundingBox.getMaximum();
            Vector3 min = globalBoundingBox.getMinimum();
            Vector3 center = globalBoundingBox.getCenter();

            // Calcular a diagonal do bounding box
            Vector3 diagonal = new Vector3(max.x - min.x, max.y - min.y, max.z - min.z);
            float radius = diagonal.length();

            // Obter o FOV da câmera
            float fovY = cam.getFOVy().valueRadians();

            // Calcular a distância horizontal e vertical necessária para o zoom
            float distanceH = radius / (float)System.Math.Tan(fovY);
            float aspectRatio = TextureWidth / TextureHeight;
            fovY *= aspectRatio;
            float distanceV = radius / (float)System.Math.Tan(fovY);

            // A distância final é a maior entre a horizontal e a vertical
            float distance = System.Math.Max(distanceH, distanceV);

            // Ajustar a distância de clipping da câmera
            if (distance > cam.getFarClipDistance())
                cam.setFarClipDistance(distance * 2);

            // Calcular a direção da câmera
            Vector3 direction = new Vector3(camNode.getPosition().x, min.y + (max.y - min.y) / 2, camNode.getPosition().z) - camNode.getPosition();

            direction.normalise();

            // Ajustar a posição da câmera na direção correta
            Vector3 vNewDirection = new Vector3(direction.x * distance, direction.y * distance, direction.z * distance);
            var newPosition = (center - vNewDirection);

            // Atualizar a posição da câmera
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

            lightCamMan.setYawPitchDist(new Radian(new Degree(camManYaw)), new Radian(new Degree(camManPitch)), camManDist);
        }

        public void ChangeCameraDirection(Variaveis.eVPCamera tag)
        {
            switch (tag)
            {
                case Variaveis.eVPCamera.eVPFRONT:
                    camManYaw = 180;
                    break;
                case Variaveis.eVPCamera.eVPNW:
                    camManYaw = 135;
                    break;
                case Variaveis.eVPCamera.eVPNE:
                    camManYaw = 225;
                    break;
                case Variaveis.eVPCamera.eVPBACK:
                    camManYaw = 0;
                    break;
                case Variaveis.eVPCamera.eVPSW:
                    camManYaw = 45;
                    break;
                case Variaveis.eVPCamera.eVPSE:
                    camManYaw = 315;
                    break;
                case Variaveis.eVPCamera.eVPRIGHT:
                    camManYaw = 270;
                    break;
                case Variaveis.eVPCamera.eVPLEFT:
                    camManYaw = 90;
                    break;
                case Variaveis.eVPCamera.eVPTop:
                    camManPitch = (camManPitch == 90) ? 0 : 90;
                    camManYaw = 0;
                    break;
            }

            if (tag == Variaveis.eVPCamera.eVPFRONT || tag == Variaveis.eVPCamera.eVPBACK ||
                tag == Variaveis.eVPCamera.eVPRIGHT || tag == Variaveis.eVPCamera.eVPLEFT)
                camManPitch = 0;
            else if (tag != Variaveis.eVPCamera.eVPTop)
                camManPitch = 30;

            ZoomAll();
        }

        public void ChangeCameraPerspective(Variaveis.eVPProjectionStyle eVPPerspective)
        {
            switch (eVPPerspective)
            {
                case Variaveis.eVPProjectionStyle.eVPPerspective:
                    cam.setProjectionType(ProjectionType.PT_PERSPECTIVE);
                    cam.setFOVy(new Radian(new Degree(45)));
                    break;

                case Variaveis.eVPProjectionStyle.eVPOrthographic:
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

        public void ChangeCameraView(Variaveis.eCameraView eCameraView)
        {
            switch (eCameraView)
            {
                case Variaveis.eCameraView.eOrbitCamera:
                    camMan.setStyle(CameraStyle.CS_ORBIT);
                    ZoomAll();
                    break;

                case Variaveis.eCameraView.eFreeLookCamera:
                    camMan.setStyle(CameraStyle.CS_FREELOOK);
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Mexe nos 3 valores da luz ambiente simultâneamente.
        /// </summary>
        /// <param name="value"></param>
        public void UpdateBrightness(float value)
        {
            realRedValue = realGreenValue = realBlueValue = value / 100;

            scnMgr.setAmbientLight(new ColourValue(realRedValue, realGreenValue, realBlueValue));
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

        /// <summary>
        /// Normaliza as cooerdenadas WPF para a cena no Ogre e chama o méto CheckRayIntersection.
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        /// <returns>entity.getName()</returns>
        public string PerformPicking(int mouseX, int mouseY)
        {
            Camera camera = viewport.getCamera();

            float normalizedX = (float)mouseX / viewport.getActualWidth();
            float normalizedY = (float)mouseY / viewport.getActualHeight();

            Ray mouseRay = camera.getCameraToViewportRay(normalizedX, normalizedY);

            return CheckRayIntersection(mouseRay);
        }

        /// <summary>
        /// Verifica se o raio bateu em uma Entity, StaticGeometry ou em nada.
        /// </summary>
        /// <param name="mouseRay"></param>
        /// <returns>entity.getName()</returns>
        private string CheckRayIntersection(Ray mouseRay)
        {
            RaySceneQuery rayQuery = scnMgr.createRayQuery(mouseRay);

            //Filtro de obejtos
            //
            rayQuery.setQueryMask(0xFFFFFFFF);
            rayQuery.setSortByDistance(true);

            RaySceneQueryResult result = rayQuery.execute();

            foreach (var hit in result)
            {
                if (hit.movable != null)
                {
                    string hitName = hit.movable.getName();
                    string hitType = hit.movable.getMovableType();

                    if (hit.movable.getMovableType() == "StaticGeometry")
                    {
                        return "StaticGeometry";
                    }
                    Entity entity = hit.movable.castEntity();

                    //Apenas para testes
                    //
                    //entity?.getParentSceneNode().getName();
                    //entity?.getName();

                    return entity?.getName();
                }
            }

            return null;
        }

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

        private void InitCompositors()
        {
            CompositorManager.getSingleton().addCompositor(viewport, "Bloom");
            CompositorManager.getSingleton().addCompositor(viewport, "Glass");
            CompositorManager.getSingleton().addCompositor(viewport, "B&W");
            CompositorManager.getSingleton().addCompositor(viewport, "Posterize");
            CompositorManager.getSingleton().addCompositor(viewport, "Tiling");
            CompositorManager.getSingleton().addCompositor(viewport, "DoF");
            CompositorManager.getSingleton().addCompositor(viewport, "Embossed");
            CompositorManager.getSingleton().addCompositor(viewport, "Invert");
            CompositorManager.getSingleton().addCompositor(viewport, "Laplace");
            CompositorManager.getSingleton().addCompositor(viewport, "Old Movie");
            CompositorManager.getSingleton().addCompositor(viewport, "Radial Blur");
            CompositorManager.getSingleton().addCompositor(viewport, "ASCII");
            CompositorManager.getSingleton().addCompositor(viewport, "Halftone");
            CompositorManager.getSingleton().addCompositor(viewport, "Dither");
            CompositorManager.getSingleton().addCompositor(viewport, "Compute");
            CompositorManager.getSingleton().addCompositor(viewport, "CubeMap");
            CompositorManager.getSingleton().addCompositor(viewport, "Fresnel");
            CompositorManager.getSingleton().addCompositor(viewport, "WBOIT");
        }

        public void ChangeCompositor(string vCompositor)
        {
            if (compositor != null)
                CompositorManager.getSingleton().setCompositorEnabled(viewport, compositor, false);

            if (vCompositor == "Normal") return;

            CompositorManager.getSingleton().setCompositorEnabled(viewport, vCompositor, true);

            compositor = vCompositor;
        }

        /// <summary>
        /// Salva imagem do Ogre no disco
        /// </summary>
        public void SalvaRender(string vArquivo)
        {
            renderTarget.writeContentsToFile(vArquivo);
        }

        /// <summary>
        /// Pega o Handle da janela WPF.
        /// </summary>
        /// <returns></returns>
        public IntPtr getHandle()
        {
            IntPtr hWnd = IntPtr.Zero;

            foreach (PresentationSource source in PresentationSource.CurrentSources)
            {
                var hwndSource = (source as HwndSource);
                if (hwndSource != null)
                {
                    hWnd = hwndSource.Handle;
                    break;
                }
            }
            return hWnd;
        }
    }
}