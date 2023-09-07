from promptflow import PFClient

pf_client = PFClient()

# Run a single test of a flow
#################################################

# Define the inputs of the flow
inputs = {
    "text": "What would you have left if you spent $3 when you only had $2 to begin with"
}

# Run the flow
flow_result = pf_client.test(flow="perform_math", inputs=inputs)

# Print the outputs of the flow
print(f"Flow outputs: {flow_result}")
