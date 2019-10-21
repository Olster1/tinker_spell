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
    public float moveSpeed;
    public Transform spell;
    public Transform tinker;
    private Transform playerTransform;
    private Transform cameraTransform;
    [HideInInspector] public bool followPlayer;
    private Rigidbody2D rigidBody;
    public Camera cam;

    [HideInInspector] public bool yStuck;

    public Animator animator;
    [HideInInspector] public LevelStateId levelToLoad;
    public SceneStateManager manager;

    void Start()
    {
        followPlayer = true;
        cameraTransform = gameObject.GetComponent<Transform>();

        rigidBody = gameObject.GetComponent<Rigidbody2D>();

       StopFollowingEntity();

        //this is to fix the sorting when using the perspective camera mode
        cam.transparencySortMode = TransparencySortMode.Orthographic;

    }

    public void FollowEntity() {
        followPlayer = true;
    }
        
    public void StopFollowingEntity() {
       followPlayer = false;
    }

    public void changeEntityToFollow(BodyToFollow toFollow) {
        if(toFollow == BodyToFollow.PLAYER_BODY) {
            playerTransform = tinker;
        } else if (toFollow == BodyToFollow.SPELL_BODY) {
            playerTransform = spell;
        }

        followPlayer = true;
    }

    // Update is called once per frame
    void LateUpdate() {
        float threshold = 0.5f;
        float lowerThreshold = 0.25f;

        // float xAxisRight = Input.GetAxis("RightVertical");
        float yAxisRight = Input.GetAxis("RightHorizontal");


        //Prevent Drift!!
        // if(Mathf.Abs(xAxisRight) < threshold) {
        //     xAxisRight = 0;
        // }

        if(manager.IsInGame()) {
            if(Mathf.Abs(yAxisRight) < threshold) {
                yAxisRight = 0;
            }

            if(yAxisRight > threshold) {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 20, Time.deltaTime);
            } else if(yAxisRight < -threshold) {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 4, Time.deltaTime);
            }
        } 
        


        if (followPlayer) {
            Vector3 newPos = new Vector3(playerTransform.position.x, playerTransform.position.y, cameraTransform.position.z);
            float x = Mathf.SmoothStep(cameraTransform.position.x, newPos.x, moveSpeed);
            float y = cameraTransform.position.y;
            if(!yStuck) {
                y = Mathf.SmoothStep(cameraTransform.position.y, newPos.y, moveSpeed);
            } 
            rigidBody.MovePosition(new Vector2(x, y));

        }
    }

    public void SetYStuck(bool val) {
        yStuck = val;
    }

    private void FixedUpdate()
    {
        
    }

    void OnDrawGizmos() {
        // Gizmos.color = new Color(1, 0, 0, 0.5f);

        // Gizmos.DrawCube(transform.position, new Vector3(2*xDiff, 2*yDiff, 1));
    }

}
