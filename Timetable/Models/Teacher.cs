using System.ComponentModel.DataAnnotations;


namespace Timetable.Models
{
    public class Teacher
    {
        [Key]
        public int Id_Teacher { get; set; } //id преподователя
        public string FIO { get; set; } = ""; // имя преподователя
        public string Specialization { get; set; } = ""; // специальность преподователя
        public string Department { get; set; } = ""; // кафедра преподователя
        public string Title { get; set; } = ""; // звание преподователя
    }
}
