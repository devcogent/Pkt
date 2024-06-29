using System.Data;

namespace Pkt.Models.Entites
{
    public class ManagePkt
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TestDuration { get; set; }
        public   DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; } 
        public int NoQuestion { get; set; } 
        public string Process { get; set; } = string.Empty;
        public int Cm_Id { get; set; } 
        public string PKTFor { get;} = string.Empty;
        public string PKTForText { get; set;} = string.Empty;
        public string Designation { get; set;} = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string UpdatesBy { get; set; } = string.Empty;
        public DateTime UpdatesOn { get; set;} = DateTime.Now;

    }
}
