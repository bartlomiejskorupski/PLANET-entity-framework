using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Models;

public class EOrder : Order
{
    public string IpAddress { get; set; } = null!;
}
