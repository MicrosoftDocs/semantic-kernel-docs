using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace Plugins;

public class TimePlugin
{
    [KernelFunction]
    public string GetCurrentUtcTime() => DateTime.UtcNow.ToString("R");
}