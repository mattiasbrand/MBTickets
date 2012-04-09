using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Domain.Entities;
using Web.Models;

namespace Web.Controllers
{
    public class TicketsController : RavenApiController
    {
        // GET /api/values
        public IEnumerable<TicketListItemModel> Get()
        {
            return RavenSession.Query<Ticket>()
                .Select(t => new TicketListItemModel { Id = t.Id, Creator = t.Creator, Title = t.Title, Description = t.Description })
                .ToList();
        }

        // GET /api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST /api/values
        public void Post(string value)
        {
        }

        // PUT /api/values/5
        public void Put(Guid id, string title, string description)
        {
            var newTicket = new Ticket(id, ApplicationContext.GetCurrentIdentity().Name, title, description);
            RavenSession.Store(newTicket);
        }

        // DELETE /api/values/5
        public void Delete(int id)
        {
        }
    }
}