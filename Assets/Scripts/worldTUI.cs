using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class worldTUI : MonoBehaviour
{
	//this is the ui element
	public GameObject amberUI;
    public GameObject manaUI;
    public GameObject healthUI;
    public Sprite sentinelSprite;
	
	public Camera cam;
	private RectTransform CanvasRect;

    // Start is called before the first frame update
    void Start()
    {	
    	//the obejct the script is on is a canvas
		CanvasRect = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createGenericUI(Vector3 worldPos, AmberType type, GameObject obj, Vector2 scale) {
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
        GameObject newAmber = Instantiate(obj, gameObject.transform);

        if(type == AmberType.AMBER_SENTINEL_HEAD) {
            newAmber.GetComponent<Image>().sprite = sentinelSprite;
        }

        RectTransform UI_Element = newAmber.GetComponent<RectTransform>();
        UI_Element.localScale = new Vector3(scale.x, scale.y, 1);
        
        Vector2 ViewportPosition = cam.WorldToViewportPoint(worldPos);

        float viewportY = (1.0f - ViewportPosition.y);
        float viewportX = ViewportPosition.x;
        Vector2 WorldObject_ScreenPosition = new Vector2(ViewportPosition.x*CanvasRect.sizeDelta.x, -viewportY*CanvasRect.sizeDelta.y);

        //now you can set the position of the ui element
        animationForAmberGoTo anim = newAmber.GetComponent<animationForAmberGoTo>();
        anim.SetStartPos(WorldObject_ScreenPosition, type);
    }
    

    public void createManaUI(Vector3 worldPos, Vector2 scale) { 
        createGenericUI(worldPos, AmberType.AMBER_MANA, manaUI, scale);
    }

    public void createHealthUI(Vector3 worldPos, Vector2 scale) { 
        createGenericUI(worldPos, AmberType.AMBER_HEALTH, healthUI, scale);

    }

    public void createSentinelHeadUI(Vector3 worldPos) {
        createGenericUI(worldPos, AmberType.AMBER_SENTINEL_HEAD, manaUI, new Vector2(1, 1));
    }
    public void createAmberUI(Vector3 worldPos) {
    	
    	//then you calculate the position of the UI element
    	//0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
    	GameObject newAmber = Instantiate(amberUI, gameObject.transform);

    	RectTransform UI_Element = newAmber.GetComponent<RectTransform>();
    	
    	Vector2 ViewportPosition = cam.WorldToViewportPoint(worldPos);

    	float viewportY = (1.0f - ViewportPosition.y);
    	Vector2 WorldObject_ScreenPosition = new Vector2(ViewportPosition.x*CanvasRect.sizeDelta.x, -viewportY*CanvasRect.sizeDelta.y);

    	//now you can set the position of the ui element
    	animationForAmberGoTo anim = newAmber.GetComponent<animationForAmberGoTo>();
    	anim.SetStartPos(WorldObject_ScreenPosition, AmberType.AMBER_AMBER);
    }
}
