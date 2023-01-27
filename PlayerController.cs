using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    //Our Input Controls created in Unity
    PlayerInputActions inputActions;

    //variable that store the input frm PlayerInputActions
    bool jump, slide;
    float horizontal = 0f;
    float hPrev, hNew;
    int currentLane = 0;
    int prevLane = 0;
    int direction;
    public int numberOfLanes { get; private set; } = 3;
    float strafeSpeed = 5f;
    public float laneWidth { get; private set; }

    [SerializeField]
    private float gravity = -9.8f;
    [SerializeField]
    private float initVelocity = 5f;

    Coroutine currentLaneChange;

    int directionBuffer = 0;
    int laneChangeStackCalls = 0; //how many calls to the coroutine in the computer memory at a time

    //animator controller on robot
    Animator anim;
    int jumpParam;
    int slideParam;

    State currentState = State.Run;

    void Awake()
    {
        anim = GetComponent<Animator>();
        jumpParam = Animator.StringToHash("Jump");
        slideParam = Animator.StringToHash("Slide");

        //Debug.Log(Factorial(5));
        //StartCoroutine(TestCoroutine());

        //determine the width of each lane\
        laneWidth = 7.5f / numberOfLanes;


        //set the robot to the middle lane
        transform.position = Vector3.zero;

        inputActions = new PlayerInputActions();

        //using a lambda expression to get input from our controls
        inputActions.Player.Horizontal.performed += context => horizontal = context.ReadValue<float>();

        //The KeyDown(performed) and KeyUp(cancelled) events for our controls
        //InputActionVariable.ActionMapName.ActionName.keydown/up
        inputActions.Player.Jump.performed += ctx => jump = ctx.ReadValue<float>() > 0f;

        //canceled jump
        inputActions.Player.Jump.canceled += ctx => jump = ctx.ReadValue<float>() > 0f;

        //performed slide
        inputActions.Player.Slide.performed += ctx => slide = ctx.ReadValue<float>() > 0f;

        //canceled slide
        inputActions.Player.Slide.canceled += ctx => slide = ctx.ReadValue<float>() > 0f;
    }

    /*
    void ReadHorizaontalValue(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<float>();
    }
    */

    void Update()
    {
        //horizontal
        hNew = horizontal;
        float hDelta = hNew - hPrev;
        if (Mathf.Abs(hDelta) > 0f && Mathf.Abs(hNew) > 0f && currentState == State.Run)
        {
            MovePlayer((int)hNew);
        }

        hPrev = hNew;

        //jump
        if (jump == true && currentState == State.Run)
        {
            currentState = State.Jump;
            StartCoroutine(Jump());
        }

        //slide
        if (slide == true)
        {
            anim.SetTrigger(slideParam);
        }

        //reset the input once actions are complete
        horizontal = 0f;
        jump = false;
        slide = false;
    }

    public void FinishSlide()
    {
        currentState = State.Run;
    }

    private void MovePlayer(int direction)
    {
        //update currentLane with direction
        //Mathf.Clamp keeps value between min and max range
        prevLane = currentLane;

        //if there's already a coroutine moving our player
        if (currentLaneChange != null)
        {
            if (currentLane + direction != prevLane)
            {
                directionBuffer = direction;
                //prevents lane regaion
                return;
            }
            StopCoroutine(currentLaneChange);
            directionBuffer = 0;
        }

        currentLane = Mathf.Clamp(currentLane + direction, numberOfLanes / -2, numberOfLanes / 2);

        currentLaneChange = StartCoroutine(LaneChange());
    }

    //pause the unity editor
    //public void PauseEditor()
    //{
      //  UnityEditor.EditorApplication.isPaused = true;
    //}

    IEnumerator LaneChange()
    {
        Vector3 fromPosition = Vector3.right * prevLane * laneWidth;

        Vector3 toPosition = Vector3.right * currentLane * laneWidth;

        float t = (laneWidth - Vector3.Distance(transform.position.x * Vector3.right, toPosition)) / laneWidth;

        //Gradual linear interpolation from a point to another point
        for (; t < 1f; t += strafeSpeed * Time.deltaTime / laneWidth)
        {
            transform.position = Vector3.Lerp(fromPosition + Vector3.up * transform.position.y, toPosition + Vector3.up * transform.position.y, t);
            //complete the frame before running next lerp action
            yield return null;
        }

        transform.position = toPosition + Vector3.up * transform.position.y;

        currentLaneChange = null;

        //if there's a direction we want to go in the buffer AND we only call this coroutine twice
        if (directionBuffer != 0 && laneChangeStackCalls++ < 2)
        {
            MovePlayer(directionBuffer);
            directionBuffer = 0;
        }

        //once coroutine is complete and there's nothing in the buffer
        laneChangeStackCalls = 0;
    }

    IEnumerator Jump()
    {
        anim.SetBool(jumpParam, true);


        //calculate total jump time
        float totalTime = (initVelocity * 2) / -gravity;
        for (float t = 0; t < totalTime; t += Time.deltaTime)
        {
            //use gravity displacement equation to calculat y pos. of robot
            float yPos = (gravity / 2) * (t * t) + (initVelocity) * (t);
            Helper.SetY(transform, yPos);

            yield return null;
        }

        //ensure player is back on ground
        Helper.SetY(transform, 0f);
        anim.SetBool(jumpParam, false);
        currentState = State.Run;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    IEnumerator TestCoroutine()
    {
        Debug.Log("Wait for 2 secs");
        //yield return pauses progress withtin the coroutine for x amount of seconds
        yield return new WaitForSeconds(2f);

        Debug.Log("Thanks, wait for another 6 seconds");
        yield return new WaitForSeconds(6f);

        Debug.Log("You're patient, thank you! Have some factorials");

        for (int i = 0; i < 10; i++)
        {
            int factorial = i;
            for (int j = i - 1; j > 0; j--)
            {
                factorial *= j;
            }
            Debug.Log($"Factorial of {i} is: {factorial}");

            //resume from this point in next frame
            yield return null;
        }

        Debug.Log("Coroutine is done.");
    }

    //Recursion is a method that calls itself to solve the problem
    public float Factorial(float number)
    {
        //default case
        if (number <= 1)
        {
            return 1;
        }
        else
        {
            //multiply the number by the smaller value (5! = 5*4*3*2*1)
            return number * Factorial(number - 1);
        }
    }
}
