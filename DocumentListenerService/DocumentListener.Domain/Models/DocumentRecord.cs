using System.ComponentModel.DataAnnotations;

namespace DocumentListenerService.Models
{
    public class DocumentRecord
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        public string file_name { get; set; }

        [Required]
        public string file_base64 { get; set; }

        public DateTime time_stamp { get; set; }
    }
}

