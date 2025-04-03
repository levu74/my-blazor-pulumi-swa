#!/bin/bash

# install nvm

# Check if nvm is already installed
if [ -d "$HOME/.nvm" ]; then
    echo "nvm is already installed."
else
    curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.2/install.sh | bash
    export NVM_DIR="$HOME/.nvm"
    [ -s "$NVM_DIR/nvm.sh" ] && . "$NVM_DIR/nvm.sh" # This loads nvm
    [ -s "$NVM_DIR/bash_completion" ] && . "$NVM_DIR/bash_completion" # This loads nvm bash_completion
fi

# install node
install_node() {
    if ! command -v node &> /dev/null; then
        echo "Installing Node.js..."
        nvm install --lts
    else
        echo "Node.js is already installed."
    fi
}

# install pnpm
install_pnpm() {
    if ! command -v pnpm &> /dev/null; then
        echo "Installing pnpm..."
        npm install -g pnpm

        # Run pnpm setup to configure environment
        echo "Setting up pnpm environment..."
        pnpm setup
    else
        echo "pnpm is already installed."
    fi

    # Ensure PNPM_HOME is set correctly in the current session
    export PNPM_HOME="$HOME/.local/share/pnpm"
    export PATH="$PNPM_HOME:$PATH"

    # Add to shell profile for persistence
    grep -q "PNPM_HOME" ~/.bashrc || {
        echo 'export PNPM_HOME="$HOME/.local/share/pnpm"' >> ~/.bashrc
        echo 'export PATH="$PNPM_HOME:$PATH"' >> ~/.bashrc
    }
}

# install azure static web apps cli
install_swa() {
    if ! command -v swa &> /dev/null; then
        echo "Installing Azure Static Web Apps CLI..."
        pnpm install -g @azure/static-web-apps-cli
    else
        echo "Azure Static Web Apps CLI is already installed."
    fi
}

# install node and pnpm
install_node
install_pnpm
install_swa
