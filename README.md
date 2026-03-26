# E-commerceProject

This project builds an e-commerce website based on .NET (C#, ASP.NET Core MVC) packaged with Docker. The interface uses a combination of HTML/CSS/JavaScript.

## 1. Key Features

- Product Management: CRUD, search, categories.
- User/Customer Management: Register, login, role-based access.
- Cart & Checkout: Add/remove products, place orders, confirmation.
- Order Management: History, status, update orders.
- Customer Page: Display products, search, filter, homepage...
- Admin Page: Manage products, users, orders, reports.
- Additional: Profile management, change password, pagination, notification.

## 2. Installation & Deployment Guide

### Requirements
- Visual Studio 2022+ (or .NET 6 SDK+)
- Docker (for containerization)
- SQL Server
- Git

### Local Installation Without Docker

1. Clone the repo:
    ```sh
    git clone https://github.com/manhtotbung/E-commerceProject.git
    cd E-commerceProject/WebBH
    ```
2. Open the solution `WebBHApplication.sln` with Visual Studio, restore NuGet packages.
3. Configure the `appsettings.json` file:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=YourDatabase;User Id=sa;Password=yourpassword;"
      }
    }
    ```
4. Initialize the database (using migration or manually).
5. Run the application (F5 in Visual Studio or `dotnet run` in `/WebBH`).

### Installation & Deployment with Docker

1. Build the image:
    ```sh
    docker build -t e-commerce:latest .
    ```
2. Run the container:
    ```sh
    docker run -e "ConnectionStrings__DefaultConnection=..." -p 8080:80 e-commerce:latest
    ```
3. Access: http://localhost:8080

### Important Configuration Variables

- **ConnectionStrings:DefaultConnection**: SQL Server connection string.
- **Logging**: Error logging.
- **Authentication/Authorization**: Security settings as required.

## 3. Contribution & Development

- Fork, create a branch, submit pull requests or issues.
- Read the code in each folder to understand the structure/data flow.