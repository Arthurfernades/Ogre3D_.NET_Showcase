using org.ogre;
using System;
using System.Windows;
using System.Windows.Interop;
using Camera = org.ogre.Camera;

namespace OgreImage
{
    public partial class Ogre
    {
        #region Variables

        public string compositor;                

        #endregion    

        /// <summary>
        /// Normalize the WPF coordinates for the scene in Ogre and call the CheckRayIntersection method.
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
        /// Verify if the ray hit an Entity or StaticGeometry.
        /// </summary>
        /// <param name="mouseRay"></param>
        /// <returns>entity.getName()</returns>
        private string CheckRayIntersection(Ray mouseRay)
        {
            RaySceneQuery rayQuery = scnMgr.createRayQuery(mouseRay);

            //Object filter.
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

                    //Just for test.
                    //
                    //entity?.getParentSceneNode().getName();
                    //entity?.getName();

                    return entity?.getName();
                }
            }

            return null;
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
        /// Save image in disk.
        /// </summary>
        public void SalvaRender(string vArquivo)
        {
            renderTarget.writeContentsToFile(vArquivo);
        }

        /// <summary>
        /// Get WPF window handle.
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