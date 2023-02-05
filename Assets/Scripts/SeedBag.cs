using UnityEngine;

public class SeedBag : MonoBehaviour
{
    [SerializeField] Seed[] seeds;

    public void OpenBag()
    {
        int seedsCount = Random.Range(1, 3);

        for (int i = 0; i < seedsCount; i++)
        {
            Instantiate(seeds[Random.Range(0, seeds.Length)], transform.position + (Vector3.up * 2f), Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
