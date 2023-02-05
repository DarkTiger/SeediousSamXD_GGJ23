//using System;
using System.Collections;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] ItemType type = ItemType.None;
    [SerializeField] float damage = 10f;
    [SerializeField] float fireRange = 10f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float aimSpeed = 5f;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float bulletSpawnDelay = 1f;
    [SerializeField] Transform firePosition = null;
    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] Material[] materials;
    [SerializeField] AudioClip FireSFX;
    [SerializeField] AudioClip DeadSFX;

    public int Level { get; set; } = 1;
    public int PlayerIndex { get; set; }
    public int Health { get; set; } = 3;

    bool armed = false;
    Player enemyPlayer = null;
    float currentFireTime = 0f;
    Animator animator = null;
    bool dead = false;
    Camera camera;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>(true);
        camera = FindObjectOfType<Camera>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);

        transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>(true).material = materials[PlayerIndex-1];      
        transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>(true).materials[1].color = new Color(0.5f, 0.5f, 0.5f);
        transform.GetComponent<Collider>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);

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
        if (dead) return;

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

        if (!dead)
        {
            if (type == ItemType.Shotgun)
            {
                for (int i = 0; i < 5; i++)
                {
                    SpawnBullet(new Vector3(0f, Random.Range(-20f, 20f), 0f), firePosition.forward * Random.Range(0f, 2.5f));
                }
            }
            else
            {
                SpawnBullet(Vector3.zero, Vector3.zero);
            }
        }
    }

    void SpawnBullet(Vector3 rotOffset, Vector3 posOffset)
    {
        AudioSource.PlayClipAtPoint(FireSFX, camera.transform.position);

        Bullet bullet = Instantiate(bulletPrefab, firePosition.position + posOffset, Quaternion.Euler(firePosition.eulerAngles + rotOffset)).GetComponent<Bullet>();
        bullet.PlayerIndex = PlayerIndex;
        bullet.Damage = damage;
        bullet.Speed = bulletSpeed;
        Destroy(bullet.gameObject, 10f);
    }

    public void TakeDamage()
    {
        Health = Mathf.Clamp(Health - 1, 0, 4);

        if (Health <= 0)
        {
            animator.SetBool("Dead", true);
            dead = true;
            StartCoroutine(Die());
        }
        else
        {
            animator.SetTrigger("TakeDamage");
        }
    }

    IEnumerator Die()
    {
        AudioSource.PlayClipAtPoint(DeadSFX, camera.transform.position);

        yield return new WaitForSeconds(1.25f);

        Destroy(gameObject);
    }

    public void LevelUp()
    {
        Level++;

        Color metalColor = new Color(0.5f, 0.5f, 0.5f);
        if (Level == 2)
        {
            metalColor = new Color(0.75f, 0.75f, 0.75f);
        }
        else if (Level == 3)
        {
            metalColor = new Color(1f, 1f, 0f);
        }
    }
}
