using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField]
    private GameObject dalek;

    private void Update()
    {
        if(RockMapController.Instance.RocksLoaded == true && dalek.activeSelf == false)
        {
            dalek.SetActive(true);
        }
    }
}
