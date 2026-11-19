//Name: David Sargent
//Date: 11/15/26
//Desc: Projectile Script for bullets PlayerTag / EnemyTag
//Attach: Bullet{Prefab}
//=======================================================
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Bullet Physics")]
    [SerializeField]
    private float BulletSpeed;
    [SerializeField]
    public float PlanetInertia;
    [SerializeField]
    public float PlanetGravity;
    private Rigidbody2D rb;

    private void Awake()
    {
        BulletSpeed = (BulletSpeed + PlanetGravity) / PlanetInertia;
    }
    public void Setup(Vector2 Direction)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Direction * BulletSpeed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check via Tags & This Objects tag
        if(collision.gameObject.CompareTag("Player") && this.gameObject.CompareTag("EnemyBullet"))
        {
            //This is enemy bullet hitting player object;             
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.CompareTag("Enemy") && this.gameObject.CompareTag("PlayerBullet"))
        {
            //Player bullet hits enemy object; Just destroy, this ensures Health on Player Side
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.CompareTag("OB"))
        {
            Destroy(this.gameObject);   
        }
    }
    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
