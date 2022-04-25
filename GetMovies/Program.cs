using TMDbLib.Objects.Movies;

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Movies; 

class Program
{

    static async Task Main(string[] args)
    {      
        Film peli = new Film();
        if (peli != null)
        {
            for(int year = 2021; year >= 1970; year--)
            {
                var movies = await peli.getMoviesDTO(year, 300);

                // save as CSV file: 
                using var fs = new StreamWriter($"movies_{year}.csv");
                using var csvWriter = new CsvWriter(fs, GetCsvConfiguration());
                AddHeaders(csvWriter);

                foreach (var movie in movies)
                {
                    csvWriter.WriteField(movie.MovieId);
                    csvWriter.WriteField(movie.Title);

                    if(movie.Characters != null)
                    {
                        for (int i = 0; i<4; i++)                    
                        {
                            if(movie.Characters.Count() > i)
                            {
                                AddCharacter(movie.Characters[i], csvWriter);
                            }
                            else
                            {
                                Character character = new Character(0, 0, "", "", Character.PersonSex.Unknown);
                                AddCharacter(character, csvWriter);
                            }                            
                        }
                    }

                    AddOverview(movie.Overview, csvWriter);
                    csvWriter.NextRecord();
                }           

                csvWriter.Flush();
            }
            
        }
    }

    private static CsvConfiguration GetCsvConfiguration()
    {
        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            Comment = '#',
            AllowComments = true,
            Delimiter = ";",
        };
        return csvConfig;
    }


    private static void AddOverview(string overview, CsvWriter csvWriter)
    {
        csvWriter.WriteField(overview.Replace(";", ","));
    }

    private static void AddCharacter(Character character, CsvWriter csvWriter)
    {
        csvWriter.WriteField(character.CharacterName);
        csvWriter.WriteField(character.ActorName);
         csvWriter.WriteField(character.Sex);
        csvWriter.WriteField(character.PersonId);
       
    }

    private static void AddHeaders(CsvWriter csvWriter)
    {
        csvWriter.WriteField("MovieId");
        csvWriter.WriteField("Title");
        
        csvWriter.WriteField("Character1");
        csvWriter.WriteField("PersonName1");
        csvWriter.WriteField("Sex1");
        csvWriter.WriteField("ActorId1");
        
        csvWriter.WriteField("Character2");
        csvWriter.WriteField("PersonName2");
        csvWriter.WriteField("Sex2");
        csvWriter.WriteField("ActorId2");

        csvWriter.WriteField("Character3");
        csvWriter.WriteField("PersonName3");
        csvWriter.WriteField("Sex3");
        csvWriter.WriteField("ActorId3");

        csvWriter.WriteField("Character4");
        csvWriter.WriteField("PersonName4");
        csvWriter.WriteField("Sex4");
        csvWriter.WriteField("ActorId4");

        csvWriter.WriteField("Overview");
        csvWriter.NextRecord();
    }
}


