using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMapController : MonoBehaviour
{
    //set up a singleton to allow access to the rock map
    public static RockMapController Instance { get; private set; }

    [SerializeField]
    private int chunkSize = 16;

    [SerializeField]
    private int chunkCount = 1;

    [SerializeField]
    private RockMapPrefabData[] rockPrefabData = null;

    //current rock map for this scene
    private Dictionary<Vector3, int> rockMap;
    private Dictionary<int, GameObject> prefabs;

    //allow public access to the rock map through the singleton
    public Dictionary<Vector3, int> Map { get { return rockMap; } }

    private void Awake()
    {
        //always re-assign the isntance to the latest created rock map controller
        Instance = this;

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

    private void Start()
    {
        //generate all the rock prefabs
        StartCoroutine(SpawnRocksCoroutine());
        
    }


    //put the rock spawning in a coroutine so it doesn't hold up the rest of the scene creation
    //if this took a significant amount of time we could add some loading stuff to the UI for it
    private IEnumerator SpawnRocksCoroutine()
    {
        //load rocks in order of value
        for(int i = 1; i <= 5; i++)
        {
            foreach (Vector3 rockPosition in rockMap.Keys)
            {
                int value = rockMap[rockPosition];
                if(value == i)
                {
                    GameObject petRockObject = Instantiate<GameObject>(prefabs[value], transform);
                    petRockObject.transform.Rotate(0f, Random.Range(0f, 360f), 0f);
                    petRockObject.transform.position = rockPosition;
                    yield return new WaitForSeconds(0.001f);
                }
                
            }
            
        }
        

        
    }
    
    
}


