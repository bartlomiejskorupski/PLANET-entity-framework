using CommunityToolkit.Mvvm.ComponentModel;
using EntityFramework.Models;

namespace EntityFramework.ViewModels;

internal class OrderViewModel : ObservableObject
{
    public Order Order { get; set; }
    public int Amount { get; set; }
    public decimal TotalPrice { get; set; }
    public string ClientName { get; set; }
    public string Completed {  get; set; }
    public string EOrder {  get; set; }

    public OrderViewModel(Order order)
    {
        Order = order;
        Amount = order.AmountOfItems();
        ClientName = order.Client.Name;
        TotalPrice = order.TotalPrice();
        Completed = order.Completed ? "Yes" : "No";
        EOrder = order is EOrder ? "Yes" : "No";
    }

}
