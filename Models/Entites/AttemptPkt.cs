namespace Pkt.Models.Entites
{
    public class AttemptPkt
    {
        public int Id { get; set; }
        public int Cm_Id { get; set; } 
        public int Status { get; set; } 
        public string PKTName { get; set; } = string.Empty;
        public string EmployeeId {  get; set; } = string.Empty;
        public string EmployeeName {  get; set; } = string.Empty;
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public string CreatedBy { get; set; } =string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
       public string UpdatedBy { get; set;} =string.Empty;
        public DateTime UpdatedOn {  get; set; } = DateTime.Now;
        public int Attempts {  get; set; } 

    }
}
