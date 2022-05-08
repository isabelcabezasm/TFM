using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MovieAnalyzer.Models;

namespace MovieAnalyzer; 

public class MovieRoles
{
    private GremlinClient gclient;
    private MovieCSV csv;

    public MovieRoles()
    {
        csv = new MovieCSV();
        gclient = new GremlinClient();        
    }

    public void InsertAdjetivesFromCharacterCSVFile()
    {
         //Get Rows
        var filerows = csv.OpenCSVFileToRead<CharacterAdjetivesCSVRow>("CSV\\individual_adjetives_with_personid.csv");
        foreach (var row in filerows)
        {
            Console.WriteLine(row.movieid + " - " + row.actorid + " - " + row.adj);            
            gclient.InsertAdjetiveToCharacter(row.movieid + "m", row.actorid + "p", row.adj);
        }
    }     
    public void InsertAdjetivesFromCSVFile()
    {
         //Get Rows
        var filerows = csv.OpenCSVFileToRead<AdjetivesCSVRow>("CSV\\all_adjetives_female.csv");
        foreach (var row in filerows)
        {
            Console.WriteLine(row.year + " - " + row.movieid + " - " + gclient.GetMovieTitle(row.movieid+ "m") + " -  " + row.adj + " - " + row.noun);            
            gclient.InsertAdjetiveToProtagonist(row.movieid + "m", row.adj, row.noun, PersonGender.Female);
        }
    }  

}

