#!/bin/bash

# Load environment variables from .env file
if [ -f .env ]; then
    export $(cat .env | grep -v '^#' | xargs)
else
    echo "Error: .env file not found"
    exit 1
fi

# Set ACR variables from environment
REGISTRY_NAME="${AZURE_ACR_REGISTRY_NAME}"
ACR_RESOURCE_GROUP="${AZURE_ACR_RESOURCE_GROUP}"
ACR_LOGIN_SERVER="${REGISTRY_NAME}.azurecr.io"
IMAGE_NAME="bafl-editor"
IMAGE_TAG="latest"

echo "=== Pushing to Azure Container Registry ==="
echo "Registry: ${ACR_LOGIN_SERVER}"
echo "Image: ${IMAGE_NAME}:${IMAGE_TAG}"
echo "ACR Resource Group: ${ACR_RESOURCE_GROUP}"
echo "Subscription: ${AZURE_SUBSCRIPTION_ID}"
echo ""

# Login to Azure (if not already logged in)
echo "Logging in to Azure..."
az account set --subscription "${AZURE_SUBSCRIPTION_ID}"

# Login to ACR
echo "Logging in to Azure Container Registry..."
az acr login --name "${REGISTRY_NAME}"

# Build the image with ACR tag
echo "Building Docker image..."
docker build -t "${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${IMAGE_TAG}" .

# Push to ACR
echo "Pushing image to ACR..."
docker push "${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${IMAGE_TAG}"

echo ""
echo "=== Push Complete ==="
echo "Image available at: ${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${IMAGE_TAG}"
echo ""
echo "To pull on target machine:"
echo "  az acr login --name ${REGISTRY_NAME}"
echo "  docker pull ${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${IMAGE_TAG}"
