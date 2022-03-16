using TMDbLib.Objects.Movies;


namespace Movies; 

    class Program
    {
    static async Task Main(string[] args)
    {
        TMDb TMDbclient = new TMDb();
        var movie = await TMDbclient.GetMovieWithCastByIdAsync(766798);
        printMovie(movie);

        Gremlin connector = new Gremlin();
        connector.InsertMovie(movie);
        // var movies = await TMDbclient.GetTopPopularMoviesByYearAsync(2021, 20);
        // printMovies(movies);





    }

    private static void printMovie(Movie movie) =>
        Console.WriteLine($"Movie: {movie.Id} - {movie.Title}, ({movie.ReleaseDate!.Value.Year}), {movie.VoteCount} " +
        $"\nOverview: {movie.Overview}");      

    private static void printMovieWithCast(Movie movie) =>
        Console.WriteLine($"Movie: {movie.Id} - {movie.Title}, ({movie.ReleaseDate!.Value.Year}), {movie.Popularity} " +
        $"\nOverview: {movie.Overview}" +
        $"\nCast: {movie.Credits.Cast[0].Name} as {movie.Credits.Cast[0].Character}"); 

    private static void printMovies(List<Movie> movies)
    {
        int index = 1;
        foreach(Movie movie in movies)
        {
            Console.WriteLine($"{index++}.- {movie.Title} ({movie.ReleaseDate!.Value.Year})");
            Console.WriteLine($"Popularity: {movie.Popularity}, Num Votes: {movie.VoteCount}, Vote Avg:: {movie.VoteAverage}  ");
        }
    }



}
