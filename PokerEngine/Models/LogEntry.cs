using System;

namespace PokerEngine.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Message { get; set; }
    }
}