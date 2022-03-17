using TMDbLib.Client;
using TMDbLib.Objects.Collections;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Search;

namespace Movies;

public class TMDb
{
    private static string TMDBtoken => Environment.GetEnvironmentVariable("TMDBtoken") ?? throw new ArgumentException("Missing env var: TMDBtoken");
    private static TMDbClient client = new TMDbClient(TMDBtoken);

    public async Task<Movie> GetMovieWithCastByIdAsync(int id)
    {
        Movie movie = await client.GetMovieAsync(id, MovieMethods.Credits);
        movie.Title = movie.Title.Replace("'", "\\'");
        
        return movie;
    }
    
    public async Task<Person> GetCastByIdAsync(int id)
    {
        Person actor = await client.GetPersonAsync(id);
        actor.Name = actor.Name.Replace("'", "\\'");
        return actor;
    }

    public List<Person> GetDirectorsFromMovie(Movie movie)
    {
        var directorCrew = movie.Credits.Crew.Where(c => c.Job =="Director" && c.Department == "Directing");
        var directors = GetPeopleFromCrew(directorCrew);
        return directors;

    }

    private List<Person> GetPeopleFromCrew(IEnumerable<Crew> directorCrew)
    {
        List<Person> directores = new List<Person>();
        foreach (var d in directorCrew)
        {
            Person director = new Person();
            director.Name = d.Name;
            director.Id = d.Id;
            director.Gender = d.Gender;

            directores.Add(director);
        }

        return directores;
    }

    public async Task<List<Movie>> GetTopPopularMoviesByYearAsync(int year, int numMovies)
    {   
        List<Movie> movies = new List<Movie>();

        int page = 1;
        int totalPages = 0;
        int index = 1;
        bool cancel = false;
        do
        {
            var dMovie = await client.DiscoverMoviesAsync()
                                .WherePrimaryReleaseDateIsAfter(new DateTime(year, 01, 01))
                                .WherePrimaryReleaseDateIsBefore(new DateTime(year+1, 01, 01))
                                .OrderBy(DiscoverMovieSortBy.VoteCountDesc)
                                .Query(page: page);

            totalPages = dMovie.TotalPages;

            foreach (var movie in dMovie.Results)
            {
                movies.Add(GetMovieFromSearchMovie(movie));                         
                if(++index> numMovies) 
                {
                    cancel = true;
                    break;                    
                }                
            }    

            page++;
            cancel = (index > numMovies);

        } while(page <= totalPages && !cancel);              
     
        return movies;
    }

    private Movie GetMovieFromSearchMovie (SearchMovie searchmovie)
    {
        Movie movie = new Movie();
        movie.Id = searchmovie.Id;
        movie.Title = searchmovie.Title;
        movie.ReleaseDate = searchmovie.ReleaseDate;
        movie.Popularity = searchmovie.Popularity;
        movie.VoteAverage = searchmovie.VoteAverage;
        movie.VoteCount = searchmovie.VoteCount;
        movie.Overview = searchmovie.Overview;
        return movie;
    }

     
}
