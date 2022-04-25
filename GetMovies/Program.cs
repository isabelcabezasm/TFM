
namespace MovieAnalyzer; 

class Program
{

    static async Task Main(string[] args)
    {      

        Console.WriteLine("Select an option: ");
        Console.WriteLine("1. Get movies from TMDB and save them in Cosmos DB");
        Console.WriteLine("2. Save movie overviews in a CSV for test analysis.");
        Console.WriteLine("3. Load adjetives into characters");
        Console.WriteLine("Other. Exit program");
        var option = 1; //Console.Read();

        if( option == 1)
        {

            Console.WriteLine("You have chosen option 1");

            var moviesWithError = new List<string>();
            var movieetl = new MovieETL();

            var watch = new System.Diagnostics.Stopwatch();            
            watch.Start();

            await movieetl.ReadAndWriteMoviesAsync(1970, 300, moviesWithError);
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
            throw new NotImplementedException("Not yet");
        }
        else 
        {
            System.Environment.Exit(0);  
        }      
    }
}


