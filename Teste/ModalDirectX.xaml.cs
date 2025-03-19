using System.Windows;

namespace Teste
{
    /// <summary>
    /// Interaction logic for ModalDirectX.xaml
    /// </summary>
    public partial class ModalDirectX : Window
    {
        public string escolha;

        public ModalDirectX()
        {
            InitializeComponent();
        }

        private void DirectX11_Click(object sender, RoutedEventArgs e)
        {
            escolha = "DirectX 11";
            DialogResult = true;
        }

        private void DirectX9_Click(object sender, RoutedEventArgs e)
        {
            escolha = "DirectX 9";
            DialogResult = true;
        }
    }
}
