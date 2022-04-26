using System.Net.WebSockets;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json;
using MovieAnalyzer.Models;

namespace MovieAnalyzer;

public class GremlinClient
{
    
    private static string Host => Environment.GetEnvironmentVariable("Host") ?? throw new ArgumentException("Missing env var: Host");

    private static string PrimaryKey => Environment.GetEnvironmentVariable("PrimaryKey") ?? throw new ArgumentException("Missing env var: PrimaryKey");
    private static string Database => Environment.GetEnvironmentVariable("DatabaseName") ?? throw new ArgumentException("Missing env var: DatabaseName");

    private static string Container => Environment.GetEnvironmentVariable("ContainerName") ?? throw new ArgumentException("Missing env var: ContainerName");

    private static int Port => 443;
    private static bool EnableSSL => true;

    private static Gremlin.Net.Driver.GremlinClient gremlinClient;


    // Constructor
    public GremlinClient()
    {
        string containerLink = "/dbs/" + Database + "/colls/" + Container;
        Console.WriteLine($"Connecting to: host: {Host}, port: {Port}, container: {containerLink}, ssl: {EnableSSL}");
        var gremlinServer = new GremlinServer(Host, Port, enableSsl: EnableSSL, 
                                                username: containerLink, 
                                                password: PrimaryKey);

        ConnectionPoolSettings connectionPoolSettings = new ConnectionPoolSettings()
        {
            MaxInProcessPerConnection = 10,
            PoolSize = 30, 
            ReconnectionAttempts= 3,
            ReconnectionBaseDelay = TimeSpan.FromMilliseconds(500)
        };

        var webSocketConfiguration =
            new Action<ClientWebSocketOptions>(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(10);
            });

        gremlinClient = new Gremlin.Net.Driver.GremlinClient(
                            gremlinServer, 
                            new GraphSON2Reader(), 
                            new GraphSON2Writer(),
                            Gremlin.Net.Driver.GremlinClient.GraphSON2MimeType, 
                            connectionPoolSettings, 
                            webSocketConfiguration);

    }


    /**Insert nodes **/
    public void InsertMovie(MovieAnalyzer.Models.Movie movie)
    {
        // we should have the modified ID CHECK
        if(!MovieExists(movie.MovieId))
        {               

            

            string query =  $"g.addV('movie')"+
                                $".property('id', '{movie.MovieId}')" + 
                                $".property('title', '{movie.Title}')"+
                                $".property('release_year', {movie.ReleaseYear})"+
                                $".property('vote_avg', {movie.VoteAverage})"+
                                $".property('vote_count', {movie.VoteCount})"+
                                $".property('pk', 'pk')" ;            

            SendRequest(query);
        }
    }
          
    public void InsertCast(MovieAnalyzer.Models.Person cast)
    {
        InsertPerson(cast, "cast");       
    }

    public void InsertDirector(MovieAnalyzer.Models.Person cast)
    {
        string type = "director";
        if(!PersonExists(cast.PersonId, type))
        {
            string query =  $"g.addV('{type}')"+
                            $".property('id', '{cast.PersonId}')" + 
                            $".property('name', '{cast.Name}')"+
                            $".property('gender', '{cast.Gender}')"+
                            $".property('pk', 'pk')" ;
        
            SendRequest(query);
        }  
        else
        {
            // Console.WriteLine($"{cast.Name} already exists.");
        }       
    }
    
    public void InsertCountry(MovieAnalyzer.Models.ProductionCountry country)
    {
        if(!CountryExists(country.Code))
        {
            string query =  $"g.addV('country')"+
                            $".property('id', '{country.Code}')" + 
                            $".property('name', '{country.Name}')"+
                            $".property('pk', 'pk')" ;  

            SendRequest(query);
        }
        
    }

    public void InsertGenre(MovieAnalyzer.Models.Genre genre)
    {
        if(!GenreExists(genre.GenreId))
        {
            string query =  $"g.addV('genre')"+
                            $".property('id', '{genre.GenreId}')" + 
                            $".property('name', '{genre.Name}')"+
                            $".property('pk', 'pk')" ;  

            SendRequest(query);
        }
    }


    /**Insert edges **/
    public void InsertInterpretation(MovieAnalyzer.Models.Character character)
    {
            if(!InterpretationExists(character.MovieId, character.PersonId))
            {

            string query =  $"g.V('{character.PersonId}')"+
                            $".addE('plays')"+
                            $".to(g.V('{character.MovieId}'))"+
                            $".property('importance_level', {character.ImportanceLevel})" +
                            $".property('age', {character.Age})" +
                            $".property('character_name', '{character.CharacterName}')";

            SendRequest(query);    
            }                   
    }

    public void InsertDirection(string movieId, string directorId)
    {
            if(!DirectionExists(movieId, directorId))
            {
                string query =  $"g.V('{movieId}')"+
                    $".addE('directedBy')"+
                    $".to(g.V('{directorId}'))";            

                SendRequest(query); 
            }
    }

    public void InsertProduction(string movieId, string countryCode)
    {

            if(!ProductionExists(movieId, countryCode))
            {
            string query =  $"g.V('{movieId}')"+
                $".addE('producedIn')"+
                $".to(g.V('{countryCode}'))";            

            SendRequest(query);  
            }
        
    }

    public void InsertClassification(string movieId, string genreId)
    {
        if(!ClassificationExists(movieId, genreId))
        {
            string query =  $"g.V('{movieId}')"+
                $".addE('classification')"+
                $".to(g.V('{genreId}'))";            

            SendRequest(query); 
        }
        
    }

    public void InsertAdjetiveToCharacter(string movieid, string actorid, string adj)
    {
        string adjetive = "adj";
        string queryEdge = $" g.V().hasId('{actorid}').outE('plays').as('e').inV().hasId('{movieid}').select('e').properties().hasKey('{adjetive}')";

        // TODO: if adjetive doesn't exist
        
        
        //look for adjetives already includes in the edge
        bool found = false;
        int index = 1;

        while(!found) 
        {
            adjetive = "adj" + index;
            queryEdge = $" g.V().hasId('{actorid}').outE('plays').as('e').inV().hasId('{movieid}').select('e').properties().hasKey('{adjetive}')";
            var resultSet = SubmitRequest(gremlinClient, queryEdge).Result;
            found = (resultSet.Count == 0);            
            if(!found)
            {
                index++;
            }
        }
        adjetive = "adj" + index;
        queryEdge = $" g.V().hasId('{actorid}').outE('plays').as('e').inV().hasId('{movieid}').select('e').property('{adjetive}', '{adj}')";
        SendRequest(queryEdge); 
    }

    public void InsertAdjetiveToProtagonist(string movieid, string adj, string noun, PersonGender gender)
    {
        //get most important character of gender "gender"
        TempCharacter first = GetFirstCharacter(movieid, gender);

        InsertAdjetiveToCharacter(first.movieId, first.actorId, adj);
        InsertAdjetiveToCharacter(first.movieId, first.actorId, noun);

    }

    //given a movie (movieid) 
    //returns the first (most important) actor or actress (depending of the parameter gender)
    private TempCharacter GetFirstCharacter(string movieid, PersonGender gender)
    {
        
        List<TempCharacter> tempCharacters = new List<TempCharacter>();
        
        var queryGetCharacters = $"g.V().hasId('{movieid}').inE('plays')";
        var charactersResultSet = SubmitRequest(gremlinClient, queryGetCharacters).Result;

        foreach(var character in charactersResultSet)
        {
            //for each character, take the actor/actress: 
            var actorId = character["outV"];
            var edgeId = character["id"];
            var properties = character["properties"];
            var importance_level = int.Parse(properties["importance_level"].ToString());

            var queryPerson = $"g.V().hasId('{actorId}')";
            var personResultSet = SubmitRequest(gremlinClient, queryPerson).Result;

            foreach(var person in personResultSet)
            {
                var properties2 = person["properties"]["gender"];
                foreach(var p in properties2)
                {
                    var personGender = p["value"].ToString();
                    if (personGender == gender.ToString())
                    {
                        //save this actor/character
                        TempCharacter c = new TempCharacter {
                            movieId = movieid, 
                            actorId= actorId, 
                            level = importance_level
                        };
                        tempCharacters.Add(c);                        
                    }
                }
            }
        }
        var first = tempCharacters.OrderBy(m=>m.level).First();

        return first;
    }

    public string GetMovieTitle(string movieid)
    {
        string title = String.Empty;
        //search movie: 
        var query = $"g.V().hasLabel('movie').has('id', '{movieid}')";
        var resultSet = SubmitRequest(gremlinClient, query).Result;
                
        foreach(var result in resultSet)
        {
            
            var properties = result["properties"]["title"]; 
            foreach (var property in properties)
            {
                title = property["value"].ToString();
            }         
        }
        return title;
    }

    public void Clean()
    {
        string query =  $"g.V().drop()";
        SendRequest(query);
    }

    public bool MovieExists(string movieId)
    {
        var query = $"g.V().hasLabel('movie').has('id', '{movieId}')";
        
        // Console.WriteLine(String.Format("Running this query: {0}", query));
        var resultSet = SubmitRequest(gremlinClient, query).Result;
        return (resultSet.Count > 0);
    }

    public bool NodeExists(string id)
    {
        var query = $"g.V().has('id', '{id}')";
        
        // Console.WriteLine(String.Format("Running this query: {0}", query));
        var resultSet = SubmitRequest(gremlinClient, query).Result;
        return (resultSet.Count > 0);
    }

    private void InsertPerson(MovieAnalyzer.Models.Person cast, string type)
    {
        if(!PersonExists(cast.PersonId, type))
        {
            string query =  $"g.addV('{type}')"+
                            $".property('id', '{cast.PersonId}')" + 
                            $".property('name', '{cast.Name}')"+
                            $".property('gender', '{cast.Gender}')"+
                            $".property('pk', 'pk')" ;
        
            SendRequest(query);
        }  
        else
        {
            // Console.WriteLine($"{cast.Name} already exists.");
        }          
    }

    private bool PersonExists(string personid, string type)
    {
        var query = $"g.V().has('id', '{personid}')";            
        // Console.WriteLine(String.Format("Running this query: {0}", query));
        var resultSet = SubmitRequest(gremlinClient, query).Result;
        return (resultSet.Count > 0);
        
    }
    
    private bool CountryExists(string countryCode)
    {
        var query = $"g.V().hasLabel('country').has('id', '{countryCode}')";
        
        // Console.WriteLine(String.Format("Running this query: {0}", query));
        var resultSet = SubmitRequest(gremlinClient, query).Result;
        return (resultSet.Count > 0);
        
    }

    private bool GenreExists(string genreId)
    {
        var query = $"g.V().hasLabel('genre').has('id', '{genreId}')";
        
        // Console.WriteLine(String.Format("Running this query: {0}", query));
        var resultSet = SubmitRequest(gremlinClient, query).Result;
        return (resultSet.Count > 0);
    }

    private bool DirectionExists(string movieId, string directorId)
    {
        string label = "directedBy";
        
        string queryEdge = $"g.V('{movieId}').outE('{label}').V('{directorId}')";         
        // Console.WriteLine(String.Format("Running this query: {0}", queryEdge));
        var resultSet = SubmitRequest(gremlinClient, queryEdge).Result;
        return (resultSet.Count > 0);            

    }

    private bool InterpretationExists (string movieId, string personId)
    {
        string label = "plays";
        
        string queryEdge = $"g.V('{movieId}').inE('{label}').outV().hasId('{personId}')";

        // Console.WriteLine(String.Format("Running this query: {0}", queryEdge));
        var resultSet = SubmitRequest(gremlinClient, queryEdge).Result;
        return (resultSet.Count > 0);     

    }
    
    private bool ClassificationExists(string movieId, string genreId)
    {
        
        string label = "classification";
        
        string queryEdge = $"g.V('{movieId}').outE('{label}').V('{genreId}')";         
        // Console.WriteLine(String.Format("Running this query: {0}", queryEdge));
        var resultSet = SubmitRequest(gremlinClient, queryEdge).Result;
        return (resultSet.Count > 0);   

    }

    private bool ProductionExists(string movieId, string countryCode)
    {
        string label = "producedIn";
        
        string queryEdge = $"g.V('{movieId}').outE('{label}').V('{countryCode}')";         
        // Console.WriteLine(String.Format("Running this query: {0}", queryEdge));
        var resultSet = SubmitRequest(gremlinClient, queryEdge).Result;
        return (resultSet.Count > 0); 

    }       





    // Gremlin Implementation details
    private void SendRequest(String query)
    {
        // Console.WriteLine(String.Format("Running this query: {0}", query));
        var resultSet = SubmitRequest(gremlinClient, query).Result;
        if (resultSet.Count > 0)
        {
            // Console.WriteLine("\tResult:");
            foreach (var result in resultSet)
            {
                // The vertex results are formed as Dictionaries with a nested dictionary for their properties
                string output = JsonConvert.SerializeObject(result);
                // Console.WriteLine($"\t{output}");
            }
            // Console.WriteLine();
        }

    }
    
    private static Task<ResultSet<dynamic>> SubmitRequest(Gremlin.Net.Driver.GremlinClient gremlinClient,string query)
    {
        try
        {
            return gremlinClient.SubmitAsync<dynamic>(query);
        }
        catch (ResponseException e)
        {
            Console.WriteLine("\tRequest Error!");

            // Print the Gremlin status code.
            Console.WriteLine($"\tStatusCode: {e.StatusCode}");

            // On error, ResponseException.StatusAttributes will include the common StatusAttributes for successful requests, as well as
            // additional attributes for retry handling and diagnostics.
            // These include:
            //  x-ms-retry-after-ms         : The number of milliseconds to wait to retry the operation after an initial operation was throttled. This will be populated when
            //                              : attribute 'x-ms-status-code' returns 429.
            //  x-ms-activity-id            : Represents a unique identifier for the operation. Commonly used for troubleshooting purposes.
            PrintStatusAttributes(e.StatusAttributes);
            Console.WriteLine($"\t[\"x-ms-retry-after-ms\"] : { GetValueAsString(e.StatusAttributes, "x-ms-retry-after-ms")}");
            Console.WriteLine($"\t[\"x-ms-activity-id\"] : { GetValueAsString(e.StatusAttributes, "x-ms-activity-id")}");

            throw;
        }
    }

    private static Task<ResultSet<dynamic>> SubmitRequest(Gremlin.Net.Driver.GremlinClient gremlinClient, KeyValuePair<string, string> query)
    {
        try
        {
            return gremlinClient.SubmitAsync<dynamic>(query.Value);
        }
        catch (ResponseException e)
        {
            Console.WriteLine("\tRequest Error!");

            // Print the Gremlin status code.
            Console.WriteLine($"\tStatusCode: {e.StatusCode}");

            // On error, ResponseException.StatusAttributes will include the common StatusAttributes for successful requests, as well as
            // additional attributes for retry handling and diagnostics.
            // These include:
            //  x-ms-retry-after-ms         : The number of milliseconds to wait to retry the operation after an initial operation was throttled. This will be populated when
            //                              : attribute 'x-ms-status-code' returns 429.
            //  x-ms-activity-id            : Represents a unique identifier for the operation. Commonly used for troubleshooting purposes.
            PrintStatusAttributes(e.StatusAttributes);
            Console.WriteLine($"\t[\"x-ms-retry-after-ms\"] : { GetValueAsString(e.StatusAttributes, "x-ms-retry-after-ms")}");
            Console.WriteLine($"\t[\"x-ms-activity-id\"] : { GetValueAsString(e.StatusAttributes, "x-ms-activity-id")}");

            throw;
        }
    }

    private static void PrintStatusAttributes(IReadOnlyDictionary<string, object> attributes)
    {
        Console.WriteLine($"\tStatusAttributes:");
        Console.WriteLine($"\t[\"x-ms-status-code\"] : { GetValueAsString(attributes, "x-ms-status-code")}");
        Console.WriteLine($"\t[\"x-ms-total-server-time-ms\"] : { GetValueAsString(attributes, "x-ms-total-server-time-ms")}");
        Console.WriteLine($"\t[\"x-ms-total-request-charge\"] : { GetValueAsString(attributes, "x-ms-total-request-charge")}");
    }
    
    public static string GetValueAsString(IReadOnlyDictionary<string, object> dictionary, string key)
    {
        return JsonConvert.SerializeObject(GetValueOrDefault(dictionary, key));
    }

    public static object GetValueOrDefault(IReadOnlyDictionary<string, object> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            return dictionary[key];
        }

        return null;
    }

}


public class TempCharacter
{
    public string movieId = String.Empty;
    public string actorId = String.Empty;
    public int level = 0;

}