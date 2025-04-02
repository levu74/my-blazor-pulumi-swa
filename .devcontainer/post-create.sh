#!/bin/bash

set -e

# Install additional global tools
dotnet tool install -g dotnet-ef
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
dotnet tool install -g Microsoft.Tye --version "0.11.0-*"


# Ensure Pulumi is in PATH
if ! command -v pulumi &> /dev/null; then
    echo "Pulumi not found in PATH, adding it..."
    # Assuming pulumi is installed in standard location by the feature
    export PATH=$PATH:$HOME/.pulumi/bin
    echo 'export PATH=$PATH:$HOME/.pulumi/bin' >> ~/.bashrc
fi

# Ensure powershell is in PATH
if ! command -v pwsh &> /dev/null; then
    echo "PowerShell not found in PATH, adding it..."
    # Assuming pwsh is installed in standard location by the feature
    export PATH=$PATH:/usr/bin/pwsh
    echo 'export PATH=$PATH:/usr/bin/pwsh' >> ~/.bashrc
fi

# Install Pulumi plugins for Azure (if not already installed via feature)
echo "Installing Pulumi plugins..."
pulumi plugin install resource azure-native 1.56.0
pulumi plugin install resource random 4.3.1

# Print versions for verification
echo "=== Environment Information ==="
dotnet --version
echo "Azure CLI $(az --version | head -n 1)"
pulumi version
echo "==========================="


# Create local SSL certificate for development
dotnet dev-certs https --trust
