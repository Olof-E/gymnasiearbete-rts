using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Vertex
{
    public Vector3 position;
    public int index;
    public int numOfCons;
    public List<Edge> connections;
    public Vertex prevNode;
    public float hScore;
    public float gScore;
    public float fScore;
    public bool visited;
    public Vertex(Vector3 _position, int _index)
    {
        position = _position;
        index = _index;
        numOfCons = 0;
    }

    public void incrementCons()
    {
        numOfCons += 1;
    }
}

public class Edge
{
    public Vertex vertexA;
    public Vertex vertexB;
    public float cost;
    public bool isRandomEdge = false;
    public Edge(Vertex _vertexA = null, Vertex _vertexB = null)
    {
        vertexA = _vertexA;
        vertexB = _vertexB;
    }

    #region Helper Functions
    public bool IsNull()
    {
        return vertexA == null && vertexB == null;
    }

    // Given three collinear Vector3s p, q, r, the function checks if
    // Vector3 q lies on line segment 'pr'
    private bool OnSegment(Vector3 p, Vector3 q, Vector3 r)
    {
        // Vector3 n = Vector3.Cross(r - p, q - p);
        // return n.magnitude < (r - p).magnitude;
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
            q.z <= Mathf.Max(p.z, r.z) && q.z >= Mathf.Min(p.z, r.z))
            return true;

        return false;
    }

    private bool IsParallel(Edge other)
    {
        return (vertexA.position.z / vertexB.position.x) == (other.vertexA.position.z / other.vertexB.position.x);
    }

    // To find orientation of ordered triplet (p, q, r).
    // The function returns following values
    // 0 --> p, q and r are collinear
    // 1 --> Clockwise
    // 2 --> Counterclockwise
    private int Orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        Vector3 v0 = q - p;
        Vector3 v1 = r - q;
        Vector3 n = Vector3.Cross(v0, v1);
        // if (Mathf.Abs(n.y) <= 1e-01)
        // {
        //     return 0;
        // };
        return n.y > 0f ? 1 : 2;
    }

    public bool Intersect(Edge other)
    {
        Vector3 p1 = vertexA.position;
        Vector3 q1 = vertexB.position;

        Vector3 p2 = other.vertexA.position;
        Vector3 q2 = other.vertexB.position;

        // Find the four orientations needed for general and
        // special cases
        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        // General case
        if (o1 != o2 && o3 != o4)
            return true;




        if (Mathf.Abs(Vector3.Dot((p1 - q1).normalized, (q2 - p2).normalized)) > 0.9f)
        {
            // Special Cases
            // p1, q1 and p2 are collinear and p2 lies on segment p1q1
            if (OnSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are collinear and q2 lies on segment p1q1
            if (OnSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are collinear and p1 lies on segment p2q2
            if (OnSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are collinear and q1 lies on segment p2q2
            if (OnSegment(p2, q1, q2)) return true;
        };


        return false;
    }

    public override int GetHashCode()
    {
        return (vertexA, vertexB).GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return (this.vertexA == ((Edge)obj).vertexA && this.vertexB == ((Edge)obj).vertexB) ||
        (this.vertexB == ((Edge)obj).vertexA && this.vertexA == ((Edge)obj).vertexB);
    }
    #endregion
}

public class Graph
{
    public Vertex[] vertices { get; private set; }
    public Edge[] edges { get; private set; }

    public void Initialize(int verticesCount, int mapSeed)
    {
        vertices = new Vertex[verticesCount];
        Random.InitState(mapSeed);
        GenerateGraphPrims();
    }

    //Main function for generating the connected graph
    private void GenerateGraphPrims()
    {

        float[] vertCost = new float[vertices.Length];
        Edge[] cheapestCostEdges = new Edge[vertices.Length];
        //Generate the vertices of the graph
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vertex
            (
                new Vector3(
                    Random.Range(-vertices.Length / 6f, vertices.Length / 6f),
                    0f,
                    Random.Range(-vertices.Length / 6f, vertices.Length / 6f)
                ),
                i
            );
            vertices[i].connections = new List<Edge>();
            vertices[i].visited = false;

            vertCost[i] = float.MaxValue;
            cheapestCostEdges[i] = new Edge();
        }
        List<Vertex> visitedSet = new List<Vertex>();
        List<Edge> tempEdges = new List<Edge>();

        //Generate the minimum spanning tree of the vertices
        while (vertices.Length != visitedSet.Count)
        {
            Vertex currVertex = null;
            if (visitedSet.Count > 0)
            {
                float minCost = float.MaxValue;
                Parallel.For(0, vertices.Length, (int i) =>
                {
                    if (!visitedSet.Contains(vertices[i]))
                    {
                        if (vertCost[i] < minCost)
                        {
                            minCost = vertCost[i];
                            currVertex = vertices[i];
                        }
                    }
                });
            }
            else
            {
                currVertex = vertices[Random.Range(0, vertices.Length)];
            }
            visitedSet.Add(currVertex);

            if (!cheapestCostEdges[currVertex.index].IsNull())
            {
                Edge cheapestEdge = cheapestCostEdges[currVertex.index];
                cheapestEdge.vertexA.incrementCons();
                cheapestEdge.vertexB.incrementCons();

                float edgeCost = Vector3.Distance(cheapestEdge.vertexA.position, cheapestEdge.vertexB.position);

                cheapestEdge.vertexA.connections.Add(cheapestEdge);
                cheapestEdge.vertexA.connections[cheapestEdge.vertexA.connections.Count - 1].cost = edgeCost;

                cheapestEdge.vertexB.connections.Add(
                    new Edge(
                        cheapestEdge.vertexB,
                        cheapestEdge.vertexA
                    ));
                cheapestEdge.vertexB.connections[cheapestEdge.vertexB.connections.Count - 1].cost = edgeCost;

                tempEdges.Add(cheapestEdge);
            }

            Parallel.For(0, vertices.Length, (int i) =>
            {
                Vertex adjVertex = vertices[i];

                if (!visitedSet.Contains(adjVertex))
                {
                    float dist = Vector3.Distance(currVertex.position, adjVertex.position);
                    if (dist < vertCost[i])
                    {
                        vertCost[i] = dist;
                        cheapestCostEdges[i] = new Edge(currVertex, adjVertex);
                    }
                }
            });
        }

        //Generate new random edges that arent already in the graph and add them
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].numOfCons < 3)
            {
                List<Edge> testedEdges = new List<Edge>();
                bool edgeNotFound = true;
                Edge bestEdge = null;
                int iter = 0;
                while (edgeNotFound && iter < 100)
                {
                    iter++;
                    float minDist = float.MaxValue;
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        float dist = Vector3.Distance(vertices[i].position, vertices[j].position);
                        Edge testEdge = new Edge(vertices[i], vertices[j]);

                        if (!tempEdges.Contains(testEdge) && !testedEdges.Contains(testEdge)
                            && vertices[i] != vertices[j] && vertices[j].numOfCons < 3 && Random.Range(0f, 1f) < 0.7f)
                        {
                            if (dist < minDist)
                            {
                                minDist = dist;
                                bestEdge = testEdge;
                            }
                        }
                    };

                    bool validEdge = true;
                    Parallel.For(0, tempEdges.Count, (int j) =>
                    {
                        if (bestEdge.Intersect(tempEdges[j]))
                        {
                            validEdge = false;
                        }
                    });

                    if (validEdge && (bestEdge.vertexA.position - bestEdge.vertexB.position).magnitude < 16f)
                    {
                        edgeNotFound = false;
                    }
                    else
                    {
                        testedEdges.Add(bestEdge);
                    }
                }

                if (!edgeNotFound)
                {
                    bestEdge.vertexA.incrementCons();
                    bestEdge.vertexB.incrementCons();
                    bestEdge.isRandomEdge = true;

                    float edgeCost = Vector3.Distance(bestEdge.vertexA.position, bestEdge.vertexB.position);

                    bestEdge.vertexA.connections.Add(bestEdge);
                    bestEdge.vertexA.connections[bestEdge.vertexA.connections.Count - 1].cost = edgeCost;

                    bestEdge.vertexB.connections.Add(new Edge(bestEdge.vertexB, bestEdge.vertexA));
                    bestEdge.vertexB.connections[bestEdge.vertexB.connections.Count - 1].cost = edgeCost;

                    tempEdges.Add(bestEdge);
                }
            }
        }
        edges = tempEdges.ToArray();
        // for (int i = 0; i < edges.Length; i++)
        // {
        //     edges[i].cost = Vector3.Distance(edges[i].vertexA.position, edges[i].vertexB.position);
        // }
    }
}
