using AutoMapper;
using Airline1.Models;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using System;
using System.Linq; // Added for convenience in potential complex mappings

namespace Airline1.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ... (Existing Mappings for Airport, Aircraft, FlightRoute, Flight, FlightBundle, FlightStatus, Passenger, Configuration, Seat, Booking) ...

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

            // --------------------------------------------------------------------------
            // ⭐ FLIGHT PRICE MAPPING (UPDATED) ⭐
            // The obsolete 'Type' is replaced by 'FlightBundleId' and 'BundleName'.
            // Mappings are added for CreateRequest to Model.
            // --------------------------------------------------------------------------
            CreateMap<CreateFlightPriceRequest, FlightPrice>();
            CreateMap<UpdateFlightPriceRequest, FlightPrice>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<FlightPrice, FlightPriceResponse>()
                // Remove obsolete Type mapping
                // .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))

                // Add new BundleName mapping using the navigation property
                .ForMember(dest => dest.BundleName, opt => opt.MapFrom(src => src.FlightBundle != null ? src.FlightBundle.Name : null))

                // Update FlightNumber mapping (optional, but good practice if available)
                .ForMember(d => d.FlightNumber, opt => opt.MapFrom(s => s.Flight != null ? s.Flight.FlightNumber : null))

                // Keep IsActive convenience mapping
                .ForMember(d => d.IsActive, opt => opt.MapFrom(s => (s.EffectiveTo == null || s.EffectiveTo > DateTime.UtcNow) && s.EffectiveFrom <= DateTime.UtcNow));

            // --------------------------------------------------------------------------

            // Flight mapping (existing)
            CreateMap<Flight, FlightResponse>()
                .ForMember(dest => dest.AircraftName, opt => opt.MapFrom(src => src.Aircraft != null ? src.Aircraft.DisplayName : null))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Route != null ? src.Route.OriginAirport.Name : null))
                .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Route != null ? src.Route.DestinationAirport.Name : null));

            // ... (All other existing mappings) ...

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
            CreateMap<Airline1.Dtos.Requests.CreateSeatRequest, Airline1.Models.Seat>();
            CreateMap<Airline1.Dtos.Requests.UpdateSeatRequest, Airline1.Models.Seat>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Airline1.Models.Seat, Airline1.Dtos.Responses.SeatResponse>();

            // Booking Mappings (existing)
            CreateMap<CreateBookingRequest, Booking>();
            CreateMap<PassengerForBookingDto, BookingPassenger>();
            CreateMap<Booking, BookingResponse>();
            CreateMap<BookingPassenger, BookingPassengerResponse>();

            // FlightAddOn Mappings (existing)
            CreateMap<CreateFlightAddOnRequest, FlightAddOn>();
            CreateMap<UpdateFlightAddOnRequest, FlightAddOn>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<FlightAddOn, FlightAddOnResponse>();

            // AddOnPrice Mappings (existing)
            CreateMap<CreateAddOnPriceRequest, AddOnPrice>();
            CreateMap<UpdateAddOnPriceRequest, AddOnPrice>();
            CreateMap<AddOnPrice, AddOnPriceResponse>();

            // Users (existing)
            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<User, UserResponse>();
        }
    }
}