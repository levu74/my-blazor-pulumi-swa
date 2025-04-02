# Base image: .NET SDK 9.0
# This provides the .NET runtime and SDK tools for building and running .NET applications
FROM mcr.microsoft.com/dotnet/sdk:9.0



# Install development tools and utilities
# - Updates package lists with apt-get update
# - Sets DEBIAN_FRONTEND to noninteractive to avoid prompts during installation
# - Installs:
#   - git: version control
#   - curl & wget: tools for transferring data
#   - vim: text editor
#   - unzip: for extracting zip archives
#   - jq: command-line JSON processor
# - Cleans up apt cache to reduce image size
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install git curl wget vim unzip jq \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/*

# Set up non-root user for improved security
# This creates a user called 'vscode' that can be used instead of root
# Args allow these values to be customized at build time
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID

# Create the user and group
# - Creates a new group with specified GID
# - Creates a new user with specified UID and adds to the group
# - Installs sudo to allow privileged operations
# - Adds the user to sudoers list without password requirement
# - Sets proper permissions on the sudoers file
RUN groupadd --gid $USER_GID $USERNAME \
    && useradd --uid $USER_UID --gid $USER_GID -m $USERNAME \
    && apt-get update \
    && apt-get install -y sudo \
    && echo $USERNAME ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/$USERNAME \
    && chmod 0440 /etc/sudoers.d/$USERNAME

# Set the default user to the non-root user we created
# This ensures that container processes run as this user instead of root
USER $USERNAME
