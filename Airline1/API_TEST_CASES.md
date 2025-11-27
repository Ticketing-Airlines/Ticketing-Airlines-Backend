Airline1 API Test Cases


---------------------------------------------------------------------
Airports
---------------------------------------------------------------------

Test ID: AR-01
Title: Get all airports
Purpose: Validate the airports listing endpoint
Preconditions: Seed at least two airports in the DB
Test Steps:
	1. GET /api/airports
Request: none
Expected Status: 200 OK
Expected Response: JSON array; length >= 2; elements conform to `AirportResponse` properties
Postconditions / Cleanup: none
Notes: Response DTO: `Airline1/Dtos/Responses/AirportResponse`

Test ID: AR-02
Title: Get airport by id (exists)
Purpose: Retrieve a single airport
Preconditions: Airport with Id=1 exists
Test Steps:
	1. GET /api/airports/1
Request: none
Expected Status: 200 OK
Expected Response: JSON object with `id` == 1 and required fields populated
Postconditions / Cleanup: none

Test ID: AR-03
Title: Get airport by id (not found)
Purpose: Ensure NotFound returned for unknown id
Preconditions: No airport with Id=9999
Test Steps:
	1. GET /api/airports/9999
Request: none
Expected Status: 404 NotFound
Expected Response: error payload or empty body 

Test ID: AR-04
Title: Create airport (valid)
Purpose: Validate creating an airport
Preconditions: none
Test Steps:
	1. POST /api/airports with valid payload
Request:
	{
		"iataCode": "ABC",
		"icaoCode": "ABCD",
		"name": "Test Airport",
		"city": "City",
		"country": "Country",
		"timeZone": "TZ"
	}
Expected Status: 201 Created
Expected Response: JSON containing new `id`, same fields as request; `Location` header points to `GET /api/airports/{id}`
Postconditions / Cleanup: DELETE created airport

Test ID: AR-05
Title: Update airport (exists)
Purpose: Validate update behavior
Preconditions: Airport with Id=1 exists
Test Steps:
	1. PUT /api/airports/1 with { "name": "Updated Name" }
Request: { "name": "Updated Name" }
Expected Status: 200 OK
Expected Response: JSON where `name` == "Updated Name"
Postconditions / Cleanup: revert to original name if needed

Test ID: AR-06
Title: Delete airport
Purpose: Validate deletion
Preconditions: Airport with Id=2 exists
Test Steps:
	1. DELETE /api/airports/2
	2. GET /api/airports/2
Request: none
Expected Status: 204 NoContent then 404 NotFound
Postconditions / Cleanup: none

---------------------------------------------------------------------
Auth
---------------------------------------------------------------------

Test ID: AU-01
Title: Login (valid)
Purpose: Verify authentication returns token/session
Preconditions: User exists with given credentials
Test Steps:
	1. POST /api/auth/login with valid credentials
Request:
	{ "email": "user@example.com", "password": "pwd" }
Expected Status: 200 OK
Expected Response: JSON containing token/session payload
Postconditions / Cleanup: none

Test ID: AU-02
Title: Login (invalid)
Purpose: Ensure invalid credentials are rejected
Preconditions: none
Test Steps:
	1. POST /api/auth/login with wrong credentials
Request:
	{ "email": "bad@example.com", "password": "wrong" }
Expected Status: 401 Unauthorized
Expected Response: error message

Test ID: AU-03
Title: Logout (valid)
Purpose: Verify logout endpoint
Preconditions: Valid session token obtained from login
Test Steps:
	1. POST /api/auth/logout with header `sessionToken: <token>`
Request: header `sessionToken`
Expected Status: 200 OK
Expected Response: success message

---------------------------------------------------------------------
Users
---------------------------------------------------------------------

Test ID: US-01
Title: Register user
Purpose: Verify user registration
Preconditions: none
Test Steps:
	1. POST /api/users/register with valid payload
Request: `RegisterRequest` / `CreateUserRequest` (email, password, profile fields)
Expected Status: 200 OK or 201 Created
Expected Response: created user object with `id`
Postconditions / Cleanup: delete created user

Test ID: US-02
Title: Get user by id (not found)
Purpose: 404 for unknown user
Preconditions: no user with id 9999
Test Steps:
	1. GET /api/users/9999
Expected Status: 404 NotFound

Test ID: US-03
Title: Update user
Purpose: Verify update
Preconditions: user exists
Test Steps:
	1. PUT /api/users/{id} with `UpdateUserRequest`
Expected Status: 200 OK
Expected Response: updated user fields

Test ID: US-04
Title: Delete user
Purpose: Verify deletion
Preconditions: user exists
Test Steps:
	1. DELETE /api/users/{id}
	2. GET /api/users/{id}
Expected Status: 204 then 404

---------------------------------------------------------------------
Bookings
---------------------------------------------------------------------

Test ID: BK-01
Title: Create booking (happy path)
Purpose: Verify booking creation and passenger assignment
Preconditions: flight exists, user exists, seats available
Test Steps:
	1. POST /api/bookings with `CreateBookingRequest`
Request: include flightId, userId, passengers array
Expected Status: 201 Created
Expected Response: `BookingResponse` with `id`, `bookingCode`, `passengers`
Postconditions / Cleanup: cancel or delete created booking

Test ID: BK-02
Title: Create booking (flight missing)
Purpose: Booking creation should fail for missing flight
Preconditions: flightId invalid
Test Steps:
	1. POST /api/bookings with invalid flightId
Expected Status: 400 BadRequest (or 404 depending on implementation)

Test ID: BK-03
Title: Get booking by id (exists)
Purpose: Retrieve booking
Preconditions: booking exists
Test Steps:
	1. GET /api/bookings/{id}
Expected Status: 200 OK
Expected Response: booking data matches created booking

Test ID: BK-04
Title: Cancel booking
Purpose: Verify cancellation
Preconditions: booking exists
Test Steps:
	1. PUT /api/bookings/{id}/cancel
Expected Status: 204 NoContent
Expected Response: none; subsequent GET shows booking status Cancelled

Test ID: BK-05
Title: Get by code
Purpose: Lookup booking by code
Preconditions: booking with code exists
Test Steps:
	1. GET /api/bookings/code/{code}
Expected Status: 200 OK
Expected Response: booking with given code

---------------------------------------------------------------------
Flights
---------------------------------------------------------------------

Test ID: FL-01
Title: List flights
Purpose: Flights listing
Preconditions: flights seeded
Test Steps:
	1. GET /api/flights
Expected Status: 200 OK
Expected Response: JSON array

Test ID: FL-02
Title: Create flight
Purpose: Verify create flight
Preconditions: related data (routes, aircraft) exist
Test Steps:
	1. POST /api/flights with `CreateFlightRequest`
Expected Status: 201 Created
Expected Response: created flight with Id

Test ID: FL-03
Title: Delete flight (not found)
Purpose: Ensure 404 for missing flight
Preconditions: flight id 9999 missing
Test Steps:
	1. DELETE /api/flights/9999
Expected Status: 404 NotFound

---------------------------------------------------------------------
Flight Routes
---------------------------------------------------------------------

Test ID: FR-01
Title: Create route (happy path)
Purpose: Create flight route when airports exist
Preconditions: origin and destination airports exist
Test Steps:
	1. POST /api/flightroutes with `CreateFlightRouteRequest`
Expected Status: 201 Created
Expected Response: `FlightRouteResponse` with Id and Code

Test ID: FR-02
Title: Create route (origin missing)
Purpose: Missing airport triggers error
Preconditions: originAirportId invalid
Test Steps:
	1. POST /api/flightroutes with invalid origin
Expected Status: 4xx (KeyNotFoundException mapped appropriately)

Test ID: FR-03
Title: Create route (duplicate)
Purpose: Prevent duplicate routes
Preconditions: route between airports already exists
Test Steps:
	1. POST /api/flightroutes for same origin/destination
Expected Status: 4xx (InvalidOperationException)

Test ID: FR-04
Title: Update route (duplicate)
Purpose: Prevent update that creates duplicates
Preconditions: existing route and another conflicting route exist
Test Steps:
	1. PUT /api/flightroutes/{id} with payload that conflicts
Expected Status: 4xx (InvalidOperationException)

Test ID: FR-05
Title: Delete route
Purpose: Verify deletion
Preconditions: route exists
Test Steps:
	1. DELETE /api/flightroutes/{id}
Expected Status: 204 NoContent

---------------------------------------------------------------------
Flight Prices / Flight Seats / Add‑Ons
---------------------------------------------------------------------

Test ID: FP-01
Title: Create flight price
Purpose: Create price entry for flight
Preconditions: referenced flight exists
Test Steps:
	1. POST /api/flightprice with `CreateFlightPriceRequest`
Expected Status: 201 Created
Expected Response: created price object with Id

Test ID: FS-01
Title: Get flight seats for flight
Purpose: Retrieve seats by flight
Preconditions: flight with seats exists
Test Steps:
	1. GET /api/flightseats/flight/{flightId}
Expected Status: 200 OK
Expected Response: list of seat entries

Test ID: FA-01
Title: Get add-ons by category
Purpose: Filter add-ons
Preconditions: add-ons seeded
Test Steps:
	1. GET /api/flightaddon/category/{category}
Expected Status: 200 OK
Expected Response: filtered list

---------------------------------------------------------------------
Passengers
---------------------------------------------------------------------

Test ID: PA-01
Title: Create passenger
Purpose: Create passenger record
Preconditions: none
Test Steps:
	1. POST /api/passengers with `CreatePassengerRequest`
Expected Status: 201 Created
Expected Response: created passenger object with Id

Test ID: PA-02
Title: Update passenger (not found)
Purpose: 404 for unknown passenger update
Preconditions: passenger id 9999 missing
Test Steps:
	1. PUT /api/passengers/9999
Expected Status: 404 NotFound

---------------------------------------------------------------------
Seats & Seating Provisioning
---------------------------------------------------------------------

Test ID: ST-01
Title: Create seat
Purpose: Create a seat
Preconditions: aircraft exists
Test Steps:
	1. POST /api/seat with `CreateSeatRequest`
Expected Status: 201 Created

Test ID: ST-02
Title: Provision seats for aircraft
Purpose: Provision seats
Preconditions: aircraft id exists
Test Steps:
	1. POST /api/seating-provisioning/provision/{aircraftId}
Expected Status: 200 OK
Expected Response: confirmation message


