using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerControllerBase : MonoBehaviour
{
    public float Speed;
    public float Gravity;
    private CharacterController cc;
    private Vector3 VelocityY;
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    public Slider ProgressionBar;
    protected bool IsInteractingWithFurniture;
    float turnSmoothVelocity;
    float originSpeed;
    protected void OnStart()
    {
        cc = this.GetComponent<CharacterController>();
        IsInteractingWithFurniture = false;
        ProgressionBar.gameObject.SetActive(false);
        originSpeed = Speed;
    }

    protected void OnUpdate()
    {
        if (IsInteractingWithFurniture)
        {
            this.GetComponentInChildren<Animator>().SetBool("RUN", false);
            this.GetComponentInChildren<Animator>().SetBool("RUN_FAST", false);
            return;
        }
        bool isGround = cc.isGrounded;
        float _v = Input.GetAxis("Vertical");
        if (!isGround)
        {
            VelocityY.y -= Gravity * Time.deltaTime;
        }
        else
        {
            VelocityY.y -= Gravity * Time.deltaTime * 0.0000001f;
            //VelocityY.y = 0;
        }
        cc.Move(VelocityY * Time.deltaTime);
        if(_v!=0)
        {
            float vertical = _v;
            Vector3 direction = new Vector3(0f, 0f, vertical).normalized;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cc.Move(moveDir.normalized * Speed * Time.deltaTime);
            if (Speed == originSpeed)
            {
                this.GetComponentInChildren<Animator>().SetBool("RUN", true);
                this.GetComponentInChildren<Animator>().SetBool("RUN_FAST", false);
            }
            else this.GetComponentInChildren<Animator>().SetBool("RUN_FAST", true);
        }
        else
        {
            this.GetComponentInChildren<Animator>().SetBool("RUN", false);
            this.GetComponentInChildren<Animator>().SetBool("RUN_FAST", false);
        }

    }


    public virtual void HandleInteractWithFurniture(UnityAction Action)
    {
        IsInteractingWithFurniture = true;
    }

    public virtual void HandleInteractionDone()
    {
        IsInteractingWithFurniture = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
