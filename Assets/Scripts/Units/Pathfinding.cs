using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{
    public static List<Vertex> CalculatePath(ref Graph graph, Vertex source, Vertex end)
    {
        foreach (Vertex node in graph.vertices)
        {
            node.prevNode = null;
            node.hScore = Vector3.Distance(node.position, end.position);
            node.fScore = float.MaxValue;
        }

        source.gScore = 0f;
        source.fScore = source.hScore;


        List<Vertex> openSet = new List<Vertex>();
        List<Vertex> closedSet = new List<Vertex>();

        openSet.Add(source);
        Vertex current = openSet[0];
        bool test = false;

        while (!test)
        {
            Debug.Log(openSet.Contains(end));
            Debug.Log(closedSet.Contains(end));
            foreach (Edge edgeCnn in current.connections)
            {
                Vertex adjNode = edgeCnn.vertexB;
                if (adjNode == end)
                {
                    adjNode.prevNode = current;
                    test = true;
                    //openSet.Clear();
                    Debug.Log("Path found");
                    break;
                }
                if (!closedSet.Contains(adjNode) && !openSet.Contains(adjNode))
                {
                    openSet.Add(adjNode);
                    //float newGScore = current.gScore + edgeCnn.cost;
                    float newFScore = adjNode.gScore + adjNode.hScore;
                    // Debug.Log($"gscore: {newGScore}");
                    // Debug.Log($"fscore: {newFScore}");
                    if (newFScore < adjNode.fScore)
                    {
                        //adjNode.gScore = newGScore;
                        adjNode.fScore = newFScore;

                        adjNode.prevNode = current;
                    }
                }
            }
            closedSet.Add(current);
            openSet = openSet.OrderBy(x => x.fScore).ToList();
            current = openSet[0];
            openSet.Remove(current);
        }
        List<Vertex> shortestPath = new List<Vertex>();
        shortestPath.Add(end);
        BuildShortestPath(shortestPath, end);
        shortestPath.Reverse();
        Debug.Log($"Path length: {shortestPath.Count}");
        return shortestPath;
    }


    private static void BuildShortestPath(List<Vertex> list, Vertex node)
    {
        if (node.prevNode == null)
            return;

        if (node.prevNode == node)
        {
            Debug.Log("Recursion detected");
            return;
        }

        list.Add(node.prevNode);
        BuildShortestPath(list, node.prevNode);
    }
}