namespace Movies;

public class Character
{
    public int MovieId { get; set; }
    public int PersonId { get; set; }
    public int? Age { get; set; }
    public string CharacterName { get; set; }
    public int? ImportanceLevel { get; set; }

    public string? ActorName { get; set; }
    public PersonSex? Sex  {get ;set; }



    public Character(int movieId, int actorId, int releaseYear, int birthYear, string characterName, int level)
    {
        MovieId = movieId;
        PersonId = actorId;
        Age = releaseYear - birthYear;
        CharacterName = characterName;
        ImportanceLevel = level;
    }

    public Character(int movieId, int actorId, string actorName, string characterName, PersonSex sex)
    {
        MovieId = movieId;
        PersonId = actorId;
        ActorName = actorName;
        CharacterName = characterName;
        Sex = sex;
    }


    public enum PersonSex
    {
        Unknown = 0,
        Female = 1,
        Male = 2,
        NonBinary = 3
    }

}



