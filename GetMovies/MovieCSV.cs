using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using MovieAnalyzer.Models;

namespace MovieAnalyzer; 

/***
* The main objective of this class is  open / read/ save  CSV files 
* Interaction with CSV files 
***/ 
public class MovieCSV
{
    private GremlinClient gclient;
    public MovieCSV()
    {
        gclient = new GremlinClient();  
    }     

    /* Given a path file (path) this method read it and extract the list of rows*/
    public List<T> OpenCSVFileToRead<T>(string path)
    {        
        //Open file
        using var streamReader = File.OpenText(path);
        using var csvReader = new CsvReader(streamReader, GetCsvConfiguration());
        var filerows = csvReader.GetRecords<T>();

        //load rows into a list
        List<T> rows = new List<T>();
        foreach(var row in filerows)
        {
            rows.Add(row);
        }

        return rows;
    }

    /* For Testing: 
    *  Get adjetives saved into the DB and save them into a CSV file
    */
    public void SaveAdjetivesAndAgeCSVFile()
    {       

        PersonGender gender = PersonGender.Male;
        var characters = gclient.GetCharactersWithAdjetives(gender);
        var rows = GetRows(characters, gender);
        SaveRows(rows);

    }

    private List<AdjetiveAndAgeCSVRow> GetRows(List<TempCharacter> characters, PersonGender gender)
    {
        List<AdjetiveAndAgeCSVRow> totalRows = new List<AdjetiveAndAgeCSVRow>();
        foreach(var character in characters)
        {
            var releaseyear = gclient.GetMovieReleaseYear(character.movieId);

            foreach (var adjetive in character.adjetives)
            {
                AdjetiveAndAgeCSVRow row = new AdjetiveAndAgeCSVRow
                {
                    year = releaseyear,
                    age = character.age, 
                    gender = gender.ToString(),
                    adj = adjetive
                };
                
                totalRows.Add(row);
            }
        }

        return totalRows;
    }
    
    private void SaveRows(List<AdjetiveAndAgeCSVRow> rows)
    {
        using var fs = new StreamWriter($"CSV\\adjetives_male.csv");
        using var csvWriter = new CsvWriter(fs, GetCsvConfiguration());
        AddHeadersAdjetivesFile(csvWriter);

        foreach (var row in rows)
        {
            csvWriter.WriteField(row.year);
            csvWriter.WriteField(row.age);
            csvWriter.WriteField(row.gender);
            csvWriter.WriteField(row.adj);
            csvWriter.NextRecord();
        }           

        csvWriter.Flush();
    }
    
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

    private static void AddHeadersAdjetivesFile(CsvWriter csvWriter)
    {
        csvWriter.WriteField("year");
        csvWriter.WriteField("age");
        csvWriter.WriteField("gender");
        csvWriter.WriteField("adj");        
        csvWriter.NextRecord();
    }

}
