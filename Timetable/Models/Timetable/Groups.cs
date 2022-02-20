using System.ComponentModel.DataAnnotations;

public enum Courses
{
    [Display(Name = "1 курс")]
    Cours1,
    [Display(Name = "2 курс")]
    Cours2,
    [Display(Name = "3 курс")]
    Cours3,
    [Display(Name = "4 курс")]
    Cours4,
    [Display(Name = "5 курс")]
    Cours5,
    [Display(Name = "1 курс магистратуры")]
    CoursM1,
    [Display(Name = "2 курс магистратуры")]
    CoursM2
}

public enum Groups
{
    [Display(Name = "1 группа")]
    Group1,
    [Display(Name = "2 группа")]
    Group2,
    [Display(Name = "3 группа")]
    Group3,
    [Display(Name = "4 группа")]
    Group4,
    [Display(Name = "5 группа")]
    Group5,
    [Display(Name = "61 группа")]
    Group61,
    [Display(Name = "62 группа")]
    Group62,
    [Display(Name = "71 группа")]
    Group71,
    [Display(Name = "9 группа")]
    Group9,
    [Display(Name = "91 группа")]
    Group91
}

public class СhoiceViewModel
{
    public Courses PeriodCourses { get; set; }
    public Groups PeriodGroups { get; set; }
}