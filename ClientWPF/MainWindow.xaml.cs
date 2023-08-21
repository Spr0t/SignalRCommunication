using Microsoft.AspNetCore.SignalR.Client;
using System.Windows;

namespace ClientWPF;

public partial class MainWindow : Window
{
    private HubConnection _connection;

    public MainWindow()
    {
        InitializeComponent();

        _connection = new HubConnectionBuilder()
             .WithUrl("http://localhost:5000/message")
             .Build();

        ConfigureRecieving(_connection);

        _connection.StartAsync().Wait();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        statusField.Text = "Pending";

        _connection.SendAsync("RecieveMessage", inputText.Text);
    }

    private void ConfigureRecieving(HubConnection connection)
    {
        connection.On<string>("RecieveMessage", (message) =>
        {
            if (message == "Recieved")
                Dispatcher.Invoke(() => statusField.Text = message);
            else
                Dispatcher.Invoke(() => responseBox.Text = message);
        });
    }
}
