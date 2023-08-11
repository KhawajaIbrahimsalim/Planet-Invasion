using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Store each of the vertex information in a triangle
public struct TrianglePoly
{
    public int triA, triB, triC;

    public TrianglePoly(int triA, int triB, int triC)
    {
        this.triA = triA;
        this.triB = triB;
        this.triC = triC;
    }
}

//Store the outer coordinates of each planet piece
public struct OutlineVector
{
    public Vector3 triA, triB, triC;

    public OutlineVector(Vector3 triA, Vector3 triB, Vector3 triC)
    {
        this.triA = triA;
        this.triB = triB;
        this.triC = triC;
    }
}

public class GenerateIcosphere : MonoBehaviour {
     /*
     * This script records the creation of Icosphere-based mesh information
     * and the range position information for each piece
     */

    protected List<List<TrianglePoly>> generateSurface;
    protected List<TrianglePoly> trianglePoly;
    protected List<Vector3> vertexes;
    protected List<OutlineVector> outlinePositions;

    //Create a Icosahedron
    protected void InitIcosahedron()
    {
        trianglePoly = new List<TrianglePoly>();
        vertexes = new List<Vector3>();

        float r = (1f + Mathf.Sqrt(5f)) / 2f;

        vertexes.Add(new Vector3(-1f, r, 0f).normalized);
        vertexes.Add(new Vector3(1f, r, 0f).normalized);
        vertexes.Add(new Vector3(-1f, -r, 0f).normalized);
        vertexes.Add(new Vector3(1f, -r, 0f).normalized);
        vertexes.Add(new Vector3(0f, -1f, r).normalized);
        vertexes.Add(new Vector3(0f, 1f, r).normalized);
        vertexes.Add(new Vector3(0f, -1f, -r).normalized);
        vertexes.Add(new Vector3(0f, 1f, -r).normalized);
        vertexes.Add(new Vector3(r, 0f, -1f).normalized);
        vertexes.Add(new Vector3(r, 0f, 1f).normalized);
        vertexes.Add(new Vector3(-r, 0f, -1f).normalized);
        vertexes.Add(new Vector3(-r, 0f, 1f).normalized);

        trianglePoly.Add(new TrianglePoly(0, 11, 5));
        trianglePoly.Add(new TrianglePoly(0, 5, 1));
        trianglePoly.Add(new TrianglePoly(0, 1, 7));
        trianglePoly.Add(new TrianglePoly(0, 7, 10));
        trianglePoly.Add(new TrianglePoly(0, 10, 11));
        trianglePoly.Add(new TrianglePoly(1, 5, 9));
        trianglePoly.Add(new TrianglePoly(5, 11, 4));
        trianglePoly.Add(new TrianglePoly(11, 10, 2));
        trianglePoly.Add(new TrianglePoly(10, 7, 6));
        trianglePoly.Add(new TrianglePoly(7, 1, 8));
        trianglePoly.Add(new TrianglePoly(3, 9, 4));
        trianglePoly.Add(new TrianglePoly(3, 4, 2));
        trianglePoly.Add(new TrianglePoly(3, 2, 6));
        trianglePoly.Add(new TrianglePoly(3, 6, 8));
        trianglePoly.Add(new TrianglePoly(3, 8, 9));
        trianglePoly.Add(new TrianglePoly(4, 9, 5));
        trianglePoly.Add(new TrianglePoly(2, 4, 11));
        trianglePoly.Add(new TrianglePoly(6, 2, 10));
        trianglePoly.Add(new TrianglePoly(8, 6, 7));
        trianglePoly.Add(new TrianglePoly(9, 8, 1));
    }

    //Divide each side of the triangle in half
    private int GetMiddlePointVertexesIndex(Dictionary<int, int> cache, int key1, int key2)
    {
        int retVerticeIndex;

        int sKey = Mathf.Min(key1, key2);
        int lKey = Mathf.Max(key1, key2);
        int key = (sKey << 16) + lKey;

        if (cache.TryGetValue(key, out retVerticeIndex))
        {
            return retVerticeIndex;
        }

        Vector3 vPoint1 = vertexes[key1];
        Vector3 vPoint2 = vertexes[key2];
        Vector3 middlePoint = Vector3.Lerp(vPoint1, vPoint2, .5f).normalized;

        retVerticeIndex = vertexes.Count;
        vertexes.Add(middlePoint);

        cache.Add(key, retVerticeIndex);

        return retVerticeIndex;
    }

    //Distribute each triangle as much detail as the desired sphere,
    //and position each planet piece by the range
    //divCount : sphere detail, generateMapRange : planet piece by the range
    protected void SubdividedTriangles(int divCount, int generateMapRange)
    {
        generateSurface = new List<List<TrianglePoly>>();
        outlinePositions = new List<OutlineVector>();
        Dictionary<int, int> midPointCache = new Dictionary<int, int>();

        for (int i = 0; i <= divCount; i++)
        {
            if (i <= generateMapRange)
            {
                List<TrianglePoly> newTriPoly = new List<TrianglePoly>();

                foreach (TrianglePoly tri in trianglePoly)
                {
                    int a = tri.triA;
                    int b = tri.triB;
                    int c = tri.triC;

                    int ab = GetMiddlePointVertexesIndex(midPointCache, a, b);
                    int bc = GetMiddlePointVertexesIndex(midPointCache, b, c);
                    int ca = GetMiddlePointVertexesIndex(midPointCache, c, a);

                    newTriPoly.Add(new TrianglePoly(a, ab, ca));
                    newTriPoly.Add(new TrianglePoly(b, bc, ab));
                    newTriPoly.Add(new TrianglePoly(c, ca, bc));
                    newTriPoly.Add(new TrianglePoly(ab, bc, ca));
                }
                if (i == generateMapRange)
                {
                    foreach (TrianglePoly tri in newTriPoly)
                    {
                        List<TrianglePoly> newSurface = new List<TrianglePoly>();
                        newSurface.Add(tri);
                        generateSurface.Add(newSurface);
                        outlinePositions.Add(new OutlineVector(
                            vertexes[tri.triA],
                            vertexes[tri.triB],
                            vertexes[tri.triC]));
                    }
                }
                else
                {
                    trianglePoly = newTriPoly;
                }
            }
            else
            {
                List<List<TrianglePoly>> newGenerateSurface = new List<List<TrianglePoly>>();

                foreach (List<TrianglePoly> polygon in generateSurface)
                {
                    List<TrianglePoly> newPoly = new List<TrianglePoly>();

                    foreach (TrianglePoly tri in polygon)
                    {
                        int a = tri.triA;
                        int b = tri.triB;
                        int c = tri.triC;

                        int ab = GetMiddlePointVertexesIndex(midPointCache, a, b);
                        int bc = GetMiddlePointVertexesIndex(midPointCache, b, c);
                        int ca = GetMiddlePointVertexesIndex(midPointCache, c, a);

                        newPoly.Add(new TrianglePoly(a, ab, ca));
                        newPoly.Add(new TrianglePoly(b, bc, ab));
                        newPoly.Add(new TrianglePoly(c, ca, bc));
                        newPoly.Add(new TrianglePoly(ab, bc, ca));
                    }
                    newGenerateSurface.Add(newPoly);
                }
                generateSurface = newGenerateSurface;
            }
        }
    }
}