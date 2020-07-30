using Microsoft.Graph;
using System;

namespace MicrosoftGraphViltService
{
    public class OnlineMeetingConfig
    {
        public bool CreatorIsAttendee { get; set; }
        public bool IsReminderOn { get; set; }
        public bool ResponseRequested { get; set; }
        public string BodyTypeName { get; set; }
        public string ProviderTypeName { get; set; }
        public BodyType BodyType() => (BodyType)Enum.Parse(typeof(BodyType), BodyTypeName);
        public OnlineMeetingProviderType ProviderType() => (
                    OnlineMeetingProviderType)Enum.Parse(typeof(OnlineMeetingProviderType), ProviderTypeName
        );

    }
}
