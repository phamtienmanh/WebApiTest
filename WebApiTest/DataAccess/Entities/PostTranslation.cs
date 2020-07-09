using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiTest.DataAccess.Entities
{
    public class PostTranslation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }
        [Required]
        [MaxLength(20)]
        public string LanguageCode { get; set; }
        [MaxLength(150)]
        public string Title { get; set; }
        public string Description { get; set; }
    }
}