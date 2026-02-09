using System;

namespace TimeReportV3
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Login{ get; set; }
        public string Email { get; set; }
        public bool IsArchive { get; set; }
        public Guid ADGuid { get; set; }
    }
}