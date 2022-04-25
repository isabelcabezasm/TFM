
using MovieAnalyzer.Models;
namespace MovieAnalyzer; 


public class MovieETL
{
    private TMDbClient tmdbclient;
    private GremlinClient connector;

    public MovieETL()
    {
        tmdbclient = new TMDbClient();
        connector = new GremlinClient();
    }  



    public async Task ReadAndWriteMoviesAsync(int year, int num, List<string> moviesError)
    {       

        var movies = await tmdbclient.GetTopPopularMoviesByYearAsync(year, num);
        int index =0; 

        foreach (var m in movies)
        {
            index++;
            Console.WriteLine(year + " - " + index);
            try
            {
                if(connector.MovieExists(m.MovieId))
                {
                    printWarning($"Movie: {m.MovieId} {m.Title} already exists");
                    continue;
                }

                var movie = await tmdbclient.GetMovieWithCastByIdAsync(m.MovieId);                  
                connector.InsertMovie(movie);
            
                
                // Insert directors 
                if(movie.Directors != null)
                {
                    foreach(var director in movie.Directors)
                    {
                        connector.InsertDirector(director);
                        connector.InsertDirection(movie.MovieId, director.PersonId);
                    }
                }                

                //Insert Countries
                if(movie.ProductionCountries != null)
                {
                    foreach(var country in movie.ProductionCountries)
                    {
                        connector.InsertCountry(country);
                        connector.InsertProduction(movie.MovieId, country.Code);
                    }
                }                    

                //Insert Genres
                if(movie.Genres != null)
                {
                    foreach(var genre in movie.Genres)
                    {
                        connector.InsertGenre(genre);
                        connector.InsertClassification(movie.MovieId, genre.GenreId);
                    }
                }

                // Insert Characters and Actors
                if(movie.Characters != null)
                {
                    foreach(var character in movie.Characters)
                    {
                        if (character.Actor != null)
                        {
                            connector.InsertCast(character.Actor);
                            connector.InsertInterpretation(character);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                moviesError.Add(m.MovieId);
                printError(ex.Message, m.MovieId);
            }

            
        }


    }

    


    private static void printError(String message, string movieId)
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
