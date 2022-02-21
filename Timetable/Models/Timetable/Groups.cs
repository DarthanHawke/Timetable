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
    [Display(Name = "6 группа")]
    Group6,
    [Display(Name = "61 группа")]
    Group61,
    [Display(Name = "62 группа")]
    Group62,
    [Display(Name = "71 группа")]
    Group71,
    [Display(Name = "9 группа")]
    Group9,
    [Display(Name = "91 группа")]
    Group91,
    [Display(Name = "10 группа")]
    Group10
}

public class СhoiceViewModel
{
    public Courses PeriodCourses { get; set; }
    public Groups PeriodGroups { get; set; }

    public string ViewCourses(Courses сourses)
    {
        string cours = "";
        switch (сourses)
        {
            case Courses.Cours1:
                cours = "1";
                break;
            case Courses.Cours2:
                cours = "2";
                break;
            case Courses.Cours3:
                cours = "3";
                break;
            case Courses.Cours4:
                cours = "4";
                break;
            case Courses.Cours5:
                cours = "5";
                break;
            case Courses.CoursM1:
                cours = "магистратура 1";
                break;
            case Courses.CoursM2:
                cours = "магистратура 2";
                break;
        }
        return cours;
    }

    public string ViewGroups(Groups groups)
    {
        string group = "";
        switch (groups)
        {
            case Groups.Group1:
                group = "1";
                break;
            case Groups.Group2:
                group = "2";
                break;
            case Groups.Group3:
                group = "3";
                break;
            case Groups.Group4:
                group = "4";
                break;
            case Groups.Group5:
                group = "5";
                break;
            case Groups.Group6:
                group = "6";
                break;
            case Groups.Group61:
                group = "61";
                break;
            case Groups.Group62:
                group = "62";
                break;
            case Groups.Group71:
                group = "71";
                break;
            case Groups.Group9:
                group = "9";
                break;
            case Groups.Group91:
                group = "91";
                break;
            case Groups.Group10:
                group = "10";
                break;
        }
        return group;
    }
}

