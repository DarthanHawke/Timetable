using System.ComponentModel.DataAnnotations;


namespace Timetable.Models
{
    public class LessonU
    {
        [Key]
        public int Id_Lesson { get; set; } //id предмета
        public string TypeLesson { get; set; } = ""; // тип предмета
        public string Name { get; set; } = ""; // название предмета
    }
}
