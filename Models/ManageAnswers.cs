namespace Pkt.Models
{
    public class ManageAnswers
    {
        public int Id { get; set; }
        public string Cm_Id { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string PKTName { get; set; } = string.Empty;
        public string QuestionId { get; set; } = string.Empty;
        public string AnsOption { get; set; } = string.Empty;
        public string CurrectAns { get; set; } = string.Empty;
        public string CreatedOn { get; set; } = DateTimeOffset.Now.ToString("dd-mm-yyyy hh:mm:ss");
    }
}
