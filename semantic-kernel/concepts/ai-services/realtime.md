---
title: Realtime AI Integrations for Semantic Kernel 
description: Learn about realtime AI integrations available in Semantic Kernel.
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 02/26/2025
ms.service: semantic-kernel
---

# Realtime API integrations for Semantic Kernel

The first realtime API integration for Semantic Kernel has been added, it is currently only available in Python and considered experimental. This is because the underlying services are still being developed and are subject to changes.

## Realtime Client abstraction

To support different realtime api's from different vendors, using different protocols, a new client abstraction has been added to the kernel. This client is used to connect to the realtime service and send and receive messages.
The client is responsible for handling the connection to the service, sending messages, and receiving messages. The client is also responsible for handling any errors that occur during the connection or message sending/receiving process.

### Realtime API

Any realtime client consists of the following methods:

| Method           | Description                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------------ |
| `create_session` | Creates a new session                                                                                              |
| `update_session` | Updates an existing session                                                                                        |
| `delete_session` | Deletes an existing session                                                                                        |
| `receive`        | This is a asynchronous generator method that listens for messages from the service and yields them as they arrive. |
| `send`           | Sends a message to the service                                                                                     |

## Python implementations

The python version of semantic kernel currently supports the following realtime clients:

| Client | Protocol  | Modalities   | Function calling enabled | Description                                                                                                                                                                                        |
| ------ | --------- | ------------ | ------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| OpenAI | Websocket | Text & Audio | Yes                      | The OpenAI Realtime API is a websocket based api that allows you to send and receive messages in realtime, this connector uses the OpenAI Python package to connect and receive and send messages. |
| OpenAI | WebRTC    | Text & Audio | Yes                      | The OpenAI Realtime API is a WebRTC based api that allows you to send and receive messages in realtime, it needs a webRTC compatible audio track at session creation time.                         |
| Azure  | Websocket | Text & Audio | Yes                      | The Azure Realtime API is a websocket based api that allows you to send and receive messages in realtime, this uses the same package as the OpenAI websocket connector.                            |

## Getting started

To get started with the Realtime API, you need to install the `semantic-kernel` package with the `realtime` extra.

```bash
pip install semantic-kernel[realtime]
```

Then you can create a kernel and add the realtime client to it.

```python
from semantic_kernel.connectors.ai.open_ai import (
    AzureRealtimeWebsocket,
    ListenEvents,
    OpenAIRealtimeExecutionSettings,
)
from semantic_kernel.contents import RealtimeAudioEvent, RealtimeTextEvent

# this will use environment variables to get the api key, endpoint, api version and deployment name.
realtime_client = AzureRealtimeWebsocket()
settings = OpenAIRealtimeExecutionSettings()
async with realtime_client(settings=settings, create_response=True):
    async for event in realtime_client.receive():
        match event:
            # receiving a piece of audio
            case RealtimeAudioEvent():
                await audio_player.add_audio(event.audio)
            # receiving a piece of audio transcript
            case RealtimeTextEvent():
                # the model returns both audio and transcript of the audio, which we will print
                print(event.text.text, end="")
            case _:
                # OpenAI Specific events
                if event.service_type == ListenEvents.SESSION_UPDATED:
                    print("Session updated")
                if event.service_type == ListenEvents.RESPONSE_CREATED:
                    print("\nMosscap (transcript): ", end="")
```

There are two important things to note, the first is that the `realtime_client` is an async context manager, this means that you can use it in an async function and use `async with` to create the session.
The second is that the `receive` method is an async generator, this means that you can use it in a for loop to receive messages as they arrive.

In this simple example, we are passing the audio to a unspecified `audio_player` object, and printing the transcript as it arrives.

There is also a `audio_output_callback` parameter on the client creation or on the `receive` method, this callback will be called first, and leads to smoother playback compared to the above example.

See the samples in our repo [link to follow].
