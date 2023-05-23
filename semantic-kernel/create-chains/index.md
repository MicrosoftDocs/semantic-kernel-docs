---
title: How to compose functions in Semantic Kernel
description: How to compose functions in Semantic Kernel
author: johnmaeda
ms.topic: creating-chains
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# Chaining functions together

[!INCLUDE [pat_large.md](../includes/pat_large.md)]

Semantic Kernel was designed in the spirit of UNIX's piping capability to take one command and stream its output to the next command in the sequence. You can see that legacy design built-in to the use of the `$INPUT` parameter as a default intake for a function to stream its output into the next `$INPUT`-ready function.

For example we can make three inline semantic functions and string their outputs into the next input, and so forth after preparing `myKernel` as [before](/semantic-kernel/howto/semanticfunctions#get-your-kernel-ready).

```csharp
string myJokePrompt = """
Tell a short joke about {{$INPUT}}.
""";

string myPoemPrompt = """
Take this "{{$INPUT}}" and convert it to a nursery rhyme.
""";

string myMenuPrompt = """
Make this poem "{{$INPUT}}" influence the three items in a coffee shop menu. 
The menu reads in enumerated form:
1.
""";

var myJokeFunction = myKernel.CreateSemanticFunction(myJokePrompt, maxTokens: 500);
var myPoemFunction = myKernel.CreateSemanticFunction(myPoemPrompt, maxTokens: 500);
var myMenuFunction = myKernel.CreateSemanticFunction(myMenuPrompt, maxTokens: 500);

var myOutput = await myKernel.RunAsync(
    new ContextVariables("Charlie Brown"),
    myJokeFunction,
    myPoemFunction,
    myMenuFunction);

Console.WriteLine(myOutput);
```

This can result in something like:

```Output
1. Charlie Brown's Surprise - A sweet and creamy latte with a hint of caramel 
2. Good Grief! - A bold espresso with a dash of cinnamon 
3. Wide Smile - A smooth cappuccino with a sprinkle of nutmeg
```

We could also have stopped the Chaining process one step shorter with:

```csharp
var myOutput = await myKernel.RunAsync(
    new ContextVariables("Charlie Brown"),
    myJokeFunction,
    myPoemFunction);
```

Which would result in something like:

```Output
Charlie Brown got a present one day
He said "Oh good grief!" in dismay
He opened it up with a smile so wide
But it wasn't what he had in mind
```

## Take the next step

> [!div class="nextstepaction"]
> [Learn more about the kernel](./kernel.md)