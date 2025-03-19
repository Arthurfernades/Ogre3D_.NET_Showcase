using org.ogre;
using System;

namespace OgreEngine
{
    public partial class Ogre : ApplicationContextBase
    {
        #region Variáveis

        public Root root;

        private RenderWindow renderWindow;

        private SceneManager scnMgr;

        private RenderSystem renderSystem;

        private ShaderGenerator shaderGen;

        #endregion

        /// <summary>
        /// Cria janela "imaginára". Não será utilizada,
        /// porém é necessário cria-la por necessidade do Ogre.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="miscParams"></param>
        /// <returns></returns>
        public override NativeWindowPair createWindow(string title, uint w, uint h, NameValueMap miscParams)
        {
            miscParams = new NameValueMap
            {
                ["FSAA"] = "8",
                ["Full Screen"] = "No",
                ["VSync"] = "Yes",
                ["sRGB Gamma Conversion"] = "Yes",
                ["externalWindowHandle"] = getHandle().ToString() //Handle da janela WPF.
            };

            NativeWindowPair nativeWindow = base.createWindow("AuE Ogre", 0, 0, miscParams); //Altura e largura sempre serão 0.

            renderWindow = nativeWindow.render;

            renderWindow.setAutoUpdated(false); //Não deixa a janela "imaginária" atualizar.

            return nativeWindow;
        }

        /// <summary>
        /// Método de inicialização automática do Ogre.
        /// </summary>
        public override void setup()
        {
            base.setup();

            root = getRoot();

            #region RenderSystem   

            renderSystem = root.getRenderSystem();

            renderSystem.setConfigOption("Full Screen", "No");
            renderSystem.setConfigOption("Video Mode", "1920 x 1080 @ 32-bit colour");
            renderSystem.setConfigOption("FSAA", "8");
            renderSystem.setConfigOption("VSync", "Yes");
            renderSystem.setConfigOption("VSync Interval", "1");
            renderSystem.setConfigOption("Allow NVPerfHUD", "No");
            renderSystem.setConfigOption("sRGB Gamma Conversion", "Yes");

            renderSystem.setConfigOption("Debug Layer", "Off");
            renderSystem.setConfigOption("Driver type", "Hardware");
            renderSystem.setConfigOption("Information Queue Exceptions Bottom Level", "Warning");
            renderSystem.setConfigOption("Max Requested Feature Levels", "11.0");
            renderSystem.setConfigOption("Min Requested Feature Levels", "10.0");

            #endregion

            scnMgr = root.createSceneManager();

            #region Shader Generator (DX11)

            shaderGen = ShaderGenerator.getSingleton();
            shaderGen.addSceneManager(scnMgr);

            #endregion

            //Criar sombra antes da cena.
            if (MostraSombras) InitShadows();

            EmptyScene();

            if (MostraTerreno) CreateTerrain();
            if (MostraSkybox) AddSkyBox("Padrão");
        }        
    }
}