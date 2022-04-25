namespace MovieAnalyzer.Models;


    public class Genre
    {
        public string GenreId { get; set; }

        public string Name { get; set; }

        public Genre(string genreid, string name)
        {
            GenreId = genreid;
            Name=name;
        }
    }
