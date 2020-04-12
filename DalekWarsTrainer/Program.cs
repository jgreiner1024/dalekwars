using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalekWarsTrainer
{
    class Program
    {
        static void Main(string[] args)
        {
            int mapCount = 20000;
            int visibleRange = 5;
            int generationCount = 1000;
            int dronesPerGeneration = 100;
            int startingDroneEnergy = 32;


            EvolutionTrainer trainer = new EvolutionTrainer();
            NeuralNetwork trainedNeuralNetwork = trainer.Train(mapCount, visibleRange, generationCount, dronesPerGeneration, startingDroneEnergy);
            trainedNeuralNetwork.Save($"DalekWars_{mapCount}_{generationCount}_{dronesPerGeneration}.dat");
        }
    }
}
