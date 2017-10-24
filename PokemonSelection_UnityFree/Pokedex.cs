using System.Collections;
using System.Collections.Generic;
using System;

public class Pokedex
{
    private List<Pokemon> pokeDex = new List<Pokemon>();
    private List<PokemonTeam> testTeams = new List<PokemonTeam>();
    private List<PokemonTeam> evalTeams = new List<PokemonTeam>();

    public Pokedex()
    {
        Console.Write("Setting up dex");
        string[] setupString;
        setupString = System.IO.File.ReadAllText("configfiles/PokemonDatabase1.txt").Split('\n');
        List<Pokemon> pD = new List<Pokemon>();
        for (int i = 0; i < setupString.Length; i++)
        {
            Pokemon p = new Pokemon();
            p.setUp(setupString[i]);
            pD.Add(p);
        }
        pokeDex = pD;

        testTeams = loadTestTeams();
        evalTeams = loadEvalTeams();
    }

    public int getSize()
    {
        return pokeDex.Count;
    }

    public List<Pokemon> getDex()
    {
        return pokeDex;
    }

    public List<PokemonTeam> getTestTeams()
    {
        return testTeams;
    }

    public List<PokemonTeam> getEvalTeams()
    {
        return evalTeams;
    }

    //Conversion Functions


    public float[] createDnaUsingPkmn(PokemonTeam pT)
    {
        float[] pkmnDNA = new float[pokeDex.Count];
        for (int i = 0; i < pkmnDNA.Length; i++)
        {
            pkmnDNA[i] = 0;
        }
        foreach (Pokemon p in pT.getMembers())
        {
            int place = -1;
            foreach (Pokemon pN in pokeDex)
            {
                if (pN.getName() == p.getName())
                {
                    place = pokeDex.IndexOf(pN);
                }
            }
            pkmnDNA[place] = 1;
        }
        return pkmnDNA;
    }

    public PokemonTeam createTeamUsingDNA(float[] pD)
    {
        PokemonTeam pT = new PokemonTeam();
        Pokemon p1, p2, p3;

        int no1 = 0, no2 = 1, no3 = 2;
        for (int i = 0; i < pD.Length; i++)
        {
            if (pD[i] > pD[no1])
            {
                no1 = i;
            }
            else if (pD[i] > pD[no2])
            {
                no2 = i;
            }
            else if (pD[i] > pD[no3])
            {
                no3 = i;
            }
        }

        p1 = pokeDex[no1];
        p2 = pokeDex[no2];
        p3 = pokeDex[no3];

        pT.setMembers(p1, p2, p3);

        return pT;
    }

    private List<PokemonTeam> loadTestTeams()
    {
        List<PokemonTeam> tt = new List<PokemonTeam>();

        #region oldSetup
        /*
        PokemonTeam t1 = new PokemonTeam();
        PokemonTeam t2 = new PokemonTeam();
        PokemonTeam t3 = new PokemonTeam();
        PokemonTeam t4 = new PokemonTeam();
        PokemonTeam t5 = new PokemonTeam();
        PokemonTeam t6 = new PokemonTeam();
        Pokemon p1 = new Pokemon();
        Pokemon p2 = new Pokemon();
        Pokemon p3 = new Pokemon();
        p1.setUp("Salazzle;Fire,Poison;Fire,Dragon,Bug,Poison");
        p2.setUp("Arcanine;Fire;Normal,Fire,Electric,Dark");
        p3.setUp("Delphox;Fire,Psychic;Ghost,Fire,Psychic,Fairy");
        t1.setMembers(p1, p2, p3);
        p1.setUp("Charizard;Fire,Flying;Fire,Dragon,Flying,Dark");
        p2.setUp("Typhlosion;Fire;Fire,Normal,Electric,Psychic");
        p3.setUp("Rapidash;Fire;Bug,Fire,Flying,Normal");
        t2.setMembers(p1, p2, p3);
        p1.setUp("Infernape;Fire,Fight;Fire,Fight,Ground,Rock");
        p2.setUp("Houndoom;Fire,Dark;Fire,Dark,Ghost,Poison");
        p3.setUp("Lycanroc;Rock;Dark,Normal,Rock");
        t3.setMembers(p1, p2, p3);
        p1.setUp("Krookodile;Ground,Dark;Dark,Ground,Poison,Fight");
        p2.setUp("Archeops;Rock,Flying;Dragon,Ground,Rock,Flying");
        p3.setUp("Rapidash;Fire;Bug,Fire,Flying,Normal");
        t4.setMembers(p1, p2, p3);
        p1.setUp("Chandelure;Ghost,Fire;Psychic,Ghost,Fire,Dark");
        p2.setUp("Nidoking;Poison,Ground;Ground,Ice,Electric,Poison");
        p3.setUp("Magmortar;Fire;Fire,Psychic,Normal,Steel");
        t5.setMembers(p1, p2, p3);
        p1.setUp("Porygon-Z;Normal;Electric,Normal,Ghost,Dark");
        p2.setUp("Pyroar;Fire,Normal;Normal,Fire,Electric,Dark");
        p3.setUp("Gardevoir;Psychic,Fairy;Fairy,Psychic,Ghost,Fight");
        t6.setMembers(p1, p2, p3);
        tt.Add(t6);
        tt.Add(t5);
        tt.Add(t4);
        tt.Add(t3);
        tt.Add(t2);
        tt.Add(t1);
        */
        #endregion

        Console.Write("\nSetting up test teams");
        string[] setupString;
        setupString = System.IO.File.ReadAllText("configfiles/testTeams.txt").Split('\n');

        for (int i = 0; i < setupString.Length; i++)
        {
            PokemonTeam pt = new PokemonTeam();
            Pokemon[] p = new Pokemon[3];
            

            string[] substring = setupString[i].Split(';');
            for(int j = 0; j < substring.Length; j++)
            {
                foreach (Pokemon pN in pokeDex)
                {
                    if (pN.getName() == substring[j])
                    {
                        p[j] = pN;
                    }
                }
            }
            pt.setMembers(p[0], p[1], p[2]);
            tt.Add(pt);
           // Console.Write("\nAdded: " + p[0].getName() + " " + p[1].getName() + " " + p[2].getName() + " to test teams!");
        }
        return tt;
    }

    private List<PokemonTeam> loadEvalTeams()
    {
        List<PokemonTeam> et = new List<PokemonTeam>();

        Console.Write("\nSetting up evaluation teams");
        string[] setupString;
        setupString = System.IO.File.ReadAllText("configfiles/evalTeams.txt").Split('\n');

        for (int i = 0; i < setupString.Length; i++)
        {
            PokemonTeam pt = new PokemonTeam();
            Pokemon[] p = new Pokemon[3];

            string[] substring = setupString[i].Split(';');
            for (int j = 0; j < substring.Length; j++)
            {
                foreach (Pokemon pN in pokeDex)
                {
                    if (pN.getName() == substring[j])
                    {
                        p[j] = pN;
                    }
                }
            }
            pt.setMembers(p[0], p[1], p[2]);
            et.Add(pt);
        }
        return et;
    }
}
