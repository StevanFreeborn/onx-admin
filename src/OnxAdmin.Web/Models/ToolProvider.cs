namespace OnxAdmin.Web.Models;

using Tool = Anthropic.SDK.Common.Tool;

interface IToolProvider
{
  List<Tool> GetTools();
}

abstract class ToolProvider : IToolProvider
{
  public List<Tool> GetTools()
  {
    return GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
      .Where(m => m.GetCustomAttribute<FunctionAttribute>() is not null)
      .Select(m => Tool.GetOrCreateTool(this, m.Name, m.GetCustomAttribute<FunctionAttribute>()!.Description))
      .ToList();
  }
}