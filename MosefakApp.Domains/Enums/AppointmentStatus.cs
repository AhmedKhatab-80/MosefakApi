namespace MosefakApp.Domains.Enums
{
    public enum AppointmentStatus
    {
        [EnumMember(Value = "PendingApproval")]
        PendingApproval,

        [EnumMember(Value = "PendingPayment")]
        PendingPayment,

        [EnumMember(Value = "Confirmed")]
        Confirmed,

        [EnumMember(Value = "Completed")]
        Completed,

        [EnumMember(Value = "CanceledByDoctor")]
        CanceledByDoctor,

        [EnumMember(Value = "CanceledByPatient")]
        CanceledByPatient,

        [EnumMember(Value = "AutoCanceled")]
        AutoCanceled
    }
}
