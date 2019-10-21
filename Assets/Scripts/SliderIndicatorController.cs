using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderIndicatorController : MonoBehaviour
{
	public SpriteRenderer[] indicators; 
	private Transform defaultParentT;
    
	void Awake() {
		defaultParentT = transform.parent.parent;
	}
    void Start()
    {

        for(int i = 0; i < indicators.Length; ++i) {
        	if(i != 0) {
        		indicators[i].enabled = false;	
        	}
        	
        }
    }

    public void UpdateIndicator(int indexTo) {
    	if(indexTo >= 0 && indexTo < indicators.Length) {
    		for(int i = 0; i < indicators.Length; ++i) {
    			if(indexTo != i) {
    				indicators[i].enabled = false;	
    			} else {
    				indicators[i].enabled = true;
    			}
    		}
    	}
    }

    public void SetParent(Transform newParent) {
    	transform.parent = newParent;
    }

    public void LoseParent() {
    	transform.parent = defaultParentT;	
    }

}
