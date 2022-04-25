namespace MovieAnalyzer.Models;
public class Person
{

    public string PersonId { get; set; }
    public string Name { get; set; }
    public int? BirthYear { get; set; }    
    public PersonGender Gender { get; set; } 


    public Person(string personId, string name,  PersonGender gender)
    {
        PersonId = personId;
        Name = name;
        Gender = gender;
    }

    public Person(string personId, string name, int birthyear, PersonGender gender)
    {
        PersonId = personId;
        Name = name;
        BirthYear = birthyear;
        Gender = gender;
    }
}
