using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTriggerMovement : MonoBehaviour, IHitBox
{
	public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        Debug.Log("Was Hit");
       if (enemyType == EnemyType.ENEMY_GOOD) 
       {	
       		Debug.Log("is GOOD");
       		Debug.Log(type);
           if(type.Equals("earth")) {
           		Debug.Log("was Earth");
           		animator.SetTrigger("open");
           }
       }
    }
}
