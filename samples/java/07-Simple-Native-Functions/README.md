# Java Samples

## TL;DR

Run with:

```shell
# AZURE:
OPENAI_CLIENT_TYPE=AZURE_OPEN_AI \
AZURE_OPEN_AI_KEY="my-key" \
AZURE_OPEN_AI_ENDPOINT="endpoint url" \
../../mvnw clean compile exec:java

# OPENAI:
OPENAI_CLIENT_TYPE=OPENAI \
OPEN_AI_KEY="my-key" \
OPEN_AI_ORGANIZATION_ID="organisation id" \
../../mvnw clean compile exec:java
```

# Compile

The sample can be compiled via:

```shell
 mvnw clean compile
```

# Configuration

You can define the provider of Open AI by setting the `OPENAI_CLIENT_TYPE`
property or environment variable to either [`OPENAI`](https://openai.com/api/)
or [`AZURE_OPEN_AI`](https://learn.microsoft.com/azure/cognitive-services/openai/).
By default, the sample will use the Open AI client.

```shell
OPENAI_CLIENT_TYPE=OPENAI ../../mvnw exec:java

OR

 mvnw exec:java -DOPENAI_CLIENT_TYPE=AZURE_OPEN_AI
```

Depending on the type of client, the sample will look for these environment variables:

For OpenAI:
```shell
OPENAI_CLIENT_TYPE=OPENAI 
OPEN_AI_KEY="my-key" 
OPEN_AI_ORGANIZATION_ID="organisation id" 
```

For Azure OpenAI:
```shell
OPENAI_CLIENT_TYPE=AZURE_OPEN_AI 
AZURE_OPEN_AI_KEY="my-key" 
AZURE_OPEN_AI_ENDPOINT="endpoint url" 
```
## Run
After compiling and configuration, the sample can be run by:

```shell
mvn exec:java
```