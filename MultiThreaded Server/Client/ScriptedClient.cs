using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MultiThreaded_Server.Common;

namespace MultiThreaded_Server.Client {

  internal class ScriptedClient {
    private readonly string host;
    private readonly int port;
    private readonly string[] script;

    private StreamReader reader;
    private StreamWriter writer;

    internal ScriptedClient(string host, int port, params string[] script) {
      this.host = host;
      this.port = port;
      this.script = script;
    }

    internal void Start() {
      new Thread(Run).Start();
    }

    private void Run() {
      try {
        WriteLine("Started");

        using (Socket connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
          connection.Connect(new IPEndPoint(IPAddress.Parse(host), port));

          using (NetworkStream stream = new NetworkStream(connection))
          using (reader = new StreamReader(stream))
          using (writer = new StreamWriter(stream))
            foreach (string line in script) {
              SendLine(line);

              if (line.ToUpper().StartsWith("GET")) {
                // GET will receive a reply
                string reply = reader.ReadLine();
                WriteLine("Received: " + reply);
              }
            }
        }

        WriteLine("Terminated");
      }
      catch {
        WriteLine("Could not connect to server");
      }
    }

    private void SendLine(string line) {
      WriteLine("Sending line: " + line);

      writer.WriteLine(line);
      writer.Flush();

      Thread.Sleep(100); // Simulating response time
    }

    private void WriteLine(string line) {
      ConsoleUtils.WriteLine(ConsoleColor.DarkGray, "[ScriptedClient] " + line);
    }
  }
}
