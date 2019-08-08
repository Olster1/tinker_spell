using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthMoveValidator : MonoBehaviour
{
	private bool[] isValid;
	private bool isOn;
	public SpriteRenderers[] sps;
	private int physicsLayerMask;
    // Start is called before the first frame update
    void Start()
    {
    	physicsLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
    	isValid = new bool[3];
    }

    bool castRayForRock(Vector2 centerPos, int rockIndex) {
    	Vector2 rayDir = Vector2.down;
    	rayDir.Normalize();
    	float sizeOfRay = 1;
    	RaycastHit2D[] hits = Physics2D.RaycastAll(centerPos, rayDir, sizeOfRay, physicsLayerMask);
    	for(int i = 0; i < hits.Length; ++i) {
    	    RaycastHit2D hitObj = hits[i];
    	    if(hitObj.collider.gameObject != gameObject && !hitObj.collider.isTrigger) {
    	        isValid[rockIndex] = true;
    	        break;
    	    }
    	}
    	return isValid[rockIndex];
    }

    // Update is called once per frame
    void Update()
    {
    	isValid[0] = false;
    	isValid[1] = false;
    	isValid[2] = false;

        if(isOn) {
        	Vector2 centerPos = new Vector2(transform.position);
        	castRayForRock(centerPos, 0));
			castRayForRock(centerPos + new Vector2(1, 0), 1));
			castRayForRock(centerPos + new Vector2(2, 0), 2));

			for(int i = 0; i < isValid.Length; ++i) {
				if(isValid[i]) {
					sps[i].color = Color.white;
				} else {
					sps[i].color = Color.red;
				}
			}
        }
    }

    public void turnOn() {
    	isOn = true;
    }

    public void turnOff() {
    	sps[0].color = Color.clear;
    	sps[1].color = Color.clear;
    	sps[2].color = Color.clear;

    	isOn = false;	
    	gameObject.isActive(false);
    }

    public bool isOk() {
    	return isValid[0];
    }
}
