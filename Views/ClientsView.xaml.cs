using EntityFramework.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace EntityFramework.Views;

public partial class ClientsView : UserControl
{
    public ClientsView()
    {
        InitializeComponent();
        DataContext = App.AppHost!.Services.GetRequiredService<ClientsViewModel>();
    }
}
