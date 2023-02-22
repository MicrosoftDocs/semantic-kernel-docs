---
title: Support for Semantic Kernel on M-series Macbooks
description: Support for Semantic Kernel on M-series Macbooks
author: johnmaeda
ms.topic: samples
ms.author: johnmaeda
ms.date: 02/07/2023
ms.service: mssearch
---
# Running Azure Functions on M-series Macbooks in early 2023


[!INCLUDE [subheader.md](../includes/pat_medium.md)]

> [!TIP]
> Save time by installing the latest [Azure CLI](https://github.com/Azure/azure-functions-core-tools) that supports M-series Macbooks.

If you’re attempting to do a:

`func start host`

and there's a complaint about architecture mismatch, then it means [you have an older version of Azure Functions installed](https://github.com/Azure/azure-functions-core-tools/issues/2834). In the event that you still want to proceed without fixing your current situation, here are instructions based upon [this post](https://github.com/Azure/azure-functions-python-worker/issues/915) to keep you going:

1. Enable Rosetta in iTerm by doing something a bit contorted: i/ duplicate iTerm, ii/ rename it as iTerm Rosetta, iii/ do a command-i to set it as a Rosetta terminal
2. Verify that it’s running the i386 architecture by typing arch at the terminal
3. Next you install the i386 version of homebrew

`arch -x86_64 /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install.sh)"`

With your new i386 brew, install azure-functions-core-tools4** and a version of python with this homebrew. Point path to the new Rosetta Python. And run:

`arch -x86_64 /usr/local/bin/func start host`

## Advanced: Managing a World of Two Brews

Pro-tip for keeping the two brews clear in your own head is [here](https://stackoverflow.com/questions/64882584/how-to-run-the-homebrew-installer-under-rosetta-2-on-m1-macbook) and method is noted as follows:

`alias ibrew='arch -x86_64 /usr/local/bin/brew'
alias mbrew='arch -arm64e /opt/homebrew/bin/brew'`

Set your path how you want it:

`path=( /opt/homebrew/bin /opt/homebrew/opt /usr/local/bin /usr/bin /bin /usr/sbin /sbin /Library/Apple/usr/bin )`

This way you can choose your brew more carefully, like when told to install azure functions and Python into that intel-based brew, this will do the trick:

`ibrew install azure-cli`

This may require a little bit of sudo-ing along the way to get it to stick. Cross your fingers and do this according to [this doc](https://github.com/Azure/azure-functions-core-tools):

`ibrew tap azure/functions
ibrew install azure-functions-core-tools@4`

Check if the function command is available:

`% ls /usr/local/bin/func
/usr/local/bin/func`

And you are now good to go:

`arch -x86_64 /usr/local/bin/func start host`

## Take the next step

> [!div class="nextstepaction"]
> [Run the samples](/semantic-kernel/support/samples)
