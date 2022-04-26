using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MovieAnalyzer.Models;

namespace MovieAnalyzer; 

public class MovieRoles
{
    public MovieRoles()
    {
        MovieCSV csv = new MovieCSV();
        GremlinClient gclient = new GremlinClient();

        //Get Rows
        var filerows = csv.OpenCSVFileToRead("CSV\\individual_adjetives_with_personid.csv");
        foreach (var row in filerows)
        {
            Console.WriteLine(row.movieid + " - " + row.actorid + " - " + row.adj);            
            gclient.InsertAdjetiveToCharacter(row.movieid + "m", row.actorid + "p", row.adj);
        }
    }       
}

