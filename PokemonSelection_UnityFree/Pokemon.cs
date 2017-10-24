using System.Collections;

public class Pokemon
{
    string pkmnName;
    string type1, type2;
    string[] attacks;
    

    public void setUp(string pokeString)
    {
        string[] subString = pokeString.Split(';');
        pkmnName = subString[0];
        string[] typeString = subString[1].Split(',');
        type1 = typeString[0];
        if(typeString.Length > 1)
        {
            type2 = typeString[1];
        }
        else
        {
            type2 = "none";
        }
        attacks = subString[2].Split(',');      
    }

    public string getName()
    {
        return pkmnName;
    }

    public string[] getAttacks()
    {
        return attacks;
    }

    public string getType1()
    {
        return type1;
    }

    public string getType2()
    {
        return type2;
    }
}
