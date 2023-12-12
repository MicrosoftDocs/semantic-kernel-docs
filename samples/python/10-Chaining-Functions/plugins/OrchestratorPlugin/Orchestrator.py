import json
from semantic_kernel import ContextVariables, Kernel
from semantic_kernel.skill_definition import (
    sk_function,
)
from semantic_kernel.orchestration.sk_context import SKContext


class Orchestrator:
    def __init__(self, kernel: Kernel):
        self._kernel = kernel

    @sk_function(
        description="Routes the request to the appropriate function",
        name="route_request",
    )
    async def RouteRequest(self, context: SKContext) -> str:
        # Save the original user request
        request = context["input"]

        # Add the list of available functions to the context variables
        variables = ContextVariables()
        variables["input"] = request
        variables["options"] = "Sqrt, Multiply"

        # Retrieve the intent from the user request
        get_intent = self._kernel.skills.get_function("OrchestratorPlugin", "getIntent")
        intent = (
            await self._kernel.run_async(get_intent, input_vars=variables)
        ).result.strip()

        # Prepare the functions to be called in the pipeline
        get_numbers = self._kernel.skills.get_function(
            "OrchestratorPlugin", "GetNumbers"
        )
        extract_numbers_from_json = self._kernel.skills.get_function(
            "OrchestratorPlugin", "ExtractNumbersFromJson"
        )
        create_response = self._kernel.skills.get_function(
            "OrchestratorPlugin", "CreateResponse"
        )

        # Retrieve the correct function based on the intent
        if intent == "Sqrt":
            math_function = self._kernel.skills.get_function("MathPlugin", "Sqrt")
        elif intent == "Multiply":
            math_function = self._kernel.skills.get_function("MathPlugin", "Multiply")
        else:
            return "I'm sorry, I don't understand."

        # Run the functions in a pipeline
        output = await self._kernel.run_async(
            get_numbers,
            extract_numbers_from_json,
            math_function,
            input_str=request,
        )

        # Create a new context object with the original request
        pipelineVariables = ContextVariables()
        pipelineVariables["original_request"] = request
        pipelineVariables["input"] = request

        # Run the functions in a pipeline with create_response
        output = await self._kernel.run_async(
            get_numbers,
            extract_numbers_from_json,
            math_function,
            create_response,
            input_vars=pipelineVariables,
        )

        return output["input"]

    @sk_function(
        description="Extracts numbers from JSON",
        name="ExtractNumbersFromJson",
    )
    def extract_numbers_from_json(self, context: SKContext):
        numbers = json.loads(context["input"])

        # Loop through numbers and add them to the context
        for key, value in numbers.items():
            if key == "number1":
                # Add the first number to the input variable
                context["input"] = str(value)
            else:
                # Add the rest of the numbers to the context
                context[key] = str(value)

        return context
