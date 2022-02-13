using System.ComponentModel.DataAnnotations;


namespace Timetable.Models
{
    public class Lesson
    {
        [Key]
        public int Id_Lesson { get; set; } //id предмета
        public string TypeLesson { get; set; } = ""; // тип предмета
        public string Name { get; set; } = ""; // название предмета
        public string Cyclicality { get; set; } = ""; // цикличность предмета
    }
}
