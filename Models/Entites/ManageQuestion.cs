namespace Pkt.Models.Entites
{
    public class ManageQuestion
    {

        public int Id { get; set; }
        public string PKTName { get; set; } = string.Empty;
        public string UploadType { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Option1 {  get; set; } = string.Empty;
        public string Option2 {  get; set; } = string.Empty;
        public string Option3 { get; set; } = string.Empty;
        public string Option4 { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string CreatedBy {  get; set; } = string.Empty;
        public string UpdatedBy { get; set;} = string.Empty;
        public DateTime CreatedOn { get; set; } 
        public DateTime UpdatedOn { get; set; } 
       
             
    }
}
