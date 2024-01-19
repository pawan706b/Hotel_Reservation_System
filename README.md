### README.md for Hosthive - Hotel Reservation System

---

# Hosthive: Hotel Reservation System

Hosthive is a comprehensive hotel reservation system built using ASP.NET MVC and Entity Framework 7.0.14. The system is designed to facilitate users in booking hotel rooms efficiently while offering hotel managers a robust platform for managing reservations, rooms, and user data.

## Features

- **User Management**: Supports different user roles including regular users and managers.
- **Hotel and Room Views**: Detailed views for each hotel and its available rooms.
- **Reservation Management**: Complete CRUD operations for reservations.
- **Dynamic Room Filtering**: Users can filter rooms based on check-in and check-out dates.
- **Billing Management**: Handle billing related to reservations.
- **Session Management**: For maintaining user sessions with a timeout feature.
- **Authentication**: Secure login system for users and managers.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine.

### Prerequisites

- .NET 7.0.14
- MySQL Server (Refer to connection string configuration)

### Installation

1. **Clone the Repository**
   ```
   git clone https://github.com/pawan706b/Hotel_Reservation_System.git
   ```
2. **Set up the Database Connection**
   - Update the connection string in `appsettings.json` with your database credentials:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "server=your_server;database=your_database;user=your_username;password=your_password;"
     }
     ```
3. **Run Migrations**
   - Ensure that Entity Framework Core tools are installed.
   - Run the following command in the project directory:
     ```
     dotnet ef database update
     ```
4. **Run the Application**
   - Execute the following command:
     ```
     dotnet run
     ```

## Usage

Navigate to `http://localhost:5234/` where `port` is the port number specified in your project settings. You will be presented with the home page where you can navigate to different sections of the application.

## Authors

- **Pawan Kumar** - [Github](https://github.com/pawan706b)
