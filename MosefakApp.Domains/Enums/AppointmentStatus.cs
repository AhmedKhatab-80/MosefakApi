namespace MosefakApp.Domains.Enums
{
    public enum AppointmentStatus
    {
        [EnumMember(Value = "Pending Approval")]
        PendingApproval,

        [EnumMember(Value = "Pending Payment")]
        PendingPayment,

        [EnumMember(Value = "Confirmed")]
        Confirmed,

        [EnumMember(Value = "Completed")]
        Completed,

        [EnumMember(Value = "Canceled By Doctor")]
        CanceledByDoctor,

        [EnumMember(Value = "Canceled By Patient")]
        CanceledByPatient,

        [EnumMember(Value = "Auto Canceled")]
        AutoCanceled
    }
}
