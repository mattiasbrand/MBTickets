using System;

namespace Core.Domain.Entities
{
    public class Ticket
    {
        public Ticket(Guid id, string creator, string title, string description)
        {
            Id = id;
            Creator = creator;
            Title = title;
            Description = description;
            Created = DateTime.Now;
        }

        public Guid Id { get; set; }
        public string Creator { get; set; }
        public DateTime Created { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}