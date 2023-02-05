using System;
using System.Collections;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] ItemType type = ItemType.None;
    [SerializeField] float damage = 10f;
    [SerializeField] float fireRange = 10f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float aimSpeed = 5f;
    [SerializeField] float bulletSpawnDelay = 1f;
    [SerializeField] Transform firePosition = null;
    [SerializeField] GameObject bulletPrefab = null;

    public int PlayerIndex { get; set; }

    bool armed = false;
    Player enemyPlayer = null;
    float currentFireTime = 0f;
    Animator animator = null;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>(true);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);

        PlayerIndex = 2;

        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].PlayerIndex != PlayerIndex)
            {
                enemyPlayer = players[i];
                break;
            }
        }

        armed = true;
    }

    private void Update()
    {
        if (armed && enemyPlayer && !enemyPlayer.Died) 
        {
            if ((enemyPlayer.transform.position - transform.position).magnitude <= fireRange)
            {
                Vector3 direction = (enemyPlayer.transform.position - transform.position).normalized;
                direction.y = 0f;
                transform.forward = Vector3.Lerp(transform.forward, direction, aimSpeed * Time.deltaTime);
            }
        
            if (Vector3.Dot((enemyPlayer.transform.position - transform.position).normalized, transform.forward) > 0.95f)
            {
                currentFireTime += Time.deltaTime;

                if (currentFireTime >= fireRate)
                {
                    StartCoroutine(Fire());
                }
            }
        }
    }

    public IEnumerator Fire()
    {
        currentFireTime = 0f;
        animator.SetTrigger("Fire");

        yield return new WaitForSeconds(bulletSpawnDelay);

        Bullet bullet = Instantiate(bulletPrefab, firePosition.position, firePosition.rotation).GetComponent<Bullet>();
        bullet.PlayerIndex = PlayerIndex;
        bullet.Damage = damage;

        Destroy(bullet.gameObject, 10f);
    }
}
