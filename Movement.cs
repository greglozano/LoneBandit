using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Component Variables")]
    public Animator playerAnim;
    public Animator WeaponAnim;

    [Header("Movement Variables")]
    public float PlayerSpeed;
    private KeyCode LastKeyPressedInFrame;
    [HideInInspector]
    public static KeyCode LastKeyDirection;
    //private Vector3 Velocity;
    private List<KeyCode> KeysPressedInFrame = new List<KeyCode>();

    public LayerMask Enemies;

    [Header("Melee Attack Variables")]
    public float MeleeAttackRadius;
    public float MeleeAttackRange;
    public float TotalMeleeTime;
    private bool isMeleeAttacking;
    private float meleeTimer;

    [Header("Ranged Attack Variables")]
    public GameObject ProjectilePrefab;
    public float RangedAttackCooldown;
    private float rangedAttackTimer;
    public LayerMask Interactables;
    public bool hasRangedAttack;

    [Header("Interact Variables")]
    public float InteractRange;
    public float InteractRadius;


   public int playerspeed = 5;
    public Vector3 Velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        LastKeyDirection = KeyCode.D;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMovementInput();
        CheckMeleeAttackingInput();
        MeleeAttackTimer();
        Interact();

    }

    void CheckMovementInput()
    {
        Camera.main.transform.position = new Vector3(transform.position.x,
          transform.position.y,
          transform.position.z - 10);

       // Velocity = Vector3.zero;
        //if (Input.GetKey(KeyCode.W))
        //{
         //   
          //  Velocity += Vector3.up;
        //}

       // else if (Input.GetKey(KeyCode.S))
        //{
          //  
            //Velocity += Vector3.down;
        //}

        //if (Input.GetKey(KeyCode.D))
        //{
          //  
            //Velocity += Vector3.right;
        //}

        //else if (Input.GetKey(KeyCode.A))
        //{
          //  
           // Velocity += Vector3.left;
       // }
        //if (Velocity == Vector3.zero)
        //{
          //  
        //}
        //transform.position += (Velocity * playerspeed) * Time.deltaTime;
          if (Input.GetKeyDown(KeyCode.W))
            KeysPressedInFrame.Add(KeyCode.W);
        else if (Input.GetKeyUp(KeyCode.W))
          KeysPressedInFrame.Remove(KeyCode.W);

        if (Input.GetKeyDown(KeyCode.S))
            KeysPressedInFrame.Add(KeyCode.S);
        else if (Input.GetKeyUp(KeyCode.S))
           KeysPressedInFrame.Remove(KeyCode.S);

        if (Input.GetKeyDown(KeyCode.A))
          KeysPressedInFrame.Add(KeyCode.A);
        else if (Input.GetKeyUp(KeyCode.A))
          KeysPressedInFrame.Remove(KeyCode.A);

         if (Input.GetKeyDown(KeyCode.D))
           KeysPressedInFrame.Add(KeyCode.D);
        else if (Input.GetKeyUp(KeyCode.D))
          KeysPressedInFrame.Remove(KeyCode.D);

        if (KeysPressedInFrame.Count > 0)
        {
           LastKeyPressedInFrame = KeysPressedInFrame[^1];
         LastKeyDirection = LastKeyPressedInFrame;
        }
        else
          LastKeyPressedInFrame = KeyCode.None;

        if (!isMeleeAttacking)
        {
            Velocity = GetDirection(LastKeyPressedInFrame) * PlayerSpeed;

            transform.position += Velocity * Time.deltaTime;

            switch(LastKeyPressedInFrame)
            {
                case KeyCode.W:
                    playerAnim.Play("PlayerWalkUp", 0);
                    break;
                case KeyCode.D:
                    playerAnim.Play("PlayerWalkRight", 0);
                    break;
                case KeyCode.A:
                    playerAnim.Play("PlayerWalkLeft", 0);
                    break;
                case KeyCode.S:
                    playerAnim.Play("PlayerWalkDown", 0);
                    break;
                default:
                    playerAnim.Play("PlayerIdle", 0);
                    break;

            }
        }
    }

    void CheckMeleeAttackingInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMeleeAttacking)
        {
            isMeleeAttacking = true;
            WeaponAnim.Play("Attack");

            Vector3 CastDirection;
            //if (GetDirection(LastKeyPressedInFrame) == Vector3.zero)
           // {
                CastDirection = transform.position + (GetDirection(LastKeyDirection) * MeleeAttackRange);
           // }

           // else
           // {
           //     CastDirection = transform.position + (GetDirection(LastKeyPressedInFrame) * MeleeAttackRange);
           // }

            Collider2D hitObj = Physics2D.OverlapCircle(CastDirection, MeleeAttackRadius, Enemies);
           
            if (hitObj != null)
                hitObj.GetComponent<Enemy>().Die();
        }
    }

        void MeleeAttackTimer()
        {
            if (isMeleeAttacking)
            {
                if (meleeTimer > TotalMeleeTime)
                {
                    meleeTimer = 0;
                    isMeleeAttacking = false;
                    WeaponAnim.Play("WeaponEmpty");
                }

                meleeTimer += Time.deltaTime;
            }

        }

        void RangedAttackTimer()
        {
            if (hasRangedAttack)
            {
                if (rangedAttackTimer > RangedAttackCooldown)
                {
                    rangedAttackTimer = 0;
                    hasRangedAttack = false;

                }
            }
        }
    void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3 CastDirection;
            if ((GetDirection(LastKeyPressedInFrame)) == Vector3.zero)
            {
                CastDirection = transform.position + (GetDirection(LastKeyDirection) * InteractRange);
            }

            else
            {
                CastDirection = transform.position + (GetDirection(LastKeyPressedInFrame) * InteractRange);
            }

            Collider2D hitObj = Physics2D.OverlapCircle(CastDirection, InteractRadius, Interactables);
            if (hitObj != null)
            {
              //  hitObj.GetComponent<Interactables>().Pickup();
            }
        }
    }

    void CheckRangedAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.G) && !isMeleeAttacking && !hasRangedAttack)
        {
            hasRangedAttack = true;

            Vector3 CastDirection;
            if (GetDirection(LastKeyPressedInFrame) == Vector3.zero)
                CastDirection = transform.position + (GetDirection(LastKeyDirection) * MeleeAttackRange);
            else
                CastDirection = transform.position + (GetDirection(LastKeyPressedInFrame) * MeleeAttackRange);

            Instantiate(ProjectilePrefab).transform.position = transform.position;
        }
    }

    static Vector3 GetDirection(KeyCode LastPressedKey) => LastPressedKey switch
        {
            KeyCode.W => Vector3.up,
            KeyCode.S => Vector3.down,
            KeyCode.A => Vector3.left,
            KeyCode.D => Vector3.right,
            KeyCode.None => Vector3.zero,
            _ => Vector3.zero
        };

    void OnDrawGizmos()
    {
        Vector3 CastDirection;
        if (isMeleeAttacking)
        {
            Gizmos.color = Color.yellow;

            if (GetDirection(LastKeyPressedInFrame) == Vector3.zero)
            {
                CastDirection = transform.position + (GetDirection(LastKeyDirection) * MeleeAttackRange);

            }
            else
            {
                CastDirection = transform.position + (GetDirection(LastKeyPressedInFrame) * MeleeAttackRange);

            }
            Gizmos.DrawWireSphere(CastDirection, MeleeAttackRadius);

            if (GetDirection(LastKeyPressedInFrame) == Vector3.zero)
            {
                CastDirection = transform.position + (GetDirection(LastKeyDirection) * InteractRange);
            }

            else
            {
                CastDirection = transform.position + (GetDirection(LastKeyPressedInFrame) * InteractRange);
            }


            Gizmos.DrawWireSphere(CastDirection, InteractRadius);

        }

    }
       
}
