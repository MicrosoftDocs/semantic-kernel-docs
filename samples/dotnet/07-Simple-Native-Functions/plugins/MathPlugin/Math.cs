// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using System.Globalization;
using Microsoft.SemanticKernel;

namespace Plugins.MathPlugin;

public class Math
{
    [SKFunction, Description("Take the square root of a number")]
    public double Sqrt([Description("The number to take a square root of")] double input)
    {
        return System.Math.Sqrt(input);
    }
}
