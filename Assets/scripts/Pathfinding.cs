﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    //Decision Making
    DecisionMaking dm;
    private int i = 0;

    public bool testState = false;

    //Waypoints
    Waypoint waypoint;

    public float rotationSpeed = 1f;
    public float walkingSpeed = 1.5f;
    public Vector3 currentWaypoint;
    public int waypointIndex = 0;

    public Transform seeker;
    public Transform target;
    public Transform player;

    private bool drawingPath = false;
    public bool startFollowingPath = false;

    public Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
        dm = GetComponent<DecisionMaking>();
    }

    void Start()
    {
        dm = new DecisionMaking();
        target = Waypoint.waypoints[waypointIndex];
        FindPath(seeker.position, target.position);
        
    }

    void Update()
    {
        //Debug.Log(target.transform.position);

        if (startFollowingPath == true)
        {
            seeker.transform.position = Vector3.MoveTowards(seeker.transform.position, grid.path[i].worldPosition, walkingSpeed * Time.deltaTime);
            
            if(Vector3.Distance(grid.path[i].worldPosition, seeker.transform.position) <= 0.1f)
            {
                i++;
            }
        }

        //Reached the waypoint
        if (Vector3.Distance(Waypoint.waypoints[waypointIndex].transform.position, seeker.transform.position) <= 1f)
        {
            //startFollowingPath = false;
            waypointIndex++;
            Debug.Log(waypointIndex);

            target = Waypoint.waypoints[waypointIndex];
            FindPath(seeker.position, target.position);

            //startFollowingPath = true;
            //StartCoroutine(FollowThePath());

            //Debug.Log((waypointIndex == Waypoint.waypoints.Length));
            StopCoroutine("FollowThePath");
        }

        /*int i = 0;
            //Vector3 directionToWaypoint = (grid.path[10].worldPosition - seeker.transform.position);
            seeker.transform.position = Vector3.MoveTowards(seeker.transform.position, target.transform.position, walkingSpeed*Time.deltaTime);
            Debug.Log(seeker.transform.position);


        //if (Vector3.Distance(target.transform.position, seeker.transform.position) <= 0.1f) { }
                //i++;
                //Debug.Log(seeker.transform.position);

    }*/
        if (DecisionMaking.aiState == DecisionMaking.States.Seek)
        {
            //startFollowingPath = true;
            //Debug.Log(DecisionMaking.aiState);
            StopCoroutine("FollowThePath");
            //StopCoroutine("FindPath");

            //target.position = dm.player.transform.position;
            //StartCoroutine(FindPath(seeker.position, target.position));

            FindPath(seeker.position, player.position);
            StartCoroutine("FollowThePath");
        }
        if(DecisionMaking.aiState == DecisionMaking.States.Wander)
        {
            //dm.wandering = false;
            //Debug.Log(dm.aiState);
            StartCoroutine("FollowThePath");
        }
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Debug.Log("findpath");
        List<Node> reachable; //Open
        List<Node> explored;  //Closed
        List<Node> path;

        reachable = new List<Node>();
        explored = new List<Node>();
        path = new List<Node>();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        reachable = new List<Node>();
        reachable.Add(startNode);

        while (reachable.Count > 0)
        {
            Node node = reachable[0];
            for (int i = 1; i < reachable.Count; i++)
            {
                if (reachable[i].f < node.f || reachable[i].f == node.f)
                {
                    if (reachable[i].h < node.h)
                        node = reachable[i];
                }
            }

            reachable.Remove(node);
            explored.Add(node);


            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                break;
            }

            foreach (Node neighbour in grid.AddAdjacent(node))
            {
                if (!neighbour.walkable || explored.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.g + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.g || !reachable.Contains(neighbour))
                {
                    neighbour.g = newCostToNeighbour;
                    neighbour.h = GetDistance(neighbour, targetNode);
                    neighbour.previous = node;

                    if (!reachable.Contains(neighbour))
                        reachable.Add(neighbour);
                }
            }
           
        }
    }


    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;

        while (currentNode != startNode)
        {        
            path.Add(currentNode);
            currentNode = currentNode.previous;
        }
        
        path.Reverse();       
        grid.path = path;

        startFollowingPath = true;
    }

    IEnumerator FollowThePath()
    {
        int i = 0;

        while(true)
        {

            if (waypointIndex == Waypoint.waypoints.Length)
            {
                //StopCoroutine("FollowThePath");
                //dm.wandering = false;
                Debug.Log("ranOut");
                waypointIndex = 0;
                target = Waypoint.waypoints[waypointIndex];
                //startFollowingPath = true;
                FindPath(seeker.position, target.position);
                StopCoroutine("FollowThePath");
                //StartCoroutine(FollowThePath());


                yield break;
            }

            

            //test();

            //seeker.transform.LookAt(grid.path[i].worldPosition);
            //seeker.transform.position = Vector3.Lerp(seeker.transform.position, grid.path[i].worldPosition, walkingSpeed);

            //seeker.transform.LookAt(target.transform.position);
            //seeker.transform.position = Vector3.Lerp(seeker.transform.position, target.transform.position, walkingSpeed);

            //Vector3 directionToWaypoint = (grid.path[i].worldPosition - seeker.transform.position);
            //seeker.transform.Translate(directionToWaypoint * 0.2f);

            startFollowingPath = true;

            //seeker.transform.position = Vector3.MoveTowards(seeker.transform.position, grid.path[i].worldPosition, walkingSpeed );

            //if (Vector3.Distance(target.transform.position, seeker.transform.position) <= 0.1f)
            i++;
            yield return null;
        }
    }


    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.graph_x - nodeB.graph_x);
        int dstY = Mathf.Abs(nodeA.graph_y - nodeB.graph_y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
