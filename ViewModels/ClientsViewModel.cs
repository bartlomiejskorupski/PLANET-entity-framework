using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntityFramework.Data;
using EntityFramework.Models;
using EntityFramework.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace EntityFramework.ViewModels;

internal class ClientsViewModel : ObservableObject
{
    public ObservableCollection<ClientViewModel> ClientList { get; set; }
    public ObservableCollection<OrderViewModel> OrderList { get; set; }
    public ObservableCollection<Item> ItemList { get; set; }

    private ClientViewModel _selectedClient = null!;
    public ClientViewModel SelectedClient
    {
        get => _selectedClient;
        set
        {
            _selectedClient = value;
            OnPropertyChanged();
            CreateOrderCommand.NotifyCanExecuteChanged();
        }
    }

    private OrderViewModel _selectedOrderVM = null!;

    public OrderViewModel SelectedOrderVM
    {
        get { return _selectedOrderVM; }
        set { 
            _selectedOrderVM = value;
            OnPropertyChanged();
            CompleteOrderCommand.NotifyCanExecuteChanged();
        }
    }

    private Item _selectedItem = null!;

    public Item SelectedItem
    {
        get { return _selectedItem; }
        set { 
            _selectedItem = value;
            OnPropertyChanged();
            WhoOrderedThisCommand.NotifyCanExecuteChanged();
        }
    }

    private string _filterText = "";
    public string FilterText
    {
        get => _filterText;
        set {
            _filterText = value; 
            OnPropertyChanged();
            UpdateClientList();
        }
    }

    private readonly int PAGE_SIZE = 5;
    private int _pageNo;
    public int PageNo
    {
        get { return _pageNo; }
        set { 
            _pageNo = value; 
            OnPropertyChanged();
        }
    }


    public RelayCommand CreateOrderCommand { get; set; }
    public RelayCommand DebugCommand { get; set; }
    public RelayCommand PreviousPageCommand { get; set; }
    public RelayCommand NextPageCommand { get; set; }
    public RelayCommand CompleteOrderCommand { get; set; }
    public RelayCommand WhoOrderedThisCommand { get; set; }

    public ClientsViewModel()
    {
        ClientList = new ObservableCollection<ClientViewModel>();
        OrderList = new ObservableCollection<OrderViewModel>();
        ItemList = new ObservableCollection<Item>();
        PageNo = 0;

        CreateOrderCommand = new RelayCommand(CreateOrderClick, () => SelectedClient != null);
        DebugCommand = new RelayCommand(DebugClick);
        PreviousPageCommand = new RelayCommand(PreviousPage, CanExecutePreviousPage);
        NextPageCommand = new RelayCommand(NextPage, CanExecuteNextPage);
        CompleteOrderCommand = new RelayCommand(CompleteOrder, CanCompleteOrder);
        WhoOrderedThisCommand = new RelayCommand(WhoOrderedThis, CanAskWhoOrderedThis);

        UpdateItemList();
        UpdateClientList();
        UpdateOrderList();
    }

    private void UpdateClientList()
    {
        ClientList.Clear();

        using var db = new LocalContext();
        var clients = db.Clients.OrderBy(c => c.Name)
            .Where(c => c.Name.ToLower().Contains(FilterText.ToLower()));

        foreach (var client in clients)
        {
            ClientList.Add(new ClientViewModel(client));
        }
    }

    private void CreateOrderClick()
    {
        var orderForm = App.AppHost!.Services.GetRequiredService<OrderForm>();
        orderForm.SetClient(SelectedClient.Client);
        orderForm.ShowDialog();
        if(orderForm.DialogResult == true && orderForm.OrderResult != null)
        {
            using var db = new LocalContext();
            var client = SelectedClient.Client;
            db.Attach(client);

            client.Orders.Add(orderForm.OrderResult);

            db.SaveChanges();
        }

        UpdateOrderList();
        NextPageCommand.NotifyCanExecuteChanged();
        PreviousPageCommand.NotifyCanExecuteChanged();
    }

    private void UpdateOrderList()
    {
        OrderList.Clear();
        using var db = new LocalContext();

        var orders = db.Orders;
        var pagedOrders = orders.Skip(PageNo * PAGE_SIZE).Take(PAGE_SIZE).ToList();

        foreach (var order in pagedOrders) 
        { 
            OrderList.Add(new OrderViewModel(order));
        }
    }

    private void PreviousPage()
    {
        if (!CanExecutePreviousPage())
            return;

        PageNo--;
        NextPageCommand.NotifyCanExecuteChanged();
        PreviousPageCommand.NotifyCanExecuteChanged();
        UpdateOrderList();
    }

    private bool CanExecutePreviousPage()
    {
        return PageNo >= 1;
    }

    private void NextPage()
    {
        if (!CanExecuteNextPage())
            return;

        PageNo++;
        NextPageCommand.NotifyCanExecuteChanged();
        PreviousPageCommand.NotifyCanExecuteChanged();
        UpdateOrderList();
    }

    private bool CanExecuteNextPage()
    {
        using var db = new LocalContext();

        var totalPages = (int)Math.Ceiling(db.Orders.Count() / (double)PAGE_SIZE);

        return PageNo < totalPages - 1;
    }

    private void CompleteOrder()
    {
        if (!CanCompleteOrder())
            return;
        
        using var db = new LocalContext();
        using var transaction = db.Database.BeginTransaction();

        var order = SelectedOrderVM.Order;
        db.Attach(order);

        try
        {
            foreach (var orderItem in order.OrderItems)
            {
                db.AddItemStock(orderItem.Item, -orderItem.Amount);
            }

            order.Completed = true;

            db.SaveChanges();
            transaction.Commit();
        } catch (Exception ex)
        {
            MessageBox.Show("Not enough items in stock", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        CompleteOrderCommand.NotifyCanExecuteChanged();
        UpdateOrderList();
        UpdateItemList();
    }

    private bool CanCompleteOrder()
    {
        return SelectedOrderVM?.Order?.Completed == false;
    }

    private void UpdateItemList()
    {
        ItemList.Clear();

        using var db = new LocalContext();

        foreach (var item in db.Items)
        {
            ItemList.Add(item);
        }
    }

    private void WhoOrderedThis()
    {
        if (!CanAskWhoOrderedThis())
            return;

        using var db = new LocalContext();

        var item = SelectedItem;

        db.Attach(item);

        var clientsWhoOrdered = db.OrderItems.Where(oi => oi.Item == item)
            .Select(oi => oi.Order.Client)
            .Distinct();

        var result = "\"" + item.Name + "\" was ordered by:\n";
        foreach (var client in clientsWhoOrdered)
        {
            result += client.Name + "\n";
        }

        MessageBox.Show(result, "Who ordered this?", MessageBoxButton.OK, MessageBoxImage.Question);
    }

    private bool CanAskWhoOrderedThis()
    {
        return SelectedItem != null;
    }

    private void DebugClick()
    {
        List<Item> restockedItems = new List<Item>()
        {
            new Item { Name="Felix Party Mix 60g", Price=7.99M, Stock=100},
            new Item { Name="Felix Party Mix 200g", Price=14.89M, Stock=100},
            new Item { Name="Felix Crispies 45g", Price=7.20M, Stock=100},
            new Item { Name="Dreamies 60g", Price=6.40M, Stock=100},
            new Item { Name="Dreamies 180g", Price=13.43M, Stock=100},
            new Item { Name="Dreamies Megatub 350g", Price=35.80M, Stock=100},
            new Item { Name="Whiskas Anti-Hairball 60g", Price=7.40M, Stock=100},
            new Item { Name="Whiskas Dentabites 60g", Price=7.60M, Stock=100},
        };

        using var db = new LocalContext();

        foreach(var item in restockedItems)
        {
            var foundItem = db.Items.Where(i => i.Name.Equals(item.Name)).FirstOrDefault();
            if(foundItem != null) 
            {
                db.AddItemStock(foundItem, item.Stock);
            }
            else
            {
                db.AddItem(item);
            }
        }

        db.SaveChanges();
        UpdateItemList();
    }
}
