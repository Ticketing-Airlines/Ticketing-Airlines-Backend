using AutoMapper;
using Airline1.Models;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using System;
using System.Linq;
using System.Collections.Generic; // Added for clarity, though System.Linq covers it

namespace Airline1.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ... (Existing Mappings for Aircraft, FlightRoutes, Flights, FlightBundles, FlightPrice, etc.) ...

            CreateMap<CreateAirportRequest, Airport>();
            CreateMap<UpdateAirportRequest, Airport>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Airport, AirportResponse>();

            // Aircraft mappings (existing)
            CreateMap<CreateAircraftRequest, Aircraft>();
            CreateMap<UpdateAircraftRequest, Aircraft>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Aircraft, AircraftResponse>()
                .ForMember(dest => dest.BaseAirportName, opt => opt.MapFrom(src => src.BaseAirport != null ? src.BaseAirport.Name : null));

            // FlightRoutes (existing)
            CreateMap<CreateFlightRouteRequest, FlightRoute>();
            CreateMap<UpdateFlightRouteRequest, FlightRoute>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<FlightRoute, FlightRouteResponse>()
                .ForMember(dest => dest.OriginAirportName, opt => opt.MapFrom(src => src.OriginAirport != null ? src.OriginAirport.Name : null))
                .ForMember(dest => dest.DestinationAirportName, opt => opt.MapFrom(src => src.DestinationAirport != null ? src.DestinationAirport.Name : null));

            // Flights (existing)
            CreateMap<CreateFlightRequest, Flight>();
            CreateMap<UpdateFlightRequest, Flight>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // FlightBundles (existing)
            CreateMap<FlightBundle, FlightBundleResponse>();
            CreateMap<CreateFlightBundleRequest, FlightBundle>();
            CreateMap<UpdateFlightBundleRequest, FlightBundle>();

            // ⭐ FLIGHT PRICE MAPPING ⭐
            CreateMap<CreateFlightPriceRequest, FlightPrice>();
            CreateMap<UpdateFlightPriceRequest, FlightPrice>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<FlightPrice, FlightPriceResponse>()
                .ForMember(dest => dest.BundleName, opt => opt.MapFrom(src => src.FlightBundle != null ? src.FlightBundle.Name : null))
                .ForMember(d => d.FlightNumber, opt => opt.MapFrom(s => s.Flight != null ? s.Flight.FlightNumber : null))
                .ForMember(d => d.IsActive, opt => opt.MapFrom(s => (s.EffectiveTo == null || s.EffectiveTo > DateTime.UtcNow) && s.EffectiveFrom <= DateTime.UtcNow));

            // Flight mapping (existing)
            CreateMap<Flight, FlightResponse>()
                .ForMember(dest => dest.AircraftName, opt => opt.MapFrom(src => src.Aircraft != null ? src.Aircraft.DisplayName : null))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Route != null ? src.Route.OriginAirport.Name : null))
                .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Route != null ? src.Route.DestinationAirport.Name : null));

            // Airline mappings
            CreateMap<Airline, AirlineSearchResponse>()
                .ForMember(dest => dest.AirlineId, opt => opt.MapFrom(src => src.Id));

            // Airport search mapping
            CreateMap<Airport, AirportSearchResponse>()
                .ForMember(dest => dest.AirportId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CountryIso2, opt => opt.MapFrom(src => src.CountryIso2 ?? src.Country));

            // Aircraft search mapping
            CreateMap<Aircraft, AircraftSearchResponse>()
                .ForMember(dest => dest.AircraftId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Configuration != null ? src.Configuration.TotalSeats : 0));

            // FlightStatus (existing)
            CreateMap<FlightStatus, FlightStatusResponse>()
                .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.Flight != null ? src.Flight.FlightNumber : string.Empty))
                .ForMember(dest => dest.ReasonCode, opt => opt.MapFrom(src => src.Reason != null ? src.Reason.Code : null))
                .ForMember(dest => dest.ReasonTitle, opt => opt.MapFrom(src => src.Reason != null ? src.Reason.Title : null));

            CreateMap<FlightStatusReason, FlightStatusReasonResponse>();

            // Passenger (existing)
            CreateMap<CreatePassengerRequest, Passenger>();
            CreateMap<UpdatePassengerRequest, Passenger>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Passenger, PassengerResponse>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src =>
                        $"{src.FirstName} {(string.IsNullOrEmpty(src.MiddleName) ? "" : src.MiddleName + " ")}{src.LastName}".Trim()))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.FlightNumber,
                    opt => opt.MapFrom(src => src.Flight != null ? src.Flight.FlightNumber : null));

            // Aircraft Configuration (existing)
            CreateMap<CreateAircraftConfigurationRequest, AircraftConfiguration>();
            CreateMap<UpdateAircraftConfigurationRequest, AircraftConfiguration>()
                .ForAllMembers(opt => opt.Condition((src, dest, val) => val != null));
            CreateMap<CabinDetailDto, CabinConfigurationDetail>();
            CreateMap<AircraftConfiguration, AircraftConfigurationResponse>();
            CreateMap<CabinConfigurationDetail, CabinDetailResponse>();

            // Seat Mappings (existing)
            CreateMap<CreateSeatRequest, Seat>();
            CreateMap<UpdateSeatRequest, Seat>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Seat, SeatResponse>();

            // FlightAddOn Mappings (existing)
            CreateMap<CreateFlightAddOnRequest, FlightAddOn>();
            CreateMap<UpdateFlightAddOnRequest, FlightAddOn>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<FlightAddOn, FlightAddOnResponse>();

            // AddOnPrice Mappings (existing)
            CreateMap<CreateAddOnPriceRequest, AddOnPrice>();
            CreateMap<UpdateAddOnPriceRequest, AddOnPrice>();
            CreateMap<AddOnPrice, AddOnPriceResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AddOnPriceId));

            // Users (existing)
            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<User, UserResponse>();

            // --------------------------------------------------------------------------
            // ⭐ BOOKING MAPPINGS (FIXED AND UPDATED) ⭐
            // The mapping for 'PassengerForBookingDto' has been replaced by 'CreateBookingPassengerRequest' 
            // to resolve the namespace/type error, reflecting the likely intended DTO structure for the POST body.
            // --------------------------------------------------------------------------

            // Booking Mappings
            CreateMap<CreateBookingRequest, Booking>()
                .ForMember(dest => dest.Pnr, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.Passengers, opt => opt.MapFrom(src => src.Passengers));

            // FIX: Replace the unknown DTO with the known DTO used in other contexts (CreateBookingPassengerRequest)
            CreateMap<CreateBookingPassengerRequest, BookingPassenger>()
                .ForMember(dest => dest.BookingPassengerId, opt => opt.Ignore())
                .ForMember(dest => dest.BookingId, opt => opt.Ignore())
                .ForMember(dest => dest.AddOns, opt => opt.Ignore());


            CreateMap<Booking, BookingResponse>()
                 .ForMember(dest => dest.Flights,
                   opt => opt.MapFrom(src => src.BookingFlights.Select(bf => bf.Flight)));

            CreateMap<BookingPassenger, BookingPassengerResponse>()
                .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.FlightSeat!.Seat!.SeatNumber))
                .ForMember(dest => dest.AddOns, opt => opt.MapFrom(src => src.AddOns));

            // BookingAddOn Mappings
            CreateMap<BookingAddOn, BookingAddOnResponse>()
                .ForMember(dest => dest.AddOnName, opt => opt.MapFrom(src => src.AddOnPrice!.AddOn!.Name))
                .ForMember(dest => dest.AddOnCode, opt => opt.MapFrom(src => src.AddOnPrice!.AddOn!.Code));
        }
    }
}