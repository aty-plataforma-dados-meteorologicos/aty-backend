using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtyBackend.Domain.Model
{
    public static class UserRoles
    {

        public const string User = "User";
        public const string Maintainer = "Maintainer";
        public const string Manager = "Manager";
        public const string Admin = "Admin";

        public static List<string> ToList()
        {
            return new List<string>
        {
            User,
            Maintainer,
            Manager,
            Admin
        };
        }

        public static string CommaSeparated()
        {
            return $"{User},{Maintainer},{Manager},{Admin}";
        }

        public static string MaintainerManagerAdmin()
        {
            return $"{Maintainer},{Manager},{Admin}";
        }
    }
}
