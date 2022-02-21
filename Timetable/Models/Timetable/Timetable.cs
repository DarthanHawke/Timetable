using System.ComponentModel.DataAnnotations;
using System;


namespace Timetable.Models
{
    public class TimetableU
    {
        [Key]
        public int Id_Date { get; set; } //id даты
        public DateTime Date { get; set; } //дата поставленной пары
        public int Id_Lesson { get; set; } //id предмета
        public int Id_Class { get; set; } //id аудитории
        public int Id_Teacher { get; set; } //id преподавателя
        public int Id_User { get; set; } //id пользователя
        public int Id_Group { get; set; } //id группы
    }
}
