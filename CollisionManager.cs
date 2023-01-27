using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    //mask the player will check for collisions on
    int collisionMask;

    //getting a referemce to our player object in our scene
    GameObject player;
    SkinnedMeshRenderer rend;
    Animator anim;
    int slideCurveParam;

    [SerializeField] float blinkRate = 0.25f, blinkDuration = 2f;

    bool invincible;

    CollisionSphere[] collisionSpheres;

    Vector3[] slideSpheres;

    void Start()
    {
        collisionMask = GetLayerMask((int)(Layers.Obstacle));

        player = GameObject.Find("Robot");
        rend = player.GetComponentInChildren<SkinnedMeshRenderer>();
        anim = player.GetComponent<Animator>();
        slideCurveParam = Animator.StringToHash("SlideCurve");

        //bring in the SphereCollider components and concert them to CollosionSpheres
        SphereCollider[] colliders = player.GetComponents<SphereCollider>();

        //arrays ' Length, lists = Count
        collisionSpheres = new CollisionSphere[colliders.Length];
        slideSpheres = new Vector3[colliders.Length];
        slideSpheres[0] = new Vector3(0f, 0.2f, 0.75f);
        slideSpheres[1] = new Vector3(0f, 0.25f, 0.25f);
        slideSpheres[2] = new Vector3(0f, 0.55f, -0.15f);
        slideSpheres[3] = new Vector3(0.4f, 0.7f, -0.28f);

        //converting Sphere colliders to CollisionSpheres
        for (int i = 0; i < colliders.Length; i++)
        {
            collisionSpheres[i] = new CollisionSphere(colliders[i].center, colliders[i].radius);
        }

        //sort all CollisionSpheres in the array
        Array.Sort(collisionSpheres, new CollisionSphereComparer());
    }

    
    void LateUpdate()
    {
        //create a list of thing we collide with
        List<Collider> collisions = new List<Collider>();

        //go through all collisionspheres
        for (int i = 0; i < collisionSpheres.Length; i++)
        {
            //modify coliders if during slide
            Vector3 slideDisplacement = slideSpheres[i] - collisionSpheres[i].offset;

            //proportional to anim curve
            //0 = snowman | 1 = slide
            slideDisplacement *= anim.GetFloat(slideCurveParam);

            //apply displacement to colliders
            Vector3 offset = collisionSpheres[i].offset + slideDisplacement;

            //physics.OverlapSphere returns an array of colliders overlapping our CollisionSphere positions
            foreach (Collider c in Physics.OverlapSphere(player.transform.position + offset, collisionSpheres[i].radius, collisionMask))
            {
                //a collision was found! add it to the list
                collisions.Add(c);
            }
        }

        //check if collisions list has a count  bigger than 0
        if (collisions.Count > 0)
        {
            Debug.Log("Collision detected" + collisions[0].gameObject.name);
            ObstacleCollision();
        }
    }

    //creat method called "ObstacleCollision"
    public void ObstacleCollision()
    {
        if(!invincible)
        {
            invincible = true;
            StartCoroutine(BlinkPlayer());
        }

        IEnumerator BlinkPlayer()
        {
            float startTime = Time.time;
            while (invincible)
            {
                //toggle visivility of renderer
                rend.enabled = !rend.enabled;

                if(Time.time >= startTime + blinkDuration)
                {
                    invincible = false;
                    rend.enabled = true;
                }

                yield return new WaitForSeconds(blinkRate);
            }
        }
    }

    int GetLayerMask(params int[] indicies)
    {
        //layermasjk default is empty
        int mask = 0;

        for (int i = 0; i < indicies.Length; i++)
        {
            //bit shift left the valyue of 1 by an index amount of times
            //combine layers together using bitwise OR operator |
            mask |= 1 << indicies[i];
        }

        return mask;
    }

    struct CollisionSphere
    {
        //location of the origin
        public Vector3 offset;
        //size of sphere
        public float radius;

        //constructor
        public CollisionSphere(Vector3 offset, float radius)
        {
            this.offset = offset;
            this.radius = radius;
        }

        //operator overloading - changing how <, > work with CollisionSpheres

        public static bool operator <(CollisionSphere lhs, CollisionSphere rhs)
        {
            return lhs.offset.y < rhs.offset.y;
        }

        public static bool operator >(CollisionSphere lhs, CollisionSphere rhs)
        {
            return lhs.offset.y > rhs.offset.y;
        }
    }

    struct CollisionSphereComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            //check if object x OR object y are collision sphere
            if (!(x is CollisionSphere) || !(y is CollisionSphere))
            {
                Debug.LogError(Environment.StackTrace);  //lets us know what caused error
                throw new ArgumentException("Cannot compare CollisionSheres to non-CollisionSpheres");
            }

            //cast x, y to Collision Spheres
            CollisionSphere lhs = (CollisionSphere)x;
            CollisionSphere rhs = (CollisionSphere)y;

            if (lhs > rhs)
            {
                return 1; //where lhs is greater, return 1 to sortt higher
            }
            else if (lhs < rhs)
            {
                return -1; //return -1 to sort lower (first)
            }
            else //equal to
            {
                return 0;
            }
        }
    }
}
