using org.ogre;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using System;

namespace OgreEngine
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

        #region DirectX 9

        public void CreateTexture9()
        {
            if (texturePtr != null)
                DisposeRenderTarget();

            texturePtr = TextureManager.getSingleton().createManual(
                            "Ogre Render",
                            ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME,
                            TextureType.TEX_TYPE_2D,
                            (uint)TextureWidth,
                            (uint)TextureHeight,
                            32,
                            0,
                            PixelFormat.PF_R8G8B8A8,
                            0x20);

            InitRenderTarget();

            InitCompositors();
        }

        #endregion

        #region DirectX 11

        /// <summary>
        /// Cria a textura no SharpDX com DirectX 11, passa o ponteiro da textura para o Ogre
        /// depois faz a interoperabildade da textura para o DirectX 9.
        /// </summary>
        public void CreateTexture11()
        {
            if (texturePtr != null)
                DisposeRenderTarget();

            #region SharpDX - Criando textura do DirectX 11

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

            #region Ogre - Criando textura com o ponteiro da textura do SharpDX

            texturePtr = TextureManager.getSingleton().create("Ogre Render", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            texturePtr.setUsage(0x20); //0x20 = RenderTarget
            texturePtr._setD3D11Surface(texture.NativePointer);
            texturePtr.load();

            #endregion

            #region SharpDX - Criando textura do DirectX 9

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

            SharedHandle = d9texture.GetSurfaceLevel(0).NativePointer; //Pega o ponteiro da textura da superfície 0 do DirectX 9.

            InitRenderTarget();

            InitCompositors();
        }

        #endregion

        #region Common

        /// <summary>
        /// Pega o Render Target da textura criada manualmente, seja DX 11 ou DX 9.
        /// </summary>
        public void InitRenderTarget()
        {
            if (renderTarget != null)
                DisposeRenderTarget();

            renderTarget = texturePtr.getBuffer().getRenderTarget();

            viewport = renderTarget.addViewport(cam); //Cria a viewport do Render Target da texutra com a câmera da cena.
            viewport.setBackgroundColour(new ColourValue(0, 0, 0, 0)); //Fundo transparente.            

            if (DX == Variaveis.Engine.DX9)
            {
                renderTarget.getCustomAttribute("DDBACKBUFFER", out IntPtr buffer);

                SharedHandle = buffer;
            }
        }

        /// <summary>
        /// Método utilizado quando for feito resize da textura. Libera as texturas e o render target.
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

            #region Dipose SharpDX (DX11)

            if (DX == Variaveis.Engine.DX11)
            {
                texture.Dispose();
                d9texture.Dispose();
            }

            #endregion
        }

        /// <summary>
        /// Faz dispose de tudo que é necessário (Ogre - SharpDX) para não deixar nada em memória.
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

            #region SharpDX (DX11)

            if (DX == Variaveis.Engine.DX11)
            {
                device.Dispose();
                device = null;

                d9context.Dispose();
                d9context = null;

                d9device.Dispose();
                d9device = null;
            }

            #endregion
        }

        #endregion
    }
}