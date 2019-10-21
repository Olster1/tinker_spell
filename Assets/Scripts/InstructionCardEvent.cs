using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionCardEvent : MonoBehaviour, IEvent
{

	private InstructionCardManager manager;
	public InstructionCardType cardType;
	private BlurPostProcess blurPostProcess;

	private bool hasRun;


	private 
    // Start is called before the first frame update
    void Start()
    {
    	hasRun = false;
        manager = Camera.main.GetComponent<InstructionCardManager>();
        blurPostProcess = Camera.main.GetComponent<BlurPostProcess>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }	

    public void Run() {
    	manager.SetInstructionCardToLookAt(cardType);
    	blurPostProcess.StartBlur(manager, 1);
    	hasRun = true;
    }

     private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !hasRun) {
        	Run();
        }
    }


}
