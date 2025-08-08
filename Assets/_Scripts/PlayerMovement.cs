using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController _controller;
    [SerializeField] private float speed = 12f;
    private Vector3 velocity;
    public float gravity = -9.81f;
    [Header("Animation")]
    [SerializeField] private Animator animator;



    public override void FixedUpdateNetwork()
    {
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("Cursor unlocked");
        }
        if (!Object.HasStateAuthority)
            return;

        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        // Hướng di chuyển theo camera
        Vector3 move = Camera.main.transform.right * x + Camera.main.transform.forward * z;
        move.y = 0f; // Không cho nhân vật bay lên

        float movementSpeed = move.magnitude;

        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(move),
                Time.deltaTime * 10f
            );
        }

        _controller.Move(move.normalized * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        _controller.Move(velocity * Time.deltaTime);

        // Update Animator
        if (animator != null)
        {
            animator.SetFloat("chay", movementSpeed);
        }

        //nhấn cách để nhảy
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_controller.transform.position.y <= 20f) 
            {
                velocity.y = Mathf.Sqrt(-3f * gravity * 3f);
            }

        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Bấm chuột phải - đánh");
            animator.SetTrigger("danh");
            //xoay nhân vật theo hướng camera
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0; 
            cameraForward.Normalize();
            transform.rotation = Quaternion.LookRotation(cameraForward);

        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("nhatdo");
        }



    }
}
