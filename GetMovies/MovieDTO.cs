namespace Movies;

public class MovieDTO
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public List<Character>? Characters { get; set; }
    public string Overview { get; set; }

    public MovieDTO(int movieId, string title, List<Character> characters, string overview)
    {
        MovieId = movieId;
        Title = title;
        Characters = characters;
        Overview = overview;
    }


}