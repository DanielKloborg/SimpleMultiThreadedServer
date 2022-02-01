using System;

namespace MultiThreaded_Server.Common {

  internal static class ConsoleUtils {
    private static readonly object lockObject = new object();

    internal static void WriteLine(ConsoleColor color, string line) {
      lock (lockObject) {
        ConsoleColor original = Console.ForegroundColor;
        {
          Console.ForegroundColor = color;
          Console.WriteLine(line);
        }
        Console.ForegroundColor = original;
      }
    }

    internal static void WriteLine(string line) {
      lock (lockObject) {
        WriteLine(Console.ForegroundColor, line);
      }
    }
  }
}
