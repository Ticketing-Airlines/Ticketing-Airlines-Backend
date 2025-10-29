using AutoMapper;
using Airline1.Models;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAirportRequest, Airport>();
            // Map UpdateAirportRequest onto Airport, but only when source member is not null
            CreateMap<UpdateAirportRequest, Airport>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Airport, AirportResponse>();

            // Aircraft mappings
            CreateMap<CreateAircraftRequest, Aircraft>();
            CreateMap<UpdateAircraftRequest, Aircraft>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Aircraft, AircraftResponse>()
                .ForMember(dest => dest.BaseAirportName, opt => opt.MapFrom(src => src.BaseAirport != null ? src.BaseAirport.Name : null));
           
            // FlightRoutes
            CreateMap<CreateFlightRouteRequest, FlightRoute>();
            CreateMap<UpdateFlightRouteRequest, FlightRoute>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<FlightRoute, FlightRouteResponse>()
                .ForMember(dest => dest.OriginAirportName, opt => opt.MapFrom(src => src.OriginAirport != null ? src.OriginAirport.Name : null))
                .ForMember(dest => dest.DestinationAirportName, opt => opt.MapFrom(src => src.DestinationAirport != null ? src.DestinationAirport.Name : null));

            CreateMap<CreateFlightRequest, Flight>();
            CreateMap<UpdateFlightRequest, Flight>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Flight, FlightResponse>()
                .ForMember(dest => dest.AircraftName, opt => opt.MapFrom(src => src.Aircraft != null ? src.Aircraft.DisplayName : null))
                .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src.Route != null ? src.Route.OriginAirport.Name : null))
                .ForMember(dest => dest.Destination, opt => opt.MapFrom(src => src.Route != null ? src.Route.DestinationAirport.Name : null));

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
            //.ForMember(dest => dest.SeatNumber,
            //    opt => opt.MapFrom(src => src.SeatNumber != null ? src.SeatNumber : "Unassigned"));
            
            // Aircraft Configuration
            CreateMap<CreateAircraftConfigurationRequest, AircraftConfiguration>();
            CreateMap<UpdateAircraftConfigurationRequest, AircraftConfiguration>()
                .ForAllMembers(opt => opt.Condition((src, dest, val) => val != null));
            CreateMap<CabinDetailDto, CabinConfigurationDetail>();
            CreateMap<AircraftConfiguration, AircraftConfigurationResponse>();
            CreateMap<CabinConfigurationDetail, CabinDetailResponse>();


            CreateMap<CreateBookingRequest, Booking>();
            CreateMap<PassengerForBookingDto, BookingPassenger>();
            CreateMap<Booking, BookingResponse>();
            CreateMap<BookingPassenger, BookingPassengerResponse>();

            // Users
            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<User, UserResponse>();
        }
    }
}
