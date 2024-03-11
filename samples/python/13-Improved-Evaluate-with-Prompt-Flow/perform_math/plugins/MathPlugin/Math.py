import math
from semantic_kernel.plugin_definition import (
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
        description="Adds two numbers together",
        name="Add",
    )
    @sk_function_context_parameter(
        name="input",
        description="The first number to add",
    )
    @sk_function_context_parameter(
        name="input2",
        description="The second number to add",
    )
    def add(self, context: SKContext) -> str:
        return str(float(context["input"]) + float(context["input2"]))

    @sk_function(
        description="Subtract two numbers. If you expect a negative result, the minuend should be the smaller number.",
        name="Subtract",
    )
    @sk_function_context_parameter(
        name="input",
        description="The number to subtract from (i.e., minuend)",
    )
    @sk_function_context_parameter(
        name="subtrahend",
        description="The number to subtract away from the input",
    )
    def subtract(self, context: SKContext) -> str:
        return str(float(context["input"]) - float(context["subtrahend"]))

    @sk_function(
        description="Multiply two numbers. When increasing by a percentage, don't forget to add 1 to the percentage.",
        name="Multiply",
    )
    @sk_function_context_parameter(
        name="input",
        description="The first number to multiply",
    )
    @sk_function_context_parameter(
        name="input2",
        description="The second number to multiply",
    )
    def multiply(self, context: SKContext) -> str:
        return str(float(context["input"]) * float(context["input2"]))

    @sk_function(
        description="Divide two numbers",
        name="Divide",
    )
    @sk_function_context_parameter(
        name="input",
        description="The first number to divide from (i.e., the numerator)",
    )
    @sk_function_context_parameter(
        name="denominator",
        description="The second number to divide by",
    )
    def divide(self, context: SKContext) -> str:
        return str(float(context["input"]) / float(context["denominator"]))
