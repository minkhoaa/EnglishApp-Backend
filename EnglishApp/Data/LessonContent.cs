using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishApp.Data
{
    public class LessonContent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContentId { get; set; }
        public int LessonId { get; set; }
        public string ContentType { get; set; }

        public string ContentText{ get; set; }
        public int SortOrder { get; set; }

        public DateTime CreatedAt{ get; set; }
        public DateTime LastUpdatedAt{ get; set; }

        public Lesson Lesson { get; set; }


    }
}
