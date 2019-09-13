using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timer_namespace;


public enum BodyToFollow {
    PLAYER_BODY,
    SPELL_BODY,
}
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

    public SpringJoint2D[] bodiesToFollow = new SpringJoint2D[2];
    private int bodyIndexFollowing;
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

       StopFollowingEntity();

       bodiesToFollow[bodyIndexFollowing].enabled = true;
        
        //this is to fix the sorting when using the perspective camera mode
        cam.transparencySortMode = TransparencySortMode.Orthographic;
        moveUpTimer = new Timer(0.4f);
        moveUpTimer.turnOff();

        moveDownTimer = new Timer(0.4f);
        moveDownTimer.turnOff();

    }

    public void FollowEntity() {
        bodiesToFollow[bodyIndexFollowing].enabled = true;
    }
        
    public void StopFollowingEntity() {
       for(int i = 0; i < bodiesToFollow.Length; ++i) {
            bodiesToFollow[i].enabled = false;
        }
    }

    public void changeEntityToFollow(BodyToFollow toFollow) {
        // playerToFollow = obj;
        // playerTransform = playerToFollow.GetComponent<Transform>();
        bodyIndexFollowing = (int)toFollow;
        bodiesToFollow[bodyIndexFollowing].enabled = true;
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
            // Vector2 forceAccel = new Vector2();
            // Vector3 newPos = new Vector3(playerTransform.position.x, playerTransform.position.y + yOffsetFromPlayer, cameraTransform.position.z);
            // Vector3 difference = newPos - cameraTransform.position;
            // if (Mathf.Abs(difference.x) > xDiff)
            // {
            //     forceAccel.x = Mathf.Sign(difference.x)*xForce;
            //     // Debug.Log("outside of x region");
            // }
            // if (Mathf.Abs(difference.y) > yDiff)
            // {
            //     forceAccel.y = Mathf.Sign(difference.y) * yForce;
            //     // Debug.Log("outside of y region");
            // }
            // float factor = Mathf.SmoothStep(0.1f, 2.0f, Mathf.Min(1.0f, difference.magnitude/10.0f));
            // rigidBody.AddForce(factor*forceAccel, ForceMode2D.Impulse);

            // Vector2 newCamPos = Time.fixedDeltaTime * Time.fixedDeltaTime * forceAccel + Time.fixedDeltaTime * cameraVelocity + new Vector2(cameraTransform.position.x, cameraTransform.position.y);
            // cameraVelocity += Time.fixedDeltaTime * forceAccel;
            // cameraTransform.position = new Vector3(newCamPos.x, newCamPos.y, cameraTransform.position.z);
            // cameraVelocity-= 0.4f * cameraVelocity;

        }
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // Gizmos.DrawCube(transform.position, new Vector3(2*xDiff, 2*yDiff, 1));
    }

}
