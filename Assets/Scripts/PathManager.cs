using UnityEngine;
using System.Collections;

public class PathManager : MonoBehaviour
{
    // The target the FileManager will try to catch up to
    public Transform target;

    // Reference to the Pathfinding script
    public Pathfinding pathfinding;

    // Coroutine for moving towards the target
    private Coroutine moveCoroutine;

    void Start()
    {
        // Check if Pathfinding component is assigned
        if (pathfinding == null)
        {
            Debug.LogError("Pathfinding component not assigned to FileManager.");
            return;
        }

        // Check if target is assigned
        if (target == null)
        {
            Debug.LogError("Target not assigned to FileManager.");
            return;
        }

        // Start the coroutine to auto-catch up to the target
        moveCoroutine = StartCoroutine(MoveToTarget());
    }

    // Coroutine for continuously moving towards the target
    IEnumerator MoveToTarget()
    {
        // Run indefinitely
        while (true)
        {
            // If there's no path or the path is empty, find a new path
            if (pathfinding.grid.path == null || pathfinding.grid.path.Count == 0)
            {
                pathfinding.FindPath(transform.position, target.position);
            }
            else
            {
                // Follow the path
                foreach (Node pathNode in pathfinding.grid.path)
                {
                    // Move towards the next node in the path
                    while (Vector3.Distance(transform.position, pathNode.worldPosition) > 0.1f)
                    {
                        Vector3 moveDir = (pathNode.worldPosition - transform.position).normalized;
                        transform.position += moveDir * 6f * Time.deltaTime;
                        yield return null; // Wait for the next frame
                    }
                }

                // Clear the path
                pathfinding.grid.path.Clear();
            }

            yield return null; // Wait for the next frame
        }
    }

    void OnDisable()
    {
        // Stop the coroutine when the object is disabled
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
    }
}
