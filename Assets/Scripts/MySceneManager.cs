using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MySceneManager : MonoBehaviour
{

    //THIS CLASS JUST EXISTS FOR PREFABS TO FIND THINGS THEY ARE REFERENCING, SINCE WE CAN'T SAVE THEM 
    //ON THE PREFAB!!  
    //IT IS STORED ON THE MAIN CAMERA (Camera.main), and we can get it with Camera.main.GetComponent<MySceneManager>()
    //and access the objects we need.

	public SceneStateManager sceneStateManager;
	public Animator sceneAnimator;
	public Animator quoteAnimator;
	public TextWriter textWriter;
	public PlayerMovement playerMovement;
	public SpellAi spellMovement;
	public Text textUI;
    public Text areaText;
    public Image fadePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
