//Name: David Sargent
//Date: 11/18/26 [11/17/26 after midnight]
//Desc: CollectableData for different items to fulfill collectable clause
//Attach: Collectable Prefabs
using System;
using UnityEngine;

public class CollectableData : MonoBehaviour
{
    [Header("Collectable Information")]
    [SerializeField]
    private int CollectableID;
    [SerializeField]
    private float FallSpeed;
    //For ID = 0; PointsDisc;
    [SerializeField]
    private float PointValue;
    private GameManager GameBrain;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameBrain = FindAnyObjectByType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.down * FallSpeed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Collectable check, cache the ShipController for usage
        if(collision.gameObject.CompareTag("Player"))
        {
            ShipController Ship = collision.GetComponent<ShipController>();
            switch(CollectableID)
            {
                //IDs: 0 - Points; 1 - Shield; 2 - Speed;
                case 0:
                    GameBrain.AddScore(PointValue);
                    break;
                case 1:
                    Ship.ActivateShield();
                    break;
                case 2:
                    Ship.ActivateBoost();
                    break;
            }
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.CompareTag("OB"))
        {
            Destroy(this.gameObject);
        }
    }
    //Cleanup Function
    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
