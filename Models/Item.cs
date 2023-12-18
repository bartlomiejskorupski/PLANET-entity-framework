﻿using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(6, 2)")]
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

