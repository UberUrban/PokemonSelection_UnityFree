using System;
using System.Collections;
using System.Collections.Generic;

public class FeedForwardNetwork
{
    //----------Values needed for calcultion---------
    float[] input;
    float[] output;
    float[] hidden1;
    float[,] synapseLayer1;
    float[,] synapseLayer2;

    int inputSize = 85 + 1;
    int hiddenSize = 20 + 1;
    int outputSize = 85;

    //-------------Miscelaneous variables------------
    float biasValue = 1f;
    System.Random rand = new System.Random();

    Pokedex pD;
    public BattleHandler b;
    List<PokemonTeam> testTeams;

    float[] bestDNA;
    int bestFitness;

    //-------------- Genetic Variables ---------------

    List<float[]> DNA = new List<float[]>();
    List<float> fitnesses = new List<float>();
    float[] dnaString;


    //------------- Training Variables -----------------
    public int trainType = 1;
    public int targetFitness = 1000;
    public int targetGeneration = 100;
    public int testGroupSize = 50;
    double mutationChance = 10;
    int gauntletVal = 5;


    public FeedForwardNetwork(Pokedex newPD)
    {
        string[] setupString;
        setupString = System.IO.File.ReadAllText("configfiles/ffConfig.txt").Split(';');
        trainType = int.Parse(setupString[0]);
        targetGeneration = int.Parse(setupString[1]);
        targetFitness = int.Parse(setupString[2]);
        testGroupSize = int.Parse(setupString[3]);
        mutationChance = double.Parse(setupString[4]);
        gauntletVal = int.Parse(setupString[5]);

        b = new BattleHandler();
        pD = newPD;
        testTeams = pD.getTestTeams();
        SetupGenetics();
        SetupNetwork();
    }

    public void train()
    {
        int passedGenerations = 0;

        if(trainType == 1)
        {
            for (int i = 0; i < targetGeneration; i++)
            {
                individualTrain();
            }
        }
        else if(trainType == 2)
        {
            int trainingFitness = 0;
            while (trainingFitness < targetFitness)
            {
                trainingFitness = individualTrain();
                passedGenerations++;
            }
        }    

        bestDNA = DNA[0];
        int indexOfBest = 0;
        for (int i = 0; i < DNA.Count; i++)
        {
            if(fitnesses[i] > fitnesses[indexOfBest])
            {
                bestDNA = DNA[i];
                indexOfBest = i;
            }
        }
        bestFitness = (int)fitnesses[indexOfBest];

        Console.Write("\nGenerations used to reach target fitness: " + passedGenerations);
    }

    private int individualTrain()
    {
        int genBestFitness = 0;
        int totalTeamBest = 0;
        foreach (PokemonTeam t in testTeams)
        {
            Console.Write("\nTraining against a team with: ");
            foreach (Pokemon p in t.getMembers())
            {
                Console.Write(p.getName());
            }
            float[] testDNA = pD.createDnaUsingPkmn(t);
            input = testDNA;

            for (int j = 0; j < DNA.Count; j++)
            {
                fillNetwork(DNA[j]);

                calculate();

                PokemonTeam attacker = pD.createTeamUsingDNA(output);

                string debug;
                debug = "\nAttacking team: ";
                foreach (Pokemon p in attacker.getMembers())
                {
                    debug = debug + p.getName() + " ";
                }
                //Console.Write(debug);

                int fitness = b.battle(attacker.getMembers(), t.getMembers());
                fitnesses[j] = fitness;
                //Console.Write("\nFeedForward Fitness = " + fitness);
                if(fitness > genBestFitness)
                {
                    genBestFitness = fitness;
                }
            }
            totalTeamBest += genBestFitness;
            evolve();
        }
        Console.Write("\nCurrent total: " + totalTeamBest);
        return totalTeamBest;
    }

    public void test()
    {
        int score = 0;

        foreach (PokemonTeam t in pD.getEvalTeams())
        {
            Console.Write("\nBattling against a team with: ");
            foreach (Pokemon p in t.getMembers())
            {
                Console.Write(p.getName() + " ");
            }
            float[] testDNA = pD.createDnaUsingPkmn(t);
            input = testDNA;


            fillNetwork(bestDNA);

            calculate();

            PokemonTeam attacker = pD.createTeamUsingDNA(output);

            score += b.battle(attacker.getMembers(), t.getMembers());

        }

        Console.Write("\nA score of " + score + " was achieved against the evaluation teams! (FF)");
    }


    void SetupNetwork()
    {
        synapseLayer1 = new float[inputSize, hiddenSize];

        synapseLayer2 = new float[hiddenSize + 1, outputSize];

        input = new float[inputSize];
        output = new float[outputSize];
        hidden1 = new float[hiddenSize];

    }

    void fillNetwork(float[] inputDNA)
    {
        for (int i = 0; i < hiddenSize - 1; i++)
        {
            for (int j = 0; j < inputSize - 1; j++)
            {
                synapseLayer1[j, i] = inputDNA[i + j];
            }
            //Add bias node
            synapseLayer1[inputSize - 1, i] = biasValue;
        }
        for (int i = 0; i < outputSize; i++)
        {
            for (int j = 0; j < hiddenSize - 1; j++)
            {
                synapseLayer2[j, i] = inputDNA[(hiddenSize * inputSize) + i + j];
            }
            //Add bias node
            synapseLayer2[hiddenSize, i] = biasValue;
        }
    }


    void calculate()
    {

        //Calculate hidden layer
        for (int i = 0; i < hiddenSize - 1; i++)
        {
            float sum = 0;
            for (int j = 0; j < inputSize - 1; j++)
            {
                sum += input[j] * synapseLayer1[j, i];
            }
            sum = (float)Sigmoid(sum);
            hidden1[i] = sum;
        }

        /*
        string s = "Hidden:";
        for(int i = 0; i < hidden1.Length; i++)
        {
            s = s + hidden1[i] + "-";
        }
        Debug.Log(s);
        */

        //Calculate output layer

        for (int i = 0; i < outputSize; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize; j++)
            {
                sum += hidden1[j] * synapseLayer2[j, i];
            }
            sum = Sigmoid(sum);
            output[i] = (float)sum;
        }

        /*
        string s = "\nOutput:";
        for (int i = 0; i < output.Length; i++)
        {
            s = s + output[i] + "-";
        }

        Console.Write(s);
        */
    }

    double Sigmoid(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }


    //---------------------------------------------------
    //-------- The Genetic Part -------------------------
    //---------------------------------------------------

    void SetupGenetics()
    {
        for (int j = 0; j < testGroupSize; j++)
        {
            dnaString = new float[inputSize * hiddenSize + hiddenSize * outputSize];

            for (int i = 0; i < dnaString.Length; i++)
            {
                dnaString[i] = rand.Next(-10, 11);
            }
            DNA.Add(dnaString);
            fitnesses.Add(0);
        }
    }

    void evolve()
    {
        List<float[]> newDNA = new List<float[]>();

        //Breeding
        for (int i = 0; i < testGroupSize; i++)
        {
            float[] childDNA = new float[inputSize * hiddenSize + hiddenSize * outputSize];
            float[] parent1 = new float[inputSize * hiddenSize + hiddenSize * outputSize];
            float[] parent2 = new float[inputSize * hiddenSize + hiddenSize * outputSize];

            //Biased Selection
            int selectionVal = rand.Next(0, DNA.Count);

            for (int j = 0; j < gauntletVal; j++)
            {
                int opponentVal = rand.Next(0, DNA.Count);
                if (fitnesses[opponentVal] > fitnesses[selectionVal])
                {
                    selectionVal = opponentVal;
                }
            }
            parent1 = DNA[selectionVal];

            selectionVal = rand.Next(0, DNA.Count);

            for (int j = 0; j < gauntletVal; j++)
            {
                int opponentVal = rand.Next(0, DNA.Count);
                if (fitnesses[opponentVal] > fitnesses[selectionVal])
                {
                    selectionVal = opponentVal;
                }
            }
            parent2 = DNA[selectionVal];

            //The breeding part
            childDNA = parent1;

            int swapStartPoint = rand.Next(0, childDNA.Length + 1);
            int swapEndPoint = rand.Next(swapStartPoint, childDNA.Length + 1);
            for (int k = swapStartPoint; k < swapEndPoint; k++)
            {
                childDNA[k] = parent2[k];
            }

            newDNA.Add(childDNA);
        }

        //Mutation

        foreach (float[] d in newDNA)
        {
            if (rand.Next(1, 101) < mutationChance)
            {
                d[rand.Next(0, d.Length)] = rand.Next(-10, 11);
            }
        }
        DNA = newDNA;
    }
}
