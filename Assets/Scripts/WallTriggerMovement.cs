using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTriggerMovement : MonoBehaviour, IHitBox
{
	public Animator animator;
  public Animator whiteWallAnimator;
  public BoxCollider2D box;
  public SpriteRenderer spRender;
  public GameObject childObject;
  public AudioSource audioSrc;
  public ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        
      
       if (enemyType == EnemyType.ENEMY_GOOD) 
       {	
       		// Debug.Log("is GOOD");
       		// Debug.Log(type);
           if(type.Equals("earth")) {
           		// Debug.Log("was Earth");
           		// animator.SetTrigger("open");
              box.enabled = false;
              spRender.enabled = false;
              childObject.SetActive(true);
              audioSrc.Play();
              ps.Play();
           } else {
            whiteWallAnimator.SetTrigger("goWhite");
            Debug.Log("went White");
             // spRender.1
          }
       } 
    }
}
