#!/bin/bash

set -e

# Check if starship is already installed
if command -v starship &> /dev/null; then
    echo "Starship is already installed."
else
    echo "Installing Starship..."
    curl -sS https://starship.rs/install.sh | sh
fi


# starship config file
mkdir -p ~/.config/starship

# copy starship config located in same directory as this script
cp "$(dirname "$0")/starship.toml" ~/.config/starship/starship.toml

# check if notfound in bashrc then add it
if ! grep -q 'export STARSHIP_CONFIG' ~/.bashrc; then
    echo "Adding STARSHIP_CONFIG to .bashrc..."
    echo 'export STARSHIP_CONFIG="$HOME/.config/starship/starship.toml"' >> ~/.bashrc
else
    echo "STARSHIP_CONFIG already exists in .bashrc"
fi

# check if not found in bashrc then add it
if ! grep -q 'eval "$(starship init bash)"' ~/.bashrc; then
    echo "Adding starship init to .bashrc..."
    # shellcheck disable=SC2016
    echo 'eval "$(starship init bash)"' >> ~/.bashrc
else
    echo "starship init already exists in .bashrc"
fi
