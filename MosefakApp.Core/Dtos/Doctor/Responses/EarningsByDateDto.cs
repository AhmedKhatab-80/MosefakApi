namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class EarningsByDateDto
    {
        public DateTime Date { get; set; } // The date of earnings
        public decimal Earnings { get; set; } // Total earnings for that date
    }
}
