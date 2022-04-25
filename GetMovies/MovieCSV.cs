using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using MovieAnalyzer.Models;

namespace MovieAnalyzer; 

public class MovieCSV
{
    public MovieCSV()
    {
    //    getMoviesAsync().Wait();
    }

    //     public async Task<List<MovieAnalyzer.Models.Movie>> getMoviesDTO(int year, int num)
    // {
    //     List<MovieAnalyzer.Models.Movie> moviesData = new List<MovieAnalyzer.Models.Movie>();

    //     var movies = await tmdbclient.GetTopPopularMoviesByYearAsync(year, num);

    //     foreach (var m in movies)
    //     {
    //         var movie = await tmdbclient.GetMovieWithCastByIdAsync(m.MovieId);
            
    //         // Characters' list
    //         List<Character> characters = new List<Character>();
    //         for (int i = 0; i < 10 && i < movie.Characters.Count; i++)
    //         {
    //                 var cast = movie.Characters[i];
    //                 var person = await tmdbclient.GetCastByIdAsync(cast.PersonId);

    //                 Character character = new Character(movie.MovieId,
    //                                                     cast.PersonId, 
    //                                                     cast.ActorName,                                                          
    //                                                     cast.CharacterName,
    //                                                     movie.ReleaseYear,
    //                                                     i);                            
                    
    //                 characters.Add(character);                    
    //         }
    //         moviesData.Add(m);
    //     }

    //     return moviesData;
    // }

    // private async Task getMoviesAsync()
    // {
    //     MovieETL etl = new MovieETL();
    //     if (etl != null)
    //     {
    //         for(int year = 2000; year >= 2000; year--)
    //         {
    //             var movies = await etl.getMoviesDTO(year, 3);

    //             // save as CSV file: 
    //             using var fs = new StreamWriter($"movies_{year}.csv");
    //             using var csvWriter = new CsvWriter(fs, GetCsvConfiguration());
    //             AddHeaders(csvWriter);

    //             foreach (var movie in movies)
    //             {
    //                 csvWriter.WriteField(movie.MovieId);
    //                 csvWriter.WriteField(movie.Title);

    //                 if(movie.Characters != null)
    //                 {
    //                     for (int i = 0; i<4; i++)                    
    //                     {
    //                         if(movie.Characters.Count() > i)
    //                         {
    //                             AddCharacter(movie.Characters[i], csvWriter);
    //                         }
    //                         else
    //                         {
    //                             Character character = new Character(0, 0, "", "", Character.PersonGender.Unknown);
    //                             AddCharacter(character, csvWriter);
    //                         }                            
    //                     }
    //                 }

    //                 AddOverview(movie.Overview, csvWriter);
    //                 csvWriter.NextRecord();
    //             }           

    //             csvWriter.Flush();
    //         }
            
    //     }

    // }
    // private static CsvConfiguration GetCsvConfiguration()
    // {
    //     var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
    //     {
    //         HasHeaderRecord = true,
    //         Comment = '#',
    //         AllowComments = true,
    //         Delimiter = ";",
    //     };
    //     return csvConfig;
    // }


    // private static void AddOverview(string overview, CsvWriter csvWriter)
    // {
    //     csvWriter.WriteField(overview.Replace(";", ","));
    // }

    // private static void AddCharacter(Character character, CsvWriter csvWriter)
    // {
    //     csvWriter.WriteField(character.CharacterName);
    //     csvWriter.WriteField(character.ActorName);
    //      csvWriter.WriteField(character.Gender);
    //     csvWriter.WriteField(character.PersonId);
       
    // }

    // private static void AddHeaders(CsvWriter csvWriter)
    // {
    //     csvWriter.WriteField("MovieId");
    //     csvWriter.WriteField("Title");
        
    //     csvWriter.WriteField("Character1");
    //     csvWriter.WriteField("PersonName1");
    //     csvWriter.WriteField("Gender1");
    //     csvWriter.WriteField("ActorId1");
        
    //     csvWriter.WriteField("Character2");
    //     csvWriter.WriteField("PersonName2");
    //     csvWriter.WriteField("Gender2");
    //     csvWriter.WriteField("ActorId2");

    //     csvWriter.WriteField("Character3");
    //     csvWriter.WriteField("PersonName3");
    //     csvWriter.WriteField("Gender3");
    //     csvWriter.WriteField("ActorId3");

    //     csvWriter.WriteField("Character4");
    //     csvWriter.WriteField("PersonName4");
    //     csvWriter.WriteField("Gender4");
    //     csvWriter.WriteField("ActorId4");

    //     csvWriter.WriteField("Overview");
    //     csvWriter.NextRecord();
    // }
    
}
