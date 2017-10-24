using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonSelection_UnityFree
{
    class Program
    {
        static void Main(string[] args)
        {
            Pokedex pD = new Pokedex();
            
            BattleHandler b = new BattleHandler();
            FeedForwardNetwork f = new FeedForwardNetwork(pD);
            NEATNetwork n = new NEATNetwork(pD);

            bool finished = false;

            while (!finished)
            {
                Console.Write("\nWhat do you want to do, select and confirm with enter:\n\n[1] Train FeedForward\n[2] Train NEAT\n[3] Test FeedForward\n[4] Test NEAT\n[5] Quit");
                string option;
                option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        f.train();
                        break;
                    case "2":
                        n.train();
                        break;
                    case "3":
                        f.test();
                        break;
                    case "4":
                        n.test();
                        break;
                    case "5":
                        finished = true;
                        break;
                    default:
                        Console.Write("\n\nNot a valid option...");
                        break;
                }

                Console.Write("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        

        void test(BattleHandler b)
        {
            Pokemon pk1 = new Pokemon();
            Pokemon pk2 = new Pokemon();
            Pokemon pk3 = new Pokemon();
            Pokemon pk4 = new Pokemon();
            Pokemon pk5 = new Pokemon();
            Pokemon pk6 = new Pokemon();

            pk4.setUp("Flygon;Dragon,Ground;Dragon,Ground,Normal,Fire,Rock,Flying");
            pk5.setUp("Zoroark;Dark;Dark,Ghost,Fire,Psychic");
            pk6.setUp("Archeops;Rock,Flying;Dark,Dragon,Normal,Ground,Fight,Rock,Flying");
            pk1.setUp("Gallade;Fight,Psychic;Fight,Grass,Psychic,Electric,Ground,Ghost,Rock,Bug,Poison,Fairy");
            pk2.setUp("Gardevoir;Psychic,Fairy;Fairy,Psychic,Electric,Ghost,Fight,Grass");
            pk3.setUp("Klingklang;Steel;Electric,Steel,Normal");

            Pokemon[] team1 = { pk1, pk2, pk3 };
            Pokemon[] team2 = { pk4, pk5, pk6 };

            b.battle(team1, team2);
        }
    }
}

