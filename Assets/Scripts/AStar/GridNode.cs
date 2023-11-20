
using UnityEngine;

public class GridNode
{
    int width;
    int height;

    Node[,] gridNodes;

    public GridNode(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNodes[x,y] = new Node(new Vector2Int(x,y));
            }
        }
    }

    public Node GetGridNode(int xPosition, int yPosition)
    {
        if (xPosition < width && yPosition < height)
        {
            //Debug.Log("宽度为" + width + " " + "长度为" + height);
            //Debug.Log(xPosition + " " + yPosition);
            return gridNodes[xPosition, yPosition];
        }
        else
        {
            Debug.Log("Requested grid node is out of range");
            return null;
        }
    }
        
}
