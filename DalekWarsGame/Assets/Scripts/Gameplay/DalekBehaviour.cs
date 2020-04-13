using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DalekBehaviour : MonoBehaviour
{
    //transforms from the model/bones itself to allow for animation or position control
    [SerializeField]
    private Transform head = null;
    [SerializeField]
    private Transform mid = null;
    [SerializeField]
    private Transform lower = null;
    [SerializeField]
    private Transform laserShot = null;
    [SerializeField]
    private AudioSource dalekLaserAudio;

    //laser bullet is used to trigger the colission on the pet rock
    //the collision is what triggers the pet rock to explode properly
    [SerializeField]
    private GameObject laserBullet = null;
    [SerializeField]
    private GameObject laserBeam = null;

    [SerializeField]
    private float movementLerpSpeed = 2f;

    [SerializeField]
    private TextMeshProUGUI energyText;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    //the core drone logic that this dalek utilizes
    private Drone drone;
    

    //tracking the current state to prevent overlapping movements
    private bool isMoving = false;
    private bool isShooting = false;

    private void Start()
    {
        //drone must be set up in start, the rock map is set up in Awake
        //this ensures the rock map will always be completed first regardless of execution order.
        drone = new Drone(RockMapController.Instance.Map, 25);
        energyText.text = drone.CurrentEnergy.ToString();
        scoreText.text = "0";
    }

    public void PerformAction(DroneAction action)
    {
        if (isMoving == true || isShooting == true)
            return;

        //perform the back end action
        bool success = drone.PerformAction(action);

        //show the action to the player
        switch (action)
        {
            case DroneAction.MoveUp:
                StartCoroutine(MoveToCoroutine(transform.position + Drone.UP, success));
                break;
            case DroneAction.MoveDown:
                StartCoroutine(MoveToCoroutine(transform.position + Drone.DOWN, success));
                break;
            case DroneAction.MoveLeft:
                StartCoroutine(MoveToCoroutine(transform.position + Drone.LEFT, success));
                break;
            case DroneAction.MoveRight:
                StartCoroutine(MoveToCoroutine(transform.position + Drone.RIGHT, success));
                break;
            case DroneAction.CollectUp:
                StartCoroutine(ShootAtCoroutine(transform.position + Drone.UP));
                break;
            case DroneAction.CollectDown:
                StartCoroutine(ShootAtCoroutine(transform.position + Drone.DOWN));
                break;
            case DroneAction.CollectLeft:
                StartCoroutine(ShootAtCoroutine(transform.position + Drone.LEFT));
                break;
            case DroneAction.CollectRight:
                StartCoroutine(ShootAtCoroutine(transform.position + Drone.RIGHT));
                break;
        }
    }

    #region primary move and shoot animations
    private IEnumerator ShootAtCoroutine(Vector3 position)
    {
        isShooting = true;

        Vector3 direction = (position - transform.position).normalized;
        yield return RotateToDirection(direction, mid, movementLerpSpeed * 6);

        dalekLaserAudio.Play();
        yield return new WaitForSeconds(0.3f);

        Vector3 laserEndPosition = new Vector3(position.x, 0.3f, position.z);
        
        laserBeam.SetActive(true);
        LineRenderer renderer = laserBeam.GetComponent<LineRenderer>();

        renderer.SetPosition(0, laserShot.position);
        renderer.SetPosition(1, laserEndPosition);

        laserBullet.SetActive(true);
        laserBullet.transform.position = laserShot.position;
        yield return MoveToPosition(laserEndPosition, laserBullet.transform, movementLerpSpeed * 3);

        laserBullet.SetActive(false);
        laserBeam.SetActive(false);

        //update HUD
        energyText.text = drone.CurrentEnergy.ToString();
        scoreText.text = drone.GetTotalScore(false).ToString();

        isShooting = false;
    }

    public IEnumerator MoveToCoroutine(Vector3 position, bool success)
    {
        isMoving = true;

        Vector3 direction = (position - transform.position).normalized;
        yield return RotateToDirection(direction, transform, movementLerpSpeed * 3f);
        yield return RotateToDirection(direction, mid, movementLerpSpeed * 6);

        if (success == true)
        {
            yield return MoveToPosition(position, transform, movementLerpSpeed);

            
        }
        else
        {
            Vector3 originalPosition = transform.position;
            position = transform.position + ((position - transform.position) / 3f);
            yield return MoveToPosition(position, transform, movementLerpSpeed);
            yield return MoveToPosition(originalPosition, transform, movementLerpSpeed);
            yield return ShakeHead(20);
        }

        //update HUD
        energyText.text = drone.CurrentEnergy.ToString();

        //TODO: do something in the UI to show the player can move again
        isMoving = false;
    }
    #endregion

    #region reusable methods for coroutines
    private IEnumerator ShakeHead(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            head.transform.Rotate(0, 1, 0);
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < amount * 2; i++)
        {
            head.transform.Rotate(0, -1, 0);
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < amount * 2; i++)
        {
            head.transform.Rotate(0, 1, 0);
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < amount; i++)
        {
            head.transform.Rotate(0, -1, 0);
            yield return new WaitForEndOfFrame();
        }

    }

    private IEnumerator RotateToDirection(Vector3 direction, Transform transformToRotate, float speed)
    {
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        float angle = Quaternion.Angle(lookRotation, transformToRotate.rotation);
        while (angle >= Time.deltaTime * speed)
        {
            transformToRotate.rotation = Quaternion.Lerp(transformToRotate.rotation, lookRotation, Time.deltaTime * speed);
            angle = Quaternion.Angle(lookRotation, transformToRotate.rotation);

            yield return new WaitForEndOfFrame();
        }

        //snap to rotation
        transformToRotate.rotation = lookRotation;
    }

    private IEnumerator MoveToPosition(Vector3 position, Transform transformToMove, float speed)
    {
        float distance = Vector3.Distance(transformToMove.position, position);
        while (distance >= Time.deltaTime * (speed / 2f))
        {
            transformToMove.position = Vector3.Lerp(transformToMove.position, position, Time.deltaTime * speed);
            distance = Vector3.Distance(transformToMove.position, position);
            yield return new WaitForEndOfFrame();
        }

        //snap to position
        transformToMove.position = position;
    }
    #endregion
}
