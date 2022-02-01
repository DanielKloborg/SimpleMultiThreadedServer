using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using MultiThreaded_Server.Common;

namespace MultiThreaded_Server.Server {

  internal abstract class AbstractClientWorker {
    private Thread thread;
    private bool stop;

    private readonly ClientManager manager;

    private readonly Socket connection;

    private StreamReader reader;
    private StreamWriter writer;

    internal AbstractClientWorker(ClientManager manager, Socket connection) {
      this.manager = manager;
      this.connection = connection;

      stop = false;
    }

    internal void Start() {
      thread = new Thread(Run);
      thread.Start();
    }

    private void Run() {
      WriteLine("New connection: " + connection.RemoteEndPoint);

      using (NetworkStream stream = new NetworkStream(connection) {
        ReadTimeout = 100
      })
        using (reader = new StreamReader(stream))
        using (writer = new StreamWriter(stream))
          while (!stop)
            try {
              string line = reader.ReadLine();

              if (!HandleIncommingLine(line))
                stop = true;
            }
            catch (IOException) {
              // ignore
              Thread.Sleep(100); // nap!
            }

      manager.Remove(this);

      WriteLine("Terminated");
    }

    protected abstract bool HandleIncommingLine(string line);

    protected string Tail(string text) {
      int pos = text.IndexOf(' ');

      if (pos >= 0)
        return text.Substring(pos + 1).Trim();
      else
        return string.Empty;
    }

    protected void SendLine(string line) {
      WriteLine("Sending line: " + line);

      writer.WriteLine(line);
      writer.Flush();

      Thread.Sleep(100); // simulating delay
    }

    public void Shutdown(bool waitForTermination=false) {
      WriteLine("Shutdown requested");

      stop = true;

      if (waitForTermination)
        thread.Join();
    }

    protected void WriteLine(string line) {
      ConsoleUtils.WriteLine(ConsoleColor.DarkGreen, "[ClientWorker] " + line);
    }
  }
}
