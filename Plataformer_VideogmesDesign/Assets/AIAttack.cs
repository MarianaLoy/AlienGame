using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttack : MonoBehaviour
{
    public int damageOnCollision = -10;


    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision with player"); 
            PlayerStatus playerStatus = collision.gameObject.GetComponent<PlayerStatus>();
            playerStatus.AdjustHealth(damageOnCollision);

        }
    }
}
