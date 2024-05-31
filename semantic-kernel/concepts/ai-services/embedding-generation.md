
::: zone pivot="programming-language-csharp,programming-language-python"

# Embedding generation

Embedding generation is the process of generating a vector representation of a text prompt. This is useful for retrieving text from memory stores.

> [!NOTE] As part of the enhancements coming to Semantic Kernel's memory connectors, we plan on making the embedding interface more generic to support additional embedding models.

::: zone-end

::: zone pivot="programming-language-csharp"

To add an embedding generation service, you can use the following code to add it to the kernel's inner service provider.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

If you're working directly with a service provider, you can also use the following methods.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

Lastly, you can create instances of the service directly so that you can either add them to a kernel later or use them directly in your code without injecting them into the kernel.

# [Azure OpenAI](#tab/AzureOpenAI)

# [OpenAI](#tab/OpenAI)

# [Hugging Face](#tab/HuggingFace)

---

::: zone-end

::: zone pivot="programming-language-python"

::: zone-end