
namespace MovieAnalyzer; 

class Program
{
    /* This is the Orchestrator class.
    This application has three main functionalities.
    It is used for ETL (extract from TMDB, transfrom and load into DB) - option 1.
    It is used for saving movie information into a CSV file for being analyzed by python code running in notebooks - option 2
    It is used for load a CSV file with adjetives, that it is the result of the notebook jupyter - option 3. */

    static async Task Main(string[] args)
    {      

        Console.WriteLine("Select an option: ");
        Console.WriteLine("1. Get movies from TMDB and save them in Cosmos DB");
        Console.WriteLine("2. Save movie overviews in a CSV for text analysis.");
        Console.WriteLine("3. Load adjetives into characters");
        Console.WriteLine("4. Load adjetives into protagonists");
        Console.WriteLine("5. Save adjetives and ages into a csv file");
        Console.WriteLine("Other. Exit program");
        var option = 5; //Console.Read();

        if( option == 1)
        {

            Console.WriteLine("You have chosen option 1");

            var moviesWithError = new List<string>();
            var movieetl = new MovieETL();
            
            //timer
            var watch = new System.Diagnostics.Stopwatch();            
            watch.Start();

            //year + number of movies for year. 
            await movieetl.ReadAndWriteMoviesAsync(2021, 3, moviesWithError);
            Console.WriteLine("Movies with error!! :");
            Console.WriteLine(string.Join(", ", moviesWithError));

            //stop timer
            watch.Stop();
            double time = watch.ElapsedMilliseconds/60000;
            Console.WriteLine($"Execution Time: {time} min");


        }
        else if (option == 2)
        {
            Console.WriteLine("You have chosen option 2");

            //this will save info into a CSV File
            var MovieCSV = new MovieCSV();
        }
        else if (option == 3)
        {
            //this will read a CSV file with the adjetives and insert them into DB
            Console.WriteLine("You have chosen option 3");
            MovieRoles roles = new MovieRoles();
            roles.InsertAdjetivesFromCharacterCSVFile();
        }
        else if (option == 4)
        {
            //test purposes  
            Console.WriteLine("You have chosen option 4");
            MovieRoles roles = new MovieRoles();
            roles.InsertAdjetivesFromCSVFile();
        }
        else if (option == 5)
        {
            //test purposes  
            Console.WriteLine("You have chosen option 4");
            MovieCSV csv = new MovieCSV();
            csv.SaveAdjetivesAndAgeCSVFile();
        }
        else 
        {
            System.Environment.Exit(0);  
        }      
    }
}


