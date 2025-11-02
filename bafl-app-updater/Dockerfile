# Use Python 3.11 slim image
FROM python:3.11-slim

# Set working directory
WORKDIR /app

# Set environment variables
ENV PYTHONUNBUFFERED=1 \
    PYTHONDONTWRITEBYTECODE=1

# Copy requirements first for better caching
COPY requirements.txt .

# Install system dependencies and Python packages
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* && \
    pip install --no-cache-dir -r requirements.txt

# Copy application files
COPY baflcomp_editor.py .
COPY azure_blob_service.py .

# Expose Streamlit default port
EXPOSE 8505

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8505/_stcore/health || exit 1

# Run streamlit
CMD ["streamlit", "run", "baflcomp_editor.py", "--server.port=8505", "--server.address=0.0.0.0"]
