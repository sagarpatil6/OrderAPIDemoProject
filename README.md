# ASP.NET Core Web API: .NET 8:
# OrderAPIDemoProject
This is demo solution with 3 sub projects, mainly implementing Web API to provide Orders APIs and Worker Service to Process RabbitMQ Message Queue Notifications.

#### Source Code
https://github.com/sagarpatil6/OrderAPIDemoProject.git

## Prerequisites & Project Structure
Ensure you have the following installed:
.NET 8.0 SDK
A preferred IDE such as Visual Studio 2022 or Visual Studio Code
PostgreSQL v15
RabbitMQ v4.2.3


├──CommonObjects: Class Library in .NET8
├──OrderAPIDemoProject: WebAPI Project for REST APIs
├──NotificationService: Worker Service for Processing Notifications

```
├── src
│   ├── Controllers                     # Controllers
│   ├── Data          			# Data Access Layer
│   ├── Models                      	# DTOs.
│   ├── Services			# Contains the core business logic and domain models, view models, etc.
└── README.md                   	# Project documentation (you are here!)
```



## Getting Started

To use this project template, follow the steps below:

1. Ensure that you have the .NET 8 SDK installed on your machine.
2. Clone or download this repository to your local machine.
3. Open the solution in your preferred IDE (e.g., Visual Studio, Visual Studio Code).
4. Build the solution to restore NuGet packages and compile the code.
5. Configure the necessary database connection settings in the `appsettings.json` file of the Infrastructure project.
6. Open the Package Manager Console, run the `Update-Database` command to create the database.
7. Build NotificationService Project. We need to run this project directly thorugh Console for testing.(Not through Visual Studio)
8. Run the `OrderAPIDemoProject` project to access the Swagger UI.
9. Access [Rabbit MQ](http://localhost:15672/#/) to see Notification getting published by OrderService.
10. Access PGAdmin to see DB table changes. All these steps are to run this solution Manually. Docker Steps : In progress

## Configuration

1. PGSQL: database connection settings in the `appsettings.json`
2. RabbitMQ: Localhost and for Management UI access: http://localhost:15672/ (Username: guest and password: guest) 

## Data Formats and Sample Files

Order API Endpoints
Endpoint: POST /api/Order
Content-Type: application/json 


###
GET {{OrderAPIDemoProject_HostAddress}}/api/Order

###
GET {{OrderAPIDemoProject_HostAddress}}/api/Order/1
Accept: application/json

###
POST {{OrderAPIDemoProject_HostAddress}}/api/Order
Content-Type: application/json
{
  "customerEmail": "a@b.com",
  "productCode": "001",
  "quantity": 50,
}

## Error-Handling Strategy

1. Exception Handling: Handling most of the exceptions and translates them into appropriate HTTP 500 Internal Server Errors with a Problem Details payload.
2. Validation Errors: Model validation failures (e.g., invalid input formats) automatically return HTTP 400 Bad Request responses, detailing which fields are incorrect.

