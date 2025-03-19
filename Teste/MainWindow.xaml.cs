using Microsoft.Win32;
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
using static OgreEngine.Variaveis;

namespace Teste
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isEyeOpened = true;

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
            imgOgre.PrintScreen(@"C:\Users\Admin\Pictures\output_new.png");
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
            imgOgre.AddNewEntityToScene(entityName);
        }

        private void AddPlane()
        {
            imgOgre.AddNewPlaneToScene();
        }

        private void ClearSceneButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.ClearScene();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            imgOgre.Dispose();
        }

        private void EyeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null && btn.Background is ImageBrush brush)
            {
                if (isEyeOpened)
                {
                    HostImgGrid.Background = Brushes.Transparent;

                    brush.ImageSource = new BitmapImage(new Uri("Teste/img/closed_eye_icon.png", UriKind.Relative));

                    isEyeOpened = false;
                }
                else
                {
                    if (selectedFilePath == null)
                        HostImgGrid.Background = Brushes.White;
                    else
                    {
                        ImageBrush gridBrush = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(selectedFilePath, UriKind.Absolute))
                        };

                        HostImgGrid.Background = gridBrush;
                    }

                    brush.ImageSource = new BitmapImage(new Uri("Teste/img/opened_eye_icon.png", UriKind.Relative));

                    isEyeOpened = true;
                }
            }
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
                BitmapImage icon = new BitmapImage(new Uri($"Teste/img/listbox/{name}.png", UriKind.Relative));

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

            imgOgre.ChangeSky(selected);
        }

        private void TerrainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TerrainComboBox.SelectedItem == null) return;

            string selected = TerrainComboBox.SelectedItem.ToString();

            imgOgre.ChangeTerrain(selected);
        }

        private void CompositorsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CompositorsBox.SelectedItem == null) return;

            string selected = CompositorsBox.SelectedItem.ToString();

            imgOgre.ChangeCompositor(selected);
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
            imgOgre.UpdateBrightness((float)LightSlider.Value);
        }

        private void ZoomAllButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.ZoomAll();
        }

        private void NWButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.VP_Camera(eVPCamera.eVPNW);
        }

        private void WButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.VP_Camera(eVPCamera.eVPLEFT);
        }

        private void SWButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.VP_Camera(eVPCamera.eVPSW);
        }

        private void NButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.VP_Camera(eVPCamera.eVPFRONT);
        }

        private void CenterButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.VP_Camera(eVPCamera.eVPTop);
        }

        private void SButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.VP_Camera(eVPCamera.eVPBACK);
        }

        private void NEButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.VP_Camera(eVPCamera.eVPNE);
        }

        private void EButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.VP_Camera(eVPCamera.eVPRIGHT);
        }

        private void SEButton_Click(object sender, RoutedEventArgs e)
        {
            imgOgre.VP_Camera(eVPCamera.eVPSE);
        }

        private void ProjectionTypeButton_Click(object sender, RoutedEventArgs e)
        {
            if (imgOgre.projectionStyle == eVPProjectionStyle.eVPPerspective)
                imgOgre.CamProjectionStyle(eVPProjectionStyle.eVPOrthographic);
            else
                imgOgre.CamProjectionStyle(eVPProjectionStyle.eVPPerspective);
        }

        private void AxisButton_Click(object sender, RoutedEventArgs e)
        {
            if (!imgOgre.isShowingGlobalGrid)
                imgOgre.ShowGlobalAxis();
            else
                imgOgre.HideGlobalAxis();
        }

        private void CameraViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (imgOgre.cameraview == eCameraView.eOrbitCamera)
                imgOgre.ChangeCameraView(eCameraView.eFreeLookCamera);
            else
                imgOgre.ChangeCameraView(eCameraView.eOrbitCamera);
        }        

        private void FogButton_Click(object sender, RoutedEventArgs e)
        {
            if (!imgOgre.isShowingFog)
                imgOgre.ShowFog();
            else
                imgOgre.HideFog();
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

                EditConfigFile(@"D:\3D\Ogre 3D\Projetos\OgreImage\Bin\x64\resources.cfg", "AuE", "Zip=" + zipFilePath);

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
                // Lê todo o conteúdo do arquivo
                var lines = File.ReadAllLines(filePath);

                bool sectionFound = false;
                bool entryAdded = false;
                var newLines = new List<string>();

                foreach (var line in lines)
                {
                    newLines.Add(line);

                    // Verifica se encontramos a seção correta
                    if (line.Trim().Equals($"[{section}]"))
                    {
                        sectionFound = true;
                    }

                    // Se a seção foi encontrada, e a entrada ainda não foi adicionada
                    if (sectionFound && !entryAdded)
                    {
                        // Adiciona a nova linha após encontrar a seção
                        newLines.Add(newEntry);
                        entryAdded = true;
                    }
                }

                // Se a seção não foi encontrada, a adiciona
                if (!sectionFound)
                {
                    newLines.Add($"[{section}]");
                    newLines.Add(newEntry);
                }

                // Grava o conteúdo de volta ao arquivo
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