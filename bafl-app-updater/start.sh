#!/bin/bash

# BAFL Competition Editor - Quick Start Script

set -e

echo "🏆 BAFL Competition Editor - Docker Setup"
echo "=========================================="
echo ""

# Check if .env exists
if [ ! -f .env ]; then
    echo "⚠️  No .env file found!"
    echo "📋 Copying .env.example to .env..."
    cp .env.example .env
    echo ""
    echo "✏️  Please edit .env file with your configuration before continuing."
    echo "   Required fields:"
    echo "   - AZURE_SUBSCRIPTION_ID"
    echo "   - AZURE_RESOURCE_GROUP"
    echo "   - AZURE_STORAGE_ACCOUNT"
    echo "   - AZURE_CONTAINER_NAME"
    echo "   - APP_USERNAME"
    echo "   - APP_PASSWORD"
    echo ""
    read -p "Press Enter after you've configured .env..."
fi

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker and try again."
    exit 1
fi

echo "🔨 Building Docker image..."
docker-compose build

echo ""
echo "🚀 Starting application..."
docker-compose up -d

echo ""
echo "✅ BAFL Competition Editor is starting!"
echo ""
echo "📍 Access the application at: http://localhost:8505"
echo ""
echo "Useful commands:"
echo "  View logs:        docker-compose logs -f"
echo "  Stop application: docker-compose down"
echo "  Restart:          docker-compose restart"
echo ""
