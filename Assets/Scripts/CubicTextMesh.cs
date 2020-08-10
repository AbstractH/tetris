using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CubicTextMesh : MonoBehaviour
{
    public string text;
    public float Scale=1;
    public bool isAnimated = false;
    private List<GameObject> CubesOnSceen;
    public string Text
    {
        get { return text; }
        set
        {
            text = value;
            CreateText();
        }
    }

    public GameObject voxel;
    private VoxelPool pool;
    private static Vector3 r = Vector3.right;
    private static Vector3 u = Vector3.up;
    private static Vector3 d = Vector3.down;
    private Quaternion rotation;
    
    private static int[,] l0 = new int[,]
    {
        {1,1,1},
        {1,0,1},
        {1,0,1},
        {1,1,1},
    }; 
    private static int[,] l1 = new int[,]
    {
        {0,0,1},
        {0,0,1},
        {0,0,1},
        {0,0,1},
    }; 
    private static int[,] l2 = new int[,]
    {
        {0,1,1},
        {0,0,1},
        {0,1,0},
        {1,1,1},
    }; 
    private static int[,] l3 = new int[,]
    {
        {1,1,1},
        {0,1,0},
        {0,0,1},
        {1,1,1},
    }; 
    private static int[,] l4 = new int[,]
    {
        {1,0,0},
        {1,0,1},
        {1,1,1},
        {0,0,1},
    }; 
    private static int[,] l5 = new int[,]
    {
        {0,1,1},
        {0,1,0},
        {0,0,1},
        {1,1,1},
    };  
    private static int[,] l6 = new int[,]
    {
        {1,1,1},
        {1,0,0},
        {1,1,0},
        {1,1,0},
    }; 
    private static int[,] l7 = new int[,]
    {
        {1,1,1},
        {0,0,1},
        {0,0,1},
        {0,0,1},
    }; 
    private static int[,] l8 = new int[,]
    {
        {0,1,1},
        {0,1,1},
        {1,0,1},
        {1,1,1},
    }; 
    private static int[,] l9 = new int[,]
    {
        {0,1,1},
        {0,1,1},
        {0,0,1},
        {1,1,1},
    }; 
    private static int[,] l = new int[,]
    {
        {0,0,0},
        {0,0,0},
        {0,0,0},
        {0,0,0},
    }; 
    private static int[,] lright = new int[,]
    {
        {0,0,0},
        {0,1,0},
        {0,1,1},
        {0,1,0},
    }; 
    private static int[,] lleft = new int[,]
    {
        {0,0,0},
        {0,1,0},
        {1,1,0},
        {0,1,0},
    }; 
    private static int[,] ldown = new int[,]
    {
        {0,0,0},
        {0,0,0},
        {1,1,1},
        {0,1,0},
    }; 
    private static int[,] ldd = new int[,]
    {
        {1,1,1},
        {0,1,0},
        {1,1,1},
        {0,1,0},
    }; 
    private static int[,] lrr = new int[,]
    {
        {0,0,0},
        {1,1,0},
        {0,0,1},
        {0,1,0},
    }; 
    private static int[,] lrl = new int[,]
    {
        {0,0,0},
        {0,1,1},
        {1,0,0},
        {0,1,0},
    }; 
    private static int[,] lA = new int[,]
    {
        {0,1,1},
        {1,0,1},
        {1,1,1},
        {1,0,1},
    }; 
    private static int[,] lD = new int[,]
    {
        {1,1,0},
        {1,0,1},
        {1,0,1},
        {1,1,0},
    }; 
    private static int[,] lS = new int[,]
    {
        {1,1,0},
        {1,1,1},
        {0,0,1},
        {1,1,1},
    }; 
    private static int[,] lE = new int[,]
    {
        {1,1,1},
        {1,1,0},
        {1,0,0},
        {1,1,1},
    }; 
    private static int[,] lQ = new int[,]
    {
        {0,1,0},
        {1,0,1},
        {1,0,1},
        {0,1,1},
    }; 
    private static int[,] lspace = new int[,]
    {
        {0,0,0},
        {0,0,0},
        {1,0,1},
        {1,1,1},
    }; 
    private static int[,] lequal = new int[,]
    {
        {0,0,0},
        {0,1,1},
        {0,0,0},
        {0,1,1},
    };
    private static int[,] lx = new int[,]
    {
        {0,0,0},
        {1,0,1},
        {0,1,0},
        {1,0,1},
    };
    private static int[,] lp = new int[,]
    {
        {0,0,0},
        {1,0,1},
        {1,1,1},
        {0,1,0},
    };
    private static int[,] lC = new int[,]
    {
        
        {0,0,1,1,1,1,1,1,1,1,1,0,0},
        {0,1,1,1,1,1,1,1,1,1,1,1,0},
        {0,1,1,1,1,1,1,1,1,1,1,1,0},
        {1,1,1,0,0,0,1,0,0,0,1,1,1},
        {1,1,0,0,0,0,1,0,0,0,0,1,1},
        {1,1,0,0,0,1,1,1,0,0,0,1,1},
        {0,1,1,1,1,1,1,1,1,1,1,1,0},
        {0,1,1,1,1,1,0,1,1,1,1,1,0},
        {0,0,0,0,1,1,1,1,1,0,0,0,0},
        {0,0,0,0,1,0,1,0,1,0,0,0,0},
        
    };

    private void Awake()
    {
        pool = new VoxelPool(voxel,this.transform);
        this.rotation = this.transform.rotation;
    }

    void Start()
    {
        Text = text;
    }

    void Update()
    {
        if(isAnimated)
            foreach (GameObject v in CubesOnSceen)
                v.transform.localScale = new Vector3(Scale,Scale,Scale);
    }

    private void CreateText()
    {
        pool.ReleaseAll();
        this.transform.rotation = Quaternion.identity;
        CubesOnSceen = new List<GameObject>();
        if (text != null)
        {
            for (int i = 0; i < Text.Length; i++)
            {
                CubesOnSceen.AddRange(BuildLetter(text[i], (Vector3.right * 4) * i));
            }
        }

        this.transform.rotation = this.rotation;
    }
    
    /* m
     * 0 0 -15
     * 100 0 0
     * l
     * 0 -11 10
     * -60 0 0
     * c
     * 5 -30 -10
     * -115 0 0
     * c2
     * 0.5 0.5 -15
     * -25 15 0
     *
     *
     */

    private List<GameObject> BuildLetter(char c, Vector3 p)
    {
        switch (c)
        {
            case '0': return BuildLetterFromArray(p,l0,4,3); break;
            case '1': return BuildLetterFromArray(p,l1,4,3); break;
            case '2': return BuildLetterFromArray(p,l2,4,3); break;
            case '3': return BuildLetterFromArray(p,l3,4,3); break;
            case '4': return BuildLetterFromArray(p,l4,4,3); break;
            case '5': return BuildLetterFromArray(p,l5,4,3); break;
            case '6': return BuildLetterFromArray(p,l6,4,3); break;
            case '7': return BuildLetterFromArray(p,l7,4,3); break;
            case '8': return BuildLetterFromArray(p,l8,4,3); break;
            case '9': return BuildLetterFromArray(p,l9,4,3); break;
            case 'A': return BuildLetterFromArray(p,lA,4,3); break;
            case 'D': return BuildLetterFromArray(p,lD,4,3); break;
            case 'S': return BuildLetterFromArray(p,lS,4,3); break;
            case 'Q': return BuildLetterFromArray(p,lQ,4,3); break;
            case 'E': return BuildLetterFromArray(p,lE,4,3); break;
            case ' ': return BuildLetterFromArray(p,lspace,4,3); break;
            case '>': return BuildLetterFromArray(p,lright,4,3); break;
            case '<': return BuildLetterFromArray(p,lleft,4,3); break;
            case 'd': return BuildLetterFromArray(p,ldown,4,3); break;
            case 'y': return BuildLetterFromArray(p,ldd,4,3); break;
            case '=': return BuildLetterFromArray(p,lequal,4,3); break;
            case 'u': return BuildLetterFromArray(p,lrr,4,3); break;
            case 'i': return BuildLetterFromArray(p,lrl,4,3); break;
            case 'x': return BuildLetterFromArray(p,lx,4,3); break;
            case 'p': return BuildLetterFromArray(p,lp,4,3); break;
            case 'C': return BuildLetterFromArray(p,lC,10,13); break;
            default: return BuildLetterFromArray(p,l,4,3); break;
        }
    }

    private void Build0(Vector3 p)
    {
        GameObject v;
        v = pool.Get(); v.transform.position = p;
        v = pool.Get(); v.transform.position = p+r;
        v = pool.Get(); v.transform.position = p+r*2;
        v = pool.Get(); v.transform.position = p+r*2+u;
        v = pool.Get(); v.transform.position = p+r*2+u*2;
        v = pool.Get(); v.transform.position = p+r*2+u*3;
        v = pool.Get(); v.transform.position = p+r+u*3;
        v = pool.Get(); v.transform.position = p+u*3;
        v = pool.Get(); v.transform.position = p+u*2;
        v = pool.Get(); v.transform.position = p+u;
    }

    private List<GameObject> BuildLetterFromArray(Vector3 p, int[,] l, int height, int width)
    {
        List<GameObject> res = new List<GameObject>();
        for (int i = 0; i < height; i++)
        for (int j = 0; j < width; j++)
            if (l[i, j] == 1)
            {
                GameObject v = pool.Get();
                v.transform.localPosition = p + r * j + d * i;
                v.transform.rotation = Quaternion.identity;
                res.Add(v);
            }

        return res;
    }
    private class VoxelPool
    {
        private GameObject voxel;
        private Transform parent;
        private Stack<GameObject> freeVoxels;
        private List<GameObject> busyVoxels;
        private static int EXTEND_SIZE = 10;
        
        public VoxelPool(GameObject voxel, Transform parent)
        {
            this.voxel = voxel;
            this.parent = parent;
            freeVoxels = new Stack<GameObject>();
            busyVoxels = new List<GameObject>();
            Extend();
        }

        private void Extend()
        {
            for (int i = 0; i < EXTEND_SIZE; i++)
            {
                GameObject v = Instantiate(voxel, Vector3.zero, Quaternion.identity, parent);
                v.SetActive(false);
                freeVoxels.Push(v);
            }
        }

        public void Release(GameObject voxel)
        {
            busyVoxels.Remove(voxel);
            voxel.SetActive(false);
            freeVoxels.Push(voxel);
        }

        public void ReleaseAll()
        {
            for (int i = 0; i < busyVoxels.Count; i++)
            {
                GameObject v = busyVoxels[i];
                v.SetActive(false);
                freeVoxels.Push(v);
            }
            busyVoxels.Clear();
        }

        public GameObject Get()
        {
            if (freeVoxels.Count == 0)
            {
                Extend();
                return Get();
            }
            else
            {
                GameObject v = freeVoxels.Pop();
                v.SetActive(true);
                busyVoxels.Add(v);
                return v;
            }
        }
    }
}
