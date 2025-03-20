using org.ogre;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using System;

namespace OgreImage
{
    public partial class Ogre
    {
        #region Variáveis Ogre

        public RenderTarget renderTarget;

        private TexturePtr texturePtr;

        public Viewport viewport;

        #endregion

        #region Variáveis SharpDx

        private SharpDX.Direct3D9.Texture d9texture;

        public SharpDX.Direct3D11.Device device;

        private SharpDX.Direct3D11.Texture2D texture;

        public SharpDX.Direct3D9.Direct3DEx d9context;

        public SharpDX.Direct3D9.Device d9device;

        #endregion

        #region Propriedades

        public IntPtr SharedHandle { get; private set; }

        public int TextureWidth { get; set; }

        public int TextureHeight { get; set; }

        #endregion

        #region DirectX 11

        /// <summary>
        /// Creates the texture in SharpDX with DirectX 11, passes the texture pointer to Ogre then interops the texture to DirectX 9.
        /// </summary>
        public void CreateTexture11()
        {
            if (texturePtr != null)
                DisposeRenderTarget();

            #region SharpDX - Creating texture with DirectX 11

            var featureLevels = new[]
            {
                SharpDX.Direct3D.FeatureLevel.Level_11_0,
                SharpDX.Direct3D.FeatureLevel.Level_10_1,
                SharpDX.Direct3D.FeatureLevel.Level_10_0
            };

            if (device == null)
            {
                device = new SharpDX.Direct3D11.Device(SharpDX.Direct3D.DriverType.Hardware,
                            SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport,
                            featureLevels);
            }

            texture = new Texture2D(device,
                          new Texture2DDescription()
                          {
                              Width = TextureWidth,
                              Height = TextureHeight,
                              ArraySize = 1,
                              BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                              Usage = ResourceUsage.Default,
                              CpuAccessFlags = CpuAccessFlags.None,
                              Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                              MipLevels = 1,
                              OptionFlags = ResourceOptionFlags.Shared,
                              SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0)
                          });

            #endregion

            #region Ogre - Creating texture with DirectX 11 texture pointer

            texturePtr = TextureManager.getSingleton().create("Ogre Render", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            texturePtr.setUsage(0x20); //0x20 = RenderTarget
            texturePtr._setD3D11Surface(texture.NativePointer);
            texturePtr.load();

            #endregion

            #region SharpDX - Creating DirectX 9 texture

            if (d9context == null)
            {
                d9context = new Direct3DEx();

                d9device = new SharpDX.Direct3D9.Device(d9context,
                                0,
                               DeviceType.Hardware,
                               IntPtr.Zero,
                               CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded,
                               new SharpDX.Direct3D9.PresentParameters()
                               {
                                   Windowed = true,
                                   SwapEffect = SharpDX.Direct3D9.SwapEffect.FlipEx,
                                   DeviceWindowHandle = getHandle(),
                                   PresentationInterval = PresentInterval.Immediate,
                               });
            }

            IntPtr handle = texture.QueryInterface<SharpDX.DXGI.Resource>().SharedHandle;

            d9texture = new SharpDX.Direct3D9.Texture(d9device,
                            TextureWidth, TextureHeight,
                            1,
                            SharpDX.Direct3D9.Usage.RenderTarget,
                            SharpDX.Direct3D9.Format.A8R8G8B8,
                            Pool.Default,
                            ref handle);

            #endregion

            SharedHandle = d9texture.GetSurfaceLevel(0).NativePointer;

            InitRenderTarget();

            InitCompositors();
        }

        #endregion

        #region Common

        /// <summary>
        /// Takes render target from the texture and creates a viewport with the scene camera.
        /// </summary>
        public void InitRenderTarget()
        {
            if (renderTarget != null)
                DisposeRenderTarget();

            renderTarget = texturePtr.getBuffer().getRenderTarget();

            viewport = renderTarget.addViewport(cam); //Cria a viewport do Render Target da texutra com a câmera da cena.
            viewport.setBackgroundColour(new ColourValue(0, 0, 0, 0)); //Fundo transparente.            
        }

        /// <summary>
        /// Used when resize the window, to update the texture size.
        /// </summary>
        private void DisposeRenderTarget()
        {
            if (renderTarget == null || texturePtr == null) return;

            #region Dispose Ogre

            renderTarget.removeAllListeners();
            renderTarget.removeAllViewports();
            renderTarget = null;

            TextureManager.getSingleton().remove(texturePtr.getHandle());
            TextureManager.getSingleton().Dispose();
            GC.SuppressFinalize(texturePtr);
            texturePtr = null;

            #endregion

            #region Dipose SharpDX

            texture.Dispose();
            d9texture.Dispose();

            #endregion
        }

        /// <summary>
        /// All disposes.
        /// </summary>
        public void DisposeOgre()
        {
            #region Ogre

            scnMgr.clearScene();
            scnMgr.Dispose();
            scnMgr = null;

            DisposeRenderTarget();

            CompositorManager.getSingleton().removeAll();
            CompositorManager.getSingleton().Dispose();

            renderWindow.destroy();
            renderWindow.Dispose();
            renderWindow = null;

            closeApp();

            #endregion

            #region SharpDX

            device.Dispose();
            device = null;

            d9context.Dispose();
            d9context = null;

            d9device.Dispose();
            d9device = null;

            #endregion
        }

        #endregion
    }
}