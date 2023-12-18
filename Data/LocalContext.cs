using System;
using System.Configuration;
using System.Linq;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Data;

public class LocalContext : DbContext
{
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<EClient> EClients { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<EOrder> EOrders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["LocalContext"].ConnectionString);
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .HasDiscriminator<char>("clientType")
            .HasValue<Client>('N')
            .HasValue<EClient>('E');

        modelBuilder.Entity<Order>()
            .HasDiscriminator<char>("orderType")
            .HasValue<Order>('N')
            .HasValue<EOrder>('E');
    }

    public void AddItem(Item item)
    {
        Items.Add(item);
        SaveChanges();
    }

    public void AddItemStock(Item item, int value)
    {
        item.Stock += value;
        if (item.Stock < 0)
            throw new InvalidOperationException();
        SaveChanges();
    }
}

