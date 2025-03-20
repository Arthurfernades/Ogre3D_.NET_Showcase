using org.ogre;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static OgreEngine.Variaveis;

namespace OgreImage
{
    /// <summary>
    /// Interaction logic for AuEOgre.xaml
    /// </summary>
    public partial class AuEOgre : UserControl
    {
        #region Variables

        private bool isCameraOrbitMoving = false;

        private bool isCameraPanMoving = false;

        private bool isOgreInitialized = false;

        public bool isLightModeOn = false;

        public bool isShowingGlobalGrid = false;

        public bool isShowingFog = false;

        public bool isMovingObject = false;

        private Point startPoint;

        public eVPProjectionStyle projectionStyle = eVPProjectionStyle.eVPPerspective;

        public eCameraView cameraview = eCameraView.eOrbitCamera;

        #endregion

        public AuEOgre()
        {
            InitializeComponent();

            myImage = new OgreEngine.OgreImage();
        }

        /// <summary>
        /// Inicializa o Ogre.
        /// </summary>
        public void Init()
        {
            img.Source = myImage;

            myImage.InitOgre();

            myImage.Resize((int)ActualWidth, (int)ActualHeight);

            isOgreInitialized = true;

            myImage.RenderOneFrame();
        }

        public void Dispose()
        {
            myImage.DisposeOgre();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!isOgreInitialized) return;

            myImage.Resize((int)ActualWidth, (int)ActualHeight);

            myImage.RenderOneFrame();
        }

        #region Manipulação Ogre

        #region Mouse Listeners

        /// <summary>
        /// Passa o evento de movimento do mouse para função responsável.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isCameraOrbitMoving && !isCameraPanMoving && !isMovingObject) return;

            Move(e.GetPosition(this));
        }

        public void EntitySize()
        {
            Vector3 size = myOgre.EntitySize();

            //MessageBox.Show("- Entity Size -\nWidht: " + size.x + "\nHeigth: " + size.y + "\nDepth: " + size.z);
        }

        /// <summary>
        /// Desabilita o movimento de câmera se o mouse sair do componente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isCameraOrbitMoving)
                isCameraOrbitMoving = false;

            if (isCameraPanMoving)
                isCameraPanMoving = false;
        }

        /// <summary>
        /// Movimenta no eixo Z. Basicamente aproxima e afasta a câmera da cena.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {            
            if(myOgre.lastEntityNode != null)
            {
                int zoomAmount = e.Delta > 0 ? -40 : 40;
                UpdateObjectPosition(0, 0, zoomAmount);
            }
            else
            {
                int zoomAmount = e.Delta > 0 ? -5 : 5;
                UpdateCameraZAxis(zoomAmount);
            }
        }

        /// <summary>
        /// Ativa movimento da câmera Orbit/Pan e click do mouse nas entidades.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //!!!!!!!! Tomar cuidado com esse método pois pode gerar gargalos, switch foi a forma mais otimizada !!!!!!!! (Arthur 20/02/2025)

            Point point = e.GetPosition(this);
            startPoint = point;

            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    isCameraOrbitMoving = true;
                    break;

                case MouseButton.Middle:
                    isCameraPanMoving = true;
                    break;

                case MouseButton.Left:
                    string entityName = myOgre.PerformPicking((int)point.X, (int)point.Y);

                    switch (entityName)
                    {
                        case "StaticGeometry":
                            MessageBox.Show("StaticGeometry foi selecionada");
                            break;

                        case null:
                            UnselectEntity();
                            break;

                        default:
                            SelectEntity(entityName);
                            EntitySize();
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Desativa câmera Orbit/Pan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    isCameraOrbitMoving = false;
                    break;

                case MouseButton.Middle:
                    isCameraPanMoving = false;
                    break;
            }
        }

        #endregion

        #region Key Listeners

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (cameraview != eCameraView.eFreeLookCamera) return;

            switch (e.Key)
            {
                case Key.W:
                    myOgre.MovePivotNode("Forward", 1);
                    break;

                case Key.A:
                    myOgre.MovePivotNode("Leftward", 1);
                    break;

                case Key.S:
                    myOgre.MovePivotNode("Backward", -1);
                    break;

                case Key.D:
                    myOgre.MovePivotNode("Rightward", -1);
                    break;

                case Key.Space:
                    myOgre.MovePivotNode("Upward", 1);
                    break;

                case Key.LeftCtrl:
                    myOgre.MovePivotNode("Downward", -1);
                    break;

            }

            myImage.RenderOneFrame();
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (cameraview != eCameraView.eFreeLookCamera && e.Key == Key.LeftCtrl)
                isMovingObject = false;
        }

        #endregion

        #region Manipulate Entity

        /// <summary>
        /// Seleciona a entidade clicada na cena.
        /// </summary>
        /// <param name="entityName"></param>
        private void SelectEntity(string entityName)
        {
            myOgre.SelectEntity(entityName);

            myImage.RenderOneFrame();
        }

        /// <summary>
        /// Tira a seleção da entidade na cena.
        /// </summary>
        private void UnselectEntity()
        {
            myOgre.UnselectEntity();

            myImage.RenderOneFrame();
        }

        #endregion

        #region Manipulate Camera

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void Move(Point currentPoint)
        {
            Vector delta = currentPoint - startPoint;

            if (isCameraOrbitMoving)
                UpdateCameraXYAxis((int)delta.X, (int)delta.Y);
            else if (isCameraPanMoving)
                UpdateCameraPosition((int)delta.X, (int)delta.Y);
            else if (isMovingObject)
                UpdateObjectPosition((int)delta.X, (int)delta.Y, 0);

            startPoint = currentPoint;
        }

        private void UpdateObjectPosition(int x, int y, int z)
        {
            myOgre.MoveObject(x, y, z);

            myImage.RenderOneFrame();
        }

        /// <summary>
        /// Move a camera em orbita.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void UpdateCameraXYAxis(int x, int y)
        {
            myOgre.MoveOrbitCam(x, y);

            myImage.RenderOneFrame();
        }

        private void UpdateCameraPosition(int x, int y)
        {
            myOgre.MovePanCam(x, y);

            myImage.RenderOneFrame();
        }

        /// <summary>
        /// Aproxima ou afasta a câmera do ponto focal.
        /// </summary>
        /// <param name="delta"></param>
        private void UpdateCameraZAxis(float delta)
        {
            myOgre.PushCamera(delta);

            myImage.RenderOneFrame();
        }

        /// <summary>
        /// Zoom para todas as entidades do ambiente
        /// </summary>
        public void ZoomAll()
        {
            myOgre.ZoomAll();

            myImage.RenderOneFrame();
        }

        /// <summary>
        /// Muda a câmera para ângulos fixos.
        /// </summary>
        /// <param name="tag"></param>
        public void VP_Camera(eVPCamera tag)
        {
            myOgre.ChangeCameraDirection(tag);

            myImage.RenderOneFrame();
        }

        /// <summary>
        /// Altera o estilo da câmera para perspectiva ou ortogonal.
        /// </summary>
        /// <param name="tag"></param>
        public void CamProjectionStyle(eVPProjectionStyle tag)
        {
            switch (tag)
            {
                case eVPProjectionStyle.eVPPerspective:
                    myOgre.ChangeCameraPerspective(eVPProjectionStyle.eVPPerspective);
                    projectionStyle = eVPProjectionStyle.eVPPerspective;
                    break;

                case eVPProjectionStyle.eVPOrthographic:
                    myOgre.ChangeCameraPerspective(eVPProjectionStyle.eVPOrthographic);
                    projectionStyle = eVPProjectionStyle.eVPOrthographic;
                    break;
            }

            myImage.RenderOneFrame();
        }

        public void ChangeCameraView(eCameraView tag)
        {
            switch (tag)
            {
                case eCameraView.eOrbitCamera:
                    myOgre.ChangeCameraView(eCameraView.eOrbitCamera);
                    cameraview = eCameraView.eOrbitCamera;
                    break;

                case eCameraView.eFreeLookCamera:
                    myOgre.ChangeCameraView(eCameraView.eFreeLookCamera);
                    cameraview = eCameraView.eFreeLookCamera;
                    Focus();
                    break;
            }

            myImage.RenderOneFrame();
        }

        #endregion

        #region Manipulate Scene

        /// <summary>
        /// Adiciona uma nova entidade a cena.
        /// </summary>
        /// <param name="entityName"></param>
        public void AddNewEntityToScene(string entityName)
        {
            myOgre.AddEntityToScene(entityName);

            myImage.RenderOneFrame();
        }

        /// <summary>
        /// Adiciona um plano a cena.
        /// </summary>
        public void AddNewPlaneToScene()
        {
            myOgre.AddPlaneToScene();

            myImage.RenderOneFrame();
        }

        /// <summary>
        /// Mexe na luz ambiete da cena.
        /// </summary>
        /// <param name="value"></param>
        public void UpdateBrightness(float value)
        {
            myOgre.UpdateBrightness(value);

            myImage.RenderOneFrame();
        }

        public void ShowGlobalAxis()
        {
            myOgre.ShowGlobalAxis();

            myImage.RenderOneFrame();

            isShowingGlobalGrid = true;
        }

        public void HideGlobalAxis()
        {
            myOgre.HideGlobalAxis();

            myImage.RenderOneFrame();

            isShowingGlobalGrid = false;
        }

        public void ShowFog()
        {
            myOgre.ShowFog();

            myImage.RenderOneFrame();

            isShowingFog = true;
        }

        public void HideFog()
        {
            myOgre.HideFog();

            myImage.RenderOneFrame();

            isShowingFog = false;
        }

        /// <summary>
        /// Tira as entidades e mesh builder da cena.
        /// </summary>
        public void ClearScene()
        {
            myOgre.CleanScene();

            myImage.RenderOneFrame();
        }

        #endregion

        /// <summary>
        /// Salva o que está sendo visto no disco
        /// </summary>
        /// <param name="vArquivo"></param>
        public void PrintScreen(string vArquivo)
        {
            myImage.SalvaRender(vArquivo);
        }

        public void ChangeSky(string selected)
        {
            if (!myOgre.MostraSkybox) return;

            myOgre.AddSkyBox(selected);

            myImage.RenderOneFrame();
        }

        public void ChangeTerrain(string selected)
        {
            if (!myOgre.MostraTerreno) return;

            myOgre.AddTerrain(selected);

            myImage.RenderOneFrame();
        }

        /// <summary>
        /// Altera os efeitos da câmera.
        /// </summary>
        /// <param name="selectedCompositor"></param>
        public void ChangeCompositor(string selected)
        {
            myOgre.ChangeCompositor(selected);

            myImage.RenderOneFrame();
        }

        #endregion

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (isMovingObject)
                isMovingObject = false;
            else
                isMovingObject = true;
        }        
    }
}
