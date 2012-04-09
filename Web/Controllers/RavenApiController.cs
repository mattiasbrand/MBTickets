using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Raven.Client;

namespace Web.Controllers
{
    public class RavenApiController : ApiController
    {
        public static IDocumentStore DocumentStore { get; set; }

        private IDocumentSession _ravenSession;
        public IDocumentSession RavenSession
        {
            get
            {
                if (_ravenSession == null) 
                    _ravenSession = (IDocumentSession)HttpContext.Current.Items["CurrentRequestRavenSession"];

                return _ravenSession;
            }
        }
    }
}