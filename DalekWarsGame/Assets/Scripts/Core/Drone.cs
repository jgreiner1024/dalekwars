using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone
{
    //re-usable vectors for easily assigning direction
    public static readonly Vector3 UP = new Vector3(0f, 0f, 1f);
    public static readonly Vector3 DOWN = new Vector3(0f, 0f, -1f);
    public static readonly Vector3 LEFT = new Vector3(-1f, 0f, 0f);
    public static readonly Vector3 RIGHT = new Vector3(1f, 0f, 0f);

    public Vector3 Position { get; set; }
    public int CurrentEnergy { get; set; }

    //track our last action to help the AI not get stuck
    private DroneAction? previousAction;

    private Dictionary<Vector3, int> rockValueMap;
    private Dictionary<Vector3, int> collectedRocks;



    public Drone(Dictionary<Vector3, int> rockMap, int startingEnergy)
    {
        Reset(rockMap, startingEnergy);
    }

    //allow resetting a drown without needing to create a new one for easier garbage collection
    public void Reset(Dictionary<Vector3, int> rockMap, int startingEnergy)
    {
        if(collectedRocks == null)
        {
            collectedRocks = new Dictionary<Vector3, int>();
        }
        else
        {
            collectedRocks.Clear();
        }

        previousAction = null;
        rockValueMap = rockMap;
        Position = new Vector3(0f, 0f, 0f);
        CurrentEnergy = startingEnergy;
    }

    //get the input values for the neuralnet based on the rock values within the visible range of the drone
    public float[] GetNeuralNetInputs(int visibleRange)
    {
        int inputCount = ((visibleRange * 2) + 1) * ((visibleRange * 2) + 1);
        float[] inputs = new float[inputCount];

        Vector3 rockPosition = new Vector3(Position.x + (visibleRange * -1), Position.y,  Position.z + (visibleRange * -1));
        for (int i = 0; i < inputs.Length; i++)
        {
            //move to the next visible row
            if (rockPosition.x > Position.x + visibleRange)
            {
                rockPosition.x = Position.x + (visibleRange * -1);
                rockPosition.z++;
            }

            //default the value to 0 if no rock is present
            inputs[i] = 0;

            //update the input value if we have a rock at this position
            if (collectedRocks.ContainsKey(rockPosition) == false && rockValueMap.ContainsKey(rockPosition) == true)
            {
                inputs[i] = rockValueMap[rockPosition];
            }
            rockPosition.x++;
        }

        return inputs;
    }

    public DroneAction GetActionFromOutput(float[] output)
    {
        int actionIndex = 0;
        float actionValue = output[0];
        DroneAction? invalidAction = GetInvalidNextAction(previousAction);
        for (int i = 1; i < output.Length; i++)
        {
            if (output[i] > actionValue &&
                (invalidAction == null || invalidAction != (DroneAction)i))
            {
                actionIndex = i;
                actionValue = output[i];
            }
        }

        return (DroneAction)actionIndex;
    }

    //simple cheat to help the AI not get stuck. Strong enough AI weights shouldn't need this
    //based on the previous action, we want to make sure we don't go in reverse, or just sit in the same spot collecting
    private DroneAction? GetInvalidNextAction(DroneAction? previousAction)
    {
        if (previousAction == null)
            return null;

        DroneAction? action = null;
        switch (previousAction)
        {
            case DroneAction.MoveUp:
                action = DroneAction.MoveDown;
                break;
            case DroneAction.MoveDown:
                action = DroneAction.MoveUp;
                break;
            case DroneAction.MoveLeft:
                action = DroneAction.MoveRight;
                break;
            case DroneAction.MoveRight:
                action = DroneAction.MoveLeft;
                break;
            case DroneAction.CollectUp:
                action = DroneAction.CollectUp;
                break;
            case DroneAction.CollectDown:
                action = DroneAction.CollectDown;
                break;
            case DroneAction.CollectLeft:
                action = DroneAction.CollectLeft;
                break;
            case DroneAction.CollectRight:
                action = DroneAction.CollectRight;
                break;
        }

        return action;
    }

    public float GetTotalScore(bool includeDistannce)
    {
        float score = 0;
        foreach(var value in collectedRocks.Values)
        {
            score += value;
        }

        //for AI training we want to account for distance in the scoring if the AI to encorage it to move instead of sitting still
        if(includeDistannce == true)
        {
            //but not by a lot, collecting rocks should always be worth more
            score += (Vector3.Distance(Position, new Vector3(0, 0, 0)) / 100f);
        }

        return score;
    }

    public void PerformAction(DroneAction action)
    {
        switch (action)
        {
            case DroneAction.MoveUp:
                Move(Position + UP);
                break;
            case DroneAction.MoveDown:
                Move(Position + DOWN);
                break;
            case DroneAction.MoveLeft:
                Move(Position + LEFT);
                break;
            case DroneAction.MoveRight:
                Move(Position + RIGHT);
                break;
            case DroneAction.CollectUp:
                CollectRock(Position + UP);
                break;
            case DroneAction.CollectDown:
                CollectRock(Position + DOWN);
                break;
            case DroneAction.CollectLeft:
                CollectRock(Position + LEFT);
                break;
            case DroneAction.CollectRight:
                CollectRock(Position + RIGHT);
                break;
        }
    }

    //returns true or false if we successfully moved
    protected virtual bool Move(Vector3 position)
    {
        if (CurrentEnergy <= 0)
            return false;

        CurrentEnergy--;

        //is there a rock here? we can't move here
        if (collectedRocks.ContainsKey(position) == false && rockValueMap.ContainsKey(position) == true)
            return false;

        Position = position;
        return true;
    }

    //returns true if we successfully collected a rock
    protected virtual bool CollectRock(Vector3 position)
    {
        if (CurrentEnergy <= 0)
            return false;

        CurrentEnergy--;

        //we have a rock here
        if (collectedRocks.ContainsKey(position) == false && rockValueMap.ContainsKey(position) == true)
        {
            //collect the rock
            collectedRocks.Add(position, rockValueMap[position]);
            return true;
        }

        return false;
    }

}
