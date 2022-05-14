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

    /** 
    * With the method "OpenCSVFileToRead" from MovieCSV, it reads a CSV file, and extract the adjetives.
    * With the GremlinClient saves the adjetives from the CSV file into the DB.
    */
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

    /**
    * This was implemented just for test purposes.
    * Wanted to check how many character that appears in the overview (known their gender)
    * is/are the main character of the same gender. 
    **/
    
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

