using System.ComponentModel.DataAnnotations;


namespace Timetable.Models
{
    public class GroupU
    {
        [Key]
        public int Id_Group { get; set; } //id группы
        public int Course { get; set; } // курс группы
        public int Number { get; set; } // колличество человек в группе
        public int NumberGroup { get; set; } // номер группы
    }
}
