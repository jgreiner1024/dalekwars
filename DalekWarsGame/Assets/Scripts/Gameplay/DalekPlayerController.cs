using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DalekPlayerController : MonoBehaviour
{

    private DalekBehaviour dalekBehaviour;

    private Dictionary<KeyCode, DroneAction> keyActions;

    private void Awake()
    {
        dalekBehaviour = GetComponent<DalekBehaviour>();

        //bind drone actions to key codes for easy looping
        keyActions = new Dictionary<KeyCode, DroneAction>();
        keyActions.Add(KeyCode.UpArrow, DroneAction.CollectUp);
        keyActions.Add(KeyCode.DownArrow, DroneAction.CollectDown);
        keyActions.Add(KeyCode.LeftArrow, DroneAction.CollectLeft);
        keyActions.Add(KeyCode.RightArrow, DroneAction.CollectRight);
        keyActions.Add(KeyCode.W, DroneAction.MoveUp);
        keyActions.Add(KeyCode.S, DroneAction.MoveDown);
        keyActions.Add(KeyCode.A, DroneAction.MoveLeft);
        keyActions.Add(KeyCode.D, DroneAction.MoveRight);

    }


    // Update is called once per frame
    private void Update()
    {
        //TODO: use unity input system instead of hard coded keys
        foreach(KeyCode key in keyActions.Keys)
        {
            if(Input.GetKeyDown(key) == true)
            {
                dalekBehaviour.PerformAction(keyActions[key]);
                break;
            }
        }

    }
}
