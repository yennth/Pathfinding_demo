using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // public bool onlyDisplayPathGizmos;
    public bool displayGridGizmos; // Whether to display grid gizmos in the editor
    public Transform player; // Reference to the player's transform (position) 
    public LayerMask unwalkableMask; // Layer mask to determine unwalkable areas
    public Vector2 gridWorldSize;     // Size of the grid in world units //100x100 

    // Radius of each node in the grid
    public float nodeRadius; //define how much space each individual node covers // 0.5
    Node[,] grid;     // 2D array to hold grid nodes // 2D array: node A* used
    float nodeDiameter; // Diameter of each node
    int gridSizeX, gridSizeY;     // Number of nodes along the X and Y axes

    void Awake(){
        nodeDiameter = nodeRadius*2;         // Calculate node diameter

        // Calculate grid size based on world size and node diameter
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter); // number of square
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        
        // Create the grid
        CreatGrid();

    }

    // Property to get the maximum size of the grid
    public int MaxSize{
        get{
            return gridSizeX * gridSizeY;
        }
    }

    // Creates the grid of nodes
    void CreatGrid(){
        // Initialize the grid array
        grid = new Node[gridSizeX, gridSizeY];
 
         // Calculate the bottom left corner of the grid
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

        // Iterate over each node in the grid   
        for(int x = 0; x < gridSizeX; x++){
            for(int y = 0; y < gridSizeY; y++){
                // Calculate the world position of the current node
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                
                // Check if the node is walkable by checking if there are any collisions within the node area
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                
                // Create a new node and add it to the grid array
                grid[x,y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // Returns a list of neighboring nodes for a given node
    public List<Node> GetNeighbours(Node node){
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <=1; x++){
            for (int y = -1; y<= 1; y++){
                if (x == 0 && y == 0){ // Skip the current node
                    continue;
                }

                // Calculate the grid coordinates of the neighboring node
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Check if the coordinates are within the grid bounds
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY){
                    neighbours.Add(grid[checkX, checkY]); // Add the neighboring node to the list
                }
            }
        }
        return neighbours;
    }

    // Converts a world position to a grid node
    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        // Calculate the percentage of the position along each axis
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;

        // Clamp the percentages to ensure they're within the grid bounds
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

        // Calculate the grid coordinates based on the percentages
		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);

        // Return the node at the calculated coordinates
		return grid[x,y];
	}

    // List to store the path nodes
    public List<Node> path;

/*
    void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        // if (onlyDisplayPathGizmos){
        //     if (path != null){
        //         foreach(Node n in path){
        //             Gizmos.color = Color.black;
        //             Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
        //         }
        //     }
        // }

        // else {
        
            if (grid != null && displayGridGizmos){
                Node playerNode = NodeFromWorldPoint(player.position);
                foreach (Node n in grid){
                    Gizmos.color = (n.walkable)?Color.white:Color.red;
                    // Gizmos.color = Color.red;
                    if (playerNode == n){
                        Gizmos.color = Color.cyan;
                    }
                    if (path != null){
                        if (path.Contains(n)){
                            Gizmos.color = Color.black;
                        }
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }

            }
        // }
    }
*/	
    
    void OnDrawGizmos() { // Draws gizmos in the scene view
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

		if (grid != null) {
            Node playerNode = NodeFromWorldPoint(player.position);
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
                if (playerNode == n){
                        Gizmos.color = Color.cyan;
                }
				if (path != null)
					if (path.Contains(n))
						Gizmos.color = Color.black;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
}

