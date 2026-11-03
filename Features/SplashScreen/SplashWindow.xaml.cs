using System.Windows;

namespace BTB.Features.SplashScreen
{
    /// <summary>
    /// Interaktionslogik für SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow(SplashViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}