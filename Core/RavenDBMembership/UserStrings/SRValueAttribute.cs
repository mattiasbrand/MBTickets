using System;

namespace RavenDBMembership.UserStrings
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class SRValueAttribute : Attribute
    {
        public string Value;

        public SRValueAttribute(string Value)
        {
            this.Value = Value;
        }
    }
}