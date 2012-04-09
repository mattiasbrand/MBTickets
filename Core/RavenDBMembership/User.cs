﻿using System;
using System.Collections.Generic;

namespace RavenDBMembership
{
    public class User
    {
        public User()
        {
            Roles = new List<string>();
            Id = "authorization/users/"; // db assigns id
        }

        public string Id { get; set; }
        public string ApplicationName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastLogin { get; set; }
        public IList<string> Roles { get; set; }
    }
}