using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Security;
using Microsoft.Practices.ServiceLocation;
using Raven.Client;

namespace RavenDBMembership.Provider
{
    public class RavenDBRoleProvider : RoleProvider
    {
        private const string ProviderName = "RavenDBRole";
        private IDocumentStore documentStore;

        public IDocumentStore DocumentStore
        {
            get
            {
                if (documentStore == null)
                {
                    throw new NullReferenceException(
                        "The DocumentStore is not set. Please set the DocumentStore or make sure that the Common Service Locator can find the IDocumentStore and call Initialize on this provider.");
                }
                return documentStore;
            }
            set { documentStore = value; }
        }

        public override string ApplicationName { get; set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            // Try to find an IDocumentStore via Common Service Locator. 
            try
            {
                IServiceLocator locator = ServiceLocator.Current;
                if (locator != null)
                {
                    DocumentStore = locator.GetInstance<IDocumentStore>();
                }
            }
            catch (NullReferenceException)
                // Swallow Nullreference expection that occurs when there is no current service locator.
            {
            }
            base.Initialize(name, config);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (usernames.Length == 0 || roleNames.Length == 0)
            {
                return;
            }
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                try
                {
                    IDocumentQuery<User> users = session.Advanced.LuceneQuery<User>().OpenSubclause();
                    foreach (string username in usernames)
                    {
                        users = users.WhereEquals("Username", username, true);
                    }
                    users = users.CloseSubclause().AndAlso().WhereEquals("ApplicationName", ApplicationName, true);

                    List<User> usersAsList = users.ToList();
                    IDocumentQuery<Role> roles = session.Advanced.LuceneQuery<Role>().OpenSubclause();
                    foreach (string roleName in roleNames)
                    {
                        roles = roles.WhereEquals("Name", roleName, true);
                    }
                    roles = roles.CloseSubclause().AndAlso().WhereEquals("ApplicationName", ApplicationName);

                    List<string> roleIds = roles.Select(r => r.Id).ToList();
                    foreach (string roleId in roleIds)
                    {
                        foreach (User user in usersAsList)
                        {
                            user.Roles.Add(roleId);
                        }
                    }
                    session.SaveChanges();
                }
                catch (Exception ex)
                {
                    // TODO: log exception properly
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        }

        public override void CreateRole(string roleName)
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                try
                {
                    var role = new Role(roleName, null);
                    role.ApplicationName = ApplicationName;

                    session.Store(role);
                    session.SaveChanges();
                }
                catch (Exception ex)
                {
                    // TODO: log exception properly
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                try
                {
                    Role role = (from r in session.Query<Role>()
                                 where r.Name == roleName && r.ApplicationName == ApplicationName
                                 select r).SingleOrDefault();
                    if (role != null)
                    {
                        // also find users that have this role
                        List<User> users = (from u in session.Query<User>()
                                            where u.Roles.Any(roleId => roleId == role.Id)
                                            select u).ToList();
                        if (users.Any() && throwOnPopulatedRole)
                        {
                            throw new Exception(String.Format("Role {0} contains members and cannot be deleted.",
                                                              role.Name));
                        }
                        foreach (User user in users)
                        {
                            user.Roles.Remove(role.Id);
                        }
                        session.Delete(role);
                        session.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    // TODO: log exception properly
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                // Get role first
                Role role = (from r in session.Query<Role>()
                             where r.Name == roleName && r.ApplicationName == ApplicationName
                             select r).SingleOrDefault();
                if (role != null)
                {
                    // Find users
                    IQueryable<string> users = from u in session.Query<User>()
                                               where
                                                   u.Roles.Any(r => r == role.Id) &&
                                                   u.Username.Contains(usernameToMatch)
                                               select u.Username;
                    return users.ToArray();
                }
                return null;
            }
        }

        public override string[] GetAllRoles()
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                List<Role> roles = (from r in session.Query<Role>()
                                    where r.ApplicationName == ApplicationName
                                    select r).ToList();
                return roles.Select(r => r.Name).ToArray();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                User user = (from u in session.Query<User>()
                             where u.Username == username && u.ApplicationName == ApplicationName
                             select u).Single();
                if (user.Roles.Any())
                {
                    List<Role> dbRoles = session.Query<Role>().ToList();
                    return dbRoles.Where(r => user.Roles.Any(role => role == r.Id)).Select(r => r.Name).ToArray();
                }
                return new string[0];
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                Role role = (from r in session.Query<Role>()
                             where r.Name == roleName && r.ApplicationName == ApplicationName
                             select r).SingleOrDefault();
                if (role != null)
                {
                    IQueryable<string> usernames = from u in session.Query<User>()
                                                   where u.Roles.Any(r => r == role.Id)
                                                   select u.Username;
                    return usernames.ToArray();
                }
                return null;
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                User user =
                    session.Query<User>().SingleOrDefault(
                        u => u.Username == username && u.ApplicationName == ApplicationName);
                if (user != null)
                {
                    Role role = (from r in session.Query<Role>()
                                 where r.Name == roleName && r.ApplicationName == ApplicationName
                                 select r).SingleOrDefault();
                    if (role != null)
                    {
                        return user.Roles.Contains(role.Id);
                    }
                }
                return false;
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            if (usernames.Length == 0 || roleNames.Length == 0)
            {
                return;
            }
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                try
                {
                    IDocumentQuery<User> users = session.Advanced.LuceneQuery<User>().OpenSubclause();
                    foreach (string username in usernames)
                    {
                        users = users.WhereEquals("Username", username, true);
                    }
                    users = users.CloseSubclause().AndAlso().WhereEquals("ApplicationName", ApplicationName, true);

                    List<User> usersAsList = users.ToList();
                    IDocumentQuery<Role> roles = session.Advanced.LuceneQuery<Role>().OpenSubclause();
                    foreach (string roleName in roleNames)
                    {
                        roles = roles.WhereEquals("Name", roleName, true);
                    }
                    roles = roles.CloseSubclause().AndAlso().WhereEquals("ApplicationName", ApplicationName);

                    List<string> roleIds = roles.Select(r => r.Id).ToList();
                    foreach (string roleId in roleIds)
                    {
                        IEnumerable<User> usersWithRole = usersAsList.Where(u => u.Roles.Contains(roleId));
                        foreach (User user in usersWithRole)
                        {
                            user.Roles.Remove(roleId);
                        }
                    }
                    session.SaveChanges();
                }
                catch (Exception ex)
                {
                    // TODO: log exception properly
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
        }

        public override bool RoleExists(string roleName)
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                return session.Query<Role>().Any(r => r.Name == roleName);
            }
        }
    }
}