namespace Pkt.Models
{
    public class QueryResultViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn1 { get; set; }
        public int Cm_Id { get; set; }
        public string Process { get; set; }
        public string PKTName { get; set; }
        public string IsCorrect { get; set; }
        public string Question { get; set; }
        public string CorrectAns { get; set; }
        public string GivenAns { get; set; }
     
       public DateTime PKTDate { get; set; }
       public string Percentage { get; set;}
       public string Wrong_Ans { get; set;}
       public string TotalQuestion { get; set; }
       public string Attemped {  get; set; }

        public string NotAttempt { get; set; } 


    }

}
