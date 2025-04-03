# Deploy to Azure Static Web Apps

## Manual Deployment using swa CLI

**Prerequisites**

- Node and npm/pnpm installed
- Azure CLI installed


**Install swa cli**

```bash
npm install -g @azure/static-web-apps-cli
```

**Login to Azure**

```bash
az login
```

**Deploy to Azure Static Web Apps**

Configure deployment token

```bash
SWA_CLI_DEPLOYMENT_TOKEN=1234567890

```

Deploy using `swa-cli.config.json`

```bash
swa deploy <config-name>
```
