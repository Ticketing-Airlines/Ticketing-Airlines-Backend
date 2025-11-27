# Airline1 API Documentation

Base URL
- Typically: `https://{host}/api` (controller routes use `api/[controller]` unless otherwise noted).

Response format
- Endpoints generally return standard IActionResult values: `Ok(...)`, `CreatedAtAction(...)`, `NoContent()`, `NotFound()`, `BadRequest(...)`, `Unauthorized(...)`.

Common DTO folders
- Request DTOs: `Airline1/Dtos/Requests`
- Response DTOs: `Airline1/Dtos/Responses`

Controllers & endpoints (overview)

- Airports (`AirportsController`)
  - `GET /api/airports` — Get all airports. Response: `List<AirportResponse>`
  - `GET /api/airports/{id}` — Get airport by id. Response: `AirportResponse` or `404` if not found.
  - `POST /api/airports` — Create new airport. Request: `CreateAirportRequest`. Response: `201 Created` with `AirportResponse`.
  - `PUT /api/airports/{id}` — Update airport. Request: `UpdateAirportRequest`. Response: `AirportResponse` or `404`.
  - `DELETE /api/airports/{id}` — Delete airport. Response: `204` on success or `404`.

- Auth (`AuthController`)
  - `POST /api/auth/login` — Authenticate. Request: `LoginRequest`. Response: authentication payload (token/session info) or `401`.
  - `POST /api/auth/logout` — Logout. Request: header `sessionToken` (string). Response: `200` or `400`.
  - `POST /api/auth/forgot-password` — Request password reset. Request: `ForgotPasswordRequest`. Response: `200` or `404`.
  - `POST /api/auth/reset-password` — Reset password. Request: `ResetPasswordRequest`. Response: `200` or `400`.

- Users (`UsersController`)
  - `POST /api/users/register` — Register user. Request: `RegisterRequest`/`CreateUserRequest` (check which DTO is used). Response: user object or `Ok`.
  - `POST /api/users/login` — Login (alternate to `AuthController` login). Request: `LoginUserRequest`. Response: user/session info or `401`.
  - `GET /api/users/{id}` — Get user by id.
  - `GET /api/users` — Get all users.
  - `PUT /api/users/{id}` — Update user. Request: `UpdateUserRequest`.
  - `DELETE /api/users/{id}` — Delete user.

- Bookings (`BookingsController`)
  - `POST /api/bookings` — Create booking. Request: `CreateBookingRequest`. Response: `CreatedAtAction` with `BookingResponse` or `400`.
  - `GET /api/bookings/{id}` — Get booking by id.
  - `PUT /api/bookings/{id}` — Update booking. Request: `UpdateBookingRequest`.
  - `GET /api/bookings/code/{code}` — Get booking by booking code.
  - `PUT /api/bookings/{id}/cancel` — Cancel booking. Response: `204` or `404`.
  - `GET /api/bookings/flight/{flightId}` — Get bookings for a flight.

- Flights (`FlightsController`)
  - `GET /api/flights` — List flights.
  - `GET /api/flights/{id}` — Get flight by id.
  - `POST /api/flights` — Create flight. Request: `CreateFlightRequest`.
  - `PUT /api/flights/{id}` — Update flight. Request: `UpdateFlightRequest`.
  - `DELETE /api/flights/{id}` — Delete flight.

- Flight Routes (`FlightRoutesController`)
  - `GET /api/flightroutes` — Get all flight routes.
  - `GET /api/flightroutes/{id}` — Get route by id.
  - `POST /api/flightroutes` — Create route. Request: `CreateFlightRouteRequest`.
  - `PUT /api/flightroutes/{id}` — Update route. Request: `UpdateFlightRouteRequest`.
  - `DELETE /api/flightroutes/{id}` — Delete route.

- Flight Prices (`FlightPriceController`)
  - `GET /api/flightprice` — List flight prices.
  - `POST /api/flightprice` — Create pricing. Request: `CreateFlightPriceRequest`.

- Flight Seats (`FlightSeatsController`)
  - `GET /api/flightseats/flight/{flightId}` — Get seats for a flight.
  - Other seat-related operations may exist (reserve, block, assign) — check `FlightSeatsController` and related request DTOs.

- Flight Add‑Ons (`FlightAddOnController`)
  - `GET /api/flightaddon` — Get all flight add-ons.
  - `GET /api/flightaddon/category/{category}` — Filter by `AddOnCategory`.
  - `GET /api/flightaddon/{id}` — Get add-on by id.
  - `POST /api/flightaddon` — Create add-on. Request: `CreateFlightAddOnRequest`.
  - `PUT /api/flightaddon/{id}` — Update add-on. Request: `UpdateFlightAddOnRequest`.
  - `DELETE /api/flightaddon/{id}` — Delete add-on.

- Flight Bundles, Flight Status, Flight Status Reasons
  - `FlightBundleController`, `FlightStatusController`, `FlightStatusReasonsController` expose CRUD endpoints and status reason management. See controllers for exact routes and DTOs: `CreateFlightBundleRequest`, `CreateFlightStatusRequest`, `FlightStatusReasonCreateRequest`, etc.

- Passengers (`PassengersController`)
  - `GET /api/passengers` — Get all passengers.
  - `GET /api/passengers/{id}` — Get passenger by id.
  - `POST /api/passengers` — Create passenger. Request: `CreatePassengerRequest`.
  - `PUT /api/passengers/{id}` — Update passenger. Request: `CreatePassengerRequest` (the code reuses the same DTO for update).
  - `DELETE /api/passengers/{id}` — Delete passenger.

- Seats (`SeatController`)
  - `POST /api/seat` — Create seat. Request: `CreateSeatRequest`.
  - `GET /api/seat/{id}` — Get seat details.
  - `GET /api/seat/aircraft/{aircraftId}` — Get seats for an aircraft.
  - `PUT /api/seat/{id}` — Update seat. Request: `UpdateSeatRequest`.
  - `DELETE /api/seat/{id}` — Delete seat.

- Seating Provisioning (`SeatingProvisioningController`)
  - `POST /api/seating-provisioning/provision/{aircraftId}` — Provision seats for an aircraft.
  - `POST /api/seating-provisioning/regenerate/{aircraftId}` — Regenerate seats for an aircraft.

Examples

- Create a flight route (example)

  POST /api/flightroutes
  Request JSON (CreateFlightRouteRequest):
  {
    "code": "FR1",
    "originAirportId": 1,
    "destinationAirportId": 2,
    "distanceKm": 1200,
    "averageFlightTimeMinutes": 150,
    "frequencyWeekly": 7,
    "isActive": true
  }

  Success response: `201 Created` with a `FlightRouteResponse` JSON body.

- Login (example)

  POST /api/auth/login
  Request JSON (LoginRequest):
  {
    "email": "user@example.com",
    "password": "p@ssw0rd"
  }

  Success response: `200 OK` with token/session payload (implementation-specific).

