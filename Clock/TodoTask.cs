using System;

namespace Clock
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; } = false;
        public TimeSpan? WorkedTime { get; set; }

        public string DisplayText
        {
            get
            {
                var deadlineText = Deadline.HasValue ? $" (Due: {Deadline.Value:MM/dd/yyyy})" : "";
                var statusText = IsCompleted ? " ?" : "";
                return $"{Name}{deadlineText}{statusText}";
            }
        }

        public Color GetDeadlineColor()
        {
            if (!Deadline.HasValue || IsCompleted)
                return Color.FromArgb(55, 65, 81); // Modern dark gray

            var daysUntilDeadline = (Deadline.Value.Date - DateTime.Now.Date).Days;
            
            if (daysUntilDeadline < 0)
                return Color.FromArgb(239, 68, 68); // Modern red - overdue
            else if (daysUntilDeadline <= 1)
                return Color.FromArgb(245, 158, 11); // Modern amber - due soon
            else if (daysUntilDeadline <= 3)
                return Color.FromArgb(249, 115, 22); // Modern orange - due this week
            else
                return Color.FromArgb(55, 65, 81); // Modern dark gray - normal
        }

        public Color GetBackgroundColor()
        {
            if (IsCompleted)
                return Color.FromArgb(240, 253, 244); // Light green background for completed

            if (!Deadline.HasValue)
                return Color.FromArgb(249, 250, 251); // Light gray for no deadline

            var daysUntilDeadline = (Deadline.Value.Date - DateTime.Now.Date).Days;
            
            if (daysUntilDeadline < 0)
                return Color.FromArgb(254, 242, 242); // Light red background - overdue
            else if (daysUntilDeadline <= 1)
                return Color.FromArgb(255, 251, 235); // Light amber background - due soon
            else if (daysUntilDeadline <= 3)
                return Color.FromArgb(255, 247, 237); // Light orange background - due this week
            else
                return Color.FromArgb(249, 250, 251); // Light gray - normal
        }
    }
}