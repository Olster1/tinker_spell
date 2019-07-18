using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject playerToFollow;
    private Transform playerTransform;
    private Transform cameraTransform;
    [HideInInspector] public bool followPlayer;
    public float xDiff;
    public float yDiff;
    // private Vector2 cameraVelocity;
    public float xForce;
    public float yForce;
    public float yOffsetFromPlayer;
    private Rigidbody2D rigidBody;
    [HideInInspector] public Timer moveUpTimer;
    [HideInInspector] public Timer moveDownTimer;
    [HideInInspector] public Vector3 startMovePos;
    public Camera cam;
    // Start is called before the first frame update

    public Animator animator;
    [HideInInspector] public LevelStateId levelToLoad;
    public SceneStateManager manager;

    void Start()
    {
        followPlayer = true;
        playerTransform = playerToFollow.GetComponent<Transform>();
        cameraTransform = gameObject.GetComponent<Transform>();
        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y + yOffsetFromPlayer, cameraTransform.position.z);
        //cameraTransform.position = playerPos;
        // cameraVelocity = new Vector2();

        rigidBody = gameObject.GetComponent<Rigidbody2D>();

        
        //this is to fix the sorting when using the perspective camera mode
        cam.transparencySortMode = TransparencySortMode.Orthographic;
        moveUpTimer = new Timer(0.4f);
        moveUpTimer.turnOff();

        moveDownTimer = new Timer(0.4f);
        moveDownTimer.turnOff();

    }

    public void changeEntityToFollow(GameObject obj) {
        playerToFollow = obj;
        playerTransform = playerToFollow.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        if(moveUpTimer.isOn()) {
            followPlayer = false;
            bool isFin = moveUpTimer.updateTimer(Time.deltaTime);
            transform.position = Vector3.Lerp(startMovePos, startMovePos + 3.0f*Vector3.up, moveUpTimer.getCanoncial());
            if(isFin) {
                moveUpTimer.turnOff();
                manager.stateToLoad = levelToLoad;
                animator.SetTrigger("FadeIn");

            }
        }

        if(moveDownTimer.isOn()) {
            followPlayer = false;
            bool isFin = moveDownTimer.updateTimer(Time.deltaTime);
            transform.position = Vector3.Lerp(startMovePos + 3.0f*Vector3.up, startMovePos, moveDownTimer.getCanoncial());
            
            if(isFin) {
                followPlayer = true;
                moveDownTimer.turnOff();
            }
        }
    }

    private void FixedUpdate()
    {
        if (followPlayer && !(moveUpTimer.isOn() || moveDownTimer.isOn()))
        {
            Vector2 forceAccel = new Vector2();
            Vector3 newPos = new Vector3(playerTransform.position.x, playerTransform.position.y + yOffsetFromPlayer, cameraTransform.position.z);
            Vector3 difference = newPos - cameraTransform.position;
            if (Mathf.Abs(difference.x) > xDiff)
            {
                forceAccel.x = Mathf.Sign(difference.x)*xForce;
            }
            if (Mathf.Abs(difference.y) > yDiff)
            {
                forceAccel.y = Mathf.Sign(difference.y) * yForce;
            }

            rigidBody.AddForce(forceAccel);

            // Vector2 newCamPos = Time.fixedDeltaTime * Time.fixedDeltaTime * forceAccel + Time.fixedDeltaTime * cameraVelocity + new Vector2(cameraTransform.position.x, cameraTransform.position.y);
            // cameraVelocity += Time.fixedDeltaTime * forceAccel;
            // cameraTransform.position = new Vector3(newCamPos.x, newCamPos.y, cameraTransform.position.z);
            // cameraVelocity-= 0.4f * cameraVelocity;

        }
    }


}
