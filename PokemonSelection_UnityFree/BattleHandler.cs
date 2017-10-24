using System;
using System.Collections;
using System.Collections.Generic;

public class BattleHandler
{

    public int battle(Pokemon[] p1, Pokemon[] p2)
    {
        // Debug.Log("Battle Start!\n p1 sent out: " + p1[0].getName() + ", p2 sent out " + p2[0].getName());

        int fitness;

        bool gotWinner = false;
        bool p1lost = false;
        bool p2lost = false;

        int p1active = 0;
        int p2active = 0;
        int[] p1hp = { 3, 3, 3 };
        int[] p2hp = { 3, 3, 3 };

        while(!gotWinner)
        {
            int optimalDamage = 0;
            string optimalType = "none";
            foreach(string s in p1[p1active].getAttacks())
            {
                int testdamage = calculateDamage(s, p2[p2active].getType1(), p2[p2active].getType2());
                if (testdamage > optimalDamage)
                {
                    optimalDamage = testdamage;
                    optimalType = s;
                }
            }

            p2hp[p2active] -= optimalDamage;
            // Debug.Log(p1[p1active].getName() + " attacked with a " + optimalType + " attack for " + optimalDamage + " damage!");

            if(p2hp[p2active] < 1)
            {
                // Debug.Log(p2[p2active].getName() + " fainted!");

                if(p2hp[1] > 0)
                {
                    p2active = 1;
                    // Debug.Log("p2 sent in " + p2[p2active].getName());
                }
                else if(p2hp[2] > 0)
                {
                    p2active = 2;
                    // Debug.Log("p2 sent in " + p2[p2active].getName());
                }
                else
                {
                    p2lost = true;
                    // Debug.Log("p2 lost...");
                }
            }

            if (!p2lost)
            {
                optimalDamage = 0;

                foreach (string s in p2[p2active].getAttacks())
                {
                    int testdamage = calculateDamage(s, p1[p1active].getType1(), p1[p1active].getType2());
                    if (testdamage > optimalDamage)
                    {
                        optimalDamage = testdamage;
                        optimalType = s;
                    }
                }

                p1hp[p1active] -= optimalDamage;
                // Debug.Log(p2[p2active].getName() + " attacked with a " + optimalType + " attack for " + optimalDamage + " damage!");

                if (p1hp[p1active] < 1)
                {
                    // Debug.Log(p1[p1active].getName() + " fainted!");

                    if (p1hp[1] > 0)
                    {
                        p1active = 1;
                        // Debug.Log("p1 sent in " + p1[p1active].getName());
                    }
                    else if (p1hp[2] > 0)
                    {
                        p1active = 2;
                        // Debug.Log("p1 sent in " + p1[p1active].getName());
                    }
                    else
                    {
                        p1lost = true;
                        // Debug.Log("p1 lost...");
                    }
                }
            }

            if(p1lost || p2lost)
            {
                gotWinner = true;
            }
        }

        fitness = (p1hp[0] + p1hp[1] + p1hp[2]) * 100;
        if(p2lost)
        {
            fitness += 900;
        }

        //Console.Write("\nThe challenger p1 gained a score of: " + fitness);
        if(fitness < 0)
        {
            fitness = 0;
        }
        return fitness;
    }

    int calculateDamage(string AType, string DType1, string DType2)
    {
        int damage = 0;
        List<string> eff = new List<string>();
        List<string> not = new List<string>();
        List<string> no = new List<string>();

        //Select Damage Table
        switch (DType1)
        {
            case "Normal":
                eff.Add("Fight");
                no.Add("Ghost");
                break;
            case "Fire":
                eff.Add("Water");
                eff.Add("Ground");
                eff.Add("Rock");
                not.Add("Fire");
                not.Add("Grass");
                not.Add("Ice");
                not.Add("Bug");
                not.Add("Steel");
                not.Add("Fairy");
                break;
            case "Water":
                eff.Add("Electric");
                eff.Add("Grass");
                not.Add("Fire");
                not.Add("Water");
                not.Add("Ice");
                not.Add("Steel");
                break;
            case "Electric":
                eff.Add("Ground");
                not.Add("Electric");
                not.Add("Flying");
                not.Add("Steel");
                break;
            case "Grass":
                eff.Add("Fire");
                eff.Add("Ice");
                eff.Add("Poison");
                eff.Add("Bug");
                eff.Add("Flying");
                not.Add("Water");
                not.Add("Electric");
                not.Add("Grass");
                not.Add("Ground");
                break;
            case "Ice":
                eff.Add("Fire");
                eff.Add("Fight");
                eff.Add("Rock");
                eff.Add("Steel");
                not.Add("Ice");
                break;
            case "Fight":
                eff.Add("Flying");
                eff.Add("Psychic");
                eff.Add("Fairy");
                not.Add("Bug");
                not.Add("Rock");
                not.Add("Dark");
                break;
            case "Poison":
                eff.Add("Ground");
                eff.Add("Psychic");
                not.Add("Grass");
                not.Add("Fight");
                not.Add("Poison");
                not.Add("Bug");
                not.Add("Fairy");
                break;
            case "Ground":
                eff.Add("Water");
                eff.Add("Grass");
                eff.Add("Ice");
                not.Add("Poison");
                not.Add("Rock");
                no.Add("Electric");
                break;
            case "Flying":
                eff.Add("Electric");
                eff.Add("Ice");
                eff.Add("Rock");
                not.Add("Grass");
                not.Add("Fight");
                not.Add("Bug");
                no.Add("Ground");
                break;
            case "Psychic":
                eff.Add("Bug");
                eff.Add("Dark");
                eff.Add("Ghost");
                not.Add("Fight");
                not.Add("Psychic");
                break;
            case "Bug":
                eff.Add("Fire");
                eff.Add("Flying");
                eff.Add("Rock");
                not.Add("Grass");
                not.Add("Fight");
                not.Add("Ground");
                break;
            case "Rock":
                eff.Add("Water");
                eff.Add("Grass");
                eff.Add("Fight");
                eff.Add("Ground");
                eff.Add("Steel");
                not.Add("Normal");
                not.Add("Fire");
                not.Add("Poison");
                not.Add("Flying");
                break;
            case "Ghost":
                eff.Add("Ghost");
                eff.Add("Dark");
                not.Add("Poison");
                not.Add("Bug");
                no.Add("Normal");
                no.Add("Fight");
                break;
            case "Dragon":
                eff.Add("Ice");
                eff.Add("Dragon");
                eff.Add("Fairy");
                not.Add("Fire");
                not.Add("Water");
                not.Add("Electric");
                not.Add("Grass");
                break;
            case "Dark":
                eff.Add("Fight");
                eff.Add("Bug");
                eff.Add("Fairy");
                not.Add("Ghost");
                not.Add("Dark");
                no.Add("Psychic");
                break;
            case "Steel":
                eff.Add("Fire");
                eff.Add("Fight");
                eff.Add("Ground");
                not.Add("Normal");
                not.Add("Grass");
                not.Add("Ice");
                not.Add("Flying");
                not.Add("Psychic");
                not.Add("Bug");
                not.Add("Rock");
                not.Add("Dragon");
                not.Add("Steel");
                not.Add("Fairy");
                no.Add("Poiosn");
                break;
            case "Fairy":
                eff.Add("Poison");
                eff.Add("Steel");
                not.Add("Fight");
                not.Add("Bug");
                not.Add("Dark");
                no.Add("Dragon");
                break;
            default:
                //nope
                break;
        }

        //Calculate Effective
        if(!multiEqual(AType, no))
        {
            if (multiEqual(AType, eff))
            {
                damage = 3;
            }
            else if(multiEqual(AType, not))
            {
                damage = 1;
            }
            else
            {
                damage = 2;
            }

            if(DType2 != "none")
            {
                eff = new List<string>();
                not = new List<string>();
                no = new List<string>();

                //Select Damage Table
                switch (DType2)
                {
                    case "Normal":
                        eff.Add("Fight");
                        no.Add("Ghost");
                        break;
                    case "Fire":
                        eff.Add("Water");
                        eff.Add("Ground");
                        eff.Add("Rock");
                        not.Add("Fire");
                        not.Add("Grass");
                        not.Add("Ice");
                        not.Add("Bug");
                        not.Add("Steel");
                        not.Add("Fairy");
                        break;
                    case "Water":
                        eff.Add("Electric");
                        eff.Add("Grass");
                        not.Add("Fire");
                        not.Add("Water");
                        not.Add("Ice");
                        not.Add("Steel");
                        break;
                    case "Electric":
                        eff.Add("Ground");
                        not.Add("Electric");
                        not.Add("Flying");
                        not.Add("Steel");
                        break;
                    case "Grass":
                        eff.Add("Fire");
                        eff.Add("Ice");
                        eff.Add("Poison");
                        eff.Add("Bug");
                        eff.Add("Flying");
                        not.Add("Water");
                        not.Add("Electric");
                        not.Add("Grass");
                        not.Add("Ground");
                        break;
                    case "Ice":
                        eff.Add("Fire");
                        eff.Add("Fight");
                        eff.Add("Rock");
                        eff.Add("Steel");
                        not.Add("Ice");
                        break;
                    case "Fight":
                        eff.Add("Flying");
                        eff.Add("Psychic");
                        eff.Add("Fairy");
                        not.Add("Bug");
                        not.Add("Rock");
                        not.Add("Dark");
                        break;
                    case "Poison":
                        eff.Add("Ground");
                        eff.Add("Psychic");
                        not.Add("Grass");
                        not.Add("Fight");
                        not.Add("Poison");
                        not.Add("Bug");
                        not.Add("Fairy");
                        break;
                    case "Ground":
                        eff.Add("Water");
                        eff.Add("Grass");
                        eff.Add("Ice");
                        not.Add("Poison");
                        not.Add("Rock");
                        no.Add("Electric");
                        break;
                    case "Flying":
                        eff.Add("Electric");
                        eff.Add("Ice");
                        eff.Add("Rock");
                        not.Add("Grass");
                        not.Add("Fight");
                        not.Add("Bug");
                        no.Add("Ground");
                        break;
                    case "Psychic":
                        eff.Add("Bug");
                        eff.Add("Dark");
                        eff.Add("Ghost");
                        not.Add("Fight");
                        not.Add("Psychic");
                        break;
                    case "Bug":
                        eff.Add("Fire");
                        eff.Add("Flying");
                        eff.Add("Rock");
                        not.Add("Grass");
                        not.Add("Fight");
                        not.Add("Ground");
                        break;
                    case "Rock":
                        eff.Add("Water");
                        eff.Add("Grass");
                        eff.Add("Fight");
                        eff.Add("Ground");
                        eff.Add("Steel");
                        not.Add("Normal");
                        not.Add("Fire");
                        not.Add("Poison");
                        not.Add("Flying");
                        break;
                    case "Ghost":
                        eff.Add("Ghost");
                        eff.Add("Dark");
                        not.Add("Poison");
                        not.Add("Bug");
                        no.Add("Normal");
                        no.Add("Fight");
                        break;
                    case "Dragon":
                        eff.Add("Ice");
                        eff.Add("Dragon");
                        eff.Add("Fairy");
                        not.Add("Fire");
                        not.Add("Water");
                        not.Add("Electric");
                        not.Add("Grass");
                        break;
                    case "Dark":
                        eff.Add("Fight");
                        eff.Add("Bug");
                        eff.Add("Fairy");
                        not.Add("Ghost");
                        not.Add("Dark");
                        no.Add("Psychic");
                        break;
                    case "Steel":
                        eff.Add("Fire");
                        eff.Add("Fight");
                        eff.Add("Ground");
                        not.Add("Normal");
                        not.Add("Grass");
                        not.Add("Ice");
                        not.Add("Flying");
                        not.Add("Psychic");
                        not.Add("Bug");
                        not.Add("Rock");
                        not.Add("Dragon");
                        not.Add("Steel");
                        not.Add("Fairy");
                        no.Add("Poiosn");
                        break;
                    case "Fairy":
                        eff.Add("Poison");
                        eff.Add("Steel");
                        not.Add("Fight");
                        not.Add("Bug");
                        not.Add("Dark");
                        no.Add("Dragon");
                        break;
                    default:
                        //nope
                        break;
                }

                if (multiEqual(AType, eff))
                {
                    damage += 1;
                }
                else if (multiEqual(AType, not) && damage != 1)
                {
                    damage -= 1;
                }
                else if (multiEqual(AType, no))
                {
                    damage = 0;
                }
            }
        }
        return damage;
    }

    bool multiEqual(string main, List<string> sub)
    {
        bool result = false;
        foreach(string t in sub)
        {
            if(main == t)
            {
                result = true;
            }
        }
        return result;
    }
}
