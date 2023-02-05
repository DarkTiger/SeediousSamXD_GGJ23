using UnityEngine;
using UnityEngine.InputSystem;

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

    PlayerInput playerInput;
    SkinnedMeshRenderer skinnedMeshRenderer;
    Animator animator;
    Rigidbody rb;
    bool isGrounded = false;
    int selectedSlot = -1;
    public bool Died { get; private set; } = false;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = GetComponentInChildren<Animator>();
        playerInput = GetComponent<PlayerInput>();
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
    }

    private void Update()
    {
        if (Died) return;

        if (JumpAction.WasPressedThisFrame() && isGrounded)
        {
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
    }

    public void SelectSlot(int slotIndex)
    {
        HUD.Instance.MoveSelectorPlayer(PlayerIndex, slotIndex);
    }

    public void Attack()
    {
        SeedBag[] seedBags = FindObjectsByType<SeedBag>(FindObjectsSortMode.None);

        for (int i = 0; i < seedBags.Length; i++)
        {
            if ((seedBags[i].transform.position - transform.position).magnitude <= 3.5f &&
                Vector3.Dot((seedBags[i].transform.position - transform.position).normalized, transform.forward) >= 0.7f)
            {
                seedBags[i].OpenBag();
            }
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
            animator.SetTrigger("TakeDamage");
        }
    }
    
    public void Die()
    {
        animator.SetBool("Died", true);
        Died = true;
    }
}
