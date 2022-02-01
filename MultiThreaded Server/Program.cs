using System;
using System.Threading;
using MultiThreaded_Server.Client;
using MultiThreaded_Server.Common;
using MultiThreaded_Server.Server;

namespace MultiThreaded_Server {

  public class Program {

    private static void Main() {
      int port = 6010;

      ClientManager manager = new ClientManager(port);
      manager.Start();

      Thread.Sleep(100); // Waiting on server being online

      RunScriptedClient(
        "127.0.0.1", port,

        "LOGIN aname",
        "MESSAGE bname Hello, how is it going?",
        "LOGOUT"
      );

      Thread.Sleep(2000); // Waiting on client to terminate

      RunScriptedClient(
        "127.0.0.1", port,

        "LOGIN bname",
        "GET",
        "GET",
        "LOGOUT"
      );

      Thread.Sleep(5000);
      manager.Shutdown(true);

      ConsoleUtils.WriteLine(ConsoleColor.Red, "[Main] Terminated");
    }

    //Clients can run at the same time.
    private static void RunScriptedClient(string host, int port, params string[] lines) {
      ScriptedClient client = new ScriptedClient(host, port, lines);
      client.Start();
    }
  }
}
