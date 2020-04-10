//no usings to avoid namepace class when used in unity and outside unity
//all classes and such are fully qualified

public class NeuralNetwork 
{
    const float constantBiasDefaultValue = 0f;

    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;

    public float Fitness { get; set; } = 0;

    public NeuralNetwork(int[] inputLayers)
    {
        layers = new int[inputLayers.Length];
        System.Array.Copy(inputLayers, 0, layers, 0, inputLayers.Length);
        
        CreateNeurons();
        CreateWeights(null);
    }

    public NeuralNetwork(NeuralNetwork source)
    {
        layers = new int[source.layers.Length];
        System.Array.Copy(source.layers, 0, layers, 0, layers.Length);

        CreateNeurons();
        CreateWeights(source.weights);
    }

    private void CreateNeurons()
    {
        neurons = new float[layers.Length][];
        for (int i = 0; i < layers.Length; i++)
        {
            neurons[i] = new float[layers[i]];
        }
    }

    private void CreateWeights(float[][][] sourceWeights)
    {

        //for indexes quick sheet l = layer, n = neuron, w = weight
        //weights have 1 less layer than neurons
        weights = new float[layers.Length - 1][][];

        //loop through all the weight layers
        for (int l = 0; l < weights.Length; l++)
        {
            //we set up a weight array for each neuron starting at the second neuron layer
            weights[l] = new float[neurons[l + 1].Length][];
            for (int n = 0; n < weights[l].Length; n++)
            {
                //each neuron from the previous layer connects to this neuron
                weights[l][n] = new float[neurons[l].Length];
                for (int w = 0; w < weights[l][n].Length; w++)
                {
                    //if sourceweight is null assign new random weight
                    weights[l][n][w] = sourceWeights == null ? RandomWrapper.Range(-0.5f, 0.5f) : sourceWeights[l][n][w];
                }
            }
        }
    }

    public float[] FeedForward(float[] inputs)
    {
        //these should always be equal, should we ensure this?
        int len = (inputs.Length < neurons[0].Length) ? inputs.Length : neurons[0].Length;

        //copy the inputs into the first layer of nuerons
        System.Array.Copy(inputs, neurons[0], len);

        //for indexes quick sheet l = layer, n = neuron, w = weight
        //we start at the second layer because the first layer was flat set from input above
        for (int l = 0; l < weights.Length; l++)
        {
            for (int n = 0; n < weights[l].Length; n++)
            {
                //constant bias default
                float value = constantBiasDefaultValue;
                for (int w = 0; w < weights[l][n].Length; w++)
                {
                    value +=
                        //previous layer nueron connected to this nueron
                        neurons[l][w] *
                        //the weight connecting the neurons
                        weights[l][n][w];

                }

                //assign the connected neuron layer to the value using hyperbolic tangent activation
                neurons[l + 1][n] = (float)System.Math.Tanh(value);
            }
        }

        //return the last neuron layer as the output
        return neurons[neurons.Length - 1];
    }
    public void Mutate()
    {
        //for indexes quick sheet l = layer, n = neuron, w = weight
        for (int l = 0; l < weights.Length; l++)
        {
            for (int n = 0; n < weights[l].Length; n++)
            {
                for (int w = 0; w < weights[l][n].Length; w++)
                {
                    //mutate chance value 
                    float randomNumber = RandomWrapper.Range(0f, 100f);
                    if (randomNumber <= 2f)
                    {
                        //flip sign of weight
                        weights[l][n][w] *= -1f;
                    }
                    else if (randomNumber <= 4f)
                    {
                        //pick random weight between -1 and 1
                        weights[l][n][w] = RandomWrapper.Range(-0.5f, 0.5f);
                    }
                    else if (randomNumber <= 6f)
                    {
                        //randomly increase by 0% to 100%
                        weights[l][n][w] *= RandomWrapper.Range(0f, 1f) + 1f;
                    }
                    else if (randomNumber <= 8f)
                    {
                        //randomly decrease by 0% to 100%
                        weights[l][n][w] *= RandomWrapper.Range(0f, 1f);
                    }
                }
            }
        }
    }

    //inside Unity we can use the Unity Engine's range system
    //outside of unity we need to use the normal .NET random
    


}
