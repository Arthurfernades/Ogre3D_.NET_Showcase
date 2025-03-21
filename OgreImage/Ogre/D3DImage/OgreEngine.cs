using System;
using System.Windows;
using System.Windows.Interop;

namespace OgreImage
{
    public class OgreEngine : D3DImage
    {
        private readonly Ogre myOgre;

        #region Variables

        public bool isCameraOrbitMoving = false;

        public bool isCameraPanMoving = false;

        public bool isOgreInitialized = false;

        public bool isLightModeOn = false;

        public bool isShowingGlobalGrid = false;

        public bool isShowingFog = false;

        public bool isMovingObject = false;

        public Point startPoint;

        public MyEnum.eVPProjectionStyle projectionStyle = MyEnum.eVPProjectionStyle.eVPPerspective;

        public MyEnum.eCameraView cameraview = MyEnum.eCameraView.eOrbitCamera;

        #endregion

        public OgreEngine()
        {
            myOgre = new Ogre();
        }

        public void InitOgre()
        {
            try
            {
                myOgre.initApp();
            }
            catch (Exception ex)
            {
                MessageBox.Show("InitOgre.Erro: " + ex.Message.ToString());
            }

            isOgreInitialized = true;
        }

        public void Resize(int width, int height)
        {
            if (!isOgreInitialized) return;

            myOgre.TextureHeight = height;
            myOgre.TextureWidth = width;

            myOgre.CreateTexture11();

            AttachRenderTargert();

            RenderOneFrame();
        }

        private void AttachRenderTargert()
        {
            if (IsFrontBufferAvailable)
                DetachRenderTarget();

            try
            {
                Lock();

                SetBackBuffer(D3DResourceType.IDirect3DSurface9, myOgre.SharedHandle, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("AttachRenderTargert.Erro: " + ex.Message.ToString());
            }
            finally
            {
                Unlock();
            }
        }

        private void DetachRenderTarget()
        {
            try
            {
                Lock();

                SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("DetachRenderTarget.Erro: " + ex.Message.ToString());
            }
            finally
            {
                Unlock();
            }
        }

        public void RenderOneFrame()
        {
            try
            {
                Lock();

                myOgre.root.renderOneFrame();

                myOgre.renderTarget.doFlush();

                AddDirtyRect(new Int32Rect(0, 0, myOgre.TextureWidth, myOgre.TextureHeight));
            }
            catch (Exception ex)
            {
                MessageBox.Show("RenderOneFrame.Erro: " + ex.Message.ToString());
            }
            finally
            {
                Unlock();
            }
        }

        public void DisposeOgre()
        {
            try
            {
                myOgre.DisposeOgre();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DisposeOgre.Erro: " + ex.Message.ToString());
            }
        }      

        #region Camera

        public void Move(Point currentPoint)
        {
            if (!isCameraOrbitMoving && !isCameraPanMoving && !isMovingObject) return;

            Vector delta = currentPoint - startPoint;

            if (isCameraOrbitMoving)
                myOgre.MoveOrbitCam((int)delta.X, (int)delta.Y);
            else if (isCameraPanMoving)
                myOgre.MovePanCam((int)delta.X, (int)delta.Y);
            else if (isMovingObject)
                myOgre.MoveObject((int)delta.X, (int)delta.Y, 0);

            startPoint = currentPoint;

            RenderOneFrame();
        }

        public void UpdatePivotPosition(string direction)
        {
            myOgre.MovePivotNode(direction, 1);
        }

        public void UpdateObjectPosition(int x, int y, int z)
        {
            myOgre.MoveObject(x, y, z);

            RenderOneFrame();
        }

        public void UpdateCameraXYAxis(int x, int y)
        {
            myOgre.MoveOrbitCam(x, y);

            RenderOneFrame();
        }

        public void UpdateCameraPosition(int x, int y)
        {
            myOgre.MovePanCam(x, y);

            RenderOneFrame();
        }

        public void UpdateCameraZAxis(float delta)
        {
            myOgre.PushCamera(delta);

            RenderOneFrame();
        }

        public void ZoomAll()
        {
            myOgre.ZoomAll();

            RenderOneFrame();
        }

        public void CamProjectionStyle(MyEnum.eVPProjectionStyle tag)
        {
            switch (tag)
            {
                case MyEnum.eVPProjectionStyle.eVPPerspective:
                    myOgre.ChangeCameraPerspective(MyEnum.eVPProjectionStyle.eVPPerspective);
                    projectionStyle = MyEnum.eVPProjectionStyle.eVPPerspective;
                    break;

                case MyEnum.eVPProjectionStyle.eVPOrthographic:
                    myOgre.ChangeCameraPerspective(MyEnum.eVPProjectionStyle.eVPOrthographic);
                    projectionStyle = MyEnum.eVPProjectionStyle.eVPOrthographic;
                    break;
            }

            RenderOneFrame();
        }

        public void ChangeCameraView(MyEnum.eCameraView tag)
        {
            switch (tag)
            {
                case MyEnum.eCameraView.eOrbitCamera:
                    myOgre.ChangeCameraView(MyEnum.eCameraView.eOrbitCamera);
                    cameraview = MyEnum.eCameraView.eOrbitCamera;
                    break;

                case MyEnum.eCameraView.eFreeLookCamera:
                    myOgre.ChangeCameraView(MyEnum.eCameraView.eFreeLookCamera);
                    cameraview = MyEnum.eCameraView.eFreeLookCamera;
                    break;
            }

            RenderOneFrame();
        }

        public void ChangeCompositor(string selected)
        {
            myOgre.ChangeCompositor(selected);

            RenderOneFrame();
        }

        public void PrintScreen(string vArquivo)
        {
            try
            {
                myOgre.SalvaRender(vArquivo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SalvaRender.Erro: " + ex.Message.ToString());
            }
        }

        public void RayPicking(int x, int y)
        {
            string entityName = myOgre.PerformPicking(x, y);

            switch (entityName)
            {
                case "StaticGeometry":
                    MessageBox.Show("StaticGeometry foi selecionada");
                    break;

                case null:
                    myOgre.UnselectEntity();
                    break;

                default:
                    myOgre.SelectEntity(entityName);
                    myOgre.EntitySize();
                    break;
            }

            RenderOneFrame();
        }

        #endregion

        #region Entity

        public void SelectEntity(string entityName)
        {
            myOgre.SelectEntity(entityName);

            RenderOneFrame();
        }

        public void UnselectEntity()
        {
            myOgre.UnselectEntity();

            RenderOneFrame();
        }

        public void EntitySize()
        {
            myOgre.EntitySize();
        }

        #endregion

        #region Scene

        public void AddNewEntityToScene(string entityName)
        {
            myOgre.AddEntityToScene(entityName);

            RenderOneFrame();
        }

        public void AddNewStaticGeometry()
        {
            myOgre.AddStaticGeometryToScene();

            RenderOneFrame();
        }

        public void AddNewPlaneToScene()
        {
            myOgre.AddPlaneToScene();

            RenderOneFrame();
        }

        public void ChangeSky(string selected)
        {
            myOgre.AddSkyBox(selected);

            RenderOneFrame();
        }

        public void ChangeTerrain(string selected)
        {
            myOgre.AddTerrain(selected);

            RenderOneFrame();
        }

        public void UpdateBrightness(float value)
        {
            myOgre.UpdateBrightness(value);

            RenderOneFrame();
        }

        public void ShowGlobalAxis()
        {
            myOgre.ShowGlobalAxis();

            RenderOneFrame();

            isShowingGlobalGrid = true;
        }

        public void HideGlobalAxis()
        {
            myOgre.HideGlobalAxis();

            RenderOneFrame();

            isShowingGlobalGrid = false;
        }

        public void ShowFog()
        {
            myOgre.ShowFog();

            RenderOneFrame();

            isShowingFog = true;
        }

        public void HideFog()
        {
            myOgre.HideFog();

            RenderOneFrame();

            isShowingFog = false;
        }

        public void DeleteEntity()
        {
            myOgre.DeleteEntity();

            RenderOneFrame();
        }

        public void ClearScene()
        {
            myOgre.CleanScene();

            RenderOneFrame();
        }

        #endregion        
    }
}