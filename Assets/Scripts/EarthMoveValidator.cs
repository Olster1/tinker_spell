using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthMoveValidator : MonoBehaviour
{
	private bool[] isValid;
	private bool isOn;
	public SpriteRenderer[] sps;
	private int physicsLayerMask;

    public RockIndicatorOverlap[] overlaps;
    // Start is called before the first frame update
    void Start()
    {
    	physicsLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
    	isValid = new bool[3];
    }

    bool castRayForRock(Vector2 centerPos, int rockIndex) {
    	isValid[rockIndex] = (overlaps[rockIndex].overlapping == 0);
        if(isValid[rockIndex]) {
            Vector2 rayDir = Vector2.down;
        	rayDir.Normalize();
        	float sizeOfRay = 3;
        	RaycastHit2D[] hits = Physics2D.RaycastAll(centerPos, rayDir, sizeOfRay, physicsLayerMask);
            
            Debug.DrawLine(centerPos, centerPos + sizeOfRay*rayDir, Color.green, 2, true);
        	for(int i = 0; i < hits.Length; ++i) {
        	    RaycastHit2D hitObj = hits[i];
        	    if(hitObj.collider.gameObject != gameObject && !hitObj.collider.isTrigger) {
        	        isValid[rockIndex] = true;
        	        break;
        	    }
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
        	Vector2 centerPos = new Vector2(transform.position. x, transform.position.y);
        	castRayForRock(centerPos + new Vector2(0, 0), 0);
			castRayForRock(centerPos + new Vector2(8, 0), 1);
			castRayForRock(centerPos + new Vector2(14, 0), 2);
            


            int maxIndex = 2;
            if(!isValid[2]) {
                if(!isValid[1]) {
                    maxIndex = 0;
                } else {
                    maxIndex = 1;
                }
            }
            Debug.Log("max index: " + maxIndex);
			for(int i = 0; i < isValid.Length; ++i) {
				if(i == maxIndex) {
                    if(isValid[i]) {
    					sps[i].color = Color.white;
    				} else {
    					sps[i].color = Color.red;
    				}
                } else {
                    sps[i].color = Color.clear;
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
    	
    }

    public int isOk() {
    	if(isValid[2]) {
            return 2;
        } else if(isValid[1]) {
            return 1;
        }  else if(isValid[0]) {
            return 0;
        }
        return -1;//not valid

    }
}
