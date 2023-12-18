using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EntityFramework.Data;
using EntityFramework.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace EntityFramework.ViewModels;

internal class OrderFormViewModel : ObservableObject
{
    private Client _client;
    public Client Client {
        get => _client; 
        set
        {
            _client = value;
            if(_client is EClient eClient)
            {
                IsEClient = true;
                IpAddress = eClient.IpAddress;
            }
        }
    }
    public ObservableCollection<Item> Items { get; set; }
    public ObservableCollection<OrderItem> OrderItems { get; set; }

    private Item _selectedItem = null!;
    public Item SelectedItem
    {
        get { return _selectedItem; }
        set { 
            _selectedItem = value;
            OnPropertyChanged();
            AddToOrderCommand.NotifyCanExecuteChanged();
        }
    }

    private OrderItem _selectedOrderItem = null!;
    public OrderItem SelectedOrderItem
    {
        get { return _selectedOrderItem; }
        set
        {
            _selectedOrderItem = value;
            OnPropertyChanged();
            RemoveOrderItemCommand.NotifyCanExecuteChanged();
        }
    }

    private string _amountText = null!;
    public string AmountText
    {
        get { return _amountText; }
        set { 
            _amountText = value;
            OnPropertyChanged();
            AddToOrderCommand.NotifyCanExecuteChanged();
        }
    }

    private string _ipAddress = null!;
    public string IpAddress
    {
        get { return _ipAddress; }
        set 
        { 
            _ipAddress = value; 
            OnPropertyChanged();
        }
    }

    private bool _isEOrder;
    public bool IsEOrder
    {
        get { return _isEOrder; }
        set { 
            _isEOrder = value;
            OnPropertyChanged();
        }
    }

    private bool _isEClient;

    public bool IsEClient
    {
        get { return _isEClient; }
        set { 
            _isEClient = value;
            OnPropertyChanged();
        }
    }



    public RelayCommand AddToOrderCommand { get; set; }
    public RelayCommand RemoveOrderItemCommand { get; set; }
    public RelayCommand OkCommand { get; set; }
    public RelayCommand CancelCommand { get; set; }
    public Action CancelAction { get; set; } = null!;
    public Action<Order> OkAction { get; set; } = null!;

    public OrderFormViewModel()
    {
        Items = new ObservableCollection<Item>();
        OrderItems = new ObservableCollection<OrderItem>();
        AddToOrderCommand = new RelayCommand(AddToOrder, CanAddToOrder);
        RemoveOrderItemCommand = new RelayCommand(RemoveOrderItem, () => SelectedOrderItem is OrderItem);
        CancelCommand = new RelayCommand(() => CancelAction?.Invoke());
        OkCommand = new RelayCommand(FinalizeOrder, CanFinalize);

        using var db = new LocalContext();

        foreach(var item in db.Items)
        {
            Items.Add(item);
        }

    }

    private void FinalizeOrder()
    {
        Order order;
        if (IsEOrder)
        {
            order = new EOrder();
        }
        else
        {
            order = new Order();
        }

        foreach(var orderItem in OrderItems)
        {
            orderItem.Order = order;
        }

        order.Client = Client;
        order.Completed = false;
        order.OrderItems = new List<OrderItem>();
        foreach(var orderItem in OrderItems)
        {
            order.OrderItems.Add(orderItem);
        }
        
        OkAction?.Invoke(order);
    }

    private bool CanFinalize()
    {
        return OrderItems.Count > 0;
    }

    private void AddToOrder()
    {
        if (_selectedItem == null)
        {
            MessageBox.Show("Item not selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        try
        {
            OrderItems.Add(new OrderItem {
                Item = SelectedItem, 
                Amount = Int32.Parse(AmountText)
            });
            OkCommand.NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error adding item", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanAddToOrder()
    {
        return Int32.TryParse(AmountText, out int amount) && SelectedItem is Item && amount > 0;
    }

    private void RemoveOrderItem()
    {
        if(SelectedOrderItem is OrderItem)
        {
            OrderItems.Remove(SelectedOrderItem);
            OkCommand.NotifyCanExecuteChanged();
        }
    }

}
