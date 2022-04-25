namespace MovieAnalyzer.Models;

public class Character
{
    public string MovieId { get; set; }
    public string PersonId { get; set; }
    public int? Age { get; set; }
    public string CharacterName { get; set; }
    public int? ImportanceLevel { get; set; }

    public string? ActorName { get; set; }
    public PersonGender? Gender  {get ;set; }



    public Character(string movieId, string actorId, int releaseYear, int birthYear, string characterName, string actorname, PersonGender gender, int level)
    {
        MovieId = movieId;
        PersonId = actorId;
        Age = releaseYear - birthYear;
        CharacterName = characterName;
        ImportanceLevel = level;
        ActorName = actorname;
        Gender = gender;
    }

    public Character(string movieId, string actorId, string actorName, string characterName, PersonGender gender)
    {
        MovieId = movieId;
        PersonId = actorId;
        ActorName = actorName;
        CharacterName = characterName;
        Gender = gender;
    }



}



