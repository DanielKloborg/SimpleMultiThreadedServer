using System.Net.Sockets;

namespace MultiThreaded_Server.Server {

  internal class MessageDomainClientWorker : AbstractClientWorker {
    private string userName;

    internal MessageDomainClientWorker(ClientManager manager, Socket connection)
      : base(manager, connection) {
    }

    // returns: true: continue running client, false: shutdown client
    protected override bool HandleIncommingLine(string line) {
      string[] tokens = line.Split();
      string command = tokens[0].ToUpper();

      switch (command) {
        case "LOGIN": {
            userName = tokens[1];
            break;
          }

        case "MESSAGE": {
            string to = tokens[1];
            string message = Tail(Tail(line));

            MessageDB.I.StoreMessage(userName, to, message);

            break;
          }

        case "GET": {
            string message = MessageDB.I.GetMessage(userName);

            if (message != null)
              SendLine(message);
            else
              SendLine("no messages");

            break;
          }

        case "LOGOUT": {
            // shutdown client
            return false;
          }
      }

      // default: continue running client
      return true;
    }
  }
}
