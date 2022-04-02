using TMDbLib.Objects.Movies;

namespace Movies; 

class Program
{
    static async Task Main(string[] args)
    {
        TMDb TMDbclient = new TMDb();
        Gremlin connector = new Gremlin();


        // connector.Clean();
        int[] moviesIdError = new int[] {131631, 864873, 672741, 664300, 603661, 785457, 747688 }; //done
        // var movies = await TMDbclient.GetTopPopularMoviesByYearAsync(2019, 300);
        int index =0; 

        foreach (var m in moviesIdError)
        {
            index++;
            Console.WriteLine(index + " - " + m.ToString() );
            try
            {
                if(connector.MovieExists(m))
                {
                    Console.WriteLine($"Movie: {m} already exists");
                    continue;
                }

                var movie = await TMDbclient.GetMovieWithCastByIdAsync(m);                  
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
                Console.WriteLine("Error!!!!!! Película: "+m);
                Console.WriteLine(ex.Message);
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

}
