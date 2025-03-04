---
title: Realtime AI Integrations for Semantic Kernel 
description: Learn about realtime multi-modal AI integrations available in Semantic Kernel.
author: eavanvalkenburg
ms.topic: conceptual
ms.author: edvan
ms.date: 02/26/2025
ms.service: semantic-kernel
---

# Realtime Multi-modal APIs

The first realtime API integration for Semantic Kernel have been added, it is currently only available in Python and considered experimental. This is because the underlying services are still being developed and are subject to changes and we might need to make breaking changes to the API in Semantic Kernel as we learn from customers how to use this and as we add other providers of these kinds of models and APIs.

## Realtime Client abstraction

To support different realtime APIs from different vendors, using different protocols, a new client abstraction has been added to the kernel. This client is used to connect to the realtime service and send and receive messages.
The client is responsible for handling the connection to the service, sending messages, and receiving messages. The client is also responsible for handling any errors that occur during the connection or message sending/receiving process.

### Realtime API

Any realtime client implements the following methods:

| Method           | Description                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------------ |
| `create_session` | Creates a new session                                                                                              |
| `update_session` | Updates an existing session                                                                                        |
| `delete_session` | Deletes an existing session                                                                                        |
| `receive`        | This is a asynchronous generator method that listens for messages from the service and yields them as they arrive. |
| `send`           | Sends a message to the service                                                                                     |

### Python implementations

The python version of Semantic Kernel currently supports the following realtime clients:

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

Depending on how you want to handle audio, you might need additional packages to interface with speakers and microphones, like `pyaudio` or `sounddevice`.

### Websocket clients

Then you can create a kernel and add the realtime client to it, this shows how to do that with a AzureRealtimeWebsocket connection, you can replace AzureRealtimeWebsocket with OpenAIRealtimeWebsocket without any further changes.

```python
from semantic_kernel.connectors.ai.open_ai import (
    AzureRealtimeWebsocket,
    AzureRealtimeExecutionSettings,
    ListenEvents,
)
from semantic_kernel.contents import RealtimeAudioEvent, RealtimeTextEvent

# this will use environment variables to get the api key, endpoint, api version and deployment name.
realtime_client = AzureRealtimeWebsocket()
settings = AzureRealtimeExecutionSettings(voice='alloy')
async with realtime_client(settings=settings, create_response=True):
    async for event in realtime_client.receive():
        match event:
            # receiving a piece of audio (and send it to a undefined audio player)
            case RealtimeAudioEvent():
                await audio_player.add_audio(event.audio)
            # receiving a piece of audio transcript
            case RealtimeTextEvent():
                # Semantic Kernel parses the transcript to a TextContent object captured in a RealtimeTextEvent
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

### WebRTC client

The setup of a WebRTC connection is a bit more complex and so we need a extra parameter when creating the client. This parameter, `audio_track` needs to be a object that implements the `MediaStreamTrack` protocol of the `aiortc` package, this is also demonstrated in the samples that are linked below.

To create a client that uses WebRTC, you would do the following:

```python
from semantic_kernel.connectors.ai.open_ai import (
    ListenEvents,
    OpenAIRealtimeExecutionSettings,
    OpenAIRealtimeWebRTC,
)
from aiortc.mediastreams import MediaStreamTrack

class AudioRecorderWebRTC(MediaStreamTrack):
    # implement the MediaStreamTrack methods.

realtime_client = OpenAIRealtimeWebRTC(audio_track=AudioRecorderWebRTC())
# Create the settings for the session
settings = OpenAIRealtimeExecutionSettings(
    instructions="""
You are a chat bot. Your name is Mosscap and
you have one goal: figure out what people need.
Your full name, should you need to know it, is
Splendid Speckled Mosscap. You communicate
effectively, but you tend to answer with long
flowery prose.
""",
    voice="shimmer",
)
audio_player = AudioPlayer
async with realtime_client(settings=settings, create_response=True):
    async for event in realtime_client.receive():
        match event.event_type:
            # receiving a piece of audio (and send it to a undefined audio player)
            case "audio":
                await audio_player.add_audio(event.audio)
            case "text":
                # the model returns both audio and transcript of the audio, which we will print
                print(event.text.text, end="")
            case "service":
                # OpenAI Specific events
                if event.service_type == ListenEvents.SESSION_UPDATED:
                    print("Session updated")
                if event.service_type == ListenEvents.RESPONSE_CREATED:
                    print("\nMosscap (transcript): ", end="")
```

Both of these samples receive the audio as RealtimeAudioEvent and then they pass that to a unspecified audio_player object.

### Audio output callback

Next to this we have a parameter called `audio_output_callback`  on the `receive` method and on the class creation. This callback will be called first before any further handling of the audio and gets a `numpy` array of the audio data, instead of it being parsed into AudioContent and returned as a RealtimeAudioEvent that you can then handle, which is what happens above. This has shown to give smoother audio output because there is less overhead between the audio data coming in and it being given to the player.

This example shows how to define and use the `audio_output_callback`:

```python
from semantic_kernel.connectors.ai.open_ai import (
    ListenEvents,
    OpenAIRealtimeExecutionSettings,
    OpenAIRealtimeWebRTC,
)
from aiortc.mediastreams import MediaStreamTrack

class AudioRecorderWebRTC(MediaStreamTrack):
    # implement the MediaStreamTrack methods.

class AudioPlayer:
    async def play_audio(self, content: np.ndarray):
        # implement the audio player

realtime_client = OpenAIRealtimeWebRTC(audio_track=AudioRecorderWebRTC())
# Create the settings for the session
settings = OpenAIRealtimeExecutionSettings(
    instructions="""
You are a chat bot. Your name is Mosscap and
you have one goal: figure out what people need.
Your full name, should you need to know it, is
Splendid Speckled Mosscap. You communicate
effectively, but you tend to answer with long
flowery prose.
""",
    voice="shimmer",
)
audio_player = AudioPlayer
async with realtime_client(settings=settings, create_response=True):
    async for event in realtime_client.receive(audio_output_callback=audio_player.play_audio):
        match event.event_type:
            # no need to handle case: "audio"
            case "text":
                # the model returns both audio and transcript of the audio, which we will print
                print(event.text.text, end="")
            case "service":
                # OpenAI Specific events
                if event.service_type == ListenEvents.SESSION_UPDATED:
                    print("Session updated")
                if event.service_type == ListenEvents.RESPONSE_CREATED:
                    print("\nMosscap (transcript): ", end="")
```

### Samples

There are four samples in [our repo](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/concepts/realtime), they cover both the basics using both websockets and WebRTC, as well as a more complex setup including function calling. Finally there is a more [complex demo](https://github.com/microsoft/semantic-kernel/tree/main/python/samples/demos/call_automation) that uses [Azure Communication Services](/azure/communication-services/) to allow you to call your Semantic Kernel enhanced realtime API.
