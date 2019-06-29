using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;
using EasyGameManager;

public class animationForAmberGoTo : MonoBehaviour
{
	public Vector2 finalPos;
	private Vector2 startPos;
	private RectTransform UI_Element;
	private Timer timer;
    public AmberType type;

    // Start is called before the first frame update
    void Start()
    {
        
        timer = new Timer(0.3f);
        timer.turnOn();
    }

    public void SetStartPos(Vector2 pos, AmberType type) {
    	UI_Element = gameObject.GetComponent<RectTransform>();
    	UI_Element.anchoredPosition = pos;
    	startPos = pos;
        this.type = type;

    }

    // Update is called once per frame
    void Update()
    {
    	if(timer.isOn()) {
    		bool fin = timer.updateTimer(Time.deltaTime);
    		float canVal = timer.getCanoncial();
    		UI_Element.anchoredPosition = Vector2.Lerp(startPos, finalPos, canVal);
    		if(fin) {
                switch(type) {
                    case AmberType.AMBER_AMBER: {
                        GameManager.amberCount++;
                    } break;
                    case AmberType.AMBER_MANA: {
                        GameManager.manaCount += 2;
                    } break;
                    case AmberType.AMBER_HEALTH: {
                        GameManager.playerHealth += 2;
                        if(GameManager.playerHealth > 100) {
                            GameManager.playerHealth = 100;
                        }
                    } break;
                    case AmberType.AMBER_SENTINEL_HEAD: {
                        
                    } break;
                    default: {

                    } break;
                }

    			
    			Destroy(gameObject);
    		}	
    	}
        
    }
}
