namespace MosefakApi.Business.Services.Authentication
{
    public class JwtSetting
    {
        [Required]
        public string Issuer { get; set; } = null!;

        [Required]
        public string Audience { get; set; } = null!;

        [Required, Range(1, int.MaxValue)]
        public int lifeTime { get; set; }

        //[Required]
        public string Key { get; set; } = null!;
    }

}
