using System.ComponentModel.DataAnnotations;
using System;


namespace Timetable.Models
{
    public class TimetableU
    {
        public DateTime Date { get; set; } //дата поставленной пары
        public int Id_Lesson { get; set; } //id предмета
        public int Id_Class { get; set; } //id аудитории
        public int Id_Teacher { get; set; } //id преподавателя
        public int Id_User { get; set; } //id пользователя
        public int Id_Group { get; set; } //id группы

        public CourseU CourseU { get; set; }
    }

    public class CourseU
    {
        [Key]
        public int Id_Group { get; set; } //id группы
        public string Course { get; set; } // курс группы
    }
}
