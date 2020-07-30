using System.Collections.Generic;

namespace vILT.Domain
{
    public class ViltSession
    {
        public string Id { get; set; }
        public ICollection<ViltSessionAttendee> Attendees { get; set; }
        public string JoinUrl { get; set; }
    }
    public class ViltSessionAttendee
    {
        public string Email { get; set; }
    }
}
