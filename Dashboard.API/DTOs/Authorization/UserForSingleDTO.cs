using System;
using System.Collections.Generic;
using Dashboard.API.Models;

namespace Dashboard.API.DTOs
{
    public class UserForSingleDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public AccessLevel UserLevel {get; set;}
        public ICollection<Setting> UserSettings { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}