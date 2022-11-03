using koljo45.MeshTriangleSeparator;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using Valve.VR;
using Valve.VR.InteractionSystem; // player


[RequireComponent(typeof(MeshFilter))]
public class FruitManager : MonoBehaviour, ITriangleSeparator
{

    [SerializeField]
    private GameObject _sword;
    // [SerializeField]
    private bool sliced = false;
    [SerializeField]
    private Material outMaterial;
    [SerializeField]
    private Material inMaterial;

    private ScoreManager scoreManager;

    private SoundManager soundManager;
    private Player player;
    public float gravityScale = 5.0f;

    /// <see cref="ITriangleSeparator"/>
    [SerializeField]
    private bool convex = false;
    [SerializeField]
    private bool deepScan = true;
    public bool capMesh = true;
    [SerializeField]
    private SeparationMode separationMode = SeparationMode.Smooth;
    [SerializeField]
    private int threads = 4;
    //Number of triangles a new chunk must contain
    public uint minChunkSize;
    public LayerMask slicableLayers;

    private Rigidbody rb;
    private static TMeshTriangleSeparator _slicer;
    Mesh _myMesh;
    //Transform vertex position from this transform to the sword transform
    Matrix4x4 _toSword;

    void Update()
    {
        Vector3 vector3 = new Vector3(0f, (-gravityScale) * Time.deltaTime,0f); 
        
        rb.velocity = rb.velocity + (vector3);
    }

    private void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log("충돌!");
        if (collision.gameObject.CompareTag("fruitSlicer") && sliced == false)
        {
            
            FruitSlice(collision);
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            DestroyFruit();
            // Destroy(this.GameObject());
        }
    }

    public void DestroyFruit()
    {
        if (!this.sliced) 
        {
            // scoreManager.Miss_Score = scoreManager.Miss_Score - 1; 
            // scoreManager.Miss_Score--; // 감점 로직 구현 - 못 잘랐음
            // UnityEngine.Debug.Log("Score : " + score);
            scoreManager.MinusLife();
        }
        else
        {
            // 잘랐음
            // scoreManager.Slice_Score++; // 잘랐음
            // scoreManager.Slice_Score = scoreManager.Slice_Score + 1;
        }
        Destroy(this.GameObject());
    }


    private void FruitSlice(Collision collision)
    {
        //UnityEngine.Debug.Log("Fruit Sliced");
        //transform matrix update
        _toSword = collision.transform.worldToLocalMatrix * transform.localToWorldMatrix;
        //only slice objects in front of the sword
        //RaycastHit hit;
        //Physics.Raycast(collision.transform.position, collision.transform.right, out hit, 100, slicableLayers);
        //UnityEngine.Debug.Log("player.position = " + player.transform.position);
        //UnityEngine.Debug.Log("hit.position : " + hit.transform.position);
        //UnityEngine.Debug.Log("hit.position : " + hit.transform.position);
        //UnityEngine.Debug.Log("hit.distance : " + hit.distance);
        //UnityEngine.Debug.Log("hit.tag : " + hit.collider.tag);
        //UnityEngine.Debug.Log("sword pos : " + collision.transform.position);
        //UnityEngine.Debug.Log("sword right : " + collision.transform.right);
        //if (hit.collider != GetComponent<Collider>()) return;
        //we have to do this because we are sharing a single slicer instance between all gameobjects, they "take turns"...
        _slicer.setTriangleSeparator(this);
        Stopwatch s1, s2, s3;

        s1 = Stopwatch.StartNew();
        s2 = new Stopwatch();
        s3 = new Stopwatch();
        //Chunks formed out of neighbouring triangles. No two chunks are interconnected
        List<Chunk>[] chunks;
        Mesh copy = null;
        if (_slicer.divideMesh(_myMesh, out chunks, out copy))
        {
            s1.Stop();
            //make sure we have another submesh for the cap
            if (!(copy.subMeshCount > 1))
            {
                copy.subMeshCount = copy.subMeshCount + 1;
                copy.SetTriangles(new int[] { 0, 0, 0 }, copy.subMeshCount - 1);
            }
            MeshChunkExtractor extractor = MeshChunkExtractor.CreateInstance(copy);
            for (int w = 0; w < 2; w++)
                foreach (Chunk chunk in chunks[w])
                {
                    if (chunk.chunk.Count <= minChunkSize) continue;
                    Mesh cm;

                    s2.Start();
                    Dictionary<int, int> reindex = extractor.extractChunk(chunk, out cm);
                    if (capMesh)
                        foreach (List<int> ed in chunk.edges)
                        {
                            try
                            {
                                List<int> edReindexed = MeshCalc.translateIndices(ed, reindex);
                                MeshChunkExtractor.capMesh(cm, edReindexed, 1);
                            }
                            catch (KeyNotFoundException)
                            {
                                UnityEngine.Debug.LogError("Mesh could not be caped with given edges");
                            }
                            catch (System.Exception e)
                            {
                                UnityEngine.Debug.LogError(e);
                            }
                        }
                    s2.Stop();
                    s3.Start();

                    GameObject go = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;

                    //go.GetComponent<ChopHandeler>().enabled = false;
                    go.GetComponentInChildren<MeshFilter>().sharedMesh = cm;

                    //BoxCollider col = go.GetComponent<BoxCollider>();
                    //if (col == null) col = go.AddComponent<BoxCollider>();
                    //col.size = chunk.bounds.size;
                    //col.center = chunk.bounds.center;

                    FruitManager fm = go.GetComponent<FruitManager>();
                    fm.sliced = true;

                    Collider col = go.GetComponent<Collider>();
                    MeshCollider mCol;
                    if (col as MeshCollider == null)
                    {
                        if (col != null)
                            Destroy(col);
                        mCol = go.AddComponent<MeshCollider>();
                    }
                    else
                        mCol = col as MeshCollider;

                    mCol.convex = true;
                    mCol.sharedMesh = cm;
                    if (go.GetComponent<Rigidbody>() == null) go.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                    Rigidbody parentRB = GetComponent<Rigidbody>();
                    if (parentRB != null)
                        go.GetComponent<Rigidbody>().velocity = parentRB.velocity;
                    s3.Stop();

                    // DestroyFruit(go);
                }
            //if(_audioSource != null)
            //{
            //    UnityEngine.Debug.Log("played!");
            //    _audioSource.Play();
            //}
            soundManager.AudioPlay();
            scoreManager.PlusScore();
            UnityEngine.Debug.Log("SUCCESS!");
            //UnityEngine.Debug.Log("Division: " + s1.ElapsedMilliseconds);
            //UnityEngine.Debug.Log("Extraction: " + s2.ElapsedMilliseconds);
            //UnityEngine.Debug.Log("Instantiation: " + s3.ElapsedMilliseconds);
            Destroy(gameObject);
        }
        else UnityEngine.Debug.Log("FAIL!");
    }

    void Awake()
    {
        //sharing a single slicer instance
        if (_slicer == null)
            _slicer = new TMeshTriangleSeparator(this);
    }

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        scoreManager = FindObjectOfType<ScoreManager>(); // scoreManager = GetComponent<ScoreManager>();
        soundManager = FindObjectOfType<SoundManager>();
        _myMesh = GetComponent<MeshFilter>().sharedMesh;
        player = FindObjectOfType<Player>();
        if (sliced)
        {
            MeshRenderer mr = this.GetComponent<MeshRenderer>();
            Material[] temp = { outMaterial, inMaterial };
            mr.materials = temp;
            //fruitSlice layer
            gameObject.layer = 8;
        }
    }

    public bool Convex
    {
        get
        {
            return convex;
        }

        set
        {
            convex = value;
        }
    }

    public int NumThreads
    {
        get
        {
            return threads;
        }

        set
        {
            threads = value;
        }
    }

    public bool DeepScan
    {
        get
        {
            return deepScan;
        }

        set
        {
            deepScan = value;
        }
    }

    public float DuplicateVertexOffset
    {
        get
        {
            return 0.1f;
        }
        set
        {

        }
    }

    public SeparationMode SeparationMode
    {
        get
        {
            return separationMode;
        }

        set
        {
            separationMode = value;
        }
    }

    public bool vertexSetCheck(Vector3 p)
    {
        return _toSword.MultiplyPoint3x4(p).z < 0;
    }

    public bool vertexSetCheck(Vector3 p, BoneWeight b)
    {
        return vertexSetCheck(p);
    }

    public bool vertexSetCheck(Vector3 p, Color32 c)
    {
        return vertexSetCheck(p);
    }

    public bool vertexSetCheck(Vector3 p, BoneWeight b, Color32 c)
    {
        return vertexSetCheck(p);
    }

    public Vector3 vertexFunction(Vector3 p1, Vector3 p2)
    {
        Vector3 p = _toSword.MultiplyPoint3x4(p1);
        Vector3 v = _toSword.MultiplyVector(p2 - p1);

        Vector3 result = new Vector3(p.x - (v.x / v.z) * p.z, p.y - (v.y / v.z) * p.z, 0);
        Vector3 inverse = _toSword.inverse.MultiplyPoint3x4(result);

        return inverse;
    }

}