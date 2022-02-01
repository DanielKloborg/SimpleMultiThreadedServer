using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MultiThreaded_Server.Common;

namespace MultiThreaded_Server.Server {

  internal class ClientManager : IShutdownable {
    private Thread thread;
    private bool stop, waitForTermination;

    private readonly List<AbstractClientWorker> workers;
    private readonly int port;

    internal ClientManager(int port) {
      this.port = port;

      stop = false;
      waitForTermination = false;
      workers = new List<AbstractClientWorker>();
    }

    internal void Start() {
      thread = new Thread(Run);
      thread.Start();
    }

    private void Run() {
      try {
        TcpListener server = new TcpListener(IPAddress.Any, port);
        server.Start();

        WriteLine("Server online");

        while (!stop)
          if (server.Pending()) {
            Socket connection = server.AcceptSocket();

            AbstractClientWorker worker = new MessageDomainClientWorker(this, connection);
            workers.Add(worker);
            worker.Start();

          } else
            Thread.Sleep(100); // nap!

        // terminate server...

        foreach (AbstractClientWorker worker in workers)
          worker.Shutdown(waitForTermination);

        WriteLine("Server offline");
      }
      catch (IOException) {
        WriteLine("I/O error");
      }
    }

    internal void Remove(AbstractClientWorker worker) {
      workers.Remove(worker);
    }

    public void Shutdown(bool waitForTermination) {
      WriteLine("Shutdown requested");

      this.waitForTermination = waitForTermination;

      stop = true;

      if (waitForTermination)
        thread.Join();
    }

    private void WriteLine(string line) {
      ConsoleUtils.WriteLine(ConsoleColor.Blue, "[ClientManager] " + line);
    }
  }
}
