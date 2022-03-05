using System;
using System.Collections.ObjectModel;
using System.Linq;
using TMDbLib.Client;
using TMDbLib.Objects.Collections;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace ConsoleApp1
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            TMDbClient client = new TMDbClient("5e45879a14ac15292119ea08a76eab2b");
            GetMovie1(client);
            Console.WriteLine("---------------------------");
            await GetMovie2Async(client);
            Console.WriteLine("---------------------------");
            Get007Movies(client);
            Console.WriteLine("---------------------------");
            GetJamesBondMovies(client);

        }



        private static void GetMovie1(TMDbClient client)
        {
            //Simple example, getting the basic info for "A good day to die hard".

            Movie movie = client.GetMovieAsync(47964).Result;
            Console.WriteLine($"Movie name: {movie.Title}");
        }

        private static async System.Threading.Tasks.Task GetMovie2Async(TMDbClient client)
        {
            //Using the extra features of TMDb, we can fetch more info in one go(here we fetch casts as well as trailers):

            Movie movie = await client.GetMovieAsync(47964, MovieMethods.Credits | MovieMethods.Videos);

            Console.WriteLine($"Movie title: {movie.Title}");
            foreach (Cast cast in movie.Credits.Cast)
                Console.WriteLine($"{cast.Name} - {cast.Character}");

            Console.WriteLine();
            foreach (Video video in movie.Videos.Results)
                Console.WriteLine($"Trailer: {video.Type} ({video.Site}), {video.Name}");
        }

        private static void Get007Movies(TMDbClient client)
        {
            //It is likewise simple to search for people or movies, 
            //for example here we search for "007".This yields basically every James Bond film ever made:

            SearchContainer <SearchMovie> results = client.SearchMovieAsync("007").Result;

            Console.WriteLine($"Got {results.Results.Count:N0} of {results.TotalResults:N0} results");
            foreach (SearchMovie result in results.Results)
                Console.WriteLine(result.Title);
        }

        private static void GetJamesBondMovies(TMDbClient client)
        {
            //However, another way to get all James Bond movies, is to use the collection-approach.
            //TMDb makes collections for series of movies, such as Die Hard and James Bond.
            //I know there is one, so I will show how to search for the collection, and then list all movies in it:

            SearchContainer <SearchCollection> collectons = client.SearchCollectionAsync("James Bond").Result;
            Console.WriteLine($"Got {collectons.Results.Count:N0} collections");

            Collection jamesBonds = client.GetCollectionAsync(collectons.Results.First().Id).Result;
            Console.WriteLine($"Collection: {jamesBonds.Name}");
            Console.WriteLine();

            Console.WriteLine($"Got {jamesBonds.Parts.Count:N0} James Bond Movies");
            foreach (SearchMovie part in jamesBonds.Parts)
                Console.WriteLine(part.Title);
        }

    }
}
