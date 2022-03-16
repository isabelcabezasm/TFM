using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies;

    public class Character
    {
        public int MovieId;
        public int PersonId;
        public int Age;
        public string CharacterName;
        public int ImportanceLevel;

        public Character(int movieId, int actorId, int releaseYear, int birthYear, string characterName, int level)
        {
            MovieId = movieId;
            PersonId = actorId;
            Age = releaseYear - birthYear;
            CharacterName = characterName;
            ImportanceLevel = level;
        }



    }
