# BAFL Competition Editor

A Streamlit web application for managing CheerComp.json and DrillComp.json files stored in Azure Blob Storage.

## Features

- 🔐 Password-protected access
- 🏆 Switch between Cheer and Drill competitions
- 📋 Edit event overview fields (Name, Date, Message, etc.)
- 📅 Manage schedule items in an interactive table
- ➕ Add new schedule entries
- 🗑️ Delete schedule entries
- 💾 Read from and write to Azure Blob Storage
- 🌐 Cross-platform web interface

## Setup

### 1. Install Dependencies

```bash
pip3 install -r requirements.txt
```

### 2. Configure Environment Variables

Copy the example environment file and fill in your Azure configuration:

```bash
cp .env.example .env
```

Edit `.env` and update with your Azure configuration:
```
AZURE_SUBSCRIPTION_ID=your-subscription-id
AZURE_RESOURCE_GROUP=your-resource-group
AZURE_STORAGE_ACCOUNT=your-storage-account
AZURE_CONTAINER_NAME=your-container-name
AZURE_CHEER_BLOB_NAME=CheerComp.json
AZURE_DRILL_BLOB_NAME=DrillComp.json

# Authentication
APP_USERNAME=admin
APP_PASSWORD=your-secure-password-here
```

**Important:** Change the default username and password to secure values!

**Note:** The `.env` file is git-ignored to keep credentials secure.

### 3. Azure Authentication

The application uses Azure's `DefaultAzureCredential`, which automatically tries multiple authentication methods in this order:

#### Option A: Azure CLI (Recommended for local development)
```bash
az login
az account set --subscription <your-subscription-id>
```

#### Option B: Environment Variable (Connection String)
Add the connection string to your `.env` file:

```
AZURE_STORAGE_CONNECTION_STRING=your-connection-string-here
```

To get your connection string:
```bash
az storage account show-connection-string \
  --name <your-storage-account> \
  --resource-group <your-resource-group> \
  --subscription <your-subscription-id>
```

#### Option C: Managed Identity
If running on Azure (VM, App Service, etc.), managed identity will be used automatically.

### 4. Verify Access

Ensure you have the necessary permissions on the storage account:
- **Storage Blob Data Contributor** role (for read/write)
- OR **Storage Blob Data Reader** role (for read-only)

Grant access if needed:
```bash
az role assignment create \
  --role "Storage Blob Data Contributor" \
  --assignee <your-email@domain.com> \
  --scope /subscriptions/<subscription-id>/resourceGroups/<resource-group>/providers/Microsoft.Storage/storageAccounts/<storage-account>
```

## Running the Application

### Option 1: Docker (Recommended)

#### Quick Start (One Command)
```bash
./start.sh
```

This script will:
- Check for `.env` configuration
- Build the Docker image
- Start the application
- Show you the access URL

#### Using Docker Compose (Easiest)
```bash
# Make sure your .env file is configured
docker-compose up -d
```

The application will be available at `http://localhost:8505`

To stop the application:
```bash
docker-compose down
```

To view logs:
```bash
docker-compose logs -f
```

#### Using Docker directly
```bash
# Build the image
docker build -t bafl-competition-editor .

# Run the container
docker run -d \
  --name bafl-editor \
  -p 8505:8505 \
  --env-file .env \
  bafl-competition-editor
```

### Option 2: Local Python

```bash
streamlit run baflcomp_editor.py
```

The application will open in your default browser at `http://localhost:8505`

## Usage

### Login
1. Navigate to `http://localhost:8505`
2. Enter the username and password configured in your `.env` file
3. Click **Login** to access the application

### Selecting Competition
1. Use the radio buttons in the sidebar to select either **Cheer** or **Drill** competition
2. The page title and icon will update accordingly
3. If you have unsaved changes, you'll be warned before switching

### Loading Data
1. Click **"🔄 Load from Azure"** to fetch the current competition file from blob storage
2. The data will populate in the form below

### Editing Overview Fields
1. Update any of the event overview fields (Name, Date, Message, etc.)
2. Click **"💾 Update Overview"** to apply changes to the in-memory data
3. Remember to save to Azure when done

### Managing Schedule Items
1. **View/Edit**: Click on any schedule item expander to edit its details
2. **Delete**: Click the **"🗑️ Delete"** button within an item to remove it
3. **Add New**: Fill out the "Add New Schedule Item" form and click **"➕ Add Item"**

### Saving Changes
1. After making all your changes, click **"💾 Save to Azure"** at the top
2. This will upload the updated JSON to Azure Blob Storage
3. A success message will confirm the save

### Logout
- Click the **"🚪 Logout"** button in the sidebar to end your session

## Troubleshooting

### Authentication Errors
- Make sure you're logged in with `az login`
- Verify you have the correct subscription selected
- Check that you have blob storage permissions

### Module Not Found
```bash
pip install -r requirements.txt
```

### Cannot Connect to Storage Account
- Verify the storage account name and container exist
- Check network connectivity
- Ensure firewall rules allow your IP address

## File Structure

```
bafl-app-updater/
├── baflcomp_editor.py       # Main Streamlit application
├── azure_blob_service.py    # Azure Blob Storage service class
├── requirements.txt         # Python dependencies
├── Dockerfile               # Docker container configuration
├── docker-compose.yml       # Docker Compose orchestration
├── .dockerignore            # Files to exclude from Docker build
├── .env                     # Azure configuration (git-ignored)
├── .env.example             # Environment variable template
├── .gitignore               # Git ignore rules
├── .flake8                  # Linting configuration
└── README.md               # This file
```

## Deployment

### Docker Deployment

The application is containerized and ready for deployment to any Docker-compatible platform:

- **Docker Hub/Registry**: Build and push the image to your registry
- **Azure Container Instances**: Deploy directly from Docker image
- **Azure App Service**: Use Docker deployment
- **Kubernetes**: Use the Docker image in your K8s cluster
- **Any VPS with Docker**: Simple `docker-compose up -d`

**Production Tips:**
- Always use strong passwords in production
- Consider using Azure Key Vault for secrets
- Use HTTPS/TLS in production environments
- Set up proper monitoring and logging

## Notes

- This application is separate from the main BAFL app and manages Azure blob updates
- Changes are only saved when you explicitly click "Save to Azure"
- The application keeps data in session state, so refreshing the page will lose unsaved changes
- All edits are validated as proper JSON before saving

## Location

This application is standalone and independent from the main BAFL app repository for version control and deployment purposes.
