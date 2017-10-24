using System.Collections;

public class PokemonTeam
{
    Pokemon[] teamMembers = new Pokemon[3];

    public void setMembers(Pokemon p1, Pokemon p2, Pokemon p3)
    {
        teamMembers[0] = p1;
        teamMembers[1] = p2;
        teamMembers[2] = p3;
    }

    public Pokemon[] getMembers()
    {
        return teamMembers;
    }
}