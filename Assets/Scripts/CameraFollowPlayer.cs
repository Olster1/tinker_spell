using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject playerToFollow;
    private Transform playerTransform;
    private Transform cameraTransform;
    private bool followPlayer;
    public float xDiff;
    public float yDiff;
    private Vector2 cameraVelocity;
    public float xForce;
    public float yForce;
    public float yOffsetFromPlayer;
    private Rigidbody2D rigidBody;
    // Start is called before the first frame update

    void Start()
    {
        followPlayer = true;
        playerTransform = playerToFollow.GetComponent<Transform>();
        cameraTransform = gameObject.GetComponent<Transform>();
        Vector3 playerPos = new Vector3(playerTransform.position.x, playerTransform.position.y + yOffsetFromPlayer, cameraTransform.position.z);
        //cameraTransform.position = playerPos;
        cameraVelocity = new Vector2();

        rigidBody = gameObject.GetComponent<Rigidbody2D>();

        Camera cam = gameObject.GetComponent<Camera>();
        //this is to fix the sorting when using the perspective camera mode
        cam.transparencySortMode = TransparencySortMode.Orthographic;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (followPlayer)
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
