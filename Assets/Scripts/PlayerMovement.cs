using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    // int wholeNumber = 3;
    // float decimalNumber = 3.45f;
    // string text = "bla";
    // bool condition = true;
    [SerializeField] float movementSpeed = 6f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;
    void Start()
    {
        // Debug.Log("Hello from Start");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        rb.velocity = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);

        // Debug.Log("Hello from Start");
        if(Input.GetButtonDown("Jump") && IsGrounded()){
            Jump();
        }
        // if(Input.GetKeyDown("space")){
            // rd.velocity = new Vector3(0,5f,0);
            // GetComponent<Rigidbody>().velocity = new Vector3(0,5,0);
        // }
        // if(Input.GetKeyDown("up")){
        //     rb.velocity = new Vector3(0,0,5f);
        // }
        // if(Input.GetKeyDown("right")){
        //     rb.velocity = new Vector3(5f,0,0);
        // }
        // if(Input.GetKeyDown("down")){
        //     rb.velocity = new Vector3(0,0,-5f);
        // }
        // if(Input.GetKeyDown("left")){
        //     rb.velocity = new Vector3(-5f,0,0);
        // }
        
    }
    void Jump(){
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }
    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("Enemy Head")){
            Destroy(collision.transform.parent.gameObject);
            Jump();
        }
    }
    bool IsGrounded(){
        return Physics.CheckSphere(groundCheck.position, .1f, ground);
    }
}
