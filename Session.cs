using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMangementSystem
{
    public static class Session
    {
        public static int UserId { get; set; }
        public static string Username { get; set; }
        public static string Role { get; set; }

        // teacher only
        public static int TeacherDbId { get; set; }
        public static string TeacherID { get; set; }
    }
}
