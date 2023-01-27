using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    GameObject[] loadedObstacles;
    [SerializeField] float distanceInterval = 20f;
    float leftoverDistance = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        loadedObstacles = Resources.LoadAll<GameObject>("Obstacles");

        //subscribe a method that follows the delegete requirements of the road manager
        // on add piece event
        RoadManager.Instance.OnAddPieceEvent += PlaceObstacles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //create a method called PlaceObstacles that takes in a gameObject parameter
    //prints a message to the unity console

    public void PlaceObstacles(GameObject roadPiece)
    {
        Debug.Log("Hit that like button!");

        //find 4 corners
        Transform beginRight = roadPiece.transform.Find("BeginRight");
        Transform beginLeft = roadPiece.transform.Find("BeginLeft");
        Transform endRight = roadPiece.transform.Find("EndRight");
        Transform endLeft = roadPiece.transform.Find("EndLeft");

        //create float variable called roadLength
        float roadLength;
        Vector3 rotationPoint = Vector3.zero;
        float radius = 0f;

        //check for straight piece
        if (roadPiece.CompareTag(Tags.straightPiece))
        {
            roadLength = Vector3.Distance(beginLeft.position, endLeft.position);
        }
        else
        {
            //get radius of the rotation point of the curved peice
            rotationPoint = RoadManager.Instance.GetRotationPoint(beginLeft, beginRight, endLeft, endRight);
            radius = Vector3.Distance(roadPiece.transform.position, rotationPoint);

            //Calculate the angle
            float angle = Vector3.Angle(beginLeft.position - beginRight.position, endLeft.position - endRight.position);

            //calculate road length - angular velocity
            roadLength = radius * angle * Mathf.Deg2Rad;
        }

        //make another float var. for halfRoadLength
        float halfRoadLength = roadLength / 2f;

        float currentDistance = distanceInterval - halfRoadLength - leftoverDistance;

        //check if the length of the raod is too small to add an obstacle road to
        if (currentDistance >= halfRoadLength)
        {
            leftoverDistance += roadLength;
        }

        //create a for loop to check how many obstacle rows to make
        for (; currentDistance < halfRoadLength; currentDistance += distanceInterval)
        {
            //contain all obstacles on a row (create empty gameobject in scene)
            GameObject obstacleRow = new GameObject("ObstacleRow");
            obstacleRow.transform.position = roadPiece.transform.position;
            obstacleRow.transform.rotation = roadPiece.transform.rotation;
            //the row must line up with the model's rotation
            obstacleRow.transform.Rotate(90f, 0f, 0f);
            //parent te row to the road piece its on
            obstacleRow.transform.parent = roadPiece.transform;

            //variable for same obstacles (to prevemt walls from blocking all lanes)
            int sameObstacleCount = 0;
            int numLanes = PlayerController.Instance.numberOfLanes;


            for (int i = numLanes / -2; i <= numLanes / 2; i++)
            {
                int randomObstacle = Random.Range(0, loadedObstacles.Length);

                //checks for if the obstacle is a wall obstacle (have to dodge around)
                if (loadedObstacles[randomObstacle].CompareTag(Tags.wall))
                {
                    //if all lanes will be blocked by walls
                    if (++sameObstacleCount >= numLanes)
                    {
                        //increment chosen obstacle to another
                        randomObstacle += 3;
                        randomObstacle %= loadedObstacles.Length;
                    }

                   
                }

                //instantiate the obstacle
                GameObject obstacle = Instantiate(loadedObstacles[randomObstacle], obstacleRow.transform.position, obstacleRow.transform.rotation, obstacleRow.transform);

                //shift obstacle over to correct lane
                obstacle.transform.Translate(Vector3.right * i * PlayerController.Instance.laneWidth, Space.Self);
            }

            

            if (roadPiece.CompareTag(Tags.straightPiece))
            {
                obstacleRow.transform.Translate(0f, 0f, currentDistance);
            }
            else
            {
                float angle = currentDistance / radius;
                //rotate the obstacle row to follow the curvature of the road piece
                obstacleRow.transform.RotateAround(rotationPoint, Vector3.up, angle * Mathf.Rad2Deg * -Mathf.Sign(roadPiece.transform.localScale.x));
            }

            leftoverDistance = halfRoadLength - currentDistance;
        }
    }
}
