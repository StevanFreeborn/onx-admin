namespace OnxAdmin.API.Diagnostics;

public class Instrumentation : IDisposable
{
  internal const string ActivitySourceName = "OnxAdmin.API";
  internal const string ActivitySourceVersion = "1.0.0";

  public Instrumentation()
  {
    ActivitySource = new ActivitySource(ActivitySourceName, ActivitySourceVersion);
  }

  public ActivitySource ActivitySource { get; }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
    ActivitySource.Dispose();
  }
}