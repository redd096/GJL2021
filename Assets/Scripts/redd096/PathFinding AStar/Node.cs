namespace redd096
{
    using UnityEngine;

    [System.Serializable]
    public class Node
    {
        #region variables contructor

        public bool isWalkable { get; private set; }
        public Vector3 worldPosition { get; private set; }
        public Vector2Int gridPosition { get; private set; }

        #endregion

        #region variables path finding

        public int gCost { get; set; }                      //distance from start point
        public int hCost { get; set; }                      //distance from end point
        public int fCost => gCost + hCost;                  //sum of G cost and H cost

        //used to retrace path
        public Node parentNode { get; set; }

        #endregion

        public Node(bool isWalkable, Vector3 worldPosition, int x, int y)
        {
            this.isWalkable = isWalkable;
            this.worldPosition = worldPosition;
            this.gridPosition = new Vector2Int(x, y);
        }
    }
}