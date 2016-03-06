/*
Fuse - Fuse is an open source action game created in Unity3D

Copyright (C) 2016  Martin Slanina

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using UnityEngine;
using System.Collections;
using CnControls;

public class PlayerController : MonoBehaviour {


    Vector3 point;
    public Camera PlayerCameraComponent;
    public float horizontalForce = 4;
    public float verticalForce = 4;
    public bool isTyping;
    float newZCoordinate;
    RaycastHit hit;
    Rigidbody rb;
    Animator anim;

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    private float nextFootstepRun;
    private float nextFootstepWalk;
    private float nextSlide;

    private Vector3 move;
    private Vector3 moveInput;

    private float turnAmount;
    private float forwardAmount;

    float height;
    Vector3 center;
    private bool changed;
    private Vector3 latest;
    float stamina = 100;

    bool run;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(transform.position.x, 1.8F, transform.position.z);
        anim = GetComponent<Animator>();
        height = GetComponent<CharacterController>().height;
        center = GetComponent<CharacterController>().center;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (Physics.Raycast(bottom.position, -bottom.up, out hit))
        //{
        //    transform.position = new Vector3(transform.position.x, hit.point.y + 1.8F, transform.position.z);
        //    Debug.DrawRay(bottom.position, -bottom.up);
        //}

#if UNITY_STANDALONE_WIN
        Plane plane = new Plane(Vector3.up, 0);
        RaycastHit dist;
        Ray ray = PlayerCameraComponent.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out dist, Mathf.Infinity, 1 << 10))
        {
            point = dist.point;
            point.y = transform.position.y;
            transform.LookAt(point);
            Debug.DrawLine(ray.origin, point);
        }
#endif

#if UNITY_ANDROID
        if (CnInputManager.GetAxis("Horizontal2")!=0&& CnInputManager.GetAxis("Vertical2")!=0)
        {
            float angle = Mathf.Atan2(CnInputManager.GetAxis("Horizontal2"), CnInputManager.GetAxis("Vertical2")) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, angle, 0);
            latest = new Vector3(0, angle, 0);
        }
        else
        {
            transform.eulerAngles = latest;
        } 
#endif

        if (!isTyping)
        {
            CharacterController controller = GetComponent<CharacterController>();
            if (controller.isGrounded)
            {
                moveDirection = new Vector3(CnInputManager.GetAxis("Horizontal"), 0, CnInputManager.GetAxis("Vertical"));
                moveDirection *= speed;
                if (CnInputManager.GetButtonDown("Jump"))
                    moveDirection.y = jumpSpeed;

            }
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
            //anim.SetFloat("Speed", Mathf.Abs(controller.velocity.x) + Mathf.Abs(controller.velocity.z));
        }
        if (stamina<=0)
        {
            anim.SetBool("Run", false);
            run = false;
            speed = 4;
        }
        if (run && (Mathf.Abs(CnInputManager.GetAxis("Horizontal")) > 0.1F || Mathf.Abs(CnInputManager.GetAxis("Vertical")) > 0.1F) && stamina > 0)
        {
            stamina = stamina - (30 * Time.deltaTime);
            Debug.Log(stamina);
        }
        else if (run && (Mathf.Abs(CnInputManager.GetAxis("Horizontal")) < 0.1F || Mathf.Abs(CnInputManager.GetAxis("Vertical")) < 0.1F) && stamina < 100)
        {
            stamina = stamina + (30 * Time.deltaTime);
            Debug.Log(stamina);
        }
        else if (!run && (Mathf.Abs(CnInputManager.GetAxis("Horizontal")) < 0.1F || Mathf.Abs(CnInputManager.GetAxis("Vertical")) < 0.1F) && stamina < 100)
        {
            stamina = stamina + (30 * Time.deltaTime);
            Debug.Log(stamina);
        }
        else if (!run && (Mathf.Abs(CnInputManager.GetAxis("Horizontal")) > 0.1F || Mathf.Abs(CnInputManager.GetAxis("Vertical")) > 0.1F) && stamina < 100)
        {
            stamina = stamina + (20 * Time.deltaTime);
            Debug.Log(stamina);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (run)
            {
                anim.SetBool("Run", false);
                run = false;
                speed = 4;
            }
            else if (stamina>0)
            {
                anim.SetBool("Run", true);
                run = true;
                speed = 7;
            }

        }

        if (Input.GetButtonUp("Crouch"))
        {
            anim.SetBool("Slide", false);
        }

        if (Input.GetButtonDown("Crouch") && (Mathf.Abs(CnInputManager.GetAxis("Horizontal")) > 0.1F || Mathf.Abs(CnInputManager.GetAxis("Vertical")) > 0.1F))
        {
            anim.SetBool("Slide", true);
        }
        if (Input.GetButtonUp("Crouch"))
        {
            anim.SetBool("Slide", false);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("RunSlide_ToRight_2"))
        {
            if (!changed)
            {
                //GameObject.Find(name).GetComponent<CharacterController>().height = 0.2F;
                //GameObject.Find(name).GetComponent<CharacterController>().center = new Vector3(0, 0.6F, 0);
                GetComponent<PlayerNetworkSetup>().CapsuleChange(transform.name, 0,height,center);
                changed = true;
            } 
        }
        else
        {
            if (changed)
            {
                //GetComponent<CharacterController>().height = height;
                //GetComponent<CharacterController>().center = center;
                GetComponent<PlayerNetworkSetup>().CapsuleChange(transform.name, 1,height,center);
                changed = false; 
            }
        }

        //if ((CnInputManager.GetAxis("Vertical") > 0.1F && (transform.eulerAngles.y > 0 && transform.eulerAngles.y < 45 || transform.eulerAngles.y > 315 && transform.eulerAngles.y < 360)) || (CnInputManager.GetAxis("Vertical") < -0.1F && transform.eulerAngles.y > 135 && transform.eulerAngles.y < 225) || (CnInputManager.GetAxis("Horizontal") > 0.1F && transform.eulerAngles.y > 45 && transform.eulerAngles.y < 135) || (CnInputManager.GetAxis("Horizontal") < -0.1F && transform.eulerAngles.y > 225 && transform.eulerAngles.y < 315))
        //{
        //    anim.SetBool("Run", true);
        //}
        //else
        //{
        //    anim.SetBool("Run", false);
        //}
        //if ((CnInputManager.GetAxis("Vertical") < -0.1 && (transform.eulerAngles.y > 0 && transform.eulerAngles.y < 45 || transform.eulerAngles.y > 315 && transform.eulerAngles.y < 360)) || (CnInputManager.GetAxis("Vertical") > 0.1F && transform.eulerAngles.y > 135 && transform.eulerAngles.y < 225) || (CnInputManager.GetAxis("Horizontal") < -0.1F && transform.eulerAngles.y > 45 && transform.eulerAngles.y < 135) || (CnInputManager.GetAxis("Horizontal") > 0.1F && transform.eulerAngles.y > 225 && transform.eulerAngles.y < 315))
        //{
        //    anim.SetBool("Back", true);
        //}
        //else
        //{
        //    anim.SetBool("Back", false);
        //}
        //if ((CnInputManager.GetAxis("Horizontal") > 0.1F && (transform.eulerAngles.y > 0 && transform.eulerAngles.y < 45 || transform.eulerAngles.y > 315 && transform.eulerAngles.y < 360)) || (CnInputManager.GetAxis("Vertical") > 0.1F && transform.eulerAngles.y > 225 && transform.eulerAngles.y < 315) || (CnInputManager.GetAxis("Horizontal") < -0.1F && transform.eulerAngles.y > 135 && transform.eulerAngles.y < 225) || (CnInputManager.GetAxis("Vertical") < -0.1F && transform.eulerAngles.y > 45 && transform.eulerAngles.y < 135))
        //{
        //    anim.SetBool("Right", true);
        //}
        //else
        //{
        //    anim.SetBool("Right", false);
        //}
        //if ((CnInputManager.GetAxis("Horizontal") < -0.1F && (transform.eulerAngles.y > 0 && transform.eulerAngles.y < 45 || transform.eulerAngles.y > 315 && transform.eulerAngles.y < 360)) || (CnInputManager.GetAxis("Vertical") < -0.1F && transform.eulerAngles.y > 225 && transform.eulerAngles.y < 315) || (CnInputManager.GetAxis("Horizontal") > 0.1F && transform.eulerAngles.y > 135 && transform.eulerAngles.y < 225) || (CnInputManager.GetAxis("Vertical") > 0.1F && transform.eulerAngles.y > 45 && transform.eulerAngles.y < 135))
        //{
        //    anim.SetBool("Left", true);
        //}
        //else
        //{
        //    anim.SetBool("Left", false);
        //}
        //if (run && forwardAmount > 0.2F && turnAmount < 0.2F && turnAmount > -0.2F && Time.time > nextFootstepRun)
        //{
        //    nextFootstepRun = Time.time + 0.30F;
        //    //PlayFootstep();
        //    GetComponent<PlayerShot>().Footstep(transform.name);
        //}
        //if (run && (turnAmount > 0.2F || turnAmount < -0.2F) && forwardAmount < 0.2F && forwardAmount > -0.2F && Time.time > nextFootstepStrafe)
        //{
        //    nextFootstepStrafe = Time.time + 0.20F;
        //    //PlayFootstep();
        //    GetComponent<PlayerShot>().Footstep(transform.name);
        //}
        //if (run && forwardAmount < -0.2F && turnAmount < 0.2F && turnAmount > -0.2F && Time.time > nextFootstepRun)
        //{
        //    nextFootstepRun = Time.time + 0.3F;
        //    //PlayFootstep();
        //    GetComponent<PlayerShot>().Footstep(transform.name);
        //}
        //if ((anim.GetCurrentAnimatorStateInfo(0).IsName("WalkBackward_NtrlFaceFwd")) && Time.time > nextFootstepRun)
        //{
        //    nextFootstepRun = Time.time + 0.5F;
        //    //PlayFootstep();
        //    GetComponent<PlayerShot>().Footstep(transform.name);
        //}
        //if ((anim.GetCurrentAnimatorStateInfo(0).IsName("RunSlide_ToRight_2")) && Time.time > nextSlide)
        //{
        //    nextSlide = Time.time + 1.5F;
        //    //PlayFootstep();
        //    GetComponent<PlayerShot>().Slide(transform.name);
        //}
        if (run && (forwardAmount > 0.2F || forwardAmount < -0.2F || turnAmount > 0.2F || turnAmount < -0.2F) && Time.time > nextFootstepRun)
        {
            nextFootstepRun = Time.time + 0.25F;
            //PlayFootstep();
            GetComponent<PlayerShot>().Footstep(transform.name);
        }
        if (!run && (forwardAmount > 0.2F || forwardAmount < -0.2F || turnAmount > 0.2F || turnAmount < -0.2F) && Time.time > nextFootstepWalk)
        {
            nextFootstepWalk = Time.time + 0.45F;
            //PlayFootstep();
            GetComponent<PlayerShot>().Footstep(transform.name);
        }
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("RunSlide_ToRight_2")) && Time.time > nextSlide)
        {
            nextSlide = Time.time + 1.5F;
            //PlayFootstep();
            GetComponent<PlayerShot>().Slide(transform.name);
        }


        float vertical = CnInputManager.GetAxis("Vertical");
        float horizontal = CnInputManager.GetAxis("Horizontal");

        move = vertical * Vector3.forward + horizontal * Vector3.right;

        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        Move(move);
    }


    void Move(Vector3 move)
    {
        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        moveInput = move;

        CovnertMoveInput();
        UpdateAnimator();
    }

    void CovnertMoveInput()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveInput);

        turnAmount = localMove.x;
        forwardAmount = localMove.z;
    }

    void UpdateAnimator()
    {
        anim.SetFloat("Forward", forwardAmount, 0.1F, Time.deltaTime);
        anim.SetFloat("Turn", turnAmount, 0.1F, Time.deltaTime);
    }

    private void PlayFootstep()
    {
        AudioSource[] sourcesForeign = GetComponents<AudioSource>();
        AudioSource sound = sourcesForeign[2];
        sound.pitch = Random.Range(0.9F, 1.1F);
        sound.Play();
    }
}
