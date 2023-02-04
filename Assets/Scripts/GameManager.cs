using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject seedBagPrefab;
    bool enabledSpawnBag = true;


    private IEnumerator Start()
    {
        while (enabledSpawnBag)
        {
            yield return new WaitForSeconds(Random.Range(10f, 20f));

            Instantiate(seedBagPrefab, new Vector3(Random.Range(-22f, 22f), 30f, Random.Range(-15f, 15f)), Quaternion.Euler(-90f, 0f, 0f));
        }
    }
}
