using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//rock map configuration data
[CreateAssetMenu(fileName = "RockPrefabData", menuName = "ScriptableObjects/RockPrefabData")]
public class RockMapPrefabData : ScriptableObject
{
    public GameObject petRockPrefab;
    public int quantity;
    public int value;
}
