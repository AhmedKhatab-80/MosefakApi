namespace MosefakApp.Domains.Enums
{
    public enum EmploymentType
    {
        [EnumMember(Value = "Full-Time")]
        FullTime,

        [EnumMember(Value = "Part-Time")]
        PartTime,

        [EnumMember(Value = "Contract")]
        Contract,

        [EnumMember(Value = "Internship")]
        Internship,
    }
}
