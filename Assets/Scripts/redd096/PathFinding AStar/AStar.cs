namespace redd096
{
    //https://www.youtube.com/watch?v=mZfyt03LDH4&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=3

    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("redd096/Path Finding A Star/A Star")]
    public class AStar : Singleton<AStar>
    {
        [Header("Default use Find Object of Type")]
        [SerializeField] GridAStar grid = default;

        protected override void SetDefaults()
        {
            base.SetDefaults();

            if (grid == null)
                grid = FindObjectOfType<GridAStar>();
        }

        /// <summary>
        /// Find path from one point to another
        /// </summary>
        public List<Node> FindPath(Vector3 startPosition, Vector3 targetPosition)
        {
            /*
             * OPEN - the set of nodes to be evaluated
             * CLOSE - the set of nodes already evaluated
             * 
             * G cost - distance from start point
             * H cost - distance from end point
             * F cost - sum of G cost and H cost
             */

            //=================================================================

            /*
             * add the start node to OPEN
             * 
             * loop
             *  Current = node in OPEN with the lowest F cost
             *  remove Current from OPEN
             *  add Current to CLOSED
             *  
             * if Current is the target node (path has been found)
             *  return path
             *  
             * foreach Neighbour of the Current node
             *  if Neighbour is not walkable or Neighbour is in CLOSED
             *      skip to the next Neighbour
             *      
             *  if new path to Neighbour is shorter OR Neighbour is not in OPEN
             *      set F cost of Neighbour
             *      set parent of Neighbour to Current
             *      if Neighbour is not in OPEN
             *          add Neighbour to OPEN
             */

            //get nodes from world position
            Node startNode = grid.NodeFromWorldPosition(startPosition);
            Node targetNode = grid.NodeFromWorldPosition(targetPosition);

            List<Node> openList = new List<Node>();             //nodes to be evaluated
            HashSet<Node> closedList = new HashSet<Node>();     //already evaluated

            //add the start node to OPEN
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                //Current = node in OPEN with the lowest F cost
                Node currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    //if F cost is lower or is the same but H cost is lower
                    if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                    {
                        currentNode = openList[i];
                    }
                }

                //remove Current from OPEN and add to CLOSED
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                //path has been found, return it
                if (currentNode == targetNode)
                    return RetracePath(startNode, currentNode);

                //foreach Neighbour of the Current node
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    //if Neighbour is not walkable or is in CLOSED
                    if (!neighbour.isWalkable || closedList.Contains(neighbour))
                        continue;

                    //get distance to Neighbour
                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                    //if new path to Neighbour is shorter or Neighbour is not in OPEN
                    if (newCostToNeighbour < neighbour.gCost || !openList.Contains(neighbour))
                    {
                        //set F cost of Neighbour
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);

                        //set parent of Neighbour to Current
                        neighbour.parentNode = currentNode;

                        //if Neighbour is not in OPEN, add it
                        if (!openList.Contains(neighbour))
                            openList.Add(neighbour);
                    }
                }
            }

            //if there is no path, return null
            return null;
        }

        /// <summary>
        /// Update grid at runtime
        /// </summary>
        public void UpdateGrid()
        {
            grid.UpdateGrid();
        }

        #region private API

        /// <summary>
        /// Calculate distance between 2 nodes
        /// </summary>
        int GetDistance(Node nodeA, Node nodeB)
        {
            /* 
             * 
             * we use 10 from one point to another ( like 1.0 )
             * we use 14 in oblique ( like hypotenuse sqr(1.0 + 1.0) )
             * 
             * so in this example, if we have nodeA at (0,0) and nodeB at (5,2)
             * we move 2 times in oblique to reach y axis, then move 3 times along x axis
             * 14y + 10(x-y)
             * 14 *2 + 10 * (5 - 2)
             * 
             * y
             * 
             * 2  /- - - B
             * 1 / 
             * A 1 2 3 4 5  x
             * 
             */

            //get distance on X and Y
            int distX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int distY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            //if distance on X is greater, move oblique (14) to reach Y axis, then move along X axis (10)
            if (distX > distY)
                return 14 * distY + 10 * (distX - distY);

            //else move oblique (14) to reach X axis, then move along Y axis (10)
            return 14 * distX + 10 * (distY - distX);
        }

        /// <summary>
        /// Retrace path from start to end
        /// </summary>
        List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();

            //start from end waypoint
            Node currentNode = endNode;

            //while not reached start waypoint
            while (currentNode != startNode)
            {
                //add current waypoint and move to next one
                path.Add(currentNode);
                currentNode = currentNode.parentNode;
            }

            //reverse list to get from start to end
            path.Reverse();

            return path;
        }

        #endregion
    }
}