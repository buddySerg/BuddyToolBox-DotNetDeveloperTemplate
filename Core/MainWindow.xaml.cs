using System.Windows;

namespace BTB.Core;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
