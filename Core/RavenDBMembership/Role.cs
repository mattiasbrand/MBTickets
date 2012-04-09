using System;

namespace RavenDBMembership
{
    public class Role
    {
        private string _id;

        public Role(string name, Role parentRole)
        {
            Name = name;
            if (parentRole != null)
            {
                ParentRoleId = parentRole.Id;
            }
        }

        public string Id
        {
            get
            {
                if (String.IsNullOrEmpty(_id))
                {
                    _id = GenerateId();
                }
                return _id;
            }
            set { _id = value; }
        }

        public string ApplicationName { get; set; }
        public string Name { get; set; }
        public string ParentRoleId { get; set; }

        private string GenerateId()
        {
            if (!String.IsNullOrEmpty(ParentRoleId))
            {
                return ParentRoleId + "/" + Name;
            }
            else
            {
                string defaultNameSpace = "authorization/roles/";
                // Also use application name for ID generation so we can have multiple roles with the same name.
                if (!String.IsNullOrEmpty(ApplicationName))
                {
                    return defaultNameSpace + ApplicationName.Replace("/", String.Empty) + "/" + Name;
                }
                else
                {
                    return defaultNameSpace + Name;
                }
            }
        }
    }
}