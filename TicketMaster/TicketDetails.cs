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
        public string Lab { get; set; }
        public string Error { get; set; }
        public string Scenario { get; set; }
        public List<string> AttachmentPath { get; set; } = new List<string>();
    }
}
