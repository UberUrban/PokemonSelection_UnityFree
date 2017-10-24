using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using SharpNeat.Phenomes;
using SharpNeat.Core;
using SharpNeat.Domains;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.SpeciationStrategies;
using System;

public class NEATNetwork
{
    static NeatEvolutionAlgorithm<NeatGenome> evo;
    public XmlDocument config = new XmlDocument();

    NeatGenome bestGenome;

    Pokedex pD;

    int testType = 1;
    int targetGeneration = 10;
    int targetFitness = 1000;

    public NEATNetwork(Pokedex newPD)
    {
        pD = newPD;
    }

    public void train()
    {
        PokemonExperiment exp = new PokemonExperiment(pD);

        config.Load("configfiles/NEATConfig.config.xml");

        exp.Initialize("Pokemon", config.DocumentElement);

        testType = XmlUtils.GetValueAsInt(config.DocumentElement, "TrainingType");
        targetFitness = XmlUtils.GetValueAsInt(config.DocumentElement, "TargetFitness");
        targetGeneration = XmlUtils.GetValueAsInt(config.DocumentElement, "TargetGeneration");

        evo = exp.CreateEvolutionAlgorithm();

        evo.UpdateEvent += new EventHandler(evo_UpdateEvent);

        evo.StartContinue();
    }

    public void test()
    {
        PokemonExperiment test = new PokemonExperiment(pD);

        config.Load("configfiles/NEATConfig.config.xml");

        test.Initialize("Pokemon", config.DocumentElement);


        var genomeDecoder = test.CreateGenomeDecoder();
        IBlackBox box = genomeDecoder.Decode(bestGenome);
        BattleHandler b = new BattleHandler();

        int score = 0;

        List<PokemonTeam> evalTeams = pD.getEvalTeams();

        foreach (PokemonTeam t in pD.getEvalTeams())
        {
            string debug = "\nBattling against a team with: ";

            foreach (Pokemon p in t.getMembers())
            {
                debug += p.getName() + " ";
            }
            Console.Write(debug);

            box.ResetState();
            ISignalArray inputArray;
            inputArray = box.InputSignalArray;
            float[] pkmnDnaIn = pD.createDnaUsingPkmn(t);

            for (int j = 0; j < t.getMembers().Length; j++)
            {
                inputArray[j] = pkmnDnaIn[j];
            }

            box.Activate();

            ISignalArray outputArray;
            outputArray = box.OutputSignalArray;
            float[] pkmnDnaOut = new float[outputArray.Length];
            for (int j = 0; j < pkmnDnaOut.Length; j++)
            {
                pkmnDnaOut[j] = (float)outputArray[j];
            }

            PokemonTeam attacker = pD.createTeamUsingDNA(pkmnDnaOut);

            score += b.battle(attacker.getMembers(), t.getMembers());
        }

        Console.Write("\nA Score of " + score + " was achieved agains the evaluation teams! (NEAT)");
        Console.Write("\nThe NEAT has a complexity of " + bestGenome.Complexity);
    }

    void evo_UpdateEvent(object sender, EventArgs e)
    {
        Console.WriteLine("\n~~~Generation " + evo.CurrentGeneration + " max fitness: " + evo.Statistics._maxFitness);
        if(testType == 1)
        {
            if(evo.CurrentGeneration >= targetGeneration)
            {
                evo.Stop();
            }
        }
        else if(testType == 2)
        {
            if (evo.Statistics._maxFitness >= targetFitness)
            {
                evo.Stop();
                Console.Write("\nGenerations used to reach target fitness: " + evo.CurrentGeneration);
            }
        }
        bestGenome = evo.CurrentChampGenome;
    }
}



public class PokemonEvaluator : IPhenomeEvaluator<IBlackBox>
{
    private ulong evaluations;
    private bool done;

    Pokedex pD;
    BattleHandler b = new BattleHandler();

    //Testing variables
    int triesPerTest = 3;

    public PokemonEvaluator(Pokedex newPD)
    {
        pD = newPD;
    }

    public ulong EvaluationCount
    {
        get
        {
            return evaluations;
        }
    }

    public bool StopConditionSatisfied
    {
        get
        {
            return done;
        }
    }

    public void Reset()
    {
        //Nothing right now
    }



    public FitnessInfo Evaluate(IBlackBox box)
    {
        int fitness = 0;

        List<PokemonTeam> testTeams = pD.getTestTeams();

        for (int i = 0; i < triesPerTest; i++)
        {
            foreach (PokemonTeam t in testTeams)
            {
                string debug = "\nTraining against a team with: ";
               
                foreach (Pokemon p in t.getMembers())
                {
                    debug += p.getName() + " ";
                }
               // Console.Write(debug);

                box.ResetState();
                ISignalArray inputArray;
                inputArray = box.InputSignalArray;
                float[] pkmnDnaIn = pD.createDnaUsingPkmn(t);

                for (int j = 0; j < t.getMembers().Length; j++)
                {
                    inputArray[j] = pkmnDnaIn[j];
                }

                box.Activate();

                ISignalArray outputArray;
                outputArray = box.OutputSignalArray;
                float[] pkmnDnaOut = new float[outputArray.Length];
                for (int j = 0; j < pkmnDnaOut.Length; j++)
                {
                    pkmnDnaOut[j] = (float)outputArray[j];
                }

                PokemonTeam attacker = pD.createTeamUsingDNA(pkmnDnaOut);

                fitness += b.battle(attacker.getMembers(), t.getMembers());

                evaluations++;
                //Console.Write("\nEvaluations = " + evaluations);

                //Console.Write("\nCurrent Neat Fitness: " + fitness);
            }
        }
        return new FitnessInfo(fitness, fitness);
    }
}

public class PokemonExperiment : SimpleNeatExperiment
{
    Pokedex pD;

    public PokemonExperiment(Pokedex newPD)
    {
        pD = newPD;
    }

    public override IPhenomeEvaluator<IBlackBox> PhenomeEvaluator
    {
        get
        {
            return new PokemonEvaluator(pD);
        }
    }

    public override int InputCount
    {
        get
        {
            return 85;
        }
    }

    public override int OutputCount
    {
        get
        {
            return 85;
        }
    }

    public override bool EvaluateParents
    {
        get
        {
            return true;
        }
    }
}

public abstract class SimpleNeatExperiment : INeatExperiment
{
    //This code is copied from A TicTacToe Tutorial, original description as follows:
    /* ***************************************************************************
     * This file is part of the NashCoding tutorial on SharpNEAT 2.
     * 
     * Copyright 2010, Wesley Tansey (wes@nashcoding.com)
     * 
     * Some code in this file may have been copied directly from SharpNEAT,
     * for learning purposes only. Any code copied from SharpNEAT 2 is 
     * copyright of Colin Green (sharpneat@gmail.com).
     *
     * Both SharpNEAT and this tutorial are free software: you can redistribute
     * it and/or modify it under the terms of the GNU General Public License
     * as published by the Free Software Foundation, either version 3 of the 
     * License, or (at your option) any later version.
     *
     * SharpNEAT is distributed in the hope that it will be useful,
     * but WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
     * GNU General Public License for more details.
     *
     * You should have received a copy of the GNU General Public License
     * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
     */

    NeatEvolutionAlgorithmParameters _eaParams;
    NeatGenomeParameters _neatGenomeParams;
    string _name;
    int _populationSize;
    int _specieCount;
    NetworkActivationScheme _activationScheme;
    string _complexityRegulationStr;
    int? _complexityThreshold;
    string _description;
    ParallelOptions _parallelOptions;

    #region Abstract properties that subclasses must implement
    public abstract IPhenomeEvaluator<IBlackBox> PhenomeEvaluator { get; }
    public abstract int InputCount { get; }
    public abstract int OutputCount { get; }
    public abstract bool EvaluateParents { get; }
    #endregion



    #region INeatExperiment Members
    public string Description
    {
        get { return _description; }
    }

    public string Name
    {
        get { return _name; }
    }

    /// <summary>
    /// Gets the default population size to use for the experiment.
    /// </summary>
    public int DefaultPopulationSize
    {
        get { return _populationSize; }
    }

    /// <summary>
    /// Gets the NeatEvolutionAlgorithmParameters to be used for the experiment. Parameters on this object can be 
    /// modified. Calls to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in 
    /// at the time of the call.
    /// </summary>
    public NeatEvolutionAlgorithmParameters NeatEvolutionAlgorithmParameters
    {
        get { return _eaParams; }
    }

    /// <summary>
    /// Gets the NeatGenomeParameters to be used for the experiment. Parameters on this object can be modified. Calls
    /// to CreateEvolutionAlgorithm() make a copy of and use this object in whatever state it is in at the time of the call.
    /// </summary>
    public NeatGenomeParameters NeatGenomeParameters
    {
        get { return _neatGenomeParams; }
    }

    /// <summary>
    /// Initialize the experiment with some optional XML configutation data.
    /// </summary>
    public void Initialize(string name, XmlElement xmlConfig)
    {
        _name = name;
        _populationSize = XmlUtils.GetValueAsInt(xmlConfig, "PopulationSize");
        _specieCount = XmlUtils.GetValueAsInt(xmlConfig, "SpecieCount");
        _activationScheme = ExperimentUtils.CreateActivationScheme(xmlConfig, "Activation");
        _complexityRegulationStr = XmlUtils.TryGetValueAsString(xmlConfig, "ComplexityRegulationStrategy");
        _complexityThreshold = XmlUtils.TryGetValueAsInt(xmlConfig, "ComplexityThreshold");
        _description = XmlUtils.TryGetValueAsString(xmlConfig, "Description");
        _parallelOptions = ExperimentUtils.ReadParallelOptions(xmlConfig);

        _eaParams = new NeatEvolutionAlgorithmParameters();
        _eaParams.SpecieCount = _specieCount;
        _neatGenomeParams = new NeatGenomeParameters();
    }

    /// <summary>
    /// Load a population of genomes from an XmlReader and returns the genomes in a new list.
    /// The genome2 factory for the genomes can be obtained from any one of the genomes.
    /// </summary>
    public List<NeatGenome> LoadPopulation(XmlReader xr)
    {
        return NeatGenomeUtils.LoadPopulation(xr, false, this.InputCount, this.OutputCount);
    }

    /// <summary>
    /// Save a population of genomes to an XmlWriter.
    /// </summary>
    public void SavePopulation(XmlWriter xw, IList<NeatGenome> genomeList)
    {
        // Writing node IDs is not necessary for NEAT.
        NeatGenomeXmlIO.WriteComplete(xw, genomeList, false);
    }

    /// <summary>
    /// Create a genome2 factory for the experiment.
    /// Create a genome2 factory with our neat genome2 parameters object and the appropriate number of input and output neuron genes.
    /// </summary>
    public IGenomeFactory<NeatGenome> CreateGenomeFactory()
    {
        return new NeatGenomeFactory(InputCount, OutputCount, _neatGenomeParams);
    }

    /// <summary>
    /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
    /// of the algorithm are also constructed and connected up.
    /// This overload requires no parameters and uses the default population size.
    /// </summary>
    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
    {
        return CreateEvolutionAlgorithm(DefaultPopulationSize);
    }

    /// <summary>
    /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
    /// of the algorithm are also constructed and connected up.
    /// This overload accepts a population size parameter that specifies how many genomes to create in an initial randomly
    /// generated population.
    /// </summary>
    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize)
    {
        // Create a genome2 factory with our neat genome2 parameters object and the appropriate number of input and output neuron genes.
        IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

        // Create an initial population of randomly generated genomes.
        List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);

        // Create evolution algorithm.
        return CreateEvolutionAlgorithm(genomeFactory, genomeList);
    }

    /// <summary>
    /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
    /// of the algorithm are also constructed and connected up.
    /// This overload accepts a pre-built genome2 population and their associated/parent genome2 factory.
    /// </summary>
    public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
    {
        // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
        IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
        ISpeciationStrategy<NeatGenome> speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, _parallelOptions);

        // Create complexity regulation strategy.
        IComplexityRegulationStrategy complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

        // Create the evolution algorithm.
        NeatEvolutionAlgorithm<NeatGenome> ea = new NeatEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy, complexityRegulationStrategy);

        // Create genome2 decoder.
        IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = new NeatGenomeDecoder(_activationScheme);

        // Create a genome2 list evaluator. This packages up the genome2 decoder with the genome2 evaluator.
        IGenomeListEvaluator<NeatGenome> genomeListEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, PhenomeEvaluator, _parallelOptions);

        // Wrap the list evaluator in a 'selective' evaulator that will only evaluate new genomes. That is, we skip re-evaluating any genomes
        // that were in the population in previous generations (elite genomes). This is determiend by examining each genome2's evaluation info object.
        if (!EvaluateParents)
            genomeListEvaluator = new SelectiveGenomeListEvaluator<NeatGenome>(genomeListEvaluator,
                                     SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());

        // Initialize the evolution algorithm.
        ea.Initialize(genomeListEvaluator, genomeFactory, genomeList);

        // Finished. Return the evolution algorithm
        return ea;
    }

    /// <summary>
    /// Creates a new genome decoder that can be used to convert a genome into a phenome.
    /// </summary>
    public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
    {
        return new NeatGenomeDecoder(_activationScheme);
    }

    #endregion
}