using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace TicketMaster
{
    public class TicketDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Branch { get; set; }
        public string LinkToExecution { get; set; }
        public List<Attachment> Attachment { get; set; }
    }
}
