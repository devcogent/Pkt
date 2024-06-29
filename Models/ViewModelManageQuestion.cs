namespace Pkt.Models
{
    public class ViewModelManageQuestion
    {
        public int Id { get; set; }
        public string PKTName { get; set; } = string.Empty;
        public string UploadType { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Option1 { get; set; } = string.Empty;
        public string Option2 { get; set; } = string.Empty;
        public string Option3 { get; set; } = string.Empty;
        public string Option4 { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public string CreatedOn { get; set; } = DateTimeOffset.Now.ToString("dd-mm-yyyy HH:MM:ss");
        public string UpdatedOn { get; set; } = DateTimeOffset.Now.ToString("dd-mmd-yyyy HH:MM:ss");
    }
}
