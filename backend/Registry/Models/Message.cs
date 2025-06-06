﻿namespace Registry.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Text { get; set; }
        public DateTime Sent { get; set; }
        public DateTime? Seen { get; set; }
        public User From { get; set; } = default!;
        public required Guid FromId { get; set; }
        public required Conversation Conversation { get; set; }
    }
}
