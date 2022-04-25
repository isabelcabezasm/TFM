namespace MovieAnalyzer.Models;

public class Movie
{
    public string MovieId { get; set; }
    public string Title { get; set; }    
    public string Overview { get; set; }
    public int ReleaseYear { get; set; }
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }

    public List<Character>? Characters { get; set; }
    public List<Person>? Directors { get; set; }
    public List<Genre>? Genres{ get; set; }
    
    public List<ProductionCountry>? ProductionCountries { get; set; }

    public Movie(string movieId, string title, string overview, int release_year, double vote_avg, int vote_count)
    {
        MovieId = movieId;
        Title = title;
        Overview = overview;
        ReleaseYear = release_year;
        VoteAverage = vote_avg;
        VoteCount = vote_count;
    }

    public Movie(string movieId, string title, List<Character> characters, string overview, int release_year, double vote_avg, int vote_count)
    {
        MovieId = movieId;
        Title = title;
        Characters = characters;
        Overview = overview;
        ReleaseYear = release_year;
        VoteAverage = vote_avg;
        VoteCount = vote_count;
    }

    public Movie(string movieId, string title, 
                 string overview, int release_year, double vote_avg, int vote_count,
                 List<Character> characters, 
                 List<Person> directors, 
                 List<ProductionCountry> prodCountries, 
                 List<Genre> genres)
    {
        MovieId = movieId;
        Title = title;
        Characters = characters;
        Directors = directors;
        ProductionCountries = prodCountries;
        Genres = genres;
        Overview = overview;
        ReleaseYear = release_year;
        VoteAverage = vote_avg;
        VoteCount = vote_count;
    }


}