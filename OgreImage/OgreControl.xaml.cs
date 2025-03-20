using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OgreImage
{
    public partial class OgreControl : UserControl
    {
        public OgreEngine myD3DImage;

        public OgreControl()
        {
            InitializeComponent();

            myD3DImage = new OgreEngine();

            img.Source = myD3DImage;
        }

        public void Init()
        {            
            myD3DImage.InitOgre();

            myD3DImage.Resize((int)ActualWidth, (int)ActualHeight);            
        }

        public void Dispose()
        {
            myD3DImage.DisposeOgre();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
            myD3DImage.Resize((int)ActualWidth, (int)ActualHeight);
        }

        #region Mouse Listeners

        private void UserControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {            
            myD3DImage.Move(e.GetPosition(this));
        }        

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (myD3DImage.isCameraOrbitMoving)
                myD3DImage.isCameraOrbitMoving = false;

            if (myD3DImage.isCameraPanMoving)
                myD3DImage.isCameraPanMoving = false;

            if(myD3DImage.isMovingObject)
                myD3DImage.isMovingObject = false;
        }

        private void UserControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {            
            if(myD3DImage.isMovingObject)
            {
                int zoomAmount = e.Delta > 0 ? -40 : 40;
                myD3DImage.UpdateObjectPosition(0, 0, zoomAmount);
            }
            else
            {
                int zoomAmount = e.Delta > 0 ? -5 : 5;
                myD3DImage.UpdateCameraZAxis(zoomAmount);
            }
        }

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(this);

            myD3DImage.startPoint = point;

            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    myD3DImage.isCameraOrbitMoving = true;
                    break;

                case MouseButton.Middle:
                    myD3DImage.isCameraPanMoving = true;
                    break;

                case MouseButton.Left:
                    myD3DImage.RayPicking((int)point.X, (int)point.Y);                    
                    break;
            }
        }

        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                    myD3DImage.isCameraOrbitMoving = false;
                    break;

                case MouseButton.Middle:
                    myD3DImage.isCameraPanMoving = false;
                    break;
            }
        }

        #endregion

        #region Key Listeners

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (myD3DImage.cameraview != MyEnum.eCameraView.eFreeLookCamera) return;

            switch (e.Key)
            {
                case Key.W:
                    myD3DImage.UpdatePivotPosition("Forward");
                    break;

                case Key.A:
                    myD3DImage.UpdatePivotPosition("Leftward");
                    break;

                case Key.S:
                    myD3DImage.UpdatePivotPosition("Backward");
                    break;

                case Key.D:
                    myD3DImage.UpdatePivotPosition("Rightward");
                    break;

                case Key.Space:
                    myD3DImage.UpdatePivotPosition("Upward");
                    break;

                case Key.LeftCtrl:
                    myD3DImage.UpdatePivotPosition("Downward");
                    break;

            }
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (myD3DImage.cameraview != MyEnum.eCameraView.eFreeLookCamera && e.Key == Key.LeftCtrl)
                myD3DImage.isMovingObject = false;
        }

        #endregion

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (myD3DImage.isMovingObject)
                myD3DImage.isMovingObject = false;
            else
                myD3DImage.isMovingObject = true;
        }        
    }
}