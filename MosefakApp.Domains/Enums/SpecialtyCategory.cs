namespace MosefakApp.Domains.Enums
{
    public enum SpecialtyCategory
    {
        [EnumMember(Value = "All")]
        All,

        [EnumMember(Value = "General")]
        General,

        [EnumMember(Value = "Heart")]
        Heart, // Cardiology

        [EnumMember(Value = "Eye")]
        Eye,   // Ophthalmology

        [EnumMember(Value = "Dermatology")]
        Dermatology,

        [EnumMember(Value = "Endocrinology")]
        Endocrinology,

        [EnumMember(Value = "Gastroenterology")]
        Gastroenterology,

        [EnumMember(Value = "Neurology")]
        Neurology,

        [EnumMember(Value = "Obstetrics and Gynecology")]
        ObstetricsAndGynecology,

        [EnumMember(Value = "Orthopedics")]
        Orthopedics,

        [EnumMember(Value = "Pediatrics")]
        Pediatrics,

        [EnumMember(Value = "Psychiatry")]
        Psychiatry,

        [EnumMember(Value = "Radiology")]
        Radiology,

        [EnumMember(Value = "Urology")]
        Urology
    }
}
