using UnityEngine;

public class Seed : MonoBehaviour
{
    [SerializeField] ItemType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject plantPrefab;

    void Start()
    {
        rb.AddExplosionForce(25f, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), -1f, Random.Range(-0.5f, 0.5f)), 2.5f);
    }
}
