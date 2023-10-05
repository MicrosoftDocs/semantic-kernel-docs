// Copyright (c) Microsoft. All rights reserved.

package plugins.mathplugin;

import com.microsoft.semantickernel.skilldefinition.annotations.DefineSKFunction;
import com.microsoft.semantickernel.skilldefinition.annotations.SKFunctionInputAttribute;

public class Math {

    public Math() {}
    @DefineSKFunction(name = "Sqrt", description = "Take the square root of a number")
    public String sqrt(@SKFunctionInputAttribute(description = "The number to take the square root of") String input) {
        return Double.toString(java.lang.Math.sqrt(Double.parseDouble(input)));
    }
    
}