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

        #region Propriedades

        public Variaveis.Engine DX { get; set; }

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

        public override void setup()
        {
            Log log = LogManager.getSingleton().createLog("OgreDX11.log", true, true, false);

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

        public void SetupDX9()
        {
            root = new Root();

            Log log = LogManager.getSingleton().createLog("OgreDX9.log", true, true, false);

            #region Puglins / Resources config


            ConfigFile configFile = new ConfigFile();

            configFile.loadDirect("resources.cfg");

            ResourceGroupManager resourceGroupManager = ResourceGroupManager.getSingleton();

            foreach (var section in configFile.getSettingsBySection())
            {
                if (section.Key != null && section.Key != "")
                {
                    string sectionName = section.Key;

                    var settings = configFile.getMultiSetting("Zip", sectionName);

                    foreach (var setting in settings)
                    {
                        resourceGroupManager.addResourceLocation(setting, "Zip", sectionName);
                    }

                    settings = configFile.getMultiSetting("FileSystem", sectionName);

                    foreach (var setting in settings)
                    {
                        resourceGroupManager.addResourceLocation(setting, "FileSystem", sectionName);
                    }
                }
            }

            #endregion

            #region RenderSystem

            bool foundit = false;
            string renderName = "";

            renderName = "Direct3D9 Rendering Subsystem";

            foreach (RenderSystem rs in root.getAvailableRenderers())
            {
                if (rs.getName() == renderName)
                {
                    root.setRenderSystem(rs);

                    renderSystem = rs;

                    foreach (var c in rs.getConfigOptions())
                    {
                        foreach (var p in c.Value.possibleValues)
                        {
                            LogManager.getSingleton().logMessage(c.Key + ": " + p);
                        }
                    }

                    foundit = true;
                    break;
                }
            }

            if (!foundit)
            {
                throw new Exception("Failed to find a compatible render system.");
            }

            renderSystem.setConfigOption("Full Screen", "No");
            renderSystem.setConfigOption("Video Mode", "1920 x 1080 @ 32-bit colour");
            renderSystem.setConfigOption("FSAA", "8");
            renderSystem.setConfigOption("VSync", "Yes");
            renderSystem.setConfigOption("VSync Interval", "1");
            renderSystem.setConfigOption("Allow NVPerfHUD", "No");
            renderSystem.setConfigOption("sRGB Gamma Conversion", "Yes");

            renderSystem.setConfigOption("Use Multihead", "Auto");
            renderSystem.setConfigOption("Fixed Pipeline Enabled", "Yes");
            renderSystem.setConfigOption("Allow DirectX9Ex", "No");
            renderSystem.setConfigOption("Floating-point mode", "Fastest");
            renderSystem.setConfigOption("Resource Creation Policy", "Create on active device");

            #endregion

            root.initialise(false);

            #region Render Window

            IntPtr hWnd = getHandle();

            NameValueMap miscParams = new NameValueMap
            {
                ["FSAA"] = "8",
                ["Full Screen"] = "No",
                ["VSync"] = "Yes",
                ["sRGB Gamma Conversion"] = "Yes",
                ["externalWindowHandle"] = hWnd.ToString()
            };

            renderWindow = root.createRenderWindow(
                "Window Forms Ogre",
                0,
                0,
                false,
                miscParams
            );

            renderWindow.setAutoUpdated(false);

            #endregion

            resourceGroupManager.initialiseAllResourceGroups();

            scnMgr = root.createSceneManager();

            //Criar sombra antes da cena.
            if (MostraSombras) InitShadows();

            EmptyScene();

            if (MostraTerreno) CreateTerrain();
            if (MostraSkybox) AddSkyBox("Padrão");
        }
    }
}