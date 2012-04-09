using System;
using System.Security.Principal;

namespace Web
{
    public static class ApplicationContext
    {
        public static Func<IIdentity> GetCurrentIdentity;
    }
}