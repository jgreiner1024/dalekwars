using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RockMap
{
    public static Dictionary<Vector3, int> GenerateRockMap(Vector3 startPosition, int chunkSize, int chunkCount, RockChunkDefinition[] definitions)
    {
        Dictionary<Vector3, int> rockMap = new Dictionary<Vector3, int>();
        for (int mapx = 0; mapx < chunkCount; mapx++)
        {
            for (int mapz = 0; mapz < chunkCount; mapz++)
            {
                //get list of available positions for this chunk;
                List<Vector3> availablePositions = new List<Vector3>();
                for (int chunkx = 0; chunkx < chunkSize; chunkx++)
                {
                    for (int chunkz = 0; chunkz < chunkSize; chunkz++)
                    {
                        availablePositions.Add(new Vector3(
                            startPosition.x + (mapx * chunkSize) + chunkx,
                            startPosition.y,
                            startPosition.y + (mapz * chunkSize) + chunkz
                        ));
                    }
                }

                //randomly apply position data for the rock count
                foreach (var definition in definitions)
                {
                    int count = definition.quantity <= availablePositions.Count ? definition.quantity : availablePositions.Count;
                    for (int i = 0; i < count; i++)
                    {
                        int index = RandomWrapper.Range(0, availablePositions.Count);
                        rockMap.Add(availablePositions[index], definition.value);

                        //remove from available positions
                        availablePositions.RemoveAt(index);
                    }
                }
            }
        }
        return rockMap;
    }

}


public struct RockChunkDefinition
{
    public int quantity;
    public int value;
}
