#!/bin/bash

# Deployment script for target machine
# This script should be run on the machine where you want to deploy the container

set -e  # Exit on error

echo "=== BAFL Editor Deployment Script ==="
echo ""

# Check if .env file exists
if [ ! -f .env ]; then
    echo "Error: .env file not found!"
    echo "Please create a .env file with the following variables:"
    echo "  AZURE_SUBSCRIPTION_ID"
    echo "  AZURE_RESOURCE_GROUP"
    echo "  AZURE_STORAGE_ACCOUNT"
    echo "  AZURE_CONTAINER_NAME"
    echo "  AZURE_CHEER_BLOB_NAME"
    echo "  AZURE_DRILL_BLOB_NAME"
    echo "  AZURE_STORAGE_CONNECTION_STRING"
    echo "  APP_USERNAME"
    echo "  APP_PASSWORD"
    echo "  AZURE_ACR_REGISTRY_NAME"
    echo "  AZURE_ACR_RESOURCE_GROUP"
    exit 1
fi

# Load environment variables
export $(cat .env | grep -v '^#' | xargs)

REGISTRY_NAME="${AZURE_ACR_REGISTRY_NAME}"
ACR_LOGIN_SERVER="${REGISTRY_NAME}.azurecr.io"
IMAGE_NAME="bafl-editor"
IMAGE_TAG="latest"

echo "Registry: ${ACR_LOGIN_SERVER}"
echo "Image: ${IMAGE_NAME}:${IMAGE_TAG}"
echo ""

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo "Error: Azure CLI is not installed"
    echo "Install it from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "Error: Docker is not installed"
    echo "Install it from: https://docs.docker.com/get-docker/"
    exit 1
fi

# Check if docker-compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "Error: docker-compose is not installed"
    echo "Install it from: https://docs.docker.com/compose/install/"
    exit 1
fi

# Login to Azure
echo "Logging in to Azure..."
az account set --subscription "${AZURE_SUBSCRIPTION_ID}"

# Login to ACR
echo "Logging in to Azure Container Registry..."
az acr login --name "${REGISTRY_NAME}"

# Pull the latest image
echo "Pulling latest image from ACR..."
docker pull "${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${IMAGE_TAG}"

# Stop existing container if running
echo "Stopping existing container (if any)..."
docker-compose -f docker-compose.prod.yml down 2>/dev/null || true

# Start the container
echo "Starting container..."
docker-compose -f docker-compose.prod.yml up -d

# Wait a moment for container to start
sleep 3

# Check container status
echo ""
echo "=== Deployment Complete ==="
echo ""
docker-compose -f docker-compose.prod.yml ps
echo ""
echo "Application should be available at: http://localhost:8505"
echo ""
echo "To view logs:"
echo "  docker-compose -f docker-compose.prod.yml logs -f"
echo ""
echo "To stop:"
echo "  docker-compose -f docker-compose.prod.yml down"
