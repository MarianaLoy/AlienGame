using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class PlayerController : MonoBehaviour
{


    //public enum GroundType
    //{
    //    None,
    //    LevelGeometry,
    //    OneWayPlatform
    //}

    
    public float walkSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float doubleJumpSpeed = 4.0f;
    public float wallJumpXAmount = 0.5f;
    public float wallJumpYAmount = 0.5f;
    public float wallRunAmount = 2f;
    public float slopeSlideSpeed = 4.0f;
    public float glideAmount = 2f;
    public float glideTimer = 2f;
    public float creepSpeed = 3.0f;
    public float powerJumpSpeed = 10.0f;
    public float stompSpeed = 4.0f;



    //Player ability toggles
    public bool canDoubleJump = true;
    public bool canWallJump = true;
    public bool canWallRun = true;
    public bool canRunAfterWallJump = true;
    public bool canGlide = true;
    public bool canPowerJump = true;
    public bool canStomp = true;

    //Payer state variables
    public bool isGrounded;
    public bool isJumping;
    public bool isFacingRight;
    public bool doubleJumped;
    public bool wallJumped;
    public bool isWallRunning;
    public bool isSlopeSliding;
    public bool isGliding;
    public bool isDucking;
    public bool isCreeping;
    public bool isPowerJumping;
    public bool isStomping;


    public LayerMask layerMask;

    //Private variable
    private CharacterController2D.CharacterCollisionState2D _flags;
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController2D _characterController;
    private bool _lastJumpWasLeft;
    private float _slopeAngle;
    private Vector3 _slopeGradient = Vector3.zero;
    private bool _startGlide;
    private float _currentGlideTimer;
    private BoxCollider2D _boxCollider;
    private Vector2 _originalBoxColliderSize;
    private Vector3 _frontTopCorner;
    private Vector3 _backTopCorner;
    //animator and tokle
    //public GroundType _groundType;
    
    void Start()
    {
        _characterController = GetComponent<CharacterController2D>();
        _currentGlideTimer = glideTimer; //Resets the currentGlideTimer = glideTimer
        _boxCollider = GetComponent<BoxCollider2D>();
        _originalBoxColliderSize = _boxCollider.size;

    }


    void Update()
    {
        if(wallJumped == false)
        {
            _moveDirection.x = Input.GetAxis("Horizontal");
            _moveDirection.x *= walkSpeed;
        }
        //Check ground angle for slope slide
        GetGroundType();
       
        
        if (isGrounded) //Player is grounded
        {
            _moveDirection.y = 0;
            isJumping = false;
            doubleJumped = false;
            isStomping = false;
            _currentGlideTimer = glideTimer;
          
            if (_moveDirection.x < 0) //P moving towards the left
            {
                transform.eulerAngles = new Vector3(0, 180, 0); // rotating P 180 degrees towards Y axis
                isFacingRight = false;
            }
            else if (_moveDirection.x > 0) // P -> right
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                isFacingRight = true;
            }

            if(isSlopeSliding)
            {
                _moveDirection = new Vector3(_slopeGradient.x * slopeSlideSpeed, -_slopeGradient.y * slopeSlideSpeed, 0f);
            }

            //JUMP && PowerJump
            if (Input.GetButtonDown("Jump"))
            {
                if(canPowerJump && isDucking)
                {
                    _moveDirection.y = jumpSpeed + powerJumpSpeed;
                    StartCoroutine("PowerJumpWaiter");
                }
                else
                {
                    _moveDirection.y = jumpSpeed;
                    isJumping = true;
                }

                
                isWallRunning = true;
            }
        }
        else //Player is in the air
        {
            if(Input.GetButtonUp("Jump"))
            {
                if(_moveDirection.y > 0 ) //P istill going up
                {
                    _moveDirection.y = _moveDirection.y * 0.5f;
                }
            }

            if(Input.GetButtonDown("Jump"))
            {
                if(canDoubleJump)
                {
                    if(!doubleJumped)
                    {
                        _moveDirection.y = doubleJumpSpeed;
                        doubleJumped = true;
                    }
                }
            }
        }
        //GRAVITY CALCULATIONS 
        if(canGlide = true && Input.GetAxis("Vertical") > 0.5f && _characterController.velocity.y < 0.2f)
        {
            if(_currentGlideTimer > 0) //We still have glide time
            {
                //Overwriting gravity calculations 
                isGliding = true; //Important for animation

                if (_startGlide)
                {
                    _moveDirection.y = 0;
                    _startGlide = false;
                }

                _moveDirection.y -= glideAmount * Time.deltaTime;
                _currentGlideTimer -= Time.deltaTime;
            }
            else
            {
                isGliding = false;     
                _moveDirection.y -= gravity * Time.deltaTime; //Player is affected by gravity. (We stop gliding)
            }
 
            
        }
        else if(canStomp && isDucking && !isPowerJumping)
        {
            _moveDirection.y -= gravity * Time.deltaTime + stompSpeed;
            isStomping = true;
        }
        else
        {
            isGliding = false;
            _startGlide = true;
            _moveDirection.y -= gravity * Time.deltaTime; //Player is affected by gravity 
        }
       
        //PLAYER MOVEMENT
        _characterController.move(_moveDirection * Time.deltaTime); //Tells CharacterController to move player left or right
        _flags = _characterController.collisionState; //Returns the flags:that tell us if its anything to our left us, right or above or below


        isGrounded = _flags.below;

        //DUCKING AND CREEPING

        _frontTopCorner = new Vector3(transform.position.x + _boxCollider.size.x / 2, transform.position.y + _boxCollider.size.y / 2, 0);
        _backTopCorner = new Vector3(transform.position.x - _boxCollider.size.x / 2, transform.position.y + _boxCollider.size.y / 2, 0);
        RaycastHit2D hitFrontCeiling = Physics2D.Raycast(_frontTopCorner, Vector2.up, 2f, layerMask); //LevelGeometryMask
        RaycastHit2D hitBackCeiling = Physics2D.Raycast(_backTopCorner, Vector2.up, 2f, layerMask); //LevelGeometryMask

        if (Input.GetAxis("Vertical") < 0 && _moveDirection.x  == 0)
        {
            if(!isDucking && !isCreeping)
            {
                _boxCollider.size = new Vector2(_boxCollider.size.x, _originalBoxColliderSize.y / 2);
                transform.position = new Vector3(transform.position.x, transform.position.y - (_originalBoxColliderSize.y / 4), 0);
                _characterController.recalculateDistanceBetweenRays();
            }

            isDucking = true;
            isCreeping = false;
        }
        else if (Input.GetAxis("Vertical") < 0 && (_moveDirection.x < 0 || _moveDirection.x > 0))
        {
            if (!isDucking && !isCreeping)
            {
                _boxCollider.size = new Vector2(_boxCollider.size.x, _originalBoxColliderSize.y / 2);
                transform.position = new Vector3(transform.position.x, transform.position.y - (_originalBoxColliderSize.y / 4), 0);
                _characterController.recalculateDistanceBetweenRays();
            }

            isDucking = false;
            isCreeping = true;
        }
        else
        {
            //Where we stand back up
            if(!hitFrontCeiling.collider && !hitBackCeiling.collider && (isDucking || isCreeping))
            {
                _boxCollider.size = new Vector2(_boxCollider.size.x, _originalBoxColliderSize.y);
                transform.position = new Vector3(transform.position.x, transform.position.y + (_originalBoxColliderSize.y / 4), 0);
                _characterController.recalculateDistanceBetweenRays();

                isDucking = false;
                isCreeping = false;
            }

        }


        if (_flags.above)
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }
        

        //WALL RUN
        if(_flags.left || _flags.right)
        {
            if (canWallRun)
            {
                if(Input.GetAxis("Vertical") > 0 && isWallRunning == true)
                {
                    _moveDirection.y = jumpSpeed / wallRunAmount;
                    StartCoroutine(WallRunWaiter());
                }
            }

            //WALL JUMP
            if (canWallJump)
            {
                if(Input.GetButtonDown("Jump") && wallJumped == false && isGrounded == false)
                {
                    if(_moveDirection.x < 0) // Left
                    {
                        _moveDirection.x = jumpSpeed * wallJumpXAmount;
                        _moveDirection.y = jumpSpeed * wallJumpYAmount;
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        //_lastJumpWasLeft = false;
                        wallJumped = true;
                        
                    } else if (_moveDirection.x > 0)
                    {
                        _moveDirection.x = -jumpSpeed * wallJumpXAmount;
                        _moveDirection.y = jumpSpeed * wallJumpYAmount;
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        //_lastJumpWasLeft = true;
                        wallJumped = true;
                    }

                    StartCoroutine(WallJumpWaiter());
                }
               
            }
        }else
        {
            if(canRunAfterWallJump)
            {
                StopCoroutine(WallRunWaiter());
                isWallRunning = true;
            }
        }
   
    }
    
    IEnumerator WallJumpWaiter()
    {
        wallJumped = true;
        yield return new WaitForSeconds(0.5f);
        wallJumped = false;
    }

    IEnumerator WallRunWaiter()
    {
        isWallRunning = true;
        yield return new WaitForSeconds(0.5f);
        isWallRunning = false;
    }

    IEnumerator PowerJumpWaiter()
    {
        isPowerJumping = true;
        yield return new WaitForSeconds(0.8f);
        isPowerJumping = false;
    }

    public void GetGroundType()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector3.up, 2f, layerMask);

        if (hit)
        {
            _slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            _slopeGradient = hit.normal;

            if (_slopeAngle > _characterController.slopeLimit)
            {
                isSlopeSliding = true;
            }
            else
            {
                isSlopeSliding = false;
            }
        }
        //string layerName = LayerMask.LayerToName(hit.transform.gameObject.layer);
        //if(layerName == "OneWayPlatform")
        //{
        //    _groundType = GroundType.OneWayPlatform;
        //}
        //else if(layerName == "LevelGeometry")
        //{
        //    _groundType = GroundType.LevelGeometry;
       
        //}
        //else
        //{
        //    _groundType = GroundType.None;
        //}
    }

}
