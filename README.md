# File-Detection-Message-Broker-in-CQRS-and-MassTransit
# Document Monitoring Service

## Overview

This project is a **Document Monitoring Service** built with **C#** and **.NET 8**, utilizing **RabbitMQ**, **MassTransit**, and **PostgreSQL**. The service monitors a specified folder for new files, processes them by converting the file to Base64, and stores essential information (GUID, file name, Base64 content, timestamp) into a PostgreSQL database. Upon successful insertion, a message is generated via RabbitMQ with the file details.

The system also provides a controller endpoint to download the inserted files by converting the stored Base64 content back into a PDF.

## Key Features

- Monitors a specified folder for new files.
- Detects and processes newly added files.
- Generates a unique **GUID** and a **Correlation ID** for each file.
- Converts the file content to **Base64**.
- Inserts file information (GUID, Base64, filename, timestamp) into **PostgreSQL**.
- Provides an API endpoint to download the stored files.
- Publishes a **DocumentReceivedMessage** to **RabbitMQ** after successful file insertion.
- Uses **Entity Framework Core** for database interaction with a **code-first approach**.
- Implements **CQRS pattern** for separating commands and queries.

## Technologies Used

- **.NET 8**
- **RabbitMQ** for message broker communication
- **MassTransit** for managing message contracts and queues
- **PostgreSQL** as the relational database
- **Entity Framework Core** for ORM with a code-first approach
- **CQRS pattern** for command-query separation
- **Base64** encoding for file storage and retrieval

## Message Structure

When a new file is detected and processed, the following message is generated and published to RabbitMQ:

### `DocumentReceivedMessage`

```json
Context
    {
  "TimeStamp": "DateTime",
  "CorrelationId": "GUID",
  "SourceServiceName": "Document Monitoring Service",
  "Domain": "Your domain",
  "CompanyName": "Your company"
    }
ReceivedMessage
    {
  "DocumentId": "GUID",
  "Source": "File path or description",
  "DocumentSrc": "Download Endpoint",
    }


