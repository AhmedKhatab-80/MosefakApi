namespace MosefakApp.Domains.Enums
{
    public enum PaymentStatus
    {

        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Paid")]
        Paid,

        [EnumMember(Value = "Refunded")]
        Refunded
    }
}
