using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using UnityEngine.Assertions;

public class EarthMoveValidator : MonoBehaviour
{
	private bool[] isValid;
	public SpriteRenderer[] sps;
	private int physicsLayerMask;
    private SpriteRenderer spPlayer;

    public RockIndicatorOverlap[] overlaps;
    public BoxCollider2D[] colliders;

    private Timer flashTimer;

    // Start is called before the first frame update
    void Start()
    {
    	physicsLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
    	isValid = new bool[3];
        spPlayer = transform.parent.GetComponent<SpriteRenderer>();
        flashTimer = new Timer(1.0f);
    }

    public void ResetColliders() {
        for(int i = 0; i < overlaps.Length; ++i) {
            overlaps[i].overlapping = 0;
        }
    }

    bool castRayForRock(Vector2 centerPos, int rockIndex) {
        bool result = false;
    	bool overlap = (overlaps[rockIndex].overlapping != 0);
        // Debug.Log("overlap " + rockIndex + " " + overlaps[rockIndex].overlapping);  
        if(!overlap) {
            Vector2 rayDir = Vector2.down;
        	rayDir.Normalize();
        	float sizeOfRay = 3;
        	RaycastHit2D[] hits = Physics2D.RaycastAll(centerPos, rayDir, sizeOfRay, physicsLayerMask);
            
            Debug.DrawLine(centerPos, centerPos + sizeOfRay*rayDir, Color.green, 2, true);
        	for(int i = 0; i < hits.Length; ++i) {
        	    RaycastHit2D hitObj = hits[i];
                // Debug.Log(hitObj.collider.gameObject.tag);
        	    if(hitObj.collider.gameObject != gameObject && !hitObj.collider.isTrigger && (hitObj.collider.gameObject.tag == "WorldGeometryEarth" || hitObj.collider.gameObject.tag == "PlatformGeometryEarth")) {
                    result = true;
        	        break;
        	    }
        	}
        }

        if(result) {
            result = !overlap;
        }  

    	return result;
    }

    // Update is called once per frame
    void Update()
    {
    	isValid[0] = false;
    	isValid[1] = false;
    	isValid[2] = false;

        //if(isOn) 
        {
            Vector3 pos = transform.localPosition;
            if(spPlayer.flipX) {
                if(pos.x > 0) pos.x *= -1;
                transform.localPosition = pos;
                for(int i = 0; i < sps.Length; ++i) {
                    sps[i].flipX = true;
                }
                 for(int i = 0; i < colliders.Length; ++i) {
                    Vector2 v = colliders[i].offset;
                    if(v.x > 0) {
                        v.x *= -1;
                    }
                    colliders[i].offset = v;
                }
            } else {
                if(pos.x < 0) pos.x *= -1;
                transform.localPosition = pos;
                for(int i = 0; i < sps.Length; ++i) {
                    sps[i].flipX = false;
                }
                for(int i = 0; i < colliders.Length; ++i) {
                    Vector2 v = colliders[i].offset;
                    if(v.x < 0) {
                        v.x *= -1;
                    }
                    colliders[i].offset = v;
                }
            }

            float modifier = 1;
            if(spPlayer.flipX) {
                modifier = -1;
            }

        	Vector2 centerPos = new Vector2(transform.position.x, transform.position.y);

        	bool r1 = castRayForRock(centerPos + new Vector2(0, 0), 0);
            bool r2 = castRayForRock(centerPos + new Vector2(4, 0), 0);
			bool r3 = castRayForRock(centerPos + new Vector2(modifier*6, 0), 1);
			bool r4 = castRayForRock(centerPos + new Vector2(modifier*8, 0), 1);
            bool r5 = castRayForRock(centerPos + new Vector2(modifier*12, 0), 2);
            bool r6 = castRayForRock(centerPos + new Vector2(modifier*14, 0), 2);
            
            //int maxIndex = -1;
            if(r1 && r2) {
               // maxIndex = 0;
                isValid[0] = true;
                if(r3 && r4) {
                   // maxIndex = 1;
                    isValid[1] = true;
                    if(r5 && r6) {
                       // maxIndex = 2;
                        isValid[2] = true;
                    }
                }
            }


            if(flashTimer.isOn()) {
                bool flashDone = flashTimer.updateTimer(Time.deltaTime);
                
                float alpha = (float)0.5f*Mathf.Cos(4*Mathf.PI*flashTimer.getCanoncial()) + 0.5f;
                //Debug.Log(alpha);
                Assert.IsTrue(alpha >= 0.0f && alpha <= 1.0f);
                Color tempColor = Color.red;
                tempColor.a = alpha;
                sps[0].color  = tempColor;
                if(flashDone) {
                    flashTimer.turnOff();
                    
                }
            } else {
                sps[0].color = Color.clear;
            }
        }
    }

    public void turnOn() {
    	flashTimer.turnOn();

    }

    public void turnOff() {
    	sps[0].color = Color.clear;
    	sps[1].color = Color.clear;
    	sps[2].color = Color.clear;
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
