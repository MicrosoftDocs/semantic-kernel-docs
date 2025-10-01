---
title: Workflow Visualization
description: Learn how to visualize workflows using the Agent Framework.
zone_pivot_groups: programming-languages
author: TaoChenOSU
ms.topic: tutorial
ms.author: taochen
ms.date: 09/29/2025
ms.service: semantic-kernel
---

# Visualizing Workflows

::: zone pivot="programming-language-python"

## Overview

The Agent Framework provides powerful visualization capabilities for workflows through the `WorkflowViz` class. This allows you to generate visual diagrams of your workflow structure in multiple formats including Mermaid flowcharts, GraphViz DOT diagrams, and exported image files (SVG, PNG, PDF).

## Getting Started with WorkflowViz

### Basic Setup

```python
from agent_framework import WorkflowBuilder, WorkflowViz

# Create your workflow
workflow = (
    WorkflowBuilder()
    .set_start_executor(start_executor)
    .add_edge(start_executor, end_executor)
    .build()
)

# Create visualization
viz = WorkflowViz(workflow)
```

### Installation Requirements

For basic text output (Mermaid and DOT), no additional dependencies are needed. For image export:

```bash
# Install the viz extra
pip install agent-framework[viz]

# Install GraphViz binaries (required for image export)
# On Ubuntu/Debian:
sudo apt-get install graphviz

# On macOS:
brew install graphviz

# On Windows: Download from https://graphviz.org/download/
```

## Visualization Formats

### Mermaid Flowcharts

Generate Mermaid syntax for modern, web-friendly diagrams:

```python
# Generate Mermaid flowchart
mermaid_content = viz.to_mermaid()
print("Mermaid flowchart:")
print(mermaid_content)

# Example output:
# flowchart TD
#   dispatcher["dispatcher (Start)"];
#   researcher["researcher"];
#   marketer["marketer"];
#   legal["legal"];
#   aggregator["aggregator"];
#   dispatcher --> researcher;
#   dispatcher --> marketer;
#   dispatcher --> legal;
#   researcher --> aggregator;
#   marketer --> aggregator;
#   legal --> aggregator;
```

### GraphViz DOT Format

Generate DOT format for detailed graph representations:

```python
# Generate DOT diagram
dot_content = viz.to_digraph()
print("DOT diagram:")
print(dot_content)

# Example output:
# digraph Workflow {
#   rankdir=TD;
#   node [shape=box, style=filled, fillcolor=lightblue];
#   "dispatcher" [fillcolor=lightgreen, label="dispatcher\n(Start)"];
#   "researcher" [label="researcher"];
#   "marketer" [label="marketer"];
#   ...
# }
```

## Image Export

### Supported Formats

Export workflows as high-quality images:

```python
try:
    # Export as SVG (vector format, recommended)
    svg_file = viz.export(format="svg")
    print(f"SVG exported to: {svg_file}")
    
    # Export as PNG (raster format)
    png_file = viz.export(format="png")
    print(f"PNG exported to: {png_file}")
    
    # Export as PDF (vector format)
    pdf_file = viz.export(format="pdf")
    print(f"PDF exported to: {pdf_file}")
    
    # Export raw DOT file
    dot_file = viz.export(format="dot")
    print(f"DOT file exported to: {dot_file}")
    
except ImportError:
    print("Install 'viz' extra and GraphViz for image export:")
    print("pip install agent-framework[viz]")
    print("Also install GraphViz binaries for your platform")
```

### Custom Filenames

Specify custom output filenames:

```python
# Export with custom filename
svg_path = viz.export(format="svg", filename="my_workflow.svg")
png_path = viz.export(format="png", filename="workflow_diagram.png")

# Convenience methods
svg_path = viz.save_svg("workflow.svg")
png_path = viz.save_png("workflow.png")
pdf_path = viz.save_pdf("workflow.pdf")
```

## Workflow Pattern Visualizations

### Fan-out/Fan-in Patterns

Visualizations automatically handle complex routing patterns:

```python
from agent_framework import (
    WorkflowBuilder, WorkflowViz, AgentExecutor,
    AgentExecutorRequest, AgentExecutorResponse
)

# Create agents
researcher = AgentExecutor(chat_client.create_agent(...), id="researcher")
marketer = AgentExecutor(chat_client.create_agent(...), id="marketer")
legal = AgentExecutor(chat_client.create_agent(...), id="legal")

# Build fan-out/fan-in workflow
workflow = (
    WorkflowBuilder()
    .set_start_executor(dispatcher)
    .add_fan_out_edges(dispatcher, [researcher, marketer, legal])  # Fan-out
    .add_fan_in_edges([researcher, marketer, legal], aggregator)   # Fan-in
    .build()
)

# Visualize
viz = WorkflowViz(workflow)
print(viz.to_mermaid())
```

Fan-in nodes are automatically rendered with special styling:

- **DOT format**: Ellipse shape with light golden background and "fan-in" label
- **Mermaid format**: Double circle nodes `((fan-in))` for clear identification

### Conditional Edges

Conditional routing is visualized with distinct styling:

```python
def spam_condition(content: str) -> bool:
    return "spam" in content.lower()

workflow = (
    WorkflowBuilder()
    .add_edge(classifier, spam_handler, condition=spam_condition)
    .add_edge(classifier, normal_processor)  # Unconditional edge
    .build()
)

viz = WorkflowViz(workflow)
print(viz.to_digraph())
```

Conditional edges appear as:

- **DOT format**: Dashed lines with "conditional" labels
- **Mermaid format**: Dotted arrows (`-.->`) with "conditional" labels

### Sub-workflows

Nested workflows are visualized as clustered subgraphs:

```python
from agent_framework import WorkflowExecutor

# Create sub-workflow
sub_workflow = WorkflowBuilder().add_edge(sub_exec1, sub_exec2).build()
sub_workflow_executor = WorkflowExecutor(sub_workflow, id="sub_workflow")

# Main workflow containing sub-workflow
main_workflow = (
    WorkflowBuilder()
    .add_edge(main_executor, sub_workflow_executor)
    .add_edge(sub_workflow_executor, final_executor)
    .build()
)

viz = WorkflowViz(main_workflow)
dot_content = viz.to_digraph()  # Shows nested clusters
mermaid_content = viz.to_mermaid()  # Shows subgraph structures
```

## Complete Example

For a comprehensive example showing workflow visualization with fan-out/fan-in patterns, custom executors, and multiple export formats, see the [Concurrent with Visualization sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/workflows/visualization/concurrent_with_visualization.py).

The sample demonstrates:

- Expert agent workflow with researcher, marketer, and legal agents
- Custom dispatcher and aggregator executors
- Mermaid and DOT visualization generation
- SVG, PNG, and PDF export capabilities
- Integration with Azure OpenAI agents

## Visualization Features

### Node Styling

- **Start executors**: Green background with "(Start)" label
- **Regular executors**: Blue background with executor ID
- **Fan-in nodes**: Golden background with ellipse shape (DOT) or double circles (Mermaid)

### Edge Styling

- **Normal edges**: Solid arrows
- **Conditional edges**: Dashed/dotted arrows with "conditional" labels
- **Fan-out/Fan-in**: Automatic routing through intermediate nodes

### Layout Options

- **Top-down layout**: Clear hierarchical flow visualization
- **Subgraph clustering**: Nested workflows shown as grouped clusters
- **Automatic positioning**: GraphViz handles optimal node placement

## Integration with Development Workflow

### Documentation Generation

```python
# Generate documentation diagrams
workflow_viz = WorkflowViz(my_workflow)
doc_diagram = workflow_viz.save_svg("docs/workflow_architecture.svg")
```

### Debugging and Analysis

```python
# Analyze workflow structure
print("Workflow complexity analysis:")
dot_content = viz.to_digraph()
edge_count = dot_content.count(" -> ")
node_count = dot_content.count('[label=')
print(f"Nodes: {node_count}, Edges: {edge_count}")
```

### CI/CD Integration

```python
# Export diagrams for automated documentation
import os
if os.getenv("CI"):
    # Export for docs during CI build
    viz.save_svg("build/artifacts/workflow.svg")
    viz.export(format="dot", filename="build/artifacts/workflow.dot")
```

## Best Practices

1. **Use descriptive executor IDs** - They become node labels in visualizations
2. **Export SVG for documentation** - Vector format scales well in docs
3. **Use Mermaid for web integration** - Copy-paste into Markdown/wiki systems
4. **Leverage fan-in/fan-out visualization** - Clearly shows parallelism patterns
5. **Include visualization in testing** - Verify workflow structure matches expectations

### Running the Example

For the complete working implementation with visualization, see the [Concurrent with Visualization sample](https://github.com/microsoft/agent-framework/blob/main/python/samples/getting_started/workflows/visualization/concurrent_with_visualization.py).

::: zone-end
