# Social Consultations Platform - Backend

This repository contains the backend implementation of a web platform and mobile application designed to facilitate and manage public consultations. The platform aims to provide an intuitive, efficient, and accessible way for users to participate in social consultations, collect feedback, and support decision-making processes.

## Table of Contents
- [Project Overview](#project-overview)
- [Technologies Used](#technologies-used)
- [Features](#features)
- [Setup and Installation](#setup-and-installation)
- [API Documentation](#api-documentation)
- [License](#license)

## Project Overview

The Social Consultations Platform is a collaborative project developed by a team of three, with each member responsible for a specific component:
- **Backend**: Developed by Piotr Mazur(https://github.com/sirmazur), responsible for server-side logic, API creation, database management, and deployment on Azure.
- **Frontend**: Developed by Bartosz Spiżarny(https://github.com/Bartolomeo26), responsible for the web interface using React.
- **Mobile App**: Developed by Paweł Rudnik(https://github.com/pabl014), responsible for the iOS application using SwiftUI.

This repository focuses on the backend component, which is built using .NET 8 and hosted on Azure. The backend provides the necessary APIs for the frontend and mobile app to interact with the platform, manage user authentication, and handle data related to social consultations.

## Technologies Used

- **Backend Framework**: .NET 8
- **Database**: Azure SQL Database
- **Hosting**: Azure App Service
- **ORM**: Entity Framework Core
- **API Documentation**: Swagger
- **Caching**: ETag-based caching
- **Data Shaping**: Dynamic data shaping for efficient data transfer
- **Authentication**: JWT (JSON Web Tokens)

## Features

- **User Authentication**: Secure user registration, login, and password recovery.
- **Community Management**: Create, manage, and join social communities.
- **Consultation Management**: Create, manage, and participate in social consultations.
- **Data Shaping**: Efficient data transfer by dynamically selecting required fields.
- **Caching**: Improved performance with ETag-based caching.
- **Pagination**: Efficient data retrieval with paginated API responses.
- **Data Retention**: Automated cleanup of unused files and data.

## Setup and Installation

### Prerequisites
- .NET 8 SDK
- Azure SQL Database
- Azure App Service (optional for local development)
- Visual Studio or Visual Studio Code

### Steps
1. **Clone the Repository**:

2.  **Set Up the Database**:
    
    -   Create an Azure SQL Database or use a local SQL Server instance.
        
    -   Update the connection string in  `appsettings.json`  or use environment variables.
        
3.  **Run Migrations**:
      
4.  **Run the Application**:
    
5.  **Access the API**:
    
    -   The API will be available at  `https://localhost:7150`  (or the configured port).
        
    -   Swagger documentation can be accessed at  `https://localhost:7150/swagger`.
        

## API Documentation

The API is documented using Swagger. You can access the Swagger UI by running the application and navigating to the  `/swagger`  endpoint. The API supports the following key endpoints:

-   **Authentication**:  `/api/auth`
    
-   **Communities**:  `/api/communities`
    
-   **Consultations**:  `/api/consultations`
    
-   **Users**:  `/api/users`
    

Each endpoint is documented with details on request/response formats, required parameters, and example payloads.
 
