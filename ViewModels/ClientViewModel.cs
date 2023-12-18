using CommunityToolkit.Mvvm.ComponentModel;
using EntityFramework.Models;

namespace EntityFramework.ViewModels;

internal class ClientViewModel : ObservableObject
{
    public Client Client { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string EClient { get; set; }

    public ClientViewModel(Client client)
    {
        Client = client;
        Name = client.Name;
        Address = client.Address;
        EClient = client is EClient ? "Yes" : "No";
    }
}
