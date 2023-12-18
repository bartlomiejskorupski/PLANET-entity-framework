using System.Windows;
using System.Windows.Controls;
using EntityFramework.Views;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainContent.Content = App.AppHost!.Services.GetRequiredService<ClientsView>();
    }
}
