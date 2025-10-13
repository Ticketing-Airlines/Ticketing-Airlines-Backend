using Airline_Ticketing.Enums;
using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model

   
{
    public class Messages
    {
        [Key]
        public int MessageID { get; set; }

        public int ChatID { get; set; }

        public SenderType SenderType { get; set; }

        public string MessageText { get; set; }

        public DateTime CreatedAt { get; set; }

        public Messages()
        {
            CreatedAt = DateTime.UtcNow;
        }


    }
}
