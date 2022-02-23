using System.ComponentModel.DataAnnotations;
using System;


namespace Timetable.Models
{
    public class TimetableU
    {
        [Key]
        public int Id_Date { get; set; } //id даты
        public string Week { get; set; } //день недели поставленной пары
        public string Time { get; set; } //время поставленной пары
        public string Integrity { get; set; } //целостность(числитель/знаменатель) поставленной пары
        public int Id_Lesson { get; set; } //id предмета
        public int Id_Class { get; set; } //id аудитории
        public int Id_Teacher { get; set; } //id преподавателя
        public int Id_Group { get; set; } //id группы
    }

    public class NewTimetable
    {
        [Key]
        public int Id_Date { get; set; } //id даты
        public string Week { get; set; } //день недели поставленной пары
        public string Time { get; set; } //время поставленной пары
        public string Integrity { get; set; } //целостность(числитель/знаменатель) поставленной пары
        public int Course { get; set; }
        public int NumberGroup { get; set; }
        public string FIOteacher { get; set; }
        public string NameLesson { get; set; }
        public string NumberClass { get; set; }
    }
}
