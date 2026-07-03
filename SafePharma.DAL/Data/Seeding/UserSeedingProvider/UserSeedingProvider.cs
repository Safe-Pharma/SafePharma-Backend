using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.DAL
{
    public class UserSeedingProvider
    {
        public static List<ApplicationUser> GetUsers()
        {
            var u1 = "99999999-9999-9999-9999-999999999999";
            var u2 = "88888888-8888-8888-8888-888888888888";

            return new List<ApplicationUser>
            {
                new ApplicationUser { Id = u1, UserName = "alice", NormalizedUserName = "ALICE", Email = "alice@example.com",EmailConfirmed = true },
                new ApplicationUser { Id = u2, UserName = "bob", NormalizedUserName = "BOB", Email = "bob@example.com", EmailConfirmed = true }
            };
        }
    }
}
