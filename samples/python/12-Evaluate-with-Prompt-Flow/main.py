from promptflow import PFClient

pf_client = PFClient()

# Run a single test of a flow
#################################################

# Define the inputs of the flow
inputs = {
    "text": "What is 2 plus 3?",
}

# Run the flow
flow_result = pf_client.test(flow="perform_math", inputs=inputs)

# Print the outputs of the flow
print(f"Flow outputs: {flow_result}")
