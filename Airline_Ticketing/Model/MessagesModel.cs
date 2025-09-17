using Airline_Ticketing.Enums;

namespace Airline_Ticketing.Model

   
{
    public class MessagesModel
    {

        public int MessageID { get; set; }

        public int ChatID { get; set; }

        public SenderType SenderType { get; set; }

        public string MessageText { get; set; }

        public DateTime CreatedAt { get; set; }

        public MessagesModel()
        {
            CreatedAt = DateTime.UtcNow;
        }


    }
}
