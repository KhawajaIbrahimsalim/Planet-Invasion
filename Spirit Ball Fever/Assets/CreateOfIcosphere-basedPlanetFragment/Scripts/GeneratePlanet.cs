using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlanet : GenerateIcosphere {
    /*
     * Inheritates GenerateIcosphere to create a planet and
     * also to create a reference position pivot object for each piece.
     * The shader provided by specifying color information for each Vertex allows the planet to be monochrome,
     * and the texture problems that appear in icosphere have also been corrected to enable material application. 
     */ 

    [Header("GeneratePlanetSetting")]
    public int generateMapRange = 0;
    public int planetDetail = 1;
    public float planetScale = 5f;

    [Header("PivotObjectSetting")]
    public bool useCenterPivotObject = false;
    public bool useOutlinePivotObject = false;
    public bool OutlinePivotObjectOverlapCorrect = false;

    [Header("TextureSetting")]
    public Material planetMaterial;
    public Color32 color1 = new Color32(20, 148, 0, 255);
    public Color32 color2 = new Color32(148, 128, 0, 255);

    private List<GameObject> planet;
    
    //Perform all functions of this script
    //1. Checks whether a planet is already created or has a vaild value
    //2. Create an icosphere-based mesh and store planetary fragment information
    //3, Create a map planet based on 2. It also modifies texture coordinates
    //4. Create a pivot object based on each piece based on user selection
    //5. Resizes and position the planets
    public void _GeneratePlanet()
    {
        if(generateMapRange >= planetDetail)
        {
            Debug.LogError("GenerateMapRange value must be less than the PlanetDetail value!");
            return;
        }

        if(planet != null)
        {
            foreach(GameObject piece in planet)
            {
                DestroyImmediate(piece);
            }
            planet.Clear();
        }

        base.InitIcosahedron();
        base.SubdividedTriangles(planetDetail, generateMapRange);

        CreatePlanet();

        if (useCenterPivotObject) GeneratePieceCenterPivotObject();
        if (useOutlinePivotObject) GeneratePieceOutlinePivotObject();

        gameObject.transform.localScale = Vector3.one * planetScale;
        gameObject.transform.position = Vector3.zero;
    }

    //Create a planet based on each piece information
    private void CreatePlanet()
    {
        planet = new List<GameObject>();

        for(int i = 0; i < base.generateSurface.Count; i++)
        {
            GameObject piece = new GameObject();
            MeshRenderer meshRenderer = piece.AddComponent<MeshRenderer>();
            meshRenderer.material = planetMaterial;

            MeshFilter meshFilter = piece.AddComponent<MeshFilter>();
            meshFilter.mesh = GenerateMesh(base.generateSurface[i]);

            MeshCollider meshCollider = piece.AddComponent<MeshCollider>();
            meshCollider.convex = true;

            piece.transform.parent = gameObject.transform;
            piece.name = "PlanetSurface_" + i.ToString();
            piece.tag = gameObject.transform.tag;
            planet.Add(piece);
        }
    }

    //Generates a mesh based on stored vertex information
    //It also modifies each vertex color information and uv coordinates
    private Mesh GenerateMesh(List<TrianglePoly> piecePoly)
    {
        Mesh mesh = new Mesh();
        int vertexesCount = piecePoly.Count * 3;
        int[] indies = new int[vertexesCount];
        Vector3[] vertexes = new Vector3[vertexesCount];
        Vector3[] normals = new Vector3[vertexesCount];
        Color32[] colors = new Color32[vertexesCount];
        Color32 groundColor = Color32.Lerp(color1, color2, Random.Range(0f, 1f));
        
        for(int i = 0; i < piecePoly.Count; i++)
        {
            TrianglePoly tri = piecePoly[i];
            
            indies[i * 3 + 0] = i * 3 + 0;
            indies[i * 3 + 1] = i * 3 + 1;
            indies[i * 3 + 2] = i * 3 + 2;

            vertexes[i * 3 + 0] = base.vertexes[tri.triA];
            vertexes[i * 3 + 1] = base.vertexes[tri.triB];
            vertexes[i * 3 + 2] = base.vertexes[tri.triC];

            normals[i * 3 + 0] = base.vertexes[tri.triA];
            normals[i * 3 + 1] = base.vertexes[tri.triB];
            normals[i * 3 + 2] = base.vertexes[tri.triC];

            colors[i * 3 + 0] = groundColor;
            colors[i * 3 + 1] = groundColor;
            colors[i * 3 + 2] = groundColor;
        }

        mesh.vertices = vertexes;
        mesh.normals = normals;
        mesh.colors32 = colors;

        Vector2[] uv = CreateUV(vertexes);
        mesh.uv = uv;

        mesh.SetTriangles(indies, 0);

        return mesh;
    }

    //Spherical mapping calculation for point given on sphere
    private Vector2[] CreateUV(Vector3[] vertexes)
    {
        Vector2[] newUV = new Vector2[vertexes.Length];

        for(int i = 0; i < vertexes.Length; i++)
        {
            Vector2 textureCoord;
            textureCoord.x = 0.5f + Mathf.Atan2(vertexes[i].x, vertexes[i].z) / (2f * Mathf.PI);
            textureCoord.y = 0.5f - Mathf.Asin(vertexes[i].y) / Mathf.PI;
            newUV[i] = textureCoord;
        }

        FixWrappedUV(newUV);

        return newUV;
    }

    //Modify Flipping piece and Fixing Polarity Sharing
    private void FixWrappedUV(Vector2[] uv)
    {
        List<int> FixUVVerticesIndex = new List<int>();

        for (int i = 0; i < uv.Length / 3; i++)
        {
            Vector3 texA = new Vector3(uv[i * 3 + 0].x,
                                       uv[i * 3 + 0].y,
                                       0);
            Vector3 texB = new Vector3(uv[i * 3 + 1].x,
                                       uv[i * 3 + 1].y,
                                       0);
            Vector3 texC = new Vector3(uv[i * 3 + 2].x,
                                       uv[i * 3 + 2].y,
                                       0);
            Vector3 texNormal = Vector3.Cross(texB - texA, texC - texA);

            if (texNormal.z > 0)
            {
                FixUVVerticesIndex.Add(i);
            }
        }

        FixZipperUV(FixUVVerticesIndex, uv);
        FixPoleUV(uv);
    }

    //Fix the flipping piece to solve the zipper problem
    private void FixZipperUV(List<int> fixUVIndex, Vector2[] uv)
    {
        foreach (int index in fixUVIndex)
        {
            if (uv[index * 3 + 0].x < 0.25f)
            {
                Vector2 tempUVA = uv[index * 3 + 0];
                tempUVA.x += 1f;
                uv[index * 3 + 0] = tempUVA;
            }
            if (uv[index * 3 + 1].x < 0.25f)
            {
                Vector2 tempUVB = uv[index * 3 + 1];
                tempUVB.x += 1f;
                uv[index * 3 + 1] = tempUVB;
            }
            if (uv[index * 3 + 2].x < 0.25f)
            {
                Vector2 tempUVC = uv[index * 3 + 2];
                tempUVC.x += 1f;
                uv[index * 3 + 2] = tempUVC;
            }
        }
    }

    //Troubleshoot the polar sharing
    private void FixPoleUV(Vector2[] uv)
    {
        for (int i = 0; i < uv.Length / 3; i++)
        {
            List<Vector2> triVtx = new List<Vector2>();

            triVtx.Add(uv[i * 3 + 0]);
            triVtx.Add(uv[i * 3 + 1]);
            triVtx.Add(uv[i * 3 + 2]);

            for (int j = 0; j < triVtx.Count; j++)
            {
                if (triVtx[j].y == 1)
                {
                    List<Vector2> newTriVtx = new List<Vector2>(triVtx);
                    Vector2 newNorth = newTriVtx[j];
                    newTriVtx.RemoveAt(j);
                    newNorth.x = (newTriVtx[0].x + newTriVtx[1].x) / 2;
                    uv[i * 3 + j] = newNorth;
                }
                if (triVtx[j].y == 0)
                {
                    List<Vector2> newTriVtx = new List<Vector2>(triVtx);
                    Vector2 newSouth = newTriVtx[j];
                    newTriVtx.RemoveAt(j);
                    newSouth.x = (newTriVtx[0].x + newTriVtx[1].x) / 2;
                    uv[i * 3 + j] = newSouth;
                }
            }
        }
    }

    //Create OutlinePivot Object. Choose to prevent pivot objects from being created in the same position.
    private void GeneratePieceOutlinePivotObject()
    {
        Dictionary<float, Vector3> cache = new Dictionary<float, Vector3>();

        for(int i = 0; i < planet.Count; i++)
        {
            Quaternion rot;
            initObjectPosition(planet[i].transform, base.outlinePositions[i], out rot);

            if (OutlinePivotObjectOverlapCorrect)
            {
                float outlineObjectKey;

                outlineObjectKey = (base.outlinePositions[i].triA.x + 16) * (base.outlinePositions[i].triA.y + 15) * (base.outlinePositions[i].triA.z + 14);

                if (!cache.ContainsKey(outlineObjectKey))
                {
                    GameObject outlineObjA = new GameObject("OutlinePivotA");
                    outlineObjA.transform.position = base.outlinePositions[i].triA;
                    outlineObjA.transform.rotation = rot;
                    outlineObjA.transform.parent = planet[i].transform;
                    cache.Add(outlineObjectKey, base.outlinePositions[i].triA);
                }

                outlineObjectKey = (base.outlinePositions[i].triB.x + 16) * (base.outlinePositions[i].triB.y + 15) * (base.outlinePositions[i].triB.z + 14);

                if (!cache.ContainsKey(outlineObjectKey))
                {
                    GameObject outlineObjB = new GameObject("OutlinePivotB");
                    outlineObjB.transform.position = base.outlinePositions[i].triB;
                    outlineObjB.transform.rotation = rot;
                    outlineObjB.transform.parent = planet[i].transform;
                    cache.Add(outlineObjectKey, base.outlinePositions[i].triB);
                }

                outlineObjectKey = (base.outlinePositions[i].triC.x + 16) * (base.outlinePositions[i].triC.y + 15) * (base.outlinePositions[i].triC.z + 14);

                if (!cache.ContainsKey(outlineObjectKey))
                {
                    GameObject outlineObjC = new GameObject("OutlinePivotC");
                    outlineObjC.transform.position = base.outlinePositions[i].triC;
                    outlineObjC.transform.rotation = rot;
                    outlineObjC.transform.parent = planet[i].transform;
                    cache.Add(outlineObjectKey, base.outlinePositions[i].triC);
                }
            }
            else
            {
                GameObject outlineObjA = new GameObject("OutlinePivotA");
                outlineObjA.transform.position = base.outlinePositions[i].triA;
                outlineObjA.transform.rotation = rot;
                outlineObjA.transform.parent = planet[i].transform;

                GameObject outlineObjB = new GameObject("OutlinePivotB");
                outlineObjB.transform.position = base.outlinePositions[i].triB;
                outlineObjB.transform.rotation = rot;
                outlineObjB.transform.parent = planet[i].transform;

                GameObject outlineObjC = new GameObject("OutlinePivotC");
                outlineObjC.transform.position = base.outlinePositions[i].triC;
                outlineObjC.transform.rotation = rot;
                outlineObjC.transform.parent = planet[i].transform;
            }
        }
    }

    //Create Center pivot object
    private void GeneratePieceCenterPivotObject()
    {
        for (int i = 0; i < planet.Count; i++)
        {
            Quaternion rot;
            Vector3 centerPos = initObjectPosition(planet[i].transform, base.outlinePositions[i], out rot);
            GameObject centerPivot = new GameObject("CenterPivot");
            centerPivot.transform.position = centerPos;
            centerPivot.transform.rotation = rot;

            centerPivot.transform.parent = planet[i].transform;
        }
    }

    //Return the center position and the correct angle based on the outline position
    private Vector3 initObjectPosition(Transform objTransform, OutlineVector outLine, out Quaternion rotation)
    {
        Vector3 v1 = outLine.triB - outLine.triA;
        Vector3 v2 = outLine.triC - outLine.triA;

        Vector3 centerNormal = Vector3.Cross(v1, v2).normalized;

        rotation = Quaternion.FromToRotation(objTransform.up, centerNormal) * objTransform.rotation;

        return new Vector3(
            (outLine.triA.x + outLine.triB.x + outLine.triC.x) / 3f,
            (outLine.triA.y + outLine.triB.y + outLine.triC.y) / 3f,
            (outLine.triA.z + outLine.triB.z + outLine.triC.z) / 3f);
    }
}