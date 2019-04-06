using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;


public class AIGroundMovement : MonoBehaviour
{
    public enum GroundMovementState
    {
        Stop,
        MoveForward,
        Jump,
        Patrol,
        Dash
    }

    public enum CollisionBehaviour
    {
        None,
        Rebound,
        Fall,
        Explode,
        Desapear
    }

    //Reference to list above
    public GroundMovementState groundMovementState;
    public CollisionBehaviour collisionBehaviour;//

    //Generic vars
    public bool usePhysics = true; //
    private Rigidbody2D rb;//
    public float thrust = 10f;//
    public float rotationSpeed = 10f;//
    private bool _isMoving;//


    public bool isGrounded;
    public float moveSpeed = 3;
    public float gravity = 20;
    public float jumpSpeed = 10;

    public Transform[] wayPoints;
   


    public bool autoTurn = true;
    public bool jumpForward = true;
    public bool jumpAndWait = false;
    public bool startFacingLeft = true;

    private CharacterController2D.CharacterCollisionState2D _flags;
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController2D _characterController;
    private bool _isFacingLeft;
    private Transform _currentTarget;
    public int _wayPointCounter = 0;


    

    void Start()
    {
        //create a link between CharacterController2d --> this script
        _characterController = gameObject.GetComponent<CharacterController2D>();
        if (startFacingLeft)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            _isFacingLeft = true;
        }
    }

    
    void Update()
    {
        if (isGrounded)
        {
            //STOP
            if (groundMovementState.Equals(GroundMovementState.Stop))
            {
                _moveDirection = Vector3.zero;
                if (isGrounded && !jumpAndWait)
                {
                    StartCoroutine("MoveForwardFromStop");
                }
                //rb.velocity = new Vector2(0, 0);
                //rb.angularVelocity = 0f;
            }
            //MOVE FORWARD
            else if (groundMovementState.Equals(GroundMovementState.MoveForward))
            {
                

                if (_isFacingLeft)
                    _moveDirection.x = -moveSpeed;
                else
                    _moveDirection.x = moveSpeed;

            }
            //JUMP
            else if (groundMovementState.Equals(GroundMovementState.Jump))
            {
                jumpAndWait = true;
                if (jumpAndWait)
                {
                    StartCoroutine(JumpAndWait());
                }
                _moveDirection.y = jumpSpeed;

                if(jumpForward && _isFacingLeft)
                {
                    _moveDirection.x = -moveSpeed;
                }
                else if(jumpForward && !_isFacingLeft)
                {
                    _moveDirection.x = moveSpeed;
                }

                
            }
            ////PATROL
            //else if (groundMovementState.Equals(GroundMovementState.Patrol))
            //{
            //    if (!_currentTarget)
            //        _currentTarget = wayPoints[_wayPointCounter];

            //    Vector3 difference = _currentTarget.position - transform.position;
            //    float distanceX = Mathf.Abs(difference.x);

            //    if(distanceX > 0.1f)
            //    {
            //        //current target is to the right of the enemy
            //        if(difference.x > 0f)
            //        {
            //            _moveDirection.x = moveSpeed;
            //            transform.eulerAngles = new Vector3(0, 180, 0);
            //        }
            //       else if (difference.x < 0f)
            //        {
            //            _moveDirection.x = -moveSpeed;
            //            transform.eulerAngles = new Vector3(0, 0, 0);
            //        }
            //    }
            //    else
            //    {
            //        StartCoroutine("ArriveAtWaypoint");
            //    }
            //}
            //else if (groundMovementState.Equals(GroundMovementState.Dash))
            //{

            //}
        }
        // if isGrounded Ends HERE


        _moveDirection.y -= gravity * Time.deltaTime;
        _characterController.move(_moveDirection * Time.deltaTime);
        //flags, will tell you if your controler has collided (asdw)
        _flags = _characterController.collisionState;
        //detect if the E is on the ground, if it is then isGrounded = true
        isGrounded = _flags.below;

        if (autoTurn)
        {
            if (_flags.left && _isFacingLeft)
            {
                Turn();
            }
            else if (_flags.right && !_isFacingLeft)
            {
                Turn();
            }
        }
    }

    public void Turn()
    {
        if (_isFacingLeft)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            _isFacingLeft = false;
        }
        else if (!_isFacingLeft)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            _isFacingLeft = true;
        }
        _moveDirection.x = -_moveDirection.x;
    }
    IEnumerator JumpAndWait()
    {
        groundMovementState = GroundMovementState.Stop;
        yield return new WaitForSeconds(2.0f);
        groundMovementState = GroundMovementState.Jump;
    }

    IEnumerator MoveForewardFromStop()
    {
        groundMovementState = GroundMovementState.Stop;
        yield return new WaitForSeconds(1.0f);
        groundMovementState = GroundMovementState.MoveForward;
    }

    //IEnumerator ArriveAtWaypoint()
    //{
    //    groundMovementState = GroundMovementState.Stop;
    //    yield return new WaitForSeconds(2f);
    //    _wayPointCounter++;
    //    if(_wayPointCounter > wayPoints.Length -1)
    //    {
    //        _wayPointCounter = 0;
    //    }
    //    _currentTarget = wayPoints[_wayPointCounter];
    //    groundMovementState = GroundMovementState.Patrol;
    //}



    ////- Air based enemies
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionBehaviour.Equals(CollisionBehaviour.None))
        {
            return;
        }
        else if (collisionBehaviour.Equals(CollisionBehaviour.Rebound))
        {
            //Vector2 reflectedPosition = Vector2.Reflect(transform.right, collision.contacts[0].normal);
            //rb.velocity = reflectedPosition.normalized * thrust;
            //Vector3 direction = rb.velocity;
            //float angle = new Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //rb.MoveRotation(angle);
            //rb.angularVelocity = 0f;
        }
        else if (collisionBehaviour.Equals(CollisionBehaviour.Fall))
        {

        }
        else if (collisionBehaviour.Equals(CollisionBehaviour.Explode))
        {
            //TODO: Instanciate and Explosion Effect - Air based enemies
            //Change if using object pooling
            Destroy(gameObject);
        }
        else if (collisionBehaviour.Equals(CollisionBehaviour.Desapear))
        {
            //TODO: Change if using object pooling- Air based enemies
            Destroy(gameObject);
        }
    }
}
