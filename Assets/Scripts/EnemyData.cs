//Name: David Sargent
//Date: 11/17/26
//Desc: EnemyData, serialized fields for the handling of the enemies
//Attach: Enemy Prefabs for their data
//==============================================
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField]
    private int EnemyHealth;
    [SerializeField]
    private float PointValue;
    [SerializeField]
    private GameObject[] CollectablePrefabs;
    //Beam / Bullet / BoolCheck
    [SerializeField]
    private GameObject BulletPrefab;
    [SerializeField]
    private bool CanShoot;
    [SerializeField]
    private bool IsDollop;
    [SerializeField]
    private float ProjectilePeriod;
    [SerializeField]
    private Transform[] ProjectileSpawns;
    private float ProjectileTimer = 0f;
    [SerializeField]
    private float VerticalSpeed;
    [SerializeField]
    private float HorizontalSpeed;
    private int MoveDirX;
    private GameManager GameBrain;
    private Transform PlayerPOS;
    private Rigidbody2D rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameBrain = FindAnyObjectByType<GameManager>();
        PlayerPOS = FindAnyObjectByType<ShipController>().transform;
        rb = GetComponent<Rigidbody2D>();
        EnemyMove();
    }
    private void Update()
    {
        if(CanShoot == true && IsDollop == false)
        {
            ProjectileTimer += Time.deltaTime;
            if(ProjectileTimer >= ProjectilePeriod)
            {
                for (int i = 0; i < ProjectileSpawns.Length; i++)
                {
                    GameObject EnemyBullet = Instantiate(BulletPrefab, ProjectileSpawns[i].position, Quaternion.identity);
                    EnemyBullet.tag = "EnemyBullet";
                    Projectile BulletScript = EnemyBullet.GetComponent<Projectile>();
                    BulletScript.Setup(Vector2.down);                    
                }
                ProjectileTimer = 0f;
            }
        }
        else if(CanShoot == true && IsDollop == true)
        {
            ProjectileTimer += Time.deltaTime;
            if(ProjectileTimer >= ProjectilePeriod)
            {
                DollopFire();
                ProjectileTimer = 0f;
            }
            

        }        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("PlayerBullet"))
        {
            EnemyHealth--;
            if(EnemyHealth <= 0)
            {
                GameBrain.AddScore(PointValue);
                int DropRoll = Random.Range(1, 101);
                if(DropRoll <= 30 && CollectablePrefabs.Length > 0)
                {
                    int CollectableIndex = Random.Range(0, CollectablePrefabs.Length);
                    Instantiate(CollectablePrefabs[CollectableIndex], transform.position, Quaternion.identity); 
                }
                Destroy(this.gameObject);
            }
        }
    }
    //If invisible DESTROY the object, and set the points to 0;
    private void OnBecameInvisible()
    {        
        //On destroy, the points are not called, but easily can add the score if they weave it
        Destroy(this.gameObject);
    }
    private void EnemyMove()
    {
        //Randomly move L/R, check if colliding with OB then random paces away then repeat algo
        //Random 0 - 1, 0 - L, 1 - R;
        MoveDirX = Random.Range(0, 2);
        switch(MoveDirX)
        {
            case 0:
                MoveDirX = -1;
                break;
            case 1:              
                break;
        }
        rb.linearVelocity = new Vector2(MoveDirX * HorizontalSpeed, -VerticalSpeed);
    }
    private void DollopFire()
    {
        if(PlayerPOS == null)
        {
            GameObject EnemyBullet = Instantiate(BulletPrefab, ProjectileSpawns[0].position, Quaternion.identity);
            EnemyBullet.tag = "EnemyBullet";
            Projectile BulletScript = EnemyBullet.GetComponent<Projectile>();
            BulletScript.Setup(Vector2.down);
        }
        else
        {          
            Vector2 FireDirection;
            //SpawnPointPOS - PlayerPOS
            Vector2 TargetVectors = PlayerPOS.position - ProjectileSpawns[0].position;
            FireDirection = TargetVectors.normalized;
            GameObject EnemyBullet = Instantiate(BulletPrefab, ProjectileSpawns[0].position, Quaternion.identity);
            EnemyBullet.tag = "EnemyBullet";
            Projectile BulletScript = EnemyBullet.GetComponent<Projectile>();
            BulletScript.Setup(FireDirection);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("OB"))
        {
            MoveDirX *= -1;
            rb.linearVelocity = new Vector2(MoveDirX * HorizontalSpeed, -VerticalSpeed);
            if(collision.gameObject.name == "OB_B")
            {
                GameBrain.AddScore(PointValue);
                Destroy(gameObject);
            }
        }
        else if(collision.gameObject.CompareTag("OB") && transform.parent.gameObject.CompareTag("Parent"))      
        {
            //This is for the Enclave Transform Cleanup
            Destroy(transform.parent.gameObject);
        }
        
    }   
}
