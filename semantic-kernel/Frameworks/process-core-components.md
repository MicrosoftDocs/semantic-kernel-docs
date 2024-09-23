---
title: Process Framework - Core Components
description: Details on the Processes Framework in Semantic Kernel
zone_pivot_groups: programming-languages
author: evchaki            
ms.topic: tutorial
ms.author: evchaki   
ms.date: 09/28/2024
ms.service: semantic-kernel
---

# Core Components of the Process Framework
The Process Framework is built upon a modular architecture that enables developers to construct sophisticated workflows through its core components. Understanding these components is essential for effectively leveraging the framework.

## Process

A Process serves as the overarching container that orchestrates the execution of Steps. It defines the flow and routing of data between Steps, ensuring that process goals are achieved efficiently. Processes handle inputs and outputs, providing flexibility and scalability across various workflows.

## Process Features

- **Stateful:** Supports querying information such as tracking status and percent completion, as well as the ability to pause and resume.
- **Reusable:** A Process can be invoked within other processes, promoting modularity and reusability.
- **Event Driven:** Employs event-based flow with listeners to route data to Steps and other Processes.
- **Scalable:** Utilizes well-established runtimes for global scalability and rollouts.
- **Cloud Event Integrated:** Incorporates industry-standard eventing for triggering a Process or Step.

### Creating A Process

To create a new Process, add the Process Package to your project and define a name for your process.



## Step

Steps are the fundamental building blocks within a Process. Each Step corresponds to a discrete unit of work and encapsulates one or more Kernel Functions. Steps can be created independently of their use in specific Processes, enhancing their reusability. They emit events based on the work performed, which can trigger subsequent Steps.

### Step Features

- **Stateful:** Facilitates tracking information such as status and defined tags.
- **Reusable:** Steps can be employed across multiple Processes.
- **Dynamic:** Steps can be created dynamically by a Process as needed, depending on the required pattern.
- **Flexible:** Offers different types of Steps for developers by leveraging Kernel Functions, including Code-only, API calls, AI Agents, and Human-in-the-loop.
- **Auditable:** Telemetry is enabled across both Steps and Processes.

### Defining a Step

To create a Step, define a public class to name the Step and add it to the KernelStepBase. Within your class, you can incorporate one or multiple Kernel Functions.



### Register a Step into a Process

Once your class is created, you need to register it within your Process. For the first Step in the Process, add `isEntryPoint: true` so the Process knows where to start.




### Step Events

Steps have several events available, including:

- **OnEvent:** Triggered when the class completes its execution.
- **OnFunctionResult:** Activated when the defined Kernel Function emits results, allowing output to be sent to one or many Steps.
- **SendOutputTo:** Defines the Step and Input for sending results to a subsequent Step.



## Pattern

Patterns standardize common process flows, simplifying the implementation of frequently used operations. They promote a consistent approach to solving recurring problems across various implementations, enhancing both maintainability and readability.

### Pattern Types

- **Fan In:** The input for the next Step is supported by multiple outputs from previous Steps.
- **Fan Out:** The output of previous Steps is directed into multiple Steps further down the Process.
- **Cycle:** Steps continue to loop until completion based on input and output.
- **Map Reduce:** Outputs from a Step are consolidated into a smaller amount and directed to the next Step's input.

### Setting up a Pattern

Once your class is created for your Step and registered within the Process, you can define the events that should be sent downstream to other Steps or set conditions for Steps to be restarted based on the output from your Step.
