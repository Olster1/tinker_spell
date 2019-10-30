using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

namespace EasyForceUpdator {

	public enum ForceType {
		FORCE_CONSTANT,
		FORCE_DECREASE,
		FORCE_INCREASE,
	}

	public class ForceToAddStruct {
	    public Timer timer;
	    public Vector2 force;
	    public ForceType type;

	    public bool finished;

	    public ForceToAddStruct(float time, Vector2 force, ForceType type = ForceType.FORCE_DECREASE) {
	        timer = new Timer(time);
	        timer.turnOn();
	        this.force = force;
	        this.type = type;
	        finished = false;

	    }

	    public void EndForce() {
	    	this.force = Vector2.zero;
	    	timer.tAt = timer.period;
	    	finished = true;
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

    	        float factor = 1.0f;
    	        if(s.type == ForceType.FORCE_DECREASE) {
    	        	factor = 1.0f - s.timer.getCanoncial();	
    	        } else if (s.type == ForceType.FORCE_INCREASE) {
    	        	factor = s.timer.getCanoncial() + 0.3f;	
    	        } else {
    	        	// Assert.IsTrue(false); //NOTE(ol): Shouldnt be here!
    	        }
    	        

    	        forceToAdd += factor*s.force;

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
