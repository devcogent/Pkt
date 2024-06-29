using System;

namespace Pkt.Models.Entites
{
    public class CommonQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;

        public string Option1 { get; set; } = string.Empty;
        public string Option2 { get; set; } = string.Empty;
        public string Option3 { get; set; } = string.Empty;
        public string Option4 { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;
       
        public string CreatedBY { get; set; } = string.Empty;

        public string CreatedOn { get; set; } = DateTimeOffset.Now.ToString("dd-MM-yyy-HH:mm:ss");


        public string UpdatedBY { get; set;} = string.Empty;
        public string UpdatedOn { get; set;} = DateTimeOffset.Now.ToString("dd-MM-yyy-HH:mm:ss");


    }
}
