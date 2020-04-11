using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target = null;


    private Vector3 offset;
    private Camera followCamera;
    

    private void Start()
    {
        followCamera = GetComponent<Camera>();
        offset = transform.position - target.position;
    }

    private void Update()
    {
        transform.position = target.position + offset;
    }
}
