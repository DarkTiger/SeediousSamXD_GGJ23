using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    public int PlayerIndex { get; set; } = -1;
    public float Damage { get; set; } = 1f;


    private void Start()
    {
        GetComponent<MeshRenderer>().material.color = PlayerIndex == 1 ? Color.red : new Color(0f, 0.5f, 1f);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player && player.PlayerIndex != PlayerIndex)
        {
            player.TakeDamage(Damage);
        }

        Destroy(gameObject);
    }
}
