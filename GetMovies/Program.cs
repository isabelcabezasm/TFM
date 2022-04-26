
namespace MovieAnalyzer; 

class Program
{

    static async Task Main(string[] args)
    {      

        Console.WriteLine("Select an option: ");
        Console.WriteLine("1. Get movies from TMDB and save them in Cosmos DB");
        Console.WriteLine("2. Save movie overviews in a CSV for text analysis.");
        Console.WriteLine("3. Load adjetives into characters");
        Console.WriteLine("4. Load adjetives into protagonists");
        Console.WriteLine("Other. Exit program");
        var option = 4; //Console.Read();

        if( option == 1)
        {

            Console.WriteLine("You have chosen option 1");

            var moviesWithError = new List<string>();
            var movieetl = new MovieETL();

            var watch = new System.Diagnostics.Stopwatch();            
            watch.Start();

            await movieetl.ReadAndWriteMoviesAsync(2021, 3, moviesWithError);
            Console.WriteLine("Movies with error!! :");
            Console.WriteLine(string.Join(", ", moviesWithError));
            watch.Stop();
            double time = watch.ElapsedMilliseconds/60000;
            Console.WriteLine($"Execution Time: {time} min");


        }
        else if (option == 2)
        {
            Console.WriteLine("You have chosen option 2");
            var MovieCSV = new MovieCSV();
        }
        else if (option == 3)
        {
            Console.WriteLine("You have chosen option 3");
            MovieRoles roles = new MovieRoles();
            roles.InsertAdjetivesFromCharacterCSVFile();
        }
        else if (option == 4)
        {
            Console.WriteLine("You have chosen option 4");
            MovieRoles roles = new MovieRoles();
            roles.InsertAdjetivesFromCSVFile();
        }
        else 
        {
            System.Environment.Exit(0);  
        }      
    }
}


