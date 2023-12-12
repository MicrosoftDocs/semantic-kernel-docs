import json
from semantic_kernel import ContextVariables, Kernel
from semantic_kernel.skill_definition import sk_function
from semantic_kernel.orchestration.sk_context import SKContext


class Orchestrator:
    def __init__(self, kernel: Kernel):
        self._kernel = kernel

    @sk_function(
        description="Routes the request to the appropriate function",
        name="RouteRequest",
    )
    async def route_request(self, context: SKContext) -> str:
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

        get_numbers = self._kernel.skills.get_function(
            "OrchestratorPlugin", "GetNumbers"
        )
        getNumberContext = (
            await self._kernel.run_async(get_numbers, input_str=request)
        ).result
        numbers = json.loads(getNumberContext)

        # Call the appropriate function
        if intent == "Sqrt":
            # Call the Sqrt function with the first number
            square_root = self._kernel.skills.get_function("MathPlugin", "Sqrt")
            sqrt_results = await self._kernel.run_async(
                square_root, input_str=numbers["number1"]
            )

            return sqrt_results["input"]
        elif intent == "Multiply":
            # Call the Multiply function with both numbers
            multiply = self._kernel.skills.get_function("MathPlugin", "Multiply")
            variables = ContextVariables()
            variables["input"] = numbers["number1"]
            variables["number2"] = numbers["number2"]
            multiply_results = await self._kernel.run_async(
                multiply, input_vars=variables
            )

            return multiply_results["input"]
        else:
            return "I'm sorry, I don't understand."
