namespace NotifyMyWindowsPhoneClient
{
    public class NmwpNotification
    {
        public string Application { get; private set; }
        public string Event { get; private set; }
        public string Description { get; private set; }
        public NmwpPriority Priority { get; private set; }

        public NmwpNotification(string application, string ev, string description, NmwpPriority priority = NmwpPriority.Normal)
        {
            Application = application;
            Event = ev;
            Description = description;
            Priority = priority;
        }
    }
}