---
title: How to compose functions in Semantic Kernel
description: How to compose functions in Semantic Kernel
author: johnmaeda
ms.topic: skills
ms.author: johnmaeda
ms.date: 02/07/2023
ms.prod: semantic-kernel
---
# Connecting multiple functions together

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

> [!CAUTION]
> This section is still under construction

```csharp
        Console.WriteLine("======== Pipeline ========");

        IKernel kernel = Kernel.Build(s_log);

        // Load native skill
        var text = kernel.ImportSkill(new TextSkill());

        SKContext result = await kernel.RunAsync("    i n f i n i t e     s p a c e     ",
            text["LStrip"],
            text["RStrip"],
            text["Uppercase"]);

        Console.WriteLine(result);
```

## Take the next step

> [!div class="nextstepaction"]
> [Run the samples](../samples)