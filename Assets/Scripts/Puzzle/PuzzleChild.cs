using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleChild : MonoBehaviour, IHitBox
{	
	public PuzzleParent puzzleParent;
	public int puzzleValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void wasHit(int damage, string type, EnemyType enemyType, Vector2 position) {
        if(enemyType == EnemyType.ENEMY_GOOD) {
    	   puzzleParent.addValue(puzzleValue);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
