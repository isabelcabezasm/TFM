using TMDbLib.Objects.Movies;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Movies; 

class Program
{

    static async Task Main(string[] args)
    {      
        Film peli = new Film();
        if (peli != null)
        {
            for(int year = 2000; year >= 1970; year--)
            {
                var movies = await peli.getMoviesDTO(year, 300);
                string json = JsonSerializer.Serialize<List<MovieDTO>>(movies);
                File.WriteAllText($"movies_{year}.json", json);
            }
            
        }
            
        // connector.Clean();




    }
}


