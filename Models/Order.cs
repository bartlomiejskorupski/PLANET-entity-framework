using EntityFramework.Data;
using System.Collections.Generic;
using System.Linq;

namespace EntityFramework.Models;

public class Order
{
    public int Id { get; set; }
    public virtual Client Client { get; set; } = null!;
    public bool Completed { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = null!;

    public decimal TotalPrice()
    {
        decimal price = 0;
        foreach (var orderItem in OrderItems)
        {
            price += orderItem.Amount * orderItem.Item.Price;
        }
        return price;
    }

    public int AmountOfItems()
    {
        int amount = 0;
        foreach (var orderItem in OrderItems)
        {
            amount += orderItem.Amount;
        }
        return amount;
    }
}
