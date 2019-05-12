using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldTUI : MonoBehaviour
{
	//this is the ui element
	public GameObject amberUI;
	
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

    public void createAmberUI(Vector3 worldPos) {
    	
    	//then you calculate the position of the UI element
    	//0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
    	GameObject newAmber = Instantiate(amberUI, gameObject.transform);

    	RectTransform UI_Element = newAmber.GetComponent<RectTransform>();

    	
    	Vector2 ViewportPosition = cam.WorldToViewportPoint(worldPos);

    	float viewportY = (1.0f - ViewportPosition.y);
    	Vector2 WorldObject_ScreenPosition = new Vector2(ViewportPosition.x*CanvasRect.sizeDelta.x, -viewportY*CanvasRect.sizeDelta.y);

    	Debug.Log("ciewpost pos: " + CanvasRect.sizeDelta.y);
    	
    	//now you can set the position of the ui element
    	animationForAmberGoTo anim = newAmber.GetComponent<animationForAmberGoTo>();
    	anim.SetStartPos(WorldObject_ScreenPosition);
    }
}
