---
title: Evaluate your plugins with Prompt flow
description: Leverage Prompt flow to evaluate plugin descriptions .
author: matthewbolanos
ms.topic: tutorial
ms.author: mabolan
ms.date: 07/12/2023
ms.service: semantic-kernel
---

# Create a Prompt flow with Semantic Kernel

[!INCLUDE [pat_large.md](../../../includes/pat_large.md)]

In the [planner](../index.md) article, we demonstrated how you can use the sequential planner to automatically use the math plugin to answer word problems provided by the user. If you began testing your planner with additional inputs, however, you may have noticed that it doesn't always produce the desired results. In this article, we'll show you how you can create a [Prompt flow](/azure/machine-learning/prompt-flow/overview-what-is-prompt-flow) that runs your plugins and planners so that you can easily [test](./running-batches-with-prompt-flow.md), [evaluate](evaluating-plugins-and-planners-with-prompt-flow.md), and deploy them in following articles.

At the end of this article, you'll have a Prompt flow that can answer questions to math problems using Semantic Kernel.

![Semantic Kernel running inside of Prompt flow](../../../media/prompt-flow-end-result.png)

If you want to see the final solution to this article, you can check out the following samples in the public documentation repository. Use the link to the previous solution if you want to follow along.

> [!Note]
> Today Prompt flow is only available in Python, so this article will only show how to use Prompt flow to evaluate plugins using Python.

| Language | Link to previous solution | Link to final solution |
| --- | --- |
| C# |  Not applicable | Not available |
| Python |  [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/dotnet/11-Planner) | [Open solution in GitHub](https://github.com/MicrosoftDocs/semantic-kernel-docs/tree/main/samples/python/12-Evaluate-with-Prompt-Flow) |

## Create a new Prompt flow
In this tutorial, we'll be creating a flow that uses the math plugin we created in the [pervious tutorial](../index.md). We'll use the code-first approach to creating a Prompt flow. If you want full documentation on how to use the code-first approach, please refer to [Prompt flow's open source documentation](https://microsoft.github.io/promptflow/index.html).

If you would like to use the Azure portal to create your flow, you can follow the [How to develop an evaluation flow](/azure/machine-learning/prompt-flow/how-to-develop-an-evaluation-flow) tutorial in the Azure documentation.

### Install Prompt flow
1. Install the `promptflow` and `promptflow-tools` by running the following command in your terminal.

    ```bash
    pip install promptflow promptflow-tools
    ```

2. Validate that the installation was successful by running the following command in your terminal.

    ```bash
    # should print promptflow version, e.g. "0.1.0b3"
    pf -v
    ```

### Install the Prompt flow VS Code extension
We also recommend installing the Prompt flow VS Code extension to help you create and test Prompt flows directly from within VS Code.

1. Install the latest stable version of [VS Code](https://code.visualstudio.com/)
2. Install the [VS Code Python extension](https://marketplace.visualstudio.com/items?itemName=ms-python.python)
3. Install the [Prompt flow for VS Code extension](https://marketplace.visualstudio.com/items?itemName=prompt-flow.prompt-flow)

With the VS Code extension, you'll be able to view your Prompt flow in a visual editor, as well as test your Prompt flow directly from within VS Code.

:::image type="content" source="../../../media/prompt-flow-in-vs-code.png" alt-text="A graph of a Prompt flow":::

### Use the CLI to create a new Prompt flow
Prompt flow has three different types of flows: standard, chat, and evaluation. We'll first create a flow that will run the math plugin we created in the previous tutorial using the standard flow type. This will allow us to test the flow and make sure it's working as expected.

To create an evaluation flow, navigate to the root of the previously completed solution and run the following command in your terminal. If you haven't completed the previous tutorial, you can use the link at the top of this article to download the completed previous solution.

```bash
pf flow init --flow perform_math
```

This will create a new folder with the name of the flow you specified along with some boilerplate code. You can now open this folder in VS Code to view the files that were created. The most important files to note are the following:
- _flow.dag.yaml_ – The definition of the flow; includes inputs/outputs, nodes, tools and variants.
- _.promptflow/flow.tools.json_ – Provides metadata for the nodes found in the flow.dag.yaml file.
- _Source code files (.py, .jinja2)_ – The custom scripts that are referenced by Prompt flow.
- _requirements.txt_ – The Python package dependencies for this flow; used for installing packages into the Azure environment.

### Visualize the flow in VS Code
If you want to see what your flow looks like in a visual editor, you can open the _flow.dag.yaml_ file in VS Code and click the **Visual editor** link in the top left corner of the editor.

:::image type="content" source="../../../media/visualize-flow-link.png" alt-text="A graph of a Prompt flow in the VS Code extension":::

This will open the flow in a visual editor where you can see the nodes and their connections.

## Edit the new Prompt flow
Now that we have a starter flow, we can start customizing it to use the math plugin we created in the previous tutorial. We'll start by deleting the existing nodes and creating a new node that will call the math plugin.

### Delete the existing nodes
1. Navigate to the visual editor
2. Select the delete icon (the trash can) for the *hello_prompt* and *echo_my_prompt* nodes.
3. Delete the _hello.jinja2_ and _hello.py_ files from the flow folder.

### Create a new node
To run our math plugin, we'll need to create a new node that will actually call the plugin using a Semantic Kernel planner. We'll use the Python node type to do this.

1. Navigate to the visual editor
2. Select the **+** icon to create a new node
3. Select the **Python** node type
4. Name your new node *math_planner*
5. Select **New file**.

After completing these steps, you should see a new node in the visual editor with the name *math_planner* along with a new Python file called *math_planner.py* in the flow folder.

### Wiring up the new node
In the visual editor, you'll likely see an error icon. This is because our node doesn't have any inputs or outputs defined. We'll need to define these so that Prompt flow knows how to wire up the nodes.

To do this, follow these steps:
1. Navigate to the visual editor
2. Rename the **output_prompt** output variable to **result**
3. Change the value of the **result** output variable to `${math_planner.output}`
4. Set the value of the **input1** string of the **math_planner** node to `${inputs.text}`

After completing these steps, you should see that the error icon has disappeared and the nodes are now connected.

:::image type="content" source="../../../media/wired-up-flow.png" alt-text="A valid graph of a Prompt flow":::

If you want, you can also edit the flow directly in the _flow.dag.yaml_ file. The following code shows the updated _flow.dag.yaml_ file. If you are running into issues with the visual editor, you can copy and paste this code into your _flow.dag.yaml_ file.

```yaml
id: template_standard_flow
name: Template Standard Flow
environment:
  python_requirements_txt: requirements.txt
inputs:
  text:
    type: string
outputs:
  result:
    type: string
    reference: ${math_planner.output}
nodes:
- name: math_planner
  type: python
  source:
    type: code
    path: math_planner.py
  inputs:
    input1: ${inputs.text}
```

### Calling the math plugin with the planner
Now that we have our nodes wired up, we can add the code to call the math plugin. To do this, we'll need to update the *math_planner.py* file with the code from the [planner tutorial](../index.md). Below is the updated code for the *math_planner.py* file.

When creating a node for Prompt flow, it's important to use the `@tool` decorator to indicate that the function is a tool that can be called by Prompt flow. 

:::code language="python" source="~/../samples/python/12-Evaluate-with-Prompt-Flow/perform_math/math_planner.py" range="1-6,9-17,19-38" highlight="9":::

You'll also need to copy and paste your _plugins_ folder from the previous tutorial into the flow folder so that the math plugin is available to the flow. Below is the updated directory structure for the flow.

```directory
12-Evaluate-with-Prompt-Flow
└───perform_math
|   └───.promptflow
|   |   └───flow.tools.json
|   └───plugins
│   |   └───MathPlugin
|   |       └───Math.py
|   └───.gitignore
|   └───data.jsonl
|   └───flow.dag.yaml
|   └───hello.jinja2
|   └───hello.py
|   └───requirements.txt
└───main.py
```

## Performing an ad-hoc test of your Prompt flow
Our flow is now ready to be tested. To do this, complete the following:
1. Navigate to the visual editor. 
2. Set the value of the **text** input variable to `What is 2 plus 3?`
3. Select the **Run all** button (the double play icon) in the top right corner of the visual editor.

Within the terminal, you should see the following output.

```json
{'result': '5.0'}
```

If you want to test your flow using the SDK, you can also write a simple Python script to do this.


:::code language="python" source="~/../samples/python/12-Evaluate-with-Prompt-Flow/main.py" range="1-4, 8-17":::

## Next steps
Now that you have a flow that can run your plugin, you can start to test where it's performing well and where it's not. Testing each case individually can be time consuming, so in the next article, we'll show you how you can run your flow on a large amount of data to see how well it performs with a wide range of user requests.

> [!div class="nextstepaction"]
> [Run batches with Prompt flow](./running-batches-with-prompt-flow.md)
