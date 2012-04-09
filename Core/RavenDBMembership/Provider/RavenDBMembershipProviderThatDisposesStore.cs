using System;

namespace RavenDBMembership.Provider
{
    public class RavenDBMembershipProviderThatDisposesStore : RavenDBMembershipProvider, IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            if (DocumentStore != null)
                DocumentStore.Dispose();

            DocumentStore = null;
        }

        #endregion
    }
}