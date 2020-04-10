using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DalekWarsTrainer
{
    public class EvolutionTrainer
    {
        //group size batches neural nets together for better multi-threading
        const int groupSize = 20;
        const int chunkSize = 16;

        private RockChunkDefinition[] rockLayoutDefinitions = new RockChunkDefinition[]
        {
            new RockChunkDefinition() { quantity = 16, value = 1},
            new RockChunkDefinition() { quantity = 8, value = 2},
            new RockChunkDefinition() { quantity = 4, value = 3},
            new RockChunkDefinition() { quantity = 2, value = 4},
            new RockChunkDefinition() { quantity = 1, value = 5}
        };


        public NeuralNetwork Train(int mapCount, int visibleRange, int generationCount, int dronesPerGeneration, int droneEnergy)
        {
            //for multi-threading we want to group together batches of drones
            int groupCount = dronesPerGeneration / groupSize;

            //drones per generation must be divisible by groupSize with no remainder
            dronesPerGeneration = groupSize * groupCount;

            //figure out the chunk count based 
            int chunkCount = (droneEnergy / chunkSize);
            chunkCount += (droneEnergy % chunkSize == 0) ? 0 : 1;


            //create our maps
            int offset = (chunkSize * chunkCount) * -1;
            Dictionary<Vector3, int>[] rockMaps = new Dictionary<Vector3, int>[mapCount];
            Vector3 startPosition = new Vector3(offset, 0, offset);
            for (int i = 0; i < mapCount; i++)
            {
                rockMaps[i] = RockMap.GenerateRockMap(startPosition, chunkSize, chunkCount, rockLayoutDefinitions);
            }

            //build the neural net framework
            int inputCount = ((visibleRange * 2) + 1) * ((visibleRange * 2) + 1);
            int[] layers = new int[]
            {
                inputCount, //11 by 11 grid of visable range
                inputCount / 2, //hidden middle layer to help
                Enum.GetValues(typeof(DroneAction)).Length //8 possible actions, move in 4 directions, or drill in 4 directions
            };


            //build initial set of neural nets
            NeuralNetwork currentNeuralNetworkParent = null;
            NeuralNetwork[] currentNeuralNets = new NeuralNetwork[dronesPerGeneration];

            //build rovers for each map
            Drone[,] drones = new Drone[mapCount, dronesPerGeneration];
            for (int m = 0; m < mapCount; m++)
            {
                for (int d = 0; d < dronesPerGeneration; d++)
                {
                    drones[m, d] = new Drone(rockMaps[m], droneEnergy);
                }
            }

            //keep track of when we started
            DateTime start = DateTime.Now;
            for (int g = 0; g < generationCount; g++)
            {
                //reset and regenerate neural nets
                for (int d = 0; d < dronesPerGeneration; d++)
                {
                    currentNeuralNets[d] = GetNeuralNet(layers, currentNeuralNetworkParent);
                    for (int m = 0; m < mapCount; m++)
                    {
                        drones[m, d].Reset(rockMaps[m], droneEnergy);
                    }
                }

                //SemaphoreSlim maxThread = new SemaphoreSlim(5000);
                
                Task[] tasks = new Task[groupCount];
                for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
                {
                    //maxThread.Wait();
                    tasks[groupIndex] = ExecuteNeuralNetGroup(drones, currentNeuralNets, visibleRange, groupIndex * groupSize, droneEnergy);
                }
                //wait for everything to finish
                Task.WaitAll(tasks);

                NeuralNetwork bestNeuralNet = null;
                //calculate the best neuralnet
                for (int d = 0; d < dronesPerGeneration; d++)
                {
                    currentNeuralNets[d].Fitness = 0;
                    for (int m = 0; m < mapCount; m++)
                    {
                        currentNeuralNets[d].Fitness += drones[m, d].GetTotalScore(true);
                    }

                    if (bestNeuralNet == null || currentNeuralNets[d].Fitness > bestNeuralNet.Fitness)
                    {
                        bestNeuralNet = currentNeuralNets[d];
                    }
                    else if (currentNeuralNets[d].Fitness == bestNeuralNet.Fitness)
                    {
                        int flip = UnityEngine.Random.Range(0, 1);
                        bestNeuralNet = (flip == 0) ? bestNeuralNet : currentNeuralNets[d];
                    }
                }

                currentNeuralNetworkParent = bestNeuralNet;
                Console.WriteLine($"Generation {g} completed score {currentNeuralNetworkParent.Fitness}, time elapsed {DateTime.Now.Subtract(start)}");
            }

            //currentNeuralNetParent.Save(@"C:\dev\RoverAIGameTrainer\SuperBrain.dat");
            //completed 
            Console.WriteLine($"Completed training with score {currentNeuralNetworkParent.Fitness}");
            return currentNeuralNetworkParent;

        }

        private NeuralNetwork GetNeuralNet(int[] layers, NeuralNetwork parent)
        {
            //we have no parent, generate a new random neural net
            if (parent == null)
                return new NeuralNetwork(layers);

            //copy the existing neural net
            NeuralNetwork neuralNet = new NeuralNetwork(parent);

            //then we try to mutate it
            neuralNet.Mutate();

            return neuralNet;
        }

        private async Task ExecuteNeuralNetGroup(Drone[,] drones, NeuralNetwork[] neuralNetworks, int visibleRange, int startIndex, int energy)
        {
            DateTime start = DateTime.Now;
            for (int r = startIndex; r < startIndex + groupSize; r++)
            {
                for (int m = 0; m < drones.GetLength(0); m++)
                {
                    for (int e = 0; e < energy; e++)
                    {
                        float[] inputs = drones[m, r].GetNeuralNetInputs(visibleRange);
                        float[] output = neuralNetworks[r].FeedForward(inputs);
                        DroneAction action = drones[m, r].GetActionFromOutput(output);
                        drones[m, r].PerformAction(action);
                    }
                }
                await Task.Delay(1);
            }
        }
    }
}
