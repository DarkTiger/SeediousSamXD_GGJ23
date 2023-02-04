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

    PlayerInput playerInput;
    MeshRenderer meshRenderer;
    Rigidbody rb;
    bool isGrounded = false;
    int playerIndex = -1;
    int selectedSlot = -1;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        playerInput = GetComponent<PlayerInput>();
        MoveAction = playerInput.actions["Movement"];
        VisualAction = playerInput.actions["Visual"];
        JumpAction = playerInput.actions["Jump"];
        InventoryAction = playerInput.actions["Inventory"];
        AttackAction = playerInput.actions["Attack"];
    }

    void Start()
    {
        if (FindObjectsByType<Player>(FindObjectsSortMode.None).Length > 1)
        {
            transform.position = new Vector3(18.65f, 2.83f, -3f);
            transform.localEulerAngles = new Vector3(0f, -90f, 0f);
            meshRenderer.material = materials[0];
            playerIndex = 2;
        }
        else
        {
            transform.position = new Vector3(-18.65f, 2.83f, -3f);
            transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            meshRenderer.material = materials[1];
            playerIndex = 1;
        }
    }

    void FixedUpdate()
    {
        Vector2 movement = MoveAction.ReadValue<Vector2>() * movementSpeed;      
        if (movement.magnitude > 1f)
        {
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.y);

            Vector3 rbVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            transform.forward = Vector3.Lerp(transform.forward, rbVelocity.normalized, 1f);
        }
        else 
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        isGrounded = Physics.OverlapSphere(transform.position - (transform.up * 0.475f), 0.75f).Length > 1;
    }

    private void Update()
    {
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
            Attack();
        }
    }

    public void SelectSlot(int slotIndex)
    {
        HUD.Instance.MoveSelectorPlayer(playerIndex, slotIndex);
    }

    public void Attack()
    {
        SeedBag[] seedBags = FindObjectsByType<SeedBag>(FindObjectsSortMode.None);

        for (int i = 0; i < seedBags.Length; i++)
        {
            if ((seedBags[i].transform.position - transform.position).magnitude <= 2.2f &&
                Vector3.Dot((seedBags[i].transform.position - transform.position).normalized, transform.forward) >= 0.75f)
            {
                seedBags[i].OpenBag();
            }
        }
    }

    public void TakeDamage(float damage) 
    {
        Health = Mathf.Clamp(Health - damage, 0f, 100f);

        HUD.Instance.UpdateHealthbarPlayer(playerIndex, Health);

        if (Health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {

    }
}
