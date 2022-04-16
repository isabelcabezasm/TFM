using TMDbLib.Objects.Movies;

namespace Movies; 

class Program
{
    static async Task Main(string[] args)
    {      

        var watch = new System.Diagnostics.Stopwatch();
            
        watch.Start();

        List<int> moviesWithError = new List<int>();

        for (int year = 1970; year<=1979; year++)
        {
            await readAndWriteMoviesAsync(year, 300, moviesWithError);
        }
        

        Console.WriteLine("Movies with error!! :");
        Console.WriteLine(string.Join(", ", moviesWithError));

        watch.Stop();

        double time = watch.ElapsedMilliseconds/60000;

        Console.WriteLine($"Execution Time: {time} min");

        // connector.Clean();
    }

    private static async Task readAndWriteMoviesAsync(int year, int num, List<int> moviesError)
    {
        TMDb TMDbclient = new TMDb();
        Gremlin connector = new Gremlin();

        var movies = await TMDbclient.GetTopPopularMoviesByYearAsync(year, num);
        int index =0; 

        foreach (var m in movies)
        {
            index++;
            Console.WriteLine(year + " - " + index);
            try
            {
                if(connector.MovieExists(m.Id))
                {
                    printWarning($"Movie: {m.Id} {m.Title} already exists");
                    continue;
                }

                var movie = await TMDbclient.GetMovieWithCastByIdAsync(m.Id);                  
                // printMovie(index, movie);
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


                for (int i = 0; i <10 && i < movie.Credits.Cast.Count; i++)
                {
                    Cast cast = movie.Credits.Cast[i];

                    // espera 5 segundos para no reventar la API de tmdb
                    Thread.Sleep(1000); //will sleep for 5 sec

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
                moviesError.Add(m.Id);
                printError(ex.Message, m.Id);
            }

            
        }


    }

    private static void printMovie(int index, Movie movie) =>
        Console.WriteLine($"{index} Movie: {movie.Id} - {movie.Title}, ({movie.ReleaseDate!.Value.Year}), {movie.VoteCount} " +
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

    private static void printError(String message, int movieId)
    {
        var foregroundColor= Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error!!!!!! Película: " + movieId);
        Console.WriteLine(message);
        Console.ForegroundColor = foregroundColor;
    }

    private static void printWarning(String message)
    {
        var foregroundColor= Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = foregroundColor;
    }


}
