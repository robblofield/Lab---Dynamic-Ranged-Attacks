using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] GameObject playerMesh;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = playerMesh.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float forwardMovement = Input.GetAxisRaw("Vertical");
        if (forwardMovement > 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // Check for sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("isSprinting", true);
        }
        else
        {
            animator.SetBool("isSprinting", false);
        }

        // Check for aiming
        if (Input.GetMouseButton(0))
        {
            animator.SetBool("isAiming", true);
        }
        else
        {
            animator.SetBool("isAiming", false);
        }
        
    }
}
