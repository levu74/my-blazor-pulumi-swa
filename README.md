# Fast Static Web App deployment using Pulumi

This repository contains a sample Pulumi project that demonstrates how to deploy a static web app using Azure Static Web Apps. The project is designed to be simple and easy to understand, making it a great starting point for anyone looking to learn about deploying static web apps with Pulumi.

## Prerequisites

- .NET SDK 9.0.200 or compatible
- Azure subscription
- Pulumi CLI


### Install Pulumi

Make sure you have Pulumi installed. You can install it by following the instructions in the [Pulumi installation guide](https://www.pulumi.com/docs/get-started/install/).


## Project Setup

### Infrastructure



### Static Web App using Blazor

This project uses a Blazor WebAssembly app as the static web app. The Blazor app is built using the .NET CLI and is deployed to Azure Static Web Apps.

Location: `src/BlazorApp.slnx`

#### Run the Blazor app locally

```bash
cd src/Blazor.WebAsm.Demo

dotnet run
```

#### Publish the Blazor app

We need to publish the Blazor app before deploying it to Azure Static Web Apps. The publish command will create a `publish` folder containing the static files needed for deployment.


```bash
cd src/Blazor.WebAsm.Demo

dotnet publish -c Release -o ./publish
```