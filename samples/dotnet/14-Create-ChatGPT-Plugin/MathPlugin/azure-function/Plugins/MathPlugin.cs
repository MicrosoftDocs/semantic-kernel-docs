// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Plugins;

public class MathPlugin
{
    [KernelFunction, Description("Take the square root of a number")]
    public static double Sqrt(
        [Description("The number to take a square root of")] double number1
    )
    {
        return Math.Sqrt(number1);
    }

    [KernelFunction, Description("Add two numbers")]
    public static double Add(
        [Description("The first number to add")] double number1,
        [Description("The second number to add")] double number2
    )
    {
        return number1 + number2;
    }

    [KernelFunction, Description("Subtract two numbers")]
    public static double Subtract(
        [Description("The first number to subtract from")] double number1,
        [Description("The second number to subtract away")] double number2
    )
    {
        return number1 - number2;
    }

    [KernelFunction, Description("Multiply two numbers. When increasing by a percentage, don't forget to add 1 to the percentage.")]
    public static double Multiply(
        [Description("The first number to multiply")] double number1,
        [Description("The second number to multiply")] double number2
    )
    {
        return number1 * number2;
    }

    [KernelFunction, Description("Divide two numbers")]
    public static double Divide(
        [Description("The first number to divide from")] double number1,
        [Description("The second number to divide by")] double number2
    )
    {
        return number1 / number2;
    }
}
