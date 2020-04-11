using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetRockBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject cellsObject;
    [SerializeField]
    private float explosionForce = 4f;
    [SerializeField]
    private float explosionRadius = 1f;
    [SerializeField]
    private float explosionUpForce = 0f;


    private MeshRenderer solidMeshRenderer;
    private Rigidbody solidRigidBody;
    private Collider solidCollider;

    private void Awake()
    {
        solidMeshRenderer = GetComponent<MeshRenderer>();
        solidRigidBody = GetComponent<Rigidbody>();
        solidCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Laser")
        {
            StartCoroutine(ExplodeRoutine());
        }
    }

    private IEnumerator ExplodeRoutine()
    {
        Material cellMaterial = new Material(solidMeshRenderer.material);

        solidRigidBody.isKinematic = true;
        solidCollider.enabled = false;
        solidMeshRenderer.enabled = false;
        cellsObject.SetActive(true);

        MeshRenderer[] renderers = cellsObject.GetComponentsInChildren<MeshRenderer>();
        Rigidbody[] bodies = cellsObject.GetComponentsInChildren<Rigidbody>();
        foreach (var rb in bodies)
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpForce, ForceMode.Impulse);
        }

        //add the disolve shader graph material
        foreach (var renderer in renderers)
        {
            renderer.material = cellMaterial;
        }

        yield return new WaitForSeconds(1.25f);


        for (float percent = 0f; percent <= 1f; percent += 0.1f)
        {
            //name taken from the disolve shader's percent variable
            cellMaterial.SetFloat("Vector1_Percent", percent);
            yield return new WaitForSeconds(0.05f);
        }

        //destroy ourselves
        Destroy(gameObject);

    }
}
