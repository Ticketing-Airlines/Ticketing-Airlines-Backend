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
        }
    }
}
