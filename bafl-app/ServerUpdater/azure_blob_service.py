"""
Azure Blob Storage Service
Handles reading and writing to Azure Blob Storage
"""

from azure.storage.blob import BlobServiceClient
from azure.identity import DefaultAzureCredential
import os


class AzureBlobService:
    """Service class for Azure Blob Storage operations"""
    
    def __init__(self, storage_account_name, container_name):
        """
        Initialize the Azure Blob Service
        
        Args:
            storage_account_name: Name of the Azure Storage Account
            container_name: Name of the blob container
        """
        self.storage_account_name = storage_account_name
        self.container_name = container_name
        self.blob_service_client = None
        self._authenticate()
    
    def _authenticate(self):
        """Authenticate with Azure using DefaultAzureCredential"""
        try:
            # Try connection string first (if set in environment)
            connection_string = os.getenv('AZURE_STORAGE_CONNECTION_STRING')
            if connection_string:
                self.blob_service_client = BlobServiceClient.from_connection_string(
                    connection_string
                )
            else:
                # Use DefaultAzureCredential (Azure CLI, Managed Identity, etc.)
                account_url = f"https://{self.storage_account_name}.blob.core.windows.net"
                credential = DefaultAzureCredential()
                self.blob_service_client = BlobServiceClient(
                    account_url=account_url,
                    credential=credential
                )
        except Exception as e:
            raise Exception(f"Failed to authenticate with Azure: {str(e)}")
    
    def read_blob(self, blob_name):
        """
        Read content from a blob
        
        Args:
            blob_name: Name of the blob to read
            
        Returns:
            str: Content of the blob
        """
        try:
            blob_client = self.blob_service_client.get_blob_client(
                container=self.container_name,
                blob=blob_name
            )
            download_stream = blob_client.download_blob()
            return download_stream.readall().decode('utf-8')
        except Exception as e:
            raise Exception(f"Failed to read blob '{blob_name}': {str(e)}")
    
    def write_blob(self, blob_name, content):
        """
        Write content to a blob
        
        Args:
            blob_name: Name of the blob to write
            content: Content to write (string)
        """
        try:
            blob_client = self.blob_service_client.get_blob_client(
                container=self.container_name,
                blob=blob_name
            )
            blob_client.upload_blob(
                content,
                overwrite=True,
                content_type='application/json'
            )
        except Exception as e:
            raise Exception(f"Failed to write blob '{blob_name}': {str(e)}")
    
    def list_blobs(self):
        """
        List all blobs in the container
        
        Returns:
            list: List of blob names
        """
        try:
            container_client = self.blob_service_client.get_container_client(
                self.container_name
            )
            return [blob.name for blob in container_client.list_blobs()]
        except Exception as e:
            raise Exception(f"Failed to list blobs: {str(e)}")
