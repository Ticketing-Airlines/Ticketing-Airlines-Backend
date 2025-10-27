namespace Airline1.Common
{
    public enum AircraftType
    {
        NarrowBody,
        WideBody,
        Regional,
        Turboprop,
        BusinessJet,
        Cargo
    }
    public enum CabinType
    {
        First = 1,
        Business = 2,
        PremiumEconomy = 3,
        Economy = 4
    }
    public enum SeatStatus
    {
        Available = 1,
        Occupied = 2,
        Blocked = 3,
        Unavailable = 4
    }

    public enum AircraftStatus
    {
        Active,
        Maintenance,
        Grounded,
        Decommissioned
    }
}
