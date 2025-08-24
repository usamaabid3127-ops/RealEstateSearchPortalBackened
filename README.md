\# Real Estate Portal API



A .NET Core Web API for a real estate portal where users can search properties, save favorites, and view saved listings.



\## Features



\- JWT Authentication

\- Property Search and Filtering

\- Favorite Properties Management

\- User Registration and Login

\- Entity Framework Core with SQL Server



\## Technologies



\- .NET 8.0

\- Entity Framework Core

\- SQL Server

\- JWT Authentication

\- AutoMapper

\- Swagger/OpenAPI



\## Setup



1\. Clone the repository

2\. Restore packages: `dotnet restore`

3\. Update database:

&nbsp;  - `dotnet ef migrations add InitialMigration`

&nbsp;  - `dotnet ef database update`

4\. Set up JWT secret in appsettings.json or user secrets

5\. Run the application: `dotnet run`



\## API Endpoints



\- `POST /auth/register` - User registration

\- `POST /auth/login` - User login

\- `GET /properties` - Get properties with filtering

\- `GET /properties/{id}` - Get property details

\- `POST /favorites/{propertyId}` - Toggle favorite property

\- `GET /favorites` - Get user's favorite properties



\## Database Schema



\- Users: ID, Email, PasswordHash

\- Properties: ID, Title, Price, Address, Type, Bedrooms, Bathrooms, etc.

\- Favorites: UserID, PropertyID, SavedAt

