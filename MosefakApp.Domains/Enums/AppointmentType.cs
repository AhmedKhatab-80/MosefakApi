namespace MosefakApp.Domains.Enums
{
    public enum AppointmentType
    {
        [EnumMember(Value = "Consultation")]
        Consultation,

        [EnumMember(Value = "FollowUp")]
        FollowUp,

        [EnumMember(Value = "Emergency")]
        Emergency
    }
}
