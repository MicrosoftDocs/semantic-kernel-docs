import math
from semantic_kernel.skill_definition import (
    sk_function,
    sk_function_context_parameter,
)
from semantic_kernel.orchestration.sk_context import SKContext


class Math:
    @sk_function(
        description="Takes the square root of a number",
        name="Sqrt",
        input_description="The value to take the square root of",
    )
    def square_root(self, number: str) -> str:
        return str(math.sqrt(float(number)))

    @sk_function(
        description="Multiplies two numbers together",
        name="Multiply",
    )
    @sk_function_context_parameter(
        name="input",
        description="The first number to multiply",
    )
    @sk_function_context_parameter(
        name="number2",
        description="The second number to multiply",
    )
    def add(self, context: SKContext) -> str:
        return str(float(context["input"]) * float(context["number2"]))
