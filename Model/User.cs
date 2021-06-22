using System;
using System.Collections.Generic;

#nullable disable

namespace ubereats_user_auth.Model
{
    public partial class User
    {
        public User()
        {
            InverseMentoringNavigation = new HashSet<User>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string RefreshToken { get; set; }
        public bool IsValid { get; set; }
        public int? Mentoring { get; set; }

        public virtual User MentoringNavigation { get; set; }
        public virtual ICollection<User> InverseMentoringNavigation { get; set; }
    }
}
