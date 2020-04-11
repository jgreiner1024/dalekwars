using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMapController : MonoBehaviour
{
    [SerializeField]
    private int chunkSize = 16;

    [SerializeField]
    private int chunkCount = 1;

    [SerializeField]
    private RockMapPrefabData[] rockPrefabData = null;

    //current rock map for this scene
    private Dictionary<Vector3, int> rockMap;
    private Dictionary<int, GameObject> prefabs;

    private void Awake()
    {

        //set up prefab by value index for easy access later
        prefabs = new Dictionary<int, GameObject>();

        //build our chunk definitions
        RockChunkDefinition[] rockChunkDefinitions = new RockChunkDefinition[rockPrefabData.Length];
        for(int i = 0; i < rockPrefabData.Length; i++)
        {
            if(prefabs.ContainsKey(rockPrefabData[i].value) == false)
            {
                rockChunkDefinitions[i] = new RockChunkDefinition()
                {
                    quantity = rockPrefabData[i].quantity,
                    value = rockPrefabData[i].value
                };

                prefabs.Add(rockPrefabData[i].value, rockPrefabData[i].petRockPrefab);
            }
        }

        rockMap = RockMap.GenerateRockMap(transform.position, chunkSize, chunkCount, rockChunkDefinitions);
    }

    // Start is called before the first frame update
    private void Start()
    {
        //generate all the rock prefabs
        foreach(Vector3 rockPosition in rockMap.Keys)
        {
            int value = rockMap[rockPosition];
            GameObject petRockObject = Instantiate<GameObject>(prefabs[value], transform);
            petRockObject.transform.Rotate(0f, Random.Range(0f, 360f), 0f);
            petRockObject.transform.position = rockPosition;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}


