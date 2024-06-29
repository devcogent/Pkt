namespace Pkt.Models
{
    public class ViewModelPkt 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TestDuration { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string NoQuestion { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public string Cm_Id { get; set; } = string.Empty;
        public string PKTFor { get; } = string.Empty;
        public string PKTForText { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedOn { get; set; } = DateTimeOffset.Now.ToString();
        public string UpdatesBy { get; set; } = string.Empty;
        public string UpdatesOn { get; set; } = DateTimeOffset.Now.ToString();
    }
}
