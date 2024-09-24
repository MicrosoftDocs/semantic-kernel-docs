---
title: Process Framework Deployments
description: Details on the Deployment Options in the Processes Framework from Semantic Kernel
zone_pivot_groups: 
author: evchaki            
ms.topic: tutorial
ms.author: evchaki   
ms.date: 09/28/2024
ms.service: semantic-kernel
---

# Deployment of the Process Framework
Deploying workflows built with the Process Framework can be done seamlessly across local development environments and cloud runtimes. This flexibility enables developers to choose the best approach tailored to their specific use cases.

## Local Development

The Process Framework provides an in-process runtime that allows developers to run processes directly on their local machines or servers without requiring complex setups or additional infrastructure. This runtime supports both memory and file-based persistence, ideal for rapid development and debugging. You can quickly test processes with immediate feedback, accelerating the development cycle and enhancing efficiency.

## Cloud Runtimes

For scenarios requiring scalability and distributed processing, the Process Framework supports cloud runtimes such as [**Orleans**](https://learn.microsoft.com/dotnet/orleans/overview) and [**Dapr**](https://dapr.io/). These options empower developers to deploy processes in a distributed manner, facilitating high availability and load balancing across multiple instances. By leveraging these cloud runtimes, organizations can streamline their operations and manage substantial workloads with ease.

- **Orleans Runtime:** This framework provides a programming model for building distributed applications and is particularly well-suited for handling virtual actors in a resilient manner, complementing the Process Framework’s event-driven architecture.
  
- **Dapr (Distributed Application Runtime):** Dapr simplifies microservices development by providing a foundational framework for building distributed systems. It supports state management, service invocation, and pub/sub messaging, making it easier to connect various components within a cloud environment.

Using either runtime, developers can scale applications according to demand, ensuring that processes run smoothly and efficiently, regardless of workload.

With the flexibility to choose between local testing environments and robust cloud platforms, the Process Framework is designed to meet diverse deployment needs. This enables developers to concentrate on building innovative AI-powered processes without the burden of infrastructure complexities.

> [!NOTE]
> Orleans will be supported first with the .NET Process Framework, followed by Dapr in the upcoming release of the Python version of the Process Framework.
