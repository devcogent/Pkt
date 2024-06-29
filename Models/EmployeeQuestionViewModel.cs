namespace Pkt.Models
{
    public class EmployeeQuestionViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public int TotalQuestion { get; set; }
        public int Correct_Ans { get; set; }
        public int Not_Attempt { get; set; }
        public int Wrong_Ans { get; set; }
    }
}
