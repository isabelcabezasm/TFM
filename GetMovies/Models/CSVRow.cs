namespace MovieAnalyzer.Models;
public class CharacterAdjetivesCSVRow
{   
    //row:  year,movieid,charactername1,charactername2,charactername3,sex,actorid,adj
     
    public int year {get; set; }
    public    string movieid {get; set; }
    public    string charactername1 {get; set; }
    public    string charactername2 {get; set; }
    public    string charactername3 {get; set; }
    public    string sex {get; set; }
    public    string actorid {get; set; }
    public    string adj {get; set; }
    
}

public class AdjetivesCSVRow
{   
    //row:  year,movieid,charactername1,charactername2,charactername3,sex,actorid,adj
     
    public int year {get; set; }
    public    string movieid {get; set; }
    public    string adj {get; set; }
    public    string noun {get; set; }
    
}

public class AdjetiveAndAgeCSVRow
{
    public int year {get; set; }
    public int age {get; set; }
    public string gender {get; set; }
    public string adj {get; set; }
    
}
