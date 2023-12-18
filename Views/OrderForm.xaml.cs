using EntityFramework.Models;
using EntityFramework.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace EntityFramework.Views;

public partial class OrderForm : Window
{
    public Order OrderResult = null!;
    private OrderFormViewModel ViewModel { get; set; }

    public OrderForm()
    {
        InitializeComponent();
        ViewModel = App.AppHost!.Services.GetRequiredService<OrderFormViewModel>();
        DataContext = ViewModel;

        ViewModel.OkAction = (Order order) =>
        {
            DialogResult = true;
            OrderResult = order;
            Close();
        };

        ViewModel.CancelAction = () =>
        {
            DialogResult = false;
            Close();
        };
    }

    public void SetClient(Client client)
    {
        ViewModel.Client = client;
    }
}
