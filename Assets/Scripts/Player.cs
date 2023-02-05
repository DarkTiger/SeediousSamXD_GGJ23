using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed = 0.1f;
    [SerializeField] float jumpForce = 1f;
    [SerializeField] Material[] materials;

    public float Health = 100f;

    public InputAction MoveAction { get; private set; }
    public InputAction VisualAction { get; private set; }
    public InputAction JumpAction { get; private set; }
    public InputAction InventoryAction { get; private set; }
    public InputAction AttackAction { get; private set; }
    public InputAction GatherAction { get; private set; }
    public InputAction UseObjectAction { get; private set; }
    public InputAction BucketAction { get; private set; }
    public int PlayerIndex { get; private set; } = -1;
    public bool Died { get; private set; } = false;

    public ItemType[] InventorySlots = new ItemType[4];

    [SerializeField] PlantPrefab[] plantsPrefabs;

    [Serializable]
    public struct PlantPrefab
    {
        public ItemType Type;
        public GameObject Prefab;
    }

    PlayerInput playerInput;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Animator animator;
    Camera camera;
    public Rigidbody rb { get; private set; }
    bool isGrounded = false;
    bool lastGrounded = false;
    int selectedSlot = 0;

    [SerializeField] AudioClip JumpP1SFX;
    [SerializeField] AudioClip JumpP2SFX;
    [SerializeField] AudioClip AttackEmptySFX;
    [SerializeField] AudioClip AttackHitSFX;
    [SerializeField] AudioClip[] TakeDamageP1SFX;
    [SerializeField] AudioClip[] TakeDamageP2SFX;
    [SerializeField] AudioClip LandingP1SFx;
    [SerializeField] AudioClip LandingP2SFX;
    [SerializeField] AudioClip[] PlantSFX;
    [SerializeField] AudioClip GatherSFX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
        camera = FindObjectOfType<Camera>();

        MoveAction = playerInput.actions["Movement"];
        VisualAction = playerInput.actions["Visual"];
        JumpAction = playerInput.actions["Jump"];
        InventoryAction = playerInput.actions["Inventory"];
        AttackAction = playerInput.actions["Attack"];
        GatherAction = playerInput.actions["Gather"];
        UseObjectAction = playerInput.actions["UseObject"];
        BucketAction = playerInput.actions["Bucket"];
    }

    void Start()
    {
        if (FindObjectsByType<Player>(FindObjectsSortMode.None).Length > 1)
        {
            transform.position = new Vector3(18.65f, 2.83f, -3f);
            transform.localEulerAngles = new Vector3(0f, -90f, 0f);
            skinnedMeshRenderer.material = materials[0];
            PlayerIndex = 2;
        }
        else
        {
            transform.position = new Vector3(-18.65f, 2.83f, -3f);
            transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            skinnedMeshRenderer.material = materials[1];
            PlayerIndex = 1;
        }
    }

    void FixedUpdate()
    {
        if (Died) return;

        Vector2 movement = MoveAction.ReadValue<Vector2>() * movementSpeed;      
        if (movement.magnitude > 7.5f)
        {
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.y);
        }
        else 
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        isGrounded = Physics.OverlapSphere(transform.position - (transform.up * 0.475f), 0.75f).Length > 1;

        if (lastGrounded != isGrounded)
        {
            AudioSource.PlayClipAtPoint(PlayerIndex == 1? LandingP1SFx : LandingP2SFX, camera.transform.position);
            lastGrounded = isGrounded;
        }
    }

    private void Update()
    {
        if (Died) return;

        if (JumpAction.WasPressedThisFrame() && isGrounded)
        {
            AudioSource.PlayClipAtPoint(PlayerIndex == 1? JumpP1SFX : JumpP2SFX, camera.transform.position);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        Vector2 inventoryInput = InventoryAction.ReadValue<Vector2>();
        if (inventoryInput.y == 1)
        {
            SelectSlot(0);
        }
        else if (inventoryInput.y == -1)
        {
            SelectSlot(2);
        }
        else if (inventoryInput.x == 1)
        {
            SelectSlot(3);
        }
        else if (inventoryInput.x == -1)
        {
            SelectSlot(1);
        }

        if (AttackAction.WasPressedThisFrame()) 
        {
            animator.SetTrigger("Attack");
        }

        Vector2 movement = MoveAction.ReadValue<Vector2>() * movementSpeed;
        if (movement.magnitude > 10f)
        {
            Vector3 rbVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (rbVelocity != Vector3.zero)
            {
                transform.forward = rbVelocity.normalized;
            }
        }

        animator.SetBool("Move", rb.velocity.magnitude > 5f);

        if (GatherAction.WasPressedThisFrame())
        {
            Gather();
        }

        if (UseObjectAction.WasPressedThisFrame())
        {
            UseObject();
        }
    }

    public void SelectSlot(int slotIndex)
    {
        HUD.Instance.MoveSelectorPlayer(PlayerIndex, slotIndex);
        selectedSlot = slotIndex;
    }

    private void Gather()
    {
        Seed[] seeds = FindObjectsByType<Seed>(FindObjectsSortMode.None);

        for (int i = 0; i < seeds.Length; i++)
        {
            if ((seeds[i].transform.position - transform.position).magnitude <= 2.5f &&
                Vector3.Dot((seeds[i].transform.position - transform.position).normalized, transform.forward) >= 0.7f)
            {
                for (int s = 0; s < InventorySlots.Length; s++)
                {
                    if (i < InventorySlots.Length && InventorySlots[s] == ItemType.None)
                    {
                        AudioSource.PlayClipAtPoint(GatherSFX, camera.transform.position);
                        InventorySlots[s] = seeds[i].ItemType;
                        HUD.Instance.UpdateInventory(this);
                        Destroy(seeds[i].gameObject);
                        return;
                    }
                }
            }
        }
    }

    private void UseObject()
    {
        ItemType itemType = InventorySlots[selectedSlot];

        if (itemType == ItemType.Sniper || itemType == ItemType.SMG || itemType == ItemType.Shotgun || itemType == ItemType.Pistol)
        {
            for (int i = 0; i < plantsPrefabs.Length; i++)
            {
                if (plantsPrefabs[i].Type == itemType)
                {
                    if (plantsPrefabs[i].Prefab)
                    {
                        AudioSource.PlayClipAtPoint(PlantSFX[Random.Range(0, PlantSFX.Length)], camera.transform.position);

                        Vector3 position = transform.position + transform.forward;
                        position.y = 0f;

                        Instantiate(plantsPrefabs[i].Prefab, position, transform.rotation).GetComponent<Plant>().PlayerIndex = PlayerIndex; ;
                        InventorySlots[selectedSlot] = ItemType.None;
                        HUD.Instance.UpdateInventory(this);
                        animator.SetTrigger("Plant");
                        break;
                    }   
                }
        }
            }           
        else
        {

        }
    }

    public void Attack()
    {
        bool emptyHit = true;

        SeedBag[] seedBags = FindObjectsByType<SeedBag>(FindObjectsSortMode.None);
        for (int i = 0; i < seedBags.Length; i++)
        {
            if ((seedBags[i].transform.position - transform.position).magnitude <= 3.5f &&
                Vector3.Dot((seedBags[i].transform.position - transform.position).normalized, transform.forward) >= 0.7f)
            {
                emptyHit = false;
                AudioSource.PlayClipAtPoint(AttackHitSFX, camera.transform.position);
                seedBags[i].OpenBag();
                break;
            }
        }

        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == this) continue;

            if ((players[i].transform.position - transform.position).magnitude <= 3.5f &&
                Vector3.Dot((players[i].transform.position - transform.position).normalized, transform.forward) >= 0.7f)
            {
                emptyHit = false;
                AudioSource.PlayClipAtPoint(AttackHitSFX, camera.transform.position);
                players[i].rb.AddForce((transform.forward) + (Vector3.up ) * 50f, ForceMode.Impulse);
                break;
            }
        }

        Plant[] plants = FindObjectsByType<Plant>(FindObjectsSortMode.None);
        for (int i = 0; i < plants.Length; i++)
        {
            if (plants[i].PlayerIndex == PlayerIndex) continue;

            if ((plants[i].transform.position - transform.position).magnitude <= 3.5f &&
                Vector3.Dot((plants[i].transform.position - transform.position).normalized, transform.forward) >= 0.7f)
            {
                emptyHit = false;
                AudioSource.PlayClipAtPoint(AttackHitSFX, camera.transform.position);
                plants[i].TakeDamage();
                break;
            }
        }

        if (emptyHit)
        {
            AudioSource.PlayClipAtPoint(AttackEmptySFX, camera.transform.position);
        }
    }

    public void TakeDamage(float damage) 
    {
        Health = Mathf.Clamp(Health - damage, 0f, 100f);

        HUD.Instance.UpdateHealthbarPlayer(PlayerIndex, Health);

        if (Health <= 0)
        {
            Die();
        }
        else
        {
            AudioSource.PlayClipAtPoint(PlayerIndex == 1? TakeDamageP1SFX[Random.Range(0, TakeDamageP1SFX.Length)] : TakeDamageP2SFX[Random.Range(0, TakeDamageP2SFX.Length)], camera.transform.position);
            animator.SetTrigger("TakeDamage");
        }
    }
    
    public void Die()
    {
        AudioSource.PlayClipAtPoint(PlayerIndex == 1 ? TakeDamageP1SFX[Random.Range(0, TakeDamageP1SFX.Length)] : TakeDamageP2SFX[Random.Range(0, TakeDamageP2SFX.Length)], camera.transform.position);
        animator.SetBool("Died", true);
        Died = true;
    }
}
