
using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStar 
{
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPostion, Vector3Int endGridPosition)
    {
        startGridPostion -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closedNodeHashSet = new HashSet<Node>();

        GridNode gridNode = new GridNode(room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNode.GetGridNode(startGridPostion.x, startGridPostion.y);
        Node targetNode = gridNode.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNode, openNodeList, closedNodeHashSet, room.instantiateRoom);

        if (endPathNode != null)
        {
            return CreatStackPath(endPathNode, room);
        }

        return null;
    }

    private static Stack<Vector3> CreatStackPath(Node targetNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;

        Vector3 cellMidPoint = room.instantiateRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0;

        while (nextNode != null)
        {
            Vector3 worldPosition = room.instantiateRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPosition.x + room.templateLowerBounds.x
                , nextNode.gridPosition.y + room.templateLowerBounds.y, 0));

            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;

    }

    private static Node FindShortestPath(Node startNode, Node targetNode, GridNode gridNode, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiateRoom instantiateRoom)
    {
        openNodeList.Add(startNode);

        while (openNodeList.Count > 0)
        {
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            if (currentNode == targetNode)
            {
                return currentNode;
            }

            closedNodeHashSet.Add(currentNode);

            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNode, openNodeList, closedNodeHashSet, instantiateRoom);
        }

        return null;
    }

    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNode gridNode, List<Node> openNodeList, HashSet<Node> closedNodeHashSet, InstantiateRoom instantiateRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1 ; y++)
            {
                if(x == 0 && y == 0)
                    continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + x, currentNodeGridPosition.y + y, gridNode, closedNodeHashSet, instantiateRoom);

                if (validNeighbourNode != null)
                {
                    int newCostToNeighbour;

                    int movementPenaltyForGridSpace = instantiateRoom.aStarMovementPenalty[validNeighbourNode.gridPosition.x, validNeighbourNode.gridPosition.y];

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                                
                    }
                }
            }
        }
    }

    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNode gridNode, HashSet<Node> closedNodeHashSet, InstantiateRoom instantiateRoom)
    {
        if (neighbourNodeXPosition >= instantiateRoom.room.templateUpperBounds.x - instantiateRoom.room.templateLowerBounds.x || neighbourNodeXPosition < 0
           || neighbourNodeYPosition >= instantiateRoom.room.templateUpperBounds.y - instantiateRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0)
        {
            return null;
        }

        Node neighbourNode = gridNode.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        int movementPenaltyForGridSpace = instantiateRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        int itemObstacleForGridSpace = instantiateRoom.aStarItemObstacles[neighbourNodeXPosition, neighbourNodeYPosition];


        if (movementPenaltyForGridSpace == 0 || itemObstacleForGridSpace == 0 ||closedNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }else
        {
            return neighbourNode;
        }
    }

    private static int GetDistance(Node NodeA, Node NodeB)
    {
        int dstX = Mathf.Abs(NodeA.gridPosition.x - NodeB.gridPosition.x);
        int dstY = Mathf.Abs(NodeA.gridPosition.y - NodeB.gridPosition.y);

        if(dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
