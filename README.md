# B0626---AREEBA-SAGHIR---Innovaxel---Backend-Intern
# Event Registration System API

## Overview

The Event Registration System API is a RESTful application built with ASP.NET Core that allows users to create events, register participants, manage registrations, and track seat availability. The system ensures data consistency through validation rules, registration status tracking, and concurrency-safe operations.

## Features

### Event Management

* Create new events
* Unique event name validation
* Future event date validation
* Seat capacity management
* View all events
* Sort events by date
* Filter upcoming events

### Registration Management

* Register users for events
* Prevent duplicate registrations
* Prevent overbooking
* Store registration timestamps
* Cancel registrations
* Automatic seat restoration on cancellation
* Track registration status

### Data Persistence

* JSON-based storage
* Data retained between application runs
* No external database required

### Validation and Error Handling

* Proper input validation
* Meaningful error responses
* Event capacity enforcement
* Duplicate request protection
* Consistent seat count management

## Technology Stack

* ASP.NET Core Web API
* C#
* JSON File Storage

## Project Structure

```text
EventRegistrationApi/
│
├── Controllers/
│   ├── EventsController.cs
│   └── RegistrationsController.cs
│
├── Models/
│   ├── Event.cs
│   ├── Registration.cs
│   └── RegistrationStatus.cs
│
├── Services/
│   ├── EventService.cs
│   └── RegistrationService.cs
│
├── Data/
│   └── JsonDataStore.cs
│
├── DTOs/
│
├── Storage/
│   └── data.json
│
└── Program.cs
```

## API Endpoints

### Create Event

```http
POST /api/events
```

Request Body

```json
{
  "name": "Tech Conference 2026",
  "totalSeats": 100,
  "eventDate": "2026-12-20T10:00:00Z"
}
```

Response

```json
{
  "id": "event-id",
  "name": "Tech Conference 2026",
  "totalSeats": 100,
  "availableSeats": 100,
  "eventDate": "2026-12-20T10:00:00Z"
}
```

---

### Get All Events

```http
GET /api/events
```

Optional Query Parameters

```http
GET /api/events?upcomingOnly=true
GET /api/events?sort=asc
GET /api/events?sort=desc
```

---

### Register User

```http
POST /api/registrations
```

Request Body

```json
{
  "userName": "Ali",
  "eventId": "event-id"
}
```

Response

```json
{
  "registrationId": "registration-id",
  "userName": "Ali",
  "eventId": "event-id",
  "registeredAt": "2026-08-01T12:00:00Z"
}
```

---

### View Registrations

```http
GET /api/registrations
```

---

### Cancel Registration

```http
DELETE /api/registrations/{registrationId}
```

Response

```json
{
  "message": "Registration cancelled successfully."
}
```

## Validation Rules

### Event Creation

* Event name must be unique
* Total seats must be greater than zero
* Event date must be in the future

### Registration

* Event must exist
* Event must have available seats
* Same user cannot register twice for the same event
* Registration timestamp is automatically recorded

### Cancellation

* Registration must exist
* Registration must be active
* Seat count is automatically restored

## Concurrency Handling

The application uses synchronization mechanisms to ensure:

* No overbooking occurs
* Seat counts remain accurate
* Concurrent requests are handled safely
* Data integrity is maintained

## Error Responses

Example Error Response

```json
{
  "error": "Event not found."
}
```

Other possible errors include:

* Event already exists
* Event date must be in the future
* Total seats must be greater than zero
* No seats available
* User already registered
* Registration not found

## Running the Project

### Clone Repository

```bash
git clone <repository-url>
```

### Navigate to Project Directory

```bash
cd EventRegistrationApi
```

### Restore Packages

```bash
dotnet restore
```

### Run Application

```bash
dotnet run
```

### Open Swagger

```text
https://localhost:<port>/swagger
```

## Future Improvements

* Authentication and authorization
* Email notifications
* SQLite or SQL Server integration
* Event categories
* Pagination and searching
* Idempotency key support
* Dashboard and analytics

## License

This project is developed for educational and assessment purposes.
