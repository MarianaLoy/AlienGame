using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Anima2D;


public class PlayerStatus : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;
    public int score = 0;
    public bool isDead = false;
    public bool isInvulnerable = true;
    public GameObject deathEffect;

    private PlayerController _playerController;
    private BoxCollider2D _playerCollider;
    private ImageEffectAllowedInSceneView scene;
    private Scene _scene;

    void Start()
    {
        _playerController = gameObject.GetComponent<PlayerController>();
        _playerCollider = gameObject.GetComponent<BoxCollider2D>();
        _scene = SceneManager.GetActiveScene();
        health = maxHealth;
        

    }

    //Positive values heals, negatives values damage
    public void AdjustHealth(int amount)
    {
        
        if(amount < 0)
        {
            //taking damage
            if (!isInvulnerable)
            {
                Debug.Log("Taking Damage");
                health = health + amount;
            }

            if(health <= 0)
            {
                if (!isDead)
                {
                    isDead = true;
                    Die();
                }
            }
            else
            {
                //TODO: Play a damage sound
                Debug.Log("Invulnerable Corroutine");
                StartCoroutine("Invulnerable");
            }
        }
        else if(amount > 0)
        {
            //healing
            health = health + amount;
            //TODO: play a healing noise
            //TODO: spawn a healing effect
        }

        if(health < 0)
            health = 0; 
        
        if(health > maxHealth)
            health = maxHealth;
        
    }


    public void AdjustScore(int amount)
    {

    }

    public void Die()
    {
        //TODO: Create effect on death
       // SkinnedMeshRenderer[] sprites = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

    }

    private IEnumerator Invulnerable()
    {
        isInvulnerable = true;
        SpriteMeshInstance[] sprites = gameObject.GetComponentsInChildren<SpriteMeshInstance>();
        for(int loop = 0; loop < 5; loop++)
        {
            foreach(SpriteMeshInstance sprite in sprites)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.25f);
            }
            yield return new WaitForSeconds(.1f);

            foreach (SpriteMeshInstance sprite in sprites)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
            }
            yield return new WaitForSeconds(.1f);
        }
        
        //yield return new WaitForSeconds(1f);
        isInvulnerable = false;
    }
}
