using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyAttackObjectCreator {
	public static class AttackObjectCreator
	{
	    public static GenericAttackObject initAttackObject(GameObject obj, Vector2 start, Vector2 end, 
	    	EnemyType enemyType, float period, float minDamage, float maxDamage) {

	    	GenericAttackObject attackObj = obj.GetComponent<GenericAttackObject>();
	    	attackObj.startPos = start;
	    	attackObj.endPos = end;
	    	attackObj.period = period;
	    	attackObj.minDamage = minDamage;
	    	attackObj.maxDamage = maxDamage;
	    	attackObj.type = enemyType;
	    	return attackObj;	
	    }
	}
}