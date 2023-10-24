// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using System.Globalization;
using Microsoft.SemanticKernel;

namespace Plugins.MathPlugin;

public class Math
{
    [SKFunction, Description("Take the square root of a number")]
    public static string Sqrt(string number)
    {
        return System.Math.Sqrt(Convert.ToDouble(number, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
    }

    [SKFunction, Description("Multiply two numbers")]
    public static double Multiply(
        [Description("The first number to multiply")] double input,
        [Description("The second number to multiply")] double number2
    )
    {
        return input * number2;
    }
}
