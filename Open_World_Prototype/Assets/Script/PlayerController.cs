using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // variables du mouvement
    public float run = 8.0f;
    public float walk = 2.0f;
    public float jump = 6.0f;
    private Vector3 direction;
    private Rigidbody rb;
    private bool isRunning;
    private bool isJumping;
    private bool isFalling;

    //variables de la camera
    public float mouseSensitivity = 1f;
    private float mouseX;
    private float mouseY;
    private float rotAmoutX;
    private float rotAmoutY;
    public Transform monPivot;
    public Transform raycast;
    public GameObject monJoueur;

    //variable du jump
    private bool grounded;
    private bool preGrounded;
    public float offSetGroundCheck;
    public float offSetPreGroundCheck;
    private CapsuleCollider monCollider;
    private bool raycastMyFeet;
    private bool raycastUnderMyFeet;
    private Collider[] trucSousMesPieds;
    private Collider[] trucVaEtreSousMesPieds;

    //variable d'attaque
    private bool isAttacking = false;
    public bool axeInHand = false;
    public bool pickaxeInHand = false;

    //Variable d'animation
    public Animator animPlayer;
    private Vector3 relative;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        monCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        mouvementCheck();
        cameraCheck();
        animCheck();
        if (!grounded)
        {
            isJumping = false;
        }
    }

    void mouvementCheck()
    {
        //On gère l'attaque
        if (!animPlayer.GetCurrentAnimatorStateInfo(0).IsName("isAttacking"))
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                isAttacking = true;
                RaycastHit hit;
                if (Physics.Linecast(monPivot.position, raycast.position, out hit))
                {
                    if (hit.transform.gameObject.GetComponent<Environment>() == null)
                    {
                        StartCoroutine(hit.transform.parent.transform.gameObject.GetComponent<Environment>().Attacked(axeInHand, pickaxeInHand));
                    }
                    else
                    {
                        StartCoroutine(hit.transform.gameObject.GetComponent<Environment>().Attacked(axeInHand, pickaxeInHand));
                    }
                }
            }
        }
        animPlayer.SetBool("isAttacking", isAttacking);
        isAttacking = false;

        //On gère le mouvement
        RaycastHit ground;
        grounded = Physics.Raycast(transform.position, -transform.up, out ground,.1f);

        if (Input.GetKey(KeyCode.LeftShift) && grounded)
        {

            direction.x = Input.GetAxis("Horizontal") * run;
            direction.z = Input.GetAxis("Vertical") * run;
            isRunning = true;

        }
        else
        {

            direction.x = Input.GetAxis("Horizontal") * walk;
            direction.z = Input.GetAxis("Vertical") * walk;
            isRunning = false;
        }

        direction = transform.TransformDirection(direction);


        RaycastHit preGround;
        preGrounded = Physics.Raycast(transform.position, -transform.up, out preGround, 1);

        // On Gère le saut
        if (Input.GetButtonDown("Jump") && grounded)
        {
            isJumping = true;
            rb.velocity = new Vector3(rb.velocity.x, jump, rb.velocity.z);
        }

        // Enfin on fait bouger le player en fonction des calcul effectués juste avant
        rb.velocity = new Vector3(direction.x, rb.velocity.y, direction.z);

        if (animPlayer.GetCurrentAnimatorStateInfo(0).IsName("FallingToLanding"))
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }

    void cameraCheck()
    {
        if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

        mouseX = Input.GetAxis("Mouse X");
        mouseY = -Input.GetAxis("Mouse Y");

        rotAmoutX = rotAmoutX + (mouseY * mouseSensitivity);
        rotAmoutY = rotAmoutY + (mouseX * mouseSensitivity);
        rotAmoutX = Mathf.Clamp(rotAmoutX, -89f, 89f);


        monPivot.localRotation = Quaternion.Euler(rotAmoutX, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, rotAmoutY, 0f);
    }

    void animCheck()
    {
        relative = transform.InverseTransformDirection(rb.velocity);
        animPlayer.SetFloat("velocityX", relative.x);
        animPlayer.SetFloat("velocityY", relative.y);
        animPlayer.SetFloat("velocityZ", relative.z);
        animPlayer.SetBool("grounded", grounded);
        animPlayer.SetBool("preGrounded", preGrounded);
        animPlayer.SetBool("isRunning", isRunning);
        animPlayer.SetBool("isJumping", isJumping);
        animPlayer.SetBool("isFalling", isFalling);
        animPlayer.SetBool("axeInHand", axeInHand);
        animPlayer.SetBool("pickaxeInHand", pickaxeInHand);
    }
}
