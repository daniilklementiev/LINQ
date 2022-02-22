using System;
using System.Collections.Generic;

namespace Auth
{
    public class User
    {
        public String Login { get; set; }
        public String Password { get; set; }
        public String RealName { get; set; }

        public string LastLogin { get; set; }
        public override string ToString()
        {
            return $"Login: {Login} | Last login: {LastLogin}. ";
        }
    }
}