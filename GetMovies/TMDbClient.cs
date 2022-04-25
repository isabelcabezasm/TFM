using TMDbLib.Client;
using TMDbLib.Objects.Collections;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Search;

namespace MovieAnalyzer;

public class TMDbClient
{
    private static string TMDBtoken => Environment.GetEnvironmentVariable("TMDBtoken") ?? throw new ArgumentException("Missing env var: TMDBtoken");
    private static TMDbLib.Client.TMDbClient client = new TMDbLib.Client.TMDbClient(TMDBtoken);

    public async Task<MovieAnalyzer.Models.Movie> GetMovieWithCastByIdAsync(string movieid)
    {
        int id = 0;
        bool result = int.TryParse(movieid.Substring(0,movieid.Length-1), out id);
        if (result)
        {
            Movie movie = await client.GetMovieAsync(id, MovieMethods.Credits);
            movie.Title = movie.Title.Replace("'", "\\'");

            //transform TMDbLib.Objects.Movies.Movie to MovieAnalyzer.Model

            // Get Characters
            var characters = await GetCharactersFromMovieAsync(movie);

            //Get directors
            var directors = GetDirectorsFromMovie(movie);

            //Get Production Countries
            var prodCountries = GetProductionCountriesFromMovie(movie);

            //Get Genres
            var genres = GetGenresFromMovie(movie);


            // Create MovieAnalyzer.Models from TMDbLib.Objects.Movies.Movie
            MovieAnalyzer.Models.Movie movieDTO = new Models.Movie (movieId: movie.Id.ToString()+"m", 
                                                                    title: movie.Title, 
                                                                    characters: characters, 
                                                                    directors: directors,
                                                                    prodCountries: prodCountries,
                                                                    genres: genres,
                                                                    overview: movie.Overview,
                                                                    release_year: movie.ReleaseDate != null ? movie.ReleaseDate.Value.Year : 0,
                                                                    vote_avg: movie.VoteAverage, 
                                                                    vote_count: movie.VoteCount);
            return movieDTO;

        }
        else
        {
            throw new InvalidCastException($"Error in the conversion of {movieid} to int");
        }

       
    }
    
    public async Task<MovieAnalyzer.Models.Person> GetCastByIdAsync(int id)
    {
        Person actor = await client.GetPersonAsync(id);
        actor.Name = actor.Name.Replace("'", "\\'");

        MovieAnalyzer.Models.Person person = new Models.Person(
                                                            (actor.Id.ToString()+"p"),
                                                            actor.Name, 
                                                            actor.Birthday != null ? actor.Birthday.Value.Year : 0,
                                                            (PersonGender)actor.Gender
                                                            );

        return person;
    }

    /*
    *   Returns  movies.
    *   Ordered by Popular, return the movies with index between beginIndex and endIndex
    */
    public async Task<List<MovieAnalyzer.Models.Movie>> GetTopPopularMoviesByYearAsync(int year, int beginIndex, int endIndex)
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
                if (index < endIndex && index > beginIndex)
                {
                    movies.Add(GetMovieFromSearchMovie(movie));                         
                    if(++index > endIndex) 
                    {
                        cancel = true;
                        break;                    
                    } 

                }
                else
                {
                    index++;
                }
                
            }    

            page++;
            cancel = (index > endIndex);

        } while(page <= totalPages && !cancel);              
     

        // transform TMDbLib.Objects.Movies into MovieAnalyzer.Models.Movie
        List<MovieAnalyzer.Models.Movie> returnMovies = new List<Models.Movie>();
        List<MovieAnalyzer.Models.Character> characters = new List<Models.Character>();
        
        //TODO: CHECK if arrives characters!!!
        foreach (var movie in movies)
        {
            MovieAnalyzer.Models.Movie myMovie = new Models.Movie(movie.Id.ToString()+"m", 
                                                                  movie.Title, 
                                                                  characters, 
                                                                  movie.Overview, 
                                                                  movie.ReleaseDate != null? movie.ReleaseDate.Value.Year: 0, 
                                                                  movie.VoteAverage,
                                                                  movie.VoteCount);
            returnMovies.Add(myMovie);

        }
        return returnMovies;

    }
   
   
    public async Task<List<MovieAnalyzer.Models.Movie>> GetTopPopularMoviesByYearAsync(int year, int numMovies)
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


          // transform TMDbLib.Objects.Movies into MovieAnalyzer.Models.Movie
        List<MovieAnalyzer.Models.Movie> returnMovies = new List<Models.Movie>();
        List<MovieAnalyzer.Models.Character> characters = new List<Models.Character>();
        
        foreach (var movie in movies)
        {
            MovieAnalyzer.Models.Movie myMovie = new Models.Movie(movie.Id.ToString()+"m", 
                                                                  movie.Title, 
                                                                  characters, 
                                                                  movie.Overview, 
                                                                  movie.ReleaseDate != null? movie.ReleaseDate.Value.Year: 0, 
                                                                  movie.VoteAverage,
                                                                  movie.VoteCount);
            returnMovies.Add(myMovie);

        }
        return returnMovies;

    }


    private List<MovieAnalyzer.Models.ProductionCountry> GetProductionCountriesFromMovie(Movie movie)
    {
        var countries = new List<MovieAnalyzer.Models.ProductionCountry>();
        foreach(var pc in movie.ProductionCountries)
        {
            countries.Add(new MovieAnalyzer.Models.ProductionCountry(pc.Iso_3166_1, pc.Name));
        }
                
        return countries;
    }

    private List<MovieAnalyzer.Models.Genre> GetGenresFromMovie(Movie movie)
    {
        List<MovieAnalyzer.Models.Genre> genres = new List<MovieAnalyzer.Models.Genre>();

        foreach ( var g in movie.Genres)
        {
            MovieAnalyzer.Models.Genre genre = new MovieAnalyzer.Models.Genre(g.Id.ToString()+"g", g.Name);
            genres.Add(genre);
        }        
        return genres;
    }

    private async Task<List<Models.Character>> GetCharactersFromMovieAsync(Movie movie)
    {
        var characters = new List<Models.Character>();

        for (int i = 0; i <10 && i < movie.Credits.Cast.Count; i++)
        {
            var cast = movie.Credits.Cast[i];
            var person = await GetCastByIdAsync(cast.Id);                

            if(movie.ReleaseDate != null && person.BirthYear != 0 && person.BirthYear != null)
            {
                var character = new Models.Character(movie.Id.ToString()+"m", 
                                                    cast.Id.ToString()+"p", 
                                                    movie.ReleaseDate!.Value.Year, 
                                                    person.BirthYear?? 0, 
                                                    cast.Character.Replace("'", "\\'"), 
                                                    cast.Name,
                                                    (PersonGender)cast.Gender,
                                                    i, 
                                                    person);
            
                characters.Add(character);
            }  
        }

        return characters;

    }
    
    private List<MovieAnalyzer.Models.Person> GetDirectorsFromMovie(Movie movie)
    {
        var directors = GetPeopleFromCrew(movie.Credits.Crew.Where(c => c.Job =="Director" && c.Department == "Directing"));               
        return directors;
    }

    private List<MovieAnalyzer.Models.Person> GetPeopleFromCrew(IEnumerable<Crew> directorCrew)
    {
        List<MovieAnalyzer.Models.Person> directores = new List<MovieAnalyzer.Models.Person>();

        foreach (var d in directorCrew)
        {
            MovieAnalyzer.Models.Person director = new MovieAnalyzer.Models.Person(d.Id.ToString()+"p", 
                                                                                   d.Name,
                                                                                   (PersonGender)d.Gender);   

            directores.Add(director);
        }

        return directores;
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
