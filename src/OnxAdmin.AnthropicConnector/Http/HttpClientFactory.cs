using System.Collections.Concurrent;

namespace OnxAdmin.AnthropicConnector.Http;

internal static class HttpClientFactory
{
  private static readonly ConcurrentDictionary<string, HttpClient> ClientCache = new();
  public static HttpClient GetHttpClient(string baseAddress) => ClientCache.GetOrAdd(baseAddress, (address) => new() { BaseAddress = new Uri(address) });
}