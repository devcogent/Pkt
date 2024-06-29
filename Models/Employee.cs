namespace Pkt.Models
{
    public class Employee
    {
        public string EmployeeID { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int cm_id { get; set; }  
        public string emp_status { get; set; } = string.Empty;
        public string client_name { get; set; } = string.Empty;  
        public string QH { get; set; } = string.Empty;
        public string AH { get; set; } = string.Empty;
        public string Process { get; set; } = string.Empty;
        public string sub_process { get; set; } = string.Empty;
        public string clientname { get; set; } = string.Empty;
        public string designation { get; set; } = string.Empty;
        public string emp_level { get; set; } = string.Empty;
        public DateTime DOJ { get; set; } 
        public string Gender { get; set; } = string.Empty;
        public int des_id { get; set; }
        public int status { get; set; }
        public string TH { get; set; } = string.Empty;
        public string OH { get; set; } = string.Empty;
        public string ReportTo { get; set; } = string.Empty;
        public object BatchID { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
    }
}

