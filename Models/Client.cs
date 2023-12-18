using EntityFramework.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EntityFramework.Models;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = null!;

    public decimal CompletedOrdersTotalPrice()
    {
        decimal total = 0;
        foreach (var o in Orders)
        {
            if (o.Completed)
            {
                total += o.TotalPrice();
            }
        }
        return total;
    }
}

