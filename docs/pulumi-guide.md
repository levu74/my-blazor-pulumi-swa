# Pulumi guide

## Create a new project

**Create a new Pulumi project:**

  ```bash
  pulumi new <template>
  ```

  Example

  ```bash
  pulumi new azure-csharp
  ```

## Working with Stacks

**Create different stacks using the Pulumi CLI:**

  ```bash
  pulumi stack init dev
  pulumi stack init staging
  pulumi stack init prod
  ```

**Select a stack:**

  ```bash
  pulumi stack select dev
  ```

**List all stacks:**

  ```bash
  pulumi stack ls
  ```

**Get the current stack:**

  ```bash
  pulumi stack
  ```


## Deploying Changes

**Preview changes to the current stack:**

  ```bash
  pulumi preview
  ```

**Deploy changes to the current stack:**

  ```bash
  pulumi up
  ```


## Destroying Resources

**Destroy the current stack:**

  ```bash
  pulumi destroy
  ```
