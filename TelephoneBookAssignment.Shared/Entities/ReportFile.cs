using System;

namespace TelephoneBookAssignment.Shared.Entities
{
    public class ReportFile : BaseMongoDbEntity
    {
        public string UserId { get; set; } // String from Identity
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public FileStatus FileStatus { get; set; } // 0: Requested, 1: Completed
    }
    public enum FileStatus // For Notifications
    {
        Creating,
        Completed
    }
}