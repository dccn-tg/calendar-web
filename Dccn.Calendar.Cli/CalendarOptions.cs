using JetBrains.Annotations;

namespace Dccn.Calendar.Cli
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class CalendarOptions
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public string Certificate { get; set; }

        public string Source { get; set; }
        public string Filter { get; set; }
    }
}
