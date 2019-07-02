using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Timer_namespace;

public class SoundMixer : MonoBehaviour
{
	public enum MusicId {
		NULL, 
		PORTAL_ROOM, 
		JJ_EXPLORE, 
		JJ_BOSS, 
		JJ_NATURE, 
		MUSIC_COUNT
	}

	public AudioClip[] clips;

	[HideInInspector] MusicId currentId;

	public AudioSource[] sources;

	public class SourceSettings {
		public Timer timer;
		public bool fadeOut;
		public int sourceId;
		public SourceSettings() {
			timer = new Timer(0.4f);
			timer.turnOff();
			fadeOut = false;
		}

		public void setFadeOut(float period, int id) {
			timer.period = period;
			timer.turnOn();
			fadeOut = true;
			sourceId = id;
		}

		public void setFadeIn(float period, int id) {
			timer.period = period;
			timer.turnOn();
			fadeOut = false;
			sourceId = id;
		}
	}
	
	private SourceSettings[] settings;
	private int activeSource;
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue((int)MusicId.MUSIC_COUNT == clips.Length);
        Assert.IsTrue(sources.Length == 2);

        settings = new SourceSettings[sources.Length];
        settings[0] = new SourceSettings();
        settings[1] = new SourceSettings();
        currentId =  MusicId.NULL;
        SetSound(MusicId.PORTAL_ROOM);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < settings.Length; ++i) {
        	Timer t = settings[i].timer;
        	if(t.isOn()) {
        		bool b = t.updateTimer(Time.deltaTime);
        		float f = t.getCanoncial();
        		if(settings[i].fadeOut) {
        			f = 1.0f - f;
        		}
        		sources[settings[i].sourceId].volume = Mathf.Lerp(0, 0.5f, f);
        		if(b) {
        			if(settings[i].fadeOut) {
        				sources[settings[i].sourceId].Stop();
        			}
        			t.turnOff();
        		}
        	}
        }
    }

    public void SetSound(MusicId toSetId) {
    	if(toSetId != currentId) {
    		if(currentId != MusicId.NULL) {
    			settings[activeSource].setFadeOut(0.5f, activeSource);
    		}
    		//wrap the index
    		activeSource++;
    		if(activeSource > 1) {
    			activeSource = 0;
    		}
    		if(toSetId != MusicId.NULL) {
	    		//set the right sound
	    		sources[activeSource].clip = clips[(int)toSetId];
	    		sources[activeSource].volume = 0.0f;
	    		sources[activeSource].Play();
	    		settings[activeSource].setFadeIn(0.5f, activeSource);
	    	}
    		currentId = toSetId;
    	}
    }





}
