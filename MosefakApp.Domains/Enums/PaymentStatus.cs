namespace MosefakApp.Domains.Enums
{
    public enum PaymentStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Failed")]
        Failed,

        [EnumMember(Value = "Completed")]
        Completed
    }
}
