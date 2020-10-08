using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

public class MyHub : Hub
{
  public MyHub()
  {


  }

  public void Buongiorno()
  {
    Console.WriteLine("Ricevuto messaggio di buongiorno");
    this.Clients.Client(this.Context.ConnectionId)
        .SendAsync("greetings", "Grazie mille, buona giornata anche a te");
  }

  public override Task OnConnectedAsync()
  {
    Console.WriteLine($"Nuovo utente collegato : {this.Context.ConnectionId}");
    return base.OnConnectedAsync();
  }

  public override Task OnDisconnectedAsync(Exception exception)
  {
    Console.WriteLine("C'è stata una vittima, lo abbiamo perso... ");
    return base.OnDisconnectedAsync(exception);
  }
}