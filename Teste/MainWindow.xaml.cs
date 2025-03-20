using Microsoft.Win32;
using OgreImage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Teste
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedFilePath;

        public MainWindow()
        {
            InitializeComponent();

            Title = "Ogre Showcase - (x64)";

            FillObjectListBox();
        }

        private void imgOgre_Loaded(object sender, RoutedEventArgs e)
        {
            imgOgre.Init();

            LoadSkyBox();

            LoadTerrain();

            LoadCompositors();
        }
        
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.PrintScreen(@"C:\Users\Admin\Pictures\output_new.png");
        }

        private void SinbadButton_Click(object sender, RoutedEventArgs e)
        {
            AddEntity("Sinbad");
        }

        private void OgreHeadButton_Click(object sender, RoutedEventArgs e)
        {
            AddEntity("ogrehead");
        }

        private void DragonButton_Click(object sender, RoutedEventArgs e)
        {
            AddEntity("dragon");
        }

        private void AddEntity(string entityName)
        {
            imgOgre.myD3DImage.AddNewEntityToScene(entityName);
        }

        private void AddPlane()
        {
            imgOgre.myD3DImage.AddNewPlaneToScene();
        }

        private void ClearSceneButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.ClearScene();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            imgOgre.Dispose();
        }

        private void ImgButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Selecione uma imagem"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;

                if (HostImgGrid.Background is ImageBrush brush)
                {
                    brush.ImageSource = new BitmapImage(new Uri(selectedFilePath, UriKind.Absolute));
                }
                else
                {
                    HostImgGrid.Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(selectedFilePath, UriKind.Absolute)),
                        Stretch = Stretch.UniformToFill
                    };
                }
            }
        }

        private void FillObjectListBox()
        {

            string[] meshNames =
            {
                "Sinbad", "ogrehead", "dragon", "plane",
                "athene", "Barrel", "column", "Columns",
                "cornell", "cube", "DamagedHelmet","facial",
                "fish", "geosphere4500", "jaiqua","knot",
                "ninja", "penguin","razor", "RomanBathLower",
                "RomanBathUpper", "RZR-002","ShaderSystem", "sibenik",
                "spine","TestLevel_b0", "tudorhouse", "WoodPallet",
            };

            foreach (string name in meshNames)
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Teste/img/listbox/" + name + ".png");
                BitmapImage icon = new BitmapImage(new Uri(fullPath, UriKind.Absolute));

                Image meshImage = new Image
                {
                    Width = 85,
                    Height = 75,
                    Stretch = Stretch.Uniform,
                    IsHitTestVisible = true,
                    Source = icon
                };

                if (name == "RZR-002")
                    meshImage.Name = "RZR002";
                else
                    meshImage.Name = name;

                ListBoxItem item = new ListBoxItem
                {
                    Content = meshImage,
                    Tag = name,
                    Focusable = false
                };

                meshImage.MouseDown += MeshImage_MouseDown;

                ObjectList.Items.Add(item);
            }
        }
        private void MeshImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img)
            {
                switch (img.Name)
                {
                    case "RZR002":
                        AddEntity("RZR-002");
                        break;
                    case "plane":
                        AddPlane();
                        break;
                    default:
                        AddEntity(img.Name);
                        break;
                }
            }
        }

        private void SkyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SkyComboBox.SelectedItem == null) return;

            string selected= SkyComboBox.SelectedItem.ToString();

            imgOgre.myD3DImage.ChangeSky(selected);
        }

        private void TerrainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TerrainComboBox.SelectedItem == null) return;

            string selected = TerrainComboBox.SelectedItem.ToString();

            imgOgre.myD3DImage.ChangeTerrain(selected);
        }

        private void CompositorsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CompositorsBox.SelectedItem == null) return;

            string selected = CompositorsBox.SelectedItem.ToString();

            imgOgre.myD3DImage.ChangeCompositor(selected);
        }        

        private void LoadSkyBox()
        {
            string[] skys =
            {
                "Padrão", "Noite", "IA", "Domo", "Plano"
            };

            foreach (var sky in skys)
            {
                SkyComboBox.Items.Add(sky);
            }
        }

        private void LoadTerrain()
        {
            string[] terrains =
            {
                "Grama", "Grama Desgastada", "Pedra", "Tijolo"
            };

            foreach (var terrain in terrains)
            {
                TerrainComboBox.Items.Add(terrain);
            }
        }

        private void LoadCompositors()
        {
            string[] compositors =
            {
                "Normal", "Bloom", "Glass","B&W", "Posterize", "Tiling",
                "DoF", "Embossed", "Invert", "Laplace", "Old Movie",
                "Radial Blur", "ASCII", "Halftone", "Dither",
                "Compute", "CubeMap", "Fresnel", "WBOIT",
            };

            foreach (var compositor in compositors)
            {
                CompositorsBox.Items.Add(compositor);
            }
        }

        private void LightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            imgOgre.myD3DImage.UpdateBrightness((float)LightSlider.Value);
        }

        private void ZoomAllButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.ZoomAll();
        }

        private void NWButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.VP_Camera(MyEnum.eVPCamera.eVPNW);
        }

        private void WButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.VP_Camera(MyEnum.eVPCamera.eVPLEFT);
        }

        private void SWButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.VP_Camera(MyEnum.eVPCamera.eVPSW);
        }

        private void NButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.VP_Camera(MyEnum.eVPCamera.eVPFRONT);
        }

        private void CenterButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.VP_Camera(MyEnum.eVPCamera.eVPTop);
        }

        private void SButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.VP_Camera(MyEnum.eVPCamera.eVPBACK);
        }

        private void NEButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.VP_Camera(MyEnum.eVPCamera.eVPNE);
        }

        private void EButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.VP_Camera(MyEnum.eVPCamera.eVPRIGHT);
        }

        private void SEButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.myD3DImage.VP_Camera(MyEnum.eVPCamera.eVPSE);
        }

        private void ProjectionTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (imgOgre.myD3DImage.projectionStyle == MyEnum.eVPProjectionStyle.eVPPerspective)
                imgOgre.myD3DImage.CamProjectionStyle(MyEnum.eVPProjectionStyle.eVPOrthographic);
            else
                imgOgre.myD3DImage.CamProjectionStyle(MyEnum.eVPProjectionStyle.eVPPerspective);
        }

        private void AxisButton_Click(object sender, RoutedEventArgs e)
        {
            if (!imgOgre.myD3DImage.isShowingGlobalGrid)
                imgOgre.myD3DImage.ShowGlobalAxis();
            else
                imgOgre.myD3DImage.HideGlobalAxis();
        }

        private void CameraViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (imgOgre.myD3DImage.cameraview == MyEnum.eCameraView.eOrbitCamera)
                imgOgre.myD3DImage.ChangeCameraView(MyEnum.eCameraView.eFreeLookCamera);
            else
                imgOgre.myD3DImage.ChangeCameraView(MyEnum.eCameraView.eOrbitCamera);
        }        

        private void FogButton_Click(object sender, RoutedEventArgs e)
        {
            if (!imgOgre.myD3DImage.isShowingFog)
                imgOgre.myD3DImage.ShowFog();
            else
                imgOgre.myD3DImage.HideFog();
        }
        private void NewMeshButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedFile = "";

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Selecione um Modelo 3D",
                Filter = "Modelos 3D|*.obj;*.fbx;*.dae;*.stl;*.blend;*.3ds;*.gltf;*.glb",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedFile = openFileDialog.FileName;

                string outputDirectory = @"C:\Users\Admin\Downloads\ModelosConvertidos\" + Path.GetFileNameWithoutExtension(selectedFile);

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                ConvertModel(selectedFile, outputDirectory);
            }
        }
        private void ConvertModel(string inputFile, string outputDirectory)
        {
            string converterPath = @"D:\3D\Ogre 3D\Ferramentas\Ogre-14.3.3\build\bin\Release\OgreAssimpConverter.exe";

            string modelDirectory = Path.GetDirectoryName(inputFile);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = converterPath,
                Arguments = $"\"{Path.GetFileName(inputFile)}\" \"{outputDirectory}\"",
                WorkingDirectory = modelDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process { StartInfo = psi };

            process.OutputDataReceived += (sender, e) => Console.WriteLine($"[OUT] {e.Data}");
            process.ErrorDataReceived += (sender, e) => Console.WriteLine($"[ERR] {e.Data}");

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                string zipFilePath = Path.Combine(@"C:\Users\Admin\Downloads\ModelosConvertidos", Path.GetFileNameWithoutExtension(inputFile) + ".zip");

                CreateZipFromDirectory(outputDirectory, zipFilePath);

                DeleteConvertedFiles(outputDirectory);

                EditConfigFile(@"D:\3D\Ogre 3D\Projetos\OgreImage\Bin\x64\resources.cfg", "MyResource", "Zip=" + zipFilePath);

                MessageBox.Show($"Conversão e compactação concluídas! Arquivos salvos em: {zipFilePath}", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Erro na conversão do modelo.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteConvertedFiles(string directory)
        {
            try
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    File.Delete(file);
                }

                Directory.Delete(directory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir arquivos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateZipFromDirectory(string sourceDirectory, string zipFilePath)
        {
            if (Directory.Exists(sourceDirectory))
            {
                try
                {
                    ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath);
                    Console.WriteLine($"Arquivo {zipFilePath} criado com sucesso!");
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine($"Erro ao criar o ZIP: {ioEx.Message}");
                }
            }
            else
            {
                Console.WriteLine($"O diretório {sourceDirectory} não existe.");
            }
        }

        public void EditConfigFile(string filePath, string section, string newEntry)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);

                bool sectionFound = false;
                bool entryAdded = false;
                var newLines = new List<string>();

                foreach (var line in lines)
                {
                    newLines.Add(line);

                    if (line.Trim().Equals($"[{section}]"))
                    {
                        sectionFound = true;
                    }

                    if (sectionFound && !entryAdded)
                    {
                        newLines.Add(newEntry);
                        entryAdded = true;
                    }
                }

                if (!sectionFound)
                {
                    newLines.Add($"[{section}]");
                    newLines.Add(newEntry);
                }

                File.WriteAllLines(filePath, newLines);
                Console.WriteLine("Arquivo atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao editar o arquivo: {ex.Message}");
            }
        }        
    }
}