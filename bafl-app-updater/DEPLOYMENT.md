# BAFL Editor Deployment Guide

## Deploying to Target Machine (NVIDIA Spark)

### Prerequisites on Target Machine
- Docker installed
- Docker Compose installed
- Azure CLI installed
- Internet connection

### Deployment Steps

1. **Copy files to target machine:**
   ```bash
   scp docker-compose.prod.yml scripts/deploy.sh .env user@target-machine:/path/to/deployment/
   ```

2. **SSH into target machine:**
   ```bash
   ssh user@target-machine
   cd /path/to/deployment/
   ```

3. **Run deployment script:**
   ```bash
   chmod +x deploy.sh
   ./deploy.sh
   ```

### What the deployment script does:
1. ✅ Validates environment variables in `.env` file
2. ✅ Checks for required tools (Docker, Docker Compose, Azure CLI)
3. ✅ Logs into Azure and ACR
4. ✅ Pulls the latest image from `xylasoft.azurecr.io`
5. ✅ Stops any existing container
6. ✅ Starts the new container
7. ✅ Shows container status

### Manual deployment (alternative):
```bash
# Login to ACR
az acr login --name xylasoft

# Pull image
docker pull xylasoft.azurecr.io/bafl-editor:latest

# Start container
docker-compose -f docker-compose.prod.yml up -d
```

### Useful commands:
```bash
# View logs
docker-compose -f docker-compose.prod.yml logs -f

# Stop container
docker-compose -f docker-compose.prod.yml down

# Restart container
docker-compose -f docker-compose.prod.yml restart

# Check status
docker-compose -f docker-compose.prod.yml ps
```

### Environment Variables Required
Make sure your `.env` file on the target machine contains:
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_RESOURCE_GROUP`
- `AZURE_STORAGE_ACCOUNT`
- `AZURE_CONTAINER_NAME`
- `AZURE_CHEER_BLOB_NAME`
- `AZURE_DRILL_BLOB_NAME`
- `AZURE_STORAGE_CONNECTION_STRING`
- `APP_USERNAME`
- `APP_PASSWORD`
- `AZURE_ACR_REGISTRY_NAME`
- `AZURE_ACR_RESOURCE_GROUP`

### Accessing the Application
Once deployed, access the application at:
```
http://localhost:8505
```

Or from another machine (if firewall allows):
```
http://<target-machine-ip>:8505
```
