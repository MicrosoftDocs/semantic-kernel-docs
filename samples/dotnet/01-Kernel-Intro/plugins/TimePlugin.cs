
using Microsoft.SemanticKernel;

namespace Plugins;

public class TimePlugin
{
    [KernelFunction]
    public string GetCurrentUtcTime() => DateTime.UtcNow.ToString("R");
}