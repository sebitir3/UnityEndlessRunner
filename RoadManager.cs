using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : Singleton<RoadManager>
{
    //to make a variable visible to untiy
    [SerializeField]
    int numberOfPieces = 10;
    [SerializeField]
    string hardcodedPiece = "Straight60m";

    //event and delegates
    public delegate void AddPieceHandler(GameObject roadPiece);
    public event AddPieceHandler OnAddPieceEvent;
    
    //prefab GameObject loaded from resources folder
    GameObject[] loadedPieces;

    //our list of road pieces that will change as part of the track of road
    List<GameObject> roadPieces;

    //create four Transform variables: beginLeft, beginRight, endLeft, endRight
    Transform beginLeft, beginRight, endLeft, endRight;

    //calculate the rotation point of the current road piece
    Vector3 rotationPoint = Vector3.zero;

    //create a float variable for speed = 20
    public float roadSpeed = 20;

    void Start()
    {
        //Initialize the OnAddPiece event with one subscriber (to prevent null reference exeption)
        OnAddPieceEvent += _ => { };

        Debug.Log(1000000000 + 0.0000000001);
        //Initialize the list in memory
        roadPieces = new List<GameObject>();

        //loading up prefabs from resources folder into the array
        loadedPieces = Resources.LoadAll<GameObject>("RoadPieces");

        //Make the first two straight road pieces into the game b4 the for loop
        roadPieces.Add(Instantiate(Resources.Load<GameObject>("RoadPieces/" + hardcodedPiece)));
        roadPieces.Add(Instantiate(Resources.Load<GameObject>("RoadPieces/" + hardcodedPiece)));

        //parent the first road piece in the list to the second
        roadPieces[0].transform.parent = roadPieces[1].transform;
        //position the first piece back behind the second
        float roadLength = (roadPieces[0].transform.Find("BeginLeft").position -
                            roadPieces[0].transform.Find("EndLeft").position).magnitude;
        roadPieces[0].transform.Translate(0f, 0f, -roadLength, Space.World);

        for (int i = 2; i < numberOfPieces; i++)
        {
            addPiece();
        }

        //get the 4 corner markers for the road
        SetCurrentRoadPiece();
    }

    void Update()
    {
        MoveRoadPiece(roadSpeed * Time.deltaTime);

        //pseudocode - converting code in english to programming language

        //Step 1 - determine when the parent road piece's end points passes the origin on the x axis
        //if statement!
        if (endLeft.position.z < 0f || endRight.position.z < 0f)
        {
            //snap the current road piece to the xaxis
            float resetDistance = GetResetDistance();
            MoveRoadPiece(-resetDistance);

            CycleRoadPieces();

            //re-align the newest
            MoveRoadPiece(resetDistance);

            if (roadPieces[1].CompareTag(Tags.straightPiece))
            {
                roadPieces[1].transform.rotation = new Quaternion(roadPieces[1].transform.rotation.x,
                    0f, 0f,
                    roadPieces[1].transform.rotation.w);
                roadPieces[1].transform.position = new Vector3(0f, 0f, roadPieces[1].transform.position.z);
            }
        }



    }

    //set current road piece we're moving
    private void SetCurrentRoadPiece()
    {
        //FROM START
        beginLeft = roadPieces[1].transform.Find("BeginLeft");
        beginRight = roadPieces[1].transform.Find("BeginRight");

        endLeft = roadPieces[1].transform.Find("EndLeft");
        endRight = roadPieces[1].transform.Find("EndRight");

        rotationPoint = GetRotationPoint(beginLeft, beginRight, endLeft, endRight);
    } 



    void addPiece()
    {
        //use Random in Unity
        int randomIndex = Random.Range(0, loadedPieces.Length);

        //add all created road pieces into the list based on previous road piece position and rotation
        roadPieces.Add(Instantiate(loadedPieces[randomIndex],
                              roadPieces[roadPieces.Count - 1].transform.position,
                              roadPieces[roadPieces.Count - 1].transform.rotation));

        //get referene to the last two pieces added to list - newest and previous
        Transform newPiece = roadPieces[roadPieces.Count - 1].transform;
        Transform previousPiece = roadPieces[roadPieces.Count - 2].transform;

        //Calculate the displacement between new and previous piece for alignment
        beginLeft = newPiece.Find("BeginLeft");
        beginRight = newPiece.Find("BeginRight");

        endLeft = previousPiece.Find("EndLeft");
        endRight = previousPiece.Find("EndRight");

        //compute the edges of each road piece - displacement between two points on same piece
        //End edge of previous piece and begin edge of new piece
        Vector3 beginEdge = beginRight.position - beginLeft.position;
        Vector3 endEdge = endRight.position - endLeft.position;

        //compute the angle between the edges of the new and previous piece
        //using corss product (calculate perpendicular vector) and find its sign
        //useful for right curved (positive angle) and left curved (negative angle) {mirrored}
        float angle = Vector3.Angle(beginEdge, endEdge) * Mathf.Sign(Vector3.Cross(beginEdge, endEdge).y);

        //roatate the new road piece to begin alignment with previous
        newPiece.Rotate(0f, angle, 0f, Space.World);

        Vector3 displacement = endLeft.position - beginLeft.position;

        //transform.Translate move the new piece into alignment with previous using displacmement
        newPiece.Translate(displacement, Space.World);

        //parent the new piece to the second road piece in our list/track of road
        //transform.parent
        newPiece.parent = roadPieces[1].transform;

        //Call or broadcast the event to the multiple methods subscribed to it
        OnAddPieceEvent(newPiece.gameObject);
    }

    //calculate distance required to move the road pieces and align it with x axis
    public float GetResetDistance()
    {
        if (roadPieces[1].CompareTag(Tags.straightPiece))
        {
            //return position of straight piece
            return -endLeft.position.z;
        }
        else
        {
            //get the end edge
            Vector3 endEdge = endRight.position - endLeft.position;
            float angle = Vector3.Angle(Vector3.right, endEdge);
            //get radius of our rotation point to rotate the end edge following a partial circle
            float radius = Mathf.Abs(rotationPoint.x);
            return angle * Mathf.Deg2Rad * radius;
        }
    }

    //Calculate rotation point
    public Vector3 GetRotationPoint(Transform beginLeft, Transform beginRight, Transform endLeft, Transform endRight)
    {
        //compute edges of the road piece
        Vector3 beginEdge = beginLeft.position - beginRight.position;
        Vector3 endEdge = endLeft.position - endRight.position;

        //calculate the square magnitude of the edges (scalar value of the length of the edge)
        float a = Vector3.Dot(beginEdge, beginEdge);
        float e = Vector3.Dot(endEdge, endEdge);

        //project BeginEdge onto EndEdge (casts a shadow) - scalar value
        float b = Vector3.Dot(beginEdge, endEdge);

        //difference between square magnitudes of BeginE and endE minus the square of their projections
        float difference = a * e - b * b;

        //the 3D vector between beginLeft and endLeft position
        Vector3 r = beginLeft.position - endLeft.position;

        //projection of beginE onto the 'r' vector
        float c = Vector3.Dot(beginEdge, r);
        //porjection of endE onto the 'r' vector
        float f = Vector3.Dot(endEdge, r);

        //to extend the beginEdge towards the rotation point
        float s = (b * f - c * e) / difference;

        //to extend the endEdge towards the rotation point
        float t = (a * f - c * b) / difference;

        Vector3 rotationPointBegin = beginLeft.position + beginEdge * s;
        Vector3 rotationPointEnd = endLeft.position + endEdge * t;

        //return the midpoint between the two extended edges of our road piece, getting the rotation point
        return (rotationPointBegin + rotationPointEnd) / 2f;

        //create a method with a float parameter called MoveRoadPiece
    }

    public void MoveRoadPiece(float distance)
    {
        //determine which road piece we have - a straight piece based on GameObject tag
        if (roadPieces[1].CompareTag(Tags.straightPiece))
        {
            //move the road piece back on the z axis
            roadPieces[1].transform.Translate(0f, 0f, -distance, Space.World);
        }
        else
        {
            //calculate the radius of thge farthest edge iof the road piece
            float radius = Mathf.Abs(rotationPoint.x);
            float angle = ((distance / radius) * Mathf.Sign(roadPieces[1].transform.localScale.x)) * Mathf.Rad2Deg;

            //rotate the road piece around its rotation point, which axis and how fast to rotate
            roadPieces[1].transform.RotateAround(rotationPoint, Vector3.up, angle);
        }


    }

    public void CycleRoadPieces()
    {
        //Step 2 - delete the first piece in the road list
        //delete GameObject in scene
        Destroy(roadPieces[0]);
        //remove Gameobject in list at an index value
        roadPieces.RemoveAt(0);

        //Step 3 - add new pieces to the end of the list
        addPiece();

        //Step 4 - reparent all road pieces to tje now second road piece in the list
        for (int i = roadPieces.Count - 1; i >= 0; i--)
        {
            //Step 5 - unparenenting the Gameobjts. must be done from the end of the list to the beginning
            roadPieces[i].transform.parent = null;
            roadPieces[i].transform.parent = roadPieces[1].transform;
        }

        //Step 6 - find the 4 corner of the newest second piece to start moving the road again
        SetCurrentRoadPiece();
    }
}
