

::: zone pivot="programming-language-csharp"
# Text-to-image

Text-to-image is the process of generating an image from a text prompt. This is useful for generating images for chat bots, creating images for reports, and more. Today's chat completion models currently do not support text-to-image. To recreate the experience in ChatGPT, you can wrap a text-to-image model in a plugin so that the chat completion model can call it.


To add a text-to-image service, you can use the following code to add it to the kernel's inner service provider.

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
