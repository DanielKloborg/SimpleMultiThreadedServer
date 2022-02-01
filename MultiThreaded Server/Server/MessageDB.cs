using System.Collections.Generic;

namespace MultiThreaded_Server.Server {

  internal class MessageDB {
    private readonly List<Message> messages;

    private MessageDB() {
      messages = new List<Message>();
    }

    internal void StoreMessage(string from, string to, string message) {
      messages.Add(new Message(from, to, message));
    }

    internal string GetMessage(string userName) {
      foreach (Message message in messages)
        if (message.IsTo(userName)) {
          messages.Remove(message);

          return message.Text;
        }

      return null;
    }

    /*
     * class Message
     */
    private class Message {
      private readonly string from, to;
      private readonly string message;

      internal Message(string from, string to, string message) {
        this.from = from;
        this.to = to;
        this.message = message;
      }

      internal bool IsTo(string userName) {
        return to.ToUpper() == userName.ToUpper();
      }

      internal string Text {
        get { return from + ": " + message; }
      }
    }

    /*
     * Singleton
     */
    private static MessageDB instance = null;

    internal static MessageDB I {
      get {
        if (instance == null)
          instance = new MessageDB();

        return instance;
      }
    }
  }
}
