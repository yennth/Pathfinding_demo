using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

	public Transform seeker, target;
	public Grid grid;
	private int currentPathIndex;

	void Awake() {
		grid = GetComponent<Grid> ();
	}

	void Update() {
		FindPath (seeker.position, target.position);
	}
	// void Update()
    //     {
    //         if (Input.GetMouseButtonDown(1))
    //         {
    //             //还原
    //             currentPathIndex = 0;

    //             FindPath(seeker.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    //         }

    //         if (grid.path != null && grid.path.Count > 0)
    //         {
    //             Vector3 targetPosition = grid.path[currentPathIndex].worldPosition;
    //             if (Vector3.Distance(seeker.transform.position, targetPosition) > 0.1f)
    //             {
    //                 Vector3 moveDir = (targetPosition - seeker.transform.position).normalized;
    //                 seeker.transform.position = seeker.transform.position + moveDir * 6f * Time.deltaTime;
    //             }
    //             else
    //             {
    //                 currentPathIndex++;
    //                 if (currentPathIndex >= grid.path.Count)
    //                 {
    //                     //校准和最后一个点的位置
    //                     seeker.transform.position = grid.path[currentPathIndex - 1].worldPosition;
    //                     //还原
    //                     grid.path.Clear();
    //                     grid.path = null;
    //                     currentPathIndex = 0;
    //                 }
    //             }
    //         }
    //     }

	
	public void FindPath(Vector3 startPos, Vector3 targetPos) {
		Node startNode = grid.NodeFromWorldPoint(startPos); 
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i ++) {
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode) {
				RetracePath(startNode,targetNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(node)) {
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}

	void RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		grid.path = path;

	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
}



/*------------------------------------
If using Heap to smooth
------------------------------------*/

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Diagnostics;
// using System;

// public class Pathfinding : MonoBehaviour
// {
//     // public Transform seeker, target;

//     PathRequestManager requestManager;
//     Grid grid;

//     void Awake(){
//         requestManager = GetComponent<PathRequestManager>();
//         grid = GetComponent<Grid>();
//     }

//     // void Update(){
//     //     if (Input.GetButtonDown("Jump")){
//     //         FindPath(seeker.position, target.position);
//     //     }
        
//     // }

//     public void StartFindPath(Vector3 startPos, Vector3 targetPos){
//         StartCoroutine(FindPath(startPos, targetPos)); 
//     }

//     IEnumerator FindPath(Vector3 startPos, Vector3 targetPos){

//         Stopwatch sw = new Stopwatch();
//         sw.Start();

//         Vector3[] waypoints = new Vector3[0];  
//         bool pathSuccess = false;

//         Node startNode = grid.NodeFromWorldPoint(startPos);
//         Node targetNode = grid.NodeFromWorldPoint(targetPos);

//         if(startNode.walkable && targetNode.walkable){

//             // List<Node> openSet = new List<Node>();
//             Heap<Node> openSet = new Heap<Node>(grid.MaxSize);

//             HashSet<Node> closeSet = new HashSet<Node>();

//             openSet.Add(startNode);

//             while(openSet.Count > 0){
//                 // Node currentNode = openSet[0];
//                 Node currentNode = openSet.RemoveFirst();
//                 // for (int i = 1; i < openSet.Count; i++){
//                 //     if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost){
//                 //         currentNode = openSet[i];
//                 //     }
//                 // }

//                 // openSet.Remove(currentNode);
//                 closeSet.Add(currentNode);

//                 if (currentNode == targetNode){
//                     sw.Stop();
//                     print("Path found: " + sw.ElapsedMilliseconds + "ms");
//                     pathSuccess = true;
//                     // RetracePath(startNode, targetNode);
//                     // return;
//                     break;
//                 }

//                 foreach (Node neighbours in grid.GetNeighbours(currentNode)){
//                     if(!neighbours.walkable || closeSet.Contains(neighbours)){
//                         continue;
//                     }

//                     int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbours);
//                     if (newMovementCostToNeighbour < neighbours.gCost || !openSet.Contains(neighbours)){
//                         neighbours.gCost = newMovementCostToNeighbour;
//                         neighbours.hCost = GetDistance(neighbours, targetNode);
//                         neighbours.parent = currentNode;

//                         if (!openSet.Contains(neighbours)){
//                             openSet.Add(neighbours);
//                         }
//                     }
//                 }
//             }
//         }
//         yield return null;
//         if (pathSuccess){
//             waypoints = RetracePath(startNode, targetNode);
//         }
//         requestManager.FinishedProcessingPath(waypoints, pathSuccess);
//     }
    

//     Vector3[] RetracePath(Node startNode, Node endNode){
//         List<Node> path = new List<Node>();
//         Node currentNode = endNode;

//         while(currentNode != startNode){
//             path.Add(currentNode);
//             currentNode = currentNode.parent;
//         }
//         Vector3[] waypoints = SimplifyPath(path);
//         // path.Reverse();
//         Array.Reverse(waypoints);
//         return waypoints;

//         // grid.path = path;
//     }

//     Vector3[] SimplifyPath(List<Node> path){
//         List<Vector3> waypoints = new List<Vector3>();
//         Vector2 directionOld = Vector2.zero;

//         for (int i = 1; i < path.Count; i++){
//             Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
//             if(directionNew != directionOld){
//                 waypoints.Add(path[i].worldPosition);
//             }
//             directionOld = directionNew;
//         }
//         return waypoints.ToArray();

//     }

//     int GetDistance(Node nodeA, Node nodeB){ // Diagonal distance
//         int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
//         int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

//         if (dstX > dstY){
//             return 14 * dstY + 10 * (dstX - dstY);
//         }
//         return 14 * dstX + 10 * (dstY - dstX);
//     }
// }
