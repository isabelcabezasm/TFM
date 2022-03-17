using System.Net.WebSockets;
using Movies;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;

namespace Movies;

    public class Gremlin
    {
        
        private static string Host => Environment.GetEnvironmentVariable("Host") ?? throw new ArgumentException("Missing env var: Host");


        private static string PrimaryKey => Environment.GetEnvironmentVariable("PrimaryKey") ?? throw new ArgumentException("Missing env var: PrimaryKey");
        private static string Database => Environment.GetEnvironmentVariable("DatabaseName") ?? throw new ArgumentException("Missing env var: DatabaseName");

        private static string Container => Environment.GetEnvironmentVariable("ContainerName") ?? throw new ArgumentException("Missing env var: ContainerName");        
        private static int Port => 443;
        private static bool EnableSSL => true;

        private static GremlinClient gremlinClient; 

        public Gremlin()
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

            gremlinClient = new GremlinClient(
                                gremlinServer, 
                                new GraphSON2Reader(), 
                                new GraphSON2Writer(), 
                                GremlinClient.GraphSON2MimeType, 
                                connectionPoolSettings, 
                                webSocketConfiguration);

        }

        public void InsertMovie(Movie movie)
        {
            string query =  $"g.addV('movie')"+
                                $".property('id', '{movie.Id}')" + 
                                $".property('title', '{movie.Title}')"+
                                $".property('release_year', {movie.ReleaseDate!.Value.Year})"+
                                $".property('vote_avg', {movie.VoteAverage})"+
                                $".property('vote_count', {movie.VoteCount})"+
                                $".property('pk', 'pk')" ;            

            SendRequest(query);
        }
                
        public void InsertCast(Person cast)
        {
            InsertPerson(cast, "cast");       
        }

        public void InsertDirector(Person cast)
        {
            InsertPerson(cast, "director");         
        }


        private void InsertPerson(Person cast, string type)
        {
            if(!PersonExists(cast.Id))
            {
                string query =  $"g.addV('{type}')"+
                                $".property('id', '{cast.Id}')" + 
                                $".property('name', '{cast.Name}')"+
                                $".property('sex', '{cast.Gender}')"+
                                $".property('pk', 'pk')" ;
            
                SendRequest(query);
            }            
        }

        public void InsertInterpretation(Character character)
        {
            string query =  $"g.V('{character.PersonId}')"+
                             $".addE('plays')"+
                             $".to(g.V('{character.MovieId}'))"+
                             $".property('importance_level', {character.ImportanceLevel})" +
                             $".property('age', {character.Age})";            

            SendRequest(query);                       
        }


        public void InsertDirection(int movieId, int directorId)
        {
            string query =  $"g.V('{directorId}')"+
                    $".addE('directedBy')"+
                    $".to(g.V('{movieId}'))";            

            SendRequest(query);    
        }
        private bool PersonExists(int id)
        {
            var query = $"g.V().hasLabel('cast').has('id', '{id}')";
            
            Console.WriteLine(String.Format("Running this query: {0}", query));
            var resultSet = SubmitRequest(gremlinClient, query).Result;
            return (resultSet.Count > 0);
            
        }

        private void SendRequest(String query)
        {
            Console.WriteLine(String.Format("Running this query: {0}", query));
            var resultSet = SubmitRequest(gremlinClient, query).Result;
            if (resultSet.Count > 0)
            {
                Console.WriteLine("\tResult:");
                foreach (var result in resultSet)
                {
                    // The vertex results are formed as Dictionaries with a nested dictionary for their properties
                    string output = JsonConvert.SerializeObject(result);
                    Console.WriteLine($"\t{output}");
                }
                Console.WriteLine();
            }

        }


        
        public void Clean()
        {
            string query =  $"g.V().drop()";
            SendRequest(query);
        }

        public void Query()
        {         


            // </defineClientandServerObjects>
                
                // <executeQueries>
                foreach (var query in gremlinQueries)
                {
                    Console.WriteLine(String.Format("Running this query: {0}: {1}", query.Key, query.Value));

                    // Create async task to execute the Gremlin query.
                    var resultSet = SubmitRequest(gremlinClient, query).Result;
                    if (resultSet.Count > 0)
                    {
                        Console.WriteLine("\tResult:");
                        foreach (var result in resultSet)
                        {
                            // The vertex results are formed as Dictionaries with a nested dictionary for their properties
                            string output = JsonConvert.SerializeObject(result);
                            Console.WriteLine($"\t{output}");
                        }
                        Console.WriteLine();
                    }

                    // Print the status attributes for the result set.
                    // This includes the following:
                    //  x-ms-status-code            : This is the sub-status code which is specific to Cosmos DB.
                    //  x-ms-total-request-charge   : The total request units charged for processing a request.
                    //  x-ms-total-server-time-ms   : The total time executing processing the request on the server.
                    PrintStatusAttributes(resultSet.StatusAttributes);
                    Console.WriteLine();
                }


        }

        // Gremlin queries that will be executed.
        // <defineQueries>
        private static Dictionary<string, string> gremlinQueries = new Dictionary<string, string>
        {
            //borrar todo
            { "Cleanup",        "g.V().drop()" },
            //añadimos a thomas
            { "AddVertex 1",    "g.addV('person').property('id', 'thomas').property('firstName', 'Thomas').property('age', 44).property('pk', 'pk')" },
            //añadimos a mary
            { "AddVertex 2",    "g.addV('person').property('id', 'mary').property('firstName', 'Mary').property('lastName', 'Andersen').property('age', 39).property('pk', 'pk')" },
            //añadimos a ben
            { "AddVertex 3",    "g.addV('person').property('id', 'ben').property('firstName', 'Ben').property('lastName', 'Miller').property('pk', 'pk')" },
            //añadimos a robin
            { "AddVertex 4",    "g.addV('person').property('id', 'robin').property('firstName', 'Robin').property('lastName', 'Wakefield').property('pk', 'pk')" },
            // thomas conoce a mary
            { "AddEdge 1",      "g.V('thomas').addE('knows').to(g.V('mary'))" },
            // thomas conoce a ben 
            { "AddEdge 2",      "g.V('thomas').addE('knows').to(g.V('ben'))" },
            // ben conoce a robin
            { "AddEdge 3",      "g.V('ben').addE('knows').to(g.V('robin'))" },
            // Thomas tiene 45 años
            //{ "UpdateVertex",   "g.V('thomas').property('age', 45)" },

            // cuantas personas?
            { "CountVertices",  "g.V().count()" },
            //personas con más de 40
            { "Filter Range",   "g.V().hasLabel('person').has('age', gt(40))" },

            //todos los nombres de pila
            { "Project",        "g.V().hasLabel('person').values('firstName')" },
            // todos las personas, ordenados por los nombres de pila 
            { "Sort",           "g.V().hasLabel('person').order().by('firstName', decr)" },

            //personas que conoce thomas.
            { "Traverse",       "g.V('thomas').out('knows').hasLabel('person')" },
            //personas que conocen a personas que conoces a thomas
            { "Traverse 2x",    "g.V('thomas').out('knows').hasLabel('person').out('knows').hasLabel('person')" },

            // "camino" desde thomas hasta robin
            { "Loop",           "g.V('thomas').repeat(out()).until(has('id', 'robin')).path()" },

            //{ "DropEdge",       "g.V('thomas').outE('knows').where(inV().has('id', 'mary')).drop()" },
            { "CountEdges",     "g.E().count()" },
            //{ "DropVertex",     "g.V('thomas').drop()" },
        };
        // </defineQueries>

        private static Task<ResultSet<dynamic>> SubmitRequest(GremlinClient gremlinClient,string query)
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

        private static Task<ResultSet<dynamic>> SubmitRequest(GremlinClient gremlinClient, KeyValuePair<string, string> query)
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
