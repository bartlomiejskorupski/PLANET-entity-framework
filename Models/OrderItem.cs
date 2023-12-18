using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Models;

public class OrderItem
{
    public int Id { get; set; }
    public virtual Item Item { get; set; } = null!;
    public int Amount { get; set; }
    public virtual Order Order { get; set; } = null!;
}
