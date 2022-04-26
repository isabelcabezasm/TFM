using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MovieAnalyzer.Models;

namespace MovieAnalyzer; 

public class MovieRoles
{
    public void OpenFile()
    {
        //Open file
        using var streamReader = File.OpenText("CSV\\individual_adjetives_with_personid.csv");
        using var csvReader = new CsvReader(streamReader, GetCsvConfiguration());
        var filerows = csvReader.GetRecords<CSVRow>();
        GremlinClient gclient = new GremlinClient();

        foreach (var row in filerows)
        {
            Console.WriteLine(row.movieid + " - " + row.actorid + " - " + row.adj);            
            gclient.InsertAdjetiveToCharacter(row.movieid + "m", row.actorid + "p", row.adj);
        }
    }


    //for each row
        //row:  year,movieid,charactername1,charactername2,charactername3,sex,actorid,adj
        // get "plays" edge between 
        // row[moviei] or row[1] AND 
        // row[actorid] or row[6]
        // add property "adjetive", value: row[adj] OR row[7]


        private static CsvConfiguration GetCsvConfiguration()
        {
            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = true,
                Comment = '#',
                AllowComments = true,
                Delimiter = ",",
            };
            return csvConfig;
        }
}

internal class Row
{
}