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

    [SKFunction("Subtract two numbers")]
    [SKFunctionContextParameter(Name = "input", Description = "The first number to subtract from")]
    [SKFunctionContextParameter(Name = "number2", Description = "The second number to subtract away")]
    public string Subtract(SKContext context)
    {
        return (Convert.ToDouble(context["input"]) - Convert.ToDouble(context["number2"])).ToString();
    }

    [SKFunction("Multiply two numbers. When increasing by a percentage, don't forget to add 1 to the percentage.")]
    [SKFunctionContextParameter(Name = "input", Description = "The first number to multiply")]
    [SKFunctionContextParameter(Name = "number2", Description = "The second number to multiply")]
    public string Multiply(SKContext context)
    {
        return (Convert.ToDouble(context["input"]) * Convert.ToDouble(context["number2"])).ToString();
    }

    [SKFunction("Divide two numbers")]
    [SKFunctionContextParameter(Name = "input", Description = "The first number to divide from")]
    [SKFunctionContextParameter(Name = "number2", Description = "The second number to divide by")]
    public string Divide(SKContext context)
    {
        return (Convert.ToDouble(context["input"]) / Convert.ToDouble(context["number2"])).ToString();
    }
}