using System;

namespace Web.Models
{
    public class TicketListItemModel
    {
        public Guid Id { get; set; }
        public string Creator { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}