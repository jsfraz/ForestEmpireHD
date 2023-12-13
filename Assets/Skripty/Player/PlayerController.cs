using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //pohyb: https://www.youtube.com/watch?v=CeXAiaQOzmY
    //animace: https://www.youtube.com/watch?v=rycsXRO6rpI

    public float speed = 5.5F;
    public Animator animator;
    private bool sprint = false;

    private Rigidbody2D rb;
    private Vector2 moveVelocity;

    private Animator ani;

    public Camera cmr;       //kamera
    public float max_zoom = 6.5F;
    public float min_zoom = 5.5F;
    private bool zooming = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Horizontal", Input.GetAxisRaw("Horizontal"));
        animator.SetFloat("Vertical", Input.GetAxisRaw("Vertical"));

        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //moveVelocity = moveInput.normalized * speed;
        moveVelocity = moveInput * speed;

        //sprint
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (sprint == false)
            {
                speed = 1.4F * speed;
                ani.speed = 1.6F;
                sprint = true;
            }
            else
            if (sprint == true)
            {
                speed = speed / 1.4F;
                ani.speed = 1F;
                sprint = false;
            }
        }

        //kontrola stlačených kláves (oprava bugování animací)
        if (Input.GetKey(KeyCode.W))
            ani.SetBool("upButton", true);
        else
            ani.SetBool("upButton", false);

        if (Input.GetKey(KeyCode.S))
            ani.SetBool("downButton", true);
        else
            ani.SetBool("downButton", false);

        if (Input.GetKey(KeyCode.A))
            ani.SetBool("leftButton", true);
        else
            ani.SetBool("leftButton", false);

        if (Input.GetKey(KeyCode.D))
            ani.SetBool("rightButton", true);
        else
            ani.SetBool("rightButton", false);

        //debug výpis
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            Debug.Log("velocity: " + moveVelocity + " upButton: " + ani.GetBool("upButton") + ", downButton: " + ani.GetBool("downButton") + ", leftButton: " + ani.GetBool("leftButton") + ", rightButton: " + ani.GetBool("rightButton"));
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);

        if (zooming == false)
        {
            //unzoom při chůzi
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                StartCoroutine(Unzoom(max_zoom));

            //zoom když stojí
            if (ani.GetBool("upButton") == false && ani.GetBool("downButton") == false && ani.GetBool("leftButton") == false && ani.GetBool("rightButton") == false)
                StartCoroutine(Zoom(min_zoom));
        }
    }

    IEnumerator Unzoom(float max)
    {
        zooming = true;
        for (float i = cmr.GetComponent<Camera>().orthographicSize; i < max; i += 0.1f)
        {
            yield return new WaitForSeconds(0.01F);
            cmr.GetComponent<Camera>().orthographicSize = i;
        }
        zooming = false;
    }

    IEnumerator Zoom(float min)
    {
        zooming = true;
        for (float i = cmr.GetComponent<Camera>().orthographicSize; i > min; i -= 0.1f)
        {
            yield return new WaitForSeconds(0.01F);
            cmr.GetComponent<Camera>().orthographicSize = i;
        }
        zooming = false;
    }
}
