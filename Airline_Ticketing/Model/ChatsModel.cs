namespace Airline_Ticketing.Model
{
    public class ChatsModel
    {
        public int ChatID { get; set; }

        public int?  UserID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string Status { get; set; }


    }
}
