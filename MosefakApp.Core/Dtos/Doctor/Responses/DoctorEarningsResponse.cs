namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class DoctorEarningsResponse
    {
        public decimal TotalEarnings { get; set; } // Total money earned within the given period
        public decimal TotalPaidEarnings { get; set; } // Earnings already paid & confirmed
        public decimal TotalPendingEarnings { get; set; } // Earnings from pending payments
        public decimal TotalRefundedEarnings { get; set; } // Refunded amounts

        public int TotalAppointments { get; set; } // Total count of appointments in the period
        public int CompletedAppointmentsCount { get; set; } // Successfully completed appointments
        public int PendingAppointmentsCount { get; set; } // Upcoming or pending appointments
        public int CancelledAppointmentsCount { get; set; } // Cancelled appointments count

        public decimal AverageEarningsPerAppointment => TotalAppointments > 0
            ? TotalEarnings / TotalAppointments
            : 0; // Avoid division by zero

        public List<TopPayingPatientDto> TopPayingPatients { get; set; } = new();
        public List<EarningsByDateDto> EarningsBreakdownByDate { get; set; } = new();

    }
}

/*
 {
    "totalEarnings": 15000.00,
    "totalPaidEarnings": 13000.00,
    "totalPendingEarnings": 2000.00,
    "totalRefundedEarnings": 500.00,
    "totalAppointments": 120,
    "completedAppointmentsCount": 100,
    "pendingAppointmentsCount": 15,
    "cancelledAppointmentsCount": 5,
    "averageEarningsPerAppointment": 125.00,
    "topPayingPatients": [
        {
            "patientId": 101,
            "fullName": "John Doe",
            "totalAppointments": 5,
            "totalSpent": 2500.00
        }
    ],
    "earningsBreakdownByDate": [
        {
            "date": "2024-02-01",
            "earnings": 500.00
        },
        {
            "date": "2024-02-02",
            "earnings": 700.00
        }
    ]
}

 
 */
