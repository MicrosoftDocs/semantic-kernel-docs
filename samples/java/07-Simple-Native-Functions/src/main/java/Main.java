// Copyright (c) Microsoft. All rights reserved.

import com.microsoft.semantickernel.SKBuilders;
import plugins.mathplugin.Math;

public class Main {
    public static void main(String[] args) {

        var kernel = SKBuilders.kernel().build();

        var skills = kernel.importSkill(new Math(), "MathPlugin");

        var result = kernel.runAsync("12", skills.getFunction("sqrt") ).block();

        System.out.println(result.getResult());
    }
}
