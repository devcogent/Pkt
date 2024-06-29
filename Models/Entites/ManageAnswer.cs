namespace Pkt.Models.Entites
{
    public class ManageAnswer
    {
        public int Id { get; set; }
        public string  Cm_Id { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string PKTName { get; set; } = string.Empty;
        public string QuestionId { get; set; } = string.Empty;
        public string AnsOption { get; set; } = string.Empty;
        public string CurrectAns { get;set; } = string.Empty;
        public DateTime CreatedOn {  get; set; } = DateTime.Now;

    }
}
