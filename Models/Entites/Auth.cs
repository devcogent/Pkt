using System.ComponentModel.DataAnnotations;

namespace Pkt.Models.Entites
{
    public class Auth
    {

        public int Id { get; set; }
        [MaxLength(50)]
        public string EmpID { get; set; }= string.Empty;
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string PasswordKey { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(10)]
        public string Phone { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? OH { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? AH { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? TH { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? QH { get; set; } = string.Empty;
      
        public int CMID { get; set; }
        [MaxLength(100)]
        public string Department {  get; set; } = string.Empty;
        [MaxLength(100)]
        public string Process { get; set; } = string.Empty;
        [MaxLength(100)]
        public string ClientName { get; set; } = string.Empty;
        
        public int ClientID { get; set; } 

        public DateTime DOJ {  get; set; }
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Designation { get; set; } = string.Empty;

        public byte Status { get; set; } = 0;
     
        public int? DesignationID { get; set; }
        public int? BatchID { get; set; } 

        public byte userLogin {  get; set; } = 0;

        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [MaxLength(50)]
        public string? UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set;} = DateTime.Now;



    }


}
