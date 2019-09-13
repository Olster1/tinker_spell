using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

namespace EasyForceUpdator {

	public class ForceToAddStruct {
	    public Timer timer;
	    public Vector2 force;

	    public ForceToAddStruct(float time, Vector2 force) {
	        timer = new Timer(time);
	        timer.turnOn();
	        this.force = force;
	    }

	}

	public class ForceUpdator
	{
		private List<ForceToAddStruct> forcesToAdd;
	    public ForceUpdator() {
	    	forcesToAdd = new List<ForceToAddStruct>();
	    }

	    public void ClearForces() {
	    	for(int i = 0; i < forcesToAdd.Count;) {
    	        forcesToAdd.RemoveAt(i);
	    	}
	    }

	    public Vector2 update() {
	    	Vector2 forceToAdd = new Vector2(0, 0);
	    	for(int i = 0; i < forcesToAdd.Count;) {
	    		bool increment = true;
	    	    ForceToAddStruct s = forcesToAdd[i];
	    	    Assert.IsTrue(s != null);
    	        bool fin = s.timer.updateTimer(Time.fixedDeltaTime);
    	        float canVal = 1.0f - s.timer.getCanoncial();

    	        forceToAdd += canVal*s.force;

    	        if(fin) {
    	            forcesToAdd.RemoveAt(i);
    	            increment = false;
    	        }
    	        if(increment) i++;
	    	}
	    	return forceToAdd;
	    }

	    public void AddForce(ForceToAddStruct force) {
	    	forcesToAdd.Add(force);
	    }
	}
}
