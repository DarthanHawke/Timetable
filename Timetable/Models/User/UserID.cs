using System.ComponentModel.DataAnnotations;


namespace Timetable.Models
{
    public class UserID
    {
        [Key]
        public int Id_User { get; set; } //id пользователя
        public string Login { get; set; } = ""; // логин пользователя
        public string Password { get; set; } = ""; // пароль пользователя
        public string Access { get; set; } = ""; // тип пользователя
    }
}
