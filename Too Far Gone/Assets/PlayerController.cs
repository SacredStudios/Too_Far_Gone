using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public bool isMoving;
    private Vector2 input;
    public Queue<Vector3> FollowPositions1;
    public Queue<float> FollowAnimations1;
    public Queue<float> FollowAnimations2;//follows the directional booleans from the first player
    public Queue<Vector3> FollowPositions2;
    public Vector3 CurrentFollowPosition;
    public GameObject follower1;
    public GameObject follower2;
    private Animator animator;
    public Animator follower1animations; //the actual animations for follower1
    public Animator follower2animations;
    public LayerMask solidObjects;

    void Start()
    {
         FollowPositions1 = new Queue<Vector3>();
         FollowPositions2 = new Queue<Vector3>(); //for animations, just keep 2 new queues for whichever animation was playing
        animator = GetComponent<Animator>();
        FollowAnimations1 = new Queue<float>();
        follower1animations = follower1.GetComponent<Animator>();
        FollowAnimations2 = new Queue<float>();
        follower2animations = follower2.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            if (input != Vector2.zero)
            {

                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position;
                targetPos.x += input.x / 32;
                targetPos.y += input.y / 32;
                if (IsWalkable(targetPos))
                {
                
                   // Debug.Log("Player 1-"+GetComponent<SpriteRenderer>().sortingOrder);
                   /// Debug.Log("Player 2-"+follower1.GetComponent<SpriteRenderer>().sortingOrder);
                   // Debug.Log("Player 3 -" + follower2.GetComponent<SpriteRenderer>().sortingOrder);/*
                 /*   if (transform.position.y > follower1.transform.position.y)
                    {
                       
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().sortingOrder = 1;
                    }


                    if (follower1.transform.position.y > follower2.transform.position.y)
                    {
                        follower1.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    }
                    else
                    {
                        follower1.GetComponent<SpriteRenderer>().sortingOrder = 1;
                    } */

                    StartCoroutine(Move(targetPos, input.x, input.y));
                }
            }
        }


        animator.SetBool("isMoving", isMoving);
        follower1animations.SetBool("isMoving", isMoving);
        follower2animations.SetBool("isMoving", isMoving);
    }
    IEnumerator Move(Vector3 targetPos, float inputx, float inputy)
    {

        GetComponent<SpriteRenderer>().sortingOrder = (int)(-100 * transform.position.y);
        follower1.GetComponent<SpriteRenderer>().sortingOrder = (int)(-100 * follower1.transform.position.y);
        follower2.GetComponent<SpriteRenderer>().sortingOrder = (int)(-100 * follower2.transform.position.y);

        isMoving = true;
        FollowPositions1.Enqueue(transform.position);
        //Debug.Log(inputx);
        FollowAnimations1.Enqueue(inputx);
        FollowAnimations1.Enqueue(inputy);
        if (FollowPositions1.Count > 20)
        {
            CurrentFollowPosition = FollowPositions1.Dequeue();
            follower1.transform.position = CurrentFollowPosition;
            follower1animations.SetFloat("moveX", FollowAnimations1.Dequeue());
            follower1animations.SetFloat("moveY", FollowAnimations1.Dequeue());
        }
        FollowPositions2.Enqueue(transform.position);
        FollowAnimations2.Enqueue(inputx);
        FollowAnimations2.Enqueue(inputy);
        if (FollowPositions2.Count > 40)
        {
            CurrentFollowPosition = FollowPositions2.Dequeue();
            follower2.transform.position = CurrentFollowPosition + new Vector3(0, 1/25f, 0);
            follower2animations.SetFloat("moveX", FollowAnimations2.Dequeue());
            follower2animations.SetFloat("moveY", FollowAnimations2.Dequeue());
        }
        while ((targetPos-transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }
    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos-new Vector3(0,0.25f,0), 0.1f, solidObjects) != null)
        {
            return false;
        }

        else
        {
            return true;
        }
    }
}
