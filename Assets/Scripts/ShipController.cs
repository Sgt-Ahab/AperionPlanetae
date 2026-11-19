//Name: David Sargent
//Date: 11/15/26
//Desc: ShipController for the PlayerParent/Ship Sprite
//Attach: Player/Player Sprite
//==============================================
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


//Ship moves, fires projectiles, and drops bombs on Ground Units, since any collision kills one
//Must make it inquire to restart, that is out of scope, just discussing the objects interaction;
//Polygon Collider; RigidBody for world affects + powerups
// Make a Jump motion to prevent collision and fulfill jump clause of rubric
public class ShipController : MonoBehaviour
{
    private Rigidbody2D rb;
    [Header("Ship Physics")]
    [SerializeField]
    private float ShipSpeed = 3;
    //Each planet object will have SLIGHTLY different variables. I enjoy the random and chaos!
    private float BaseSpeed;
    [SerializeField]
    public float PlanetInertia;
    [SerializeField]
    public float PlanetGravity;
    public InputAction ShipControlSuite;
    public InputAction ShipActionSuite;
    public InputAction ShipJumpAction;
    [Header("World Data")]
    [SerializeField]
    private float ProjectilePerShot;
    //ScreenCap prevents bullet spam, though I am thinking to incorporate continuous stream;
    //However is here for the case of
    [SerializeField]
    private GameObject BulletPrefab;
    [SerializeField]
    private Transform[] BulletSpawnpoints;
    [SerializeField]
    private GameObject ShieldPrefab;
    // Add in the 'GameStateManager'
    private GameManager GameBrain;
    [Header("Ship Jump Data")]
    private bool IsDiving;
    private bool HasShield;
    private bool HasBoost;
    [SerializeField]
    private float JumpDuration;
    [SerializeField]
    private PolygonCollider2D ShipCollider;
    [Header("Collectable Data")]
    [SerializeField]
    private float SpeedMultiplier;
    [SerializeField]
    private float SpeedDuration;
    private GameObject ActiveShield;
    private Vector3 OriginalScale;
    private Vector3 DiveScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Init objs
        GameBrain = FindAnyObjectByType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        ShipCollider = GetComponent<PolygonCollider2D>();
        ShipControlSuite.Enable();
        ShipActionSuite.Enable();
        ShipJumpAction.Enable();
        //ShipSpeed being 3, makes it so the Inertia being solid
        ShipSpeed = (ShipSpeed + PlanetGravity) / PlanetInertia;
        BaseSpeed = ShipSpeed;
        IsDiving = false;
        HasShield = false;
        HasBoost = false;
        OriginalScale = this.transform.localScale;
        DiveScale.x = OriginalScale.x * 1.5f;
        DiveScale.y = OriginalScale.y * 1.5f;
        DiveScale.z = OriginalScale.z;
    }

    private void FixedUpdate()
    {
        ShipInteraction();
    }
    // Update is called once per frame
    private void Update()
    {
        if(ShipActionSuite.WasPressedThisFrame() && IsDiving == false)

        {
            for (int i = 0; i < BulletSpawnpoints.Length; i++)
            {
                Vector2 Spawnpoint = BulletSpawnpoints[i].position;
                Shoot(Spawnpoint);
            }
        }
        if(ShipJumpAction.WasPressedThisFrame() && IsDiving == false)
        {
            StartCoroutine(EagleJump());
            IEnumerator EagleJump()
            {
                //This is the Jump coroutine, boolflip, deactivate collider
                IsDiving = true;
                ShipCollider.enabled = false;
                transform.localScale = DiveScale;
                yield return new WaitForSeconds(JumpDuration);
                transform.localScale = OriginalScale;
                ShipCollider.enabled = true;
                IsDiving = false;
            }
        }
    }
    private void ShipInteraction()
    {
        //Since the ControlSuite expands from WASD + ARROWS with Space for bombs,
        //and I think Ctrl L|R both can fire
        //Update Location for movement, make OB, and cannot move past it (bcuz polygon collider)
        Vector2 ShipLocation = ShipControlSuite.ReadValue<Vector2>();
        rb.linearVelocity = ShipLocation * ShipSpeed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Collision Check-chain
        if(collision.gameObject.CompareTag("Enemy"))
        {
            //Summon End Game state
            if(HasShield)
            {
                Destroy(ActiveShield);
                HasShield = false;
                Destroy(collision.gameObject);
            }
            else
            {
                GameBrain.GameOverState();
                Destroy(this.gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("EnemyBullet"))
        {
            if(HasShield)
            {
                Destroy(ActiveShield);
                HasShield = false;
                Destroy(collision.gameObject);
            }
            else
            {
                GameBrain.GameOverState();
                Destroy(this.gameObject);
            }
        }
    }
    private void Shoot(Vector2 SpawnPOS)
    {
        GameObject Bullet = Instantiate(BulletPrefab, SpawnPOS, Quaternion.identity);
        Bullet.tag = "PlayerBullet";
        Vector2 ShootDirection = Vector2.up;

        Bullet.GetComponent<Projectile>().Setup(ShootDirection);
    }
    public void ActivateShield()
    {
        if(HasShield == false)
        {
            HasShield = true;
            ActiveShield = Instantiate(ShieldPrefab, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform);
        }
        
        
    }
    public void ActivateBoost()
    {
        if(HasBoost == false)
        {
            StartCoroutine(BoostCycle());
            IEnumerator BoostCycle()
            {
                HasBoost = true;
                ShipSpeed = ShipSpeed * SpeedMultiplier;
                yield return new WaitForSeconds(SpeedDuration);
                ShipSpeed = BaseSpeed;
                HasBoost = false;
            }
        }
    }
}
