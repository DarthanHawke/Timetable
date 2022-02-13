using System.ComponentModel.DataAnnotations;


namespace Timetable.Models
{
    public class Classrooms
    {
        [Key]
        public int Id_Class { get; set; } //id аудитории
        public string TypeClass { get; set; } = ""; // тип аудитории
        public int Capacity { get; set; } // вместимость аудитории
        public int Projector { get; set; } // наличие проектора
        public int Board { get; set; } // наличие доски
        public int Equipment { get; set; } // дополнительные условия
    }
}
