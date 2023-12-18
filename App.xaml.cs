using System.Windows;
using EntityFramework.ViewModels;
using EntityFramework.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EntityFramework;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((host, services) =>
            {
                services.AddTransient<ClientsViewModel>();
                services.AddTransient<ClientsView>();

                services.AddSingleton<MainWindow>();

                services.AddTransient<OrderFormViewModel>();
                services.AddTransient<OrderForm>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        AppHost.Services.GetRequiredService<MainWindow>().Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}
