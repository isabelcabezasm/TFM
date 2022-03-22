using TMDbLib.Objects.Movies;

namespace Movies; 

class Program
{
    static async Task Main(string[] args)
    {
        TMDb TMDbclient = new TMDb();
        Gremlin connector = new Gremlin();

        // connector.Clean();

        var movies = await TMDbclient.GetTopPopularMoviesByYearAsync(2021, 1000);
        foreach (var m in movies)
        {
            try
            {
                if(connector.MovieExists(m.Id))
                {
                    continue;
                }
                
                var movie = await TMDbclient.GetMovieWithCastByIdAsync(m.Id);                  
                printMovie(movie);
                connector.InsertMovie(movie);
            
                
                // Directors
                var directors = TMDbclient.GetDirectorsFromMovie(movie);
                foreach(var director in directors)
                {
                    connector.InsertDirector(director);
                    connector.InsertDirection(movie.Id, director.Id);
                }

                //Countries
                var productionCountries = TMDbclient.GetProductionCountriesFromMovie(movie);
                foreach(var country in productionCountries)
                {
                    connector.InsertCountry(country);
                    connector.InsertProduction(movie.Id, country.Code);
                }

                //Genres
                var movieGenres = TMDbclient.GetGenresFromMovie(movie);
                foreach(var genre in movieGenres)
                {
                    connector.InsertGenre(genre);
                    connector.InsertClassification(movie.Id, genre.Id);
                }


                for (int i = 0; i <10; i++)
                {
                    Cast cast = movie.Credits.Cast[i];

                    // espera 5 segundos para no reventar la API de tmdb
                    Thread.Sleep(5000); //will sleep for 5 sec

                    var person = await TMDbclient.GetCastByIdAsync(cast.Id);                

                    if(movie.ReleaseDate != null && person.Birthday != null)
                    {
                        connector.InsertCast(person);
                        Character character = new Character(movie.Id, 
                                                        cast.Id, 
                                                        movie.ReleaseDate!.Value.Year, 
                                                        person.Birthday!.Value.Year, 
                                                        cast.Character, i);
                    
                        connector.InsertInterpretation(character);
                    }  
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Error!!!!!! Película: " + m.Id);
                Console.WriteLine(ex.Message);
            }

            
        }
    }

    private static void printMovie(Movie movie) =>
        Console.WriteLine($"Movie: {movie.Id} - {movie.Title}, ({movie.ReleaseDate!.Value.Year}), {movie.VoteCount} " +
        $"\nOverview: {movie.Overview}");      

    private static void printMovieWithCast(Movie movie) =>
        Console.WriteLine($"Movie: {movie.Id} - {movie.Title}, ({movie.ReleaseDate!.Value.Year}), {movie.Popularity} " +
        $"\nOverview: {movie.Overview}" +
        $"\nCast: {movie.Credits.Cast[0].Name} as {movie.Credits.Cast[0].Character}"); 

    private static void printMovies(List<Movie> movies)
    {
        int index = 1;
        foreach(Movie movie in movies)
        {
            Console.WriteLine($"{index++}.- {movie.Title} ({movie.ReleaseDate!.Value.Year})");
            Console.WriteLine($"Popularity: {movie.Popularity}, Num Votes: {movie.VoteCount}, Vote Avg:: {movie.VoteAverage}  ");
        }
    }

}
