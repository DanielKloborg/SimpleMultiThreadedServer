
namespace MultiThreaded_Server.Common {
  internal interface IShutdownable {
    void Shutdown(bool waitForTermination);
  }
}
