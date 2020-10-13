using System;
using System.Collections.Generic;

namespace Dashboard.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string EmailAddress { get; set; }
        public AccessLevel UserLevel {get; set;}
        public ICollection<Setting> UserSettings { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }

    public enum AccessLevel
    {
        normal = 0,
        admin = 1    
    }
}