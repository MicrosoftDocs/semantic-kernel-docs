using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Plugins;

public class MathPlugin
{
    [SKFunction("Take the square root of a number")]
    public string Sqrt(string number)
    {
        return Math.Sqrt(Convert.ToDouble(number)).ToString();
    }

    [SKFunction("Add two numbers")]
    [SKFunctionContextParameter(Name = "input", Description = "The first number to add")]
    [SKFunctionContextParameter(Name = "number2", Description = "The second number to add")]
    public string Add(SKContext context)
    {
        return (Convert.ToDouble(context["input"]) + Convert.ToDouble(context["number2"])).ToString();
    }
}