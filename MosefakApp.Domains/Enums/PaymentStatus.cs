namespace MosefakApp.Domains.Enums
{
    public enum PaymentStatus
    {

        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Paid")]
        Paid,

        [EnumMember(Value = "Refunded")]
        Refunded,

        [EnumMember(Value = "Failed")]
        Failed,

        [EnumMember(Value = "RefundPending")]
        RefundPending,

        [EnumMember(Value = "RefundFailed")]
        RefundFailed
    }
}
