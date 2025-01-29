namespace MosefakApp.Domains.Enums
{
    public enum AppointmentStatus
    {
        [EnumMember(Value = "Scheduled")]
        Scheduled,

        [EnumMember(Value = "Completed")]
        Completed,

        [EnumMember(Value = "Cancelled")]
        Cancelled,

        [EnumMember(Value = "No Show")]
        NoShow,

        [EnumMember(Value = "Rescheduled")]
        Rescheduled,
    }

}
