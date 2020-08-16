using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CubicTextMesh : MonoBehaviour
{
    [SerializeField]
    private string text;
    [FormerlySerializedAs("Scale")] [SerializeField]
    private float scale=1;
    [SerializeField]
    private bool isAnimated;
    private List<GameObject> _cubesOnScreen;
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
    private VoxelPool _pool;
    private static readonly Vector3 _R = Vector3.right;
    private static Vector3 _u = Vector3.up;
    private static readonly Vector3 _D = Vector3.down;
    private Quaternion _rotation;
    
    private static readonly int[,] _L0 = new int[,]
    {
        {1,1,1},
        {1,0,1},
        {1,0,1},
        {1,1,1},
    }; 
    private static readonly int[,] _L1 = new int[,]
    {
        {0,0,1},
        {0,0,1},
        {0,0,1},
        {0,0,1},
    }; 
    private static readonly int[,] _L2 = new int[,]
    {
        {0,1,1},
        {0,0,1},
        {0,1,0},
        {1,1,1},
    }; 
    private static readonly int[,] _L3 = new int[,]
    {
        {1,1,1},
        {0,1,0},
        {0,0,1},
        {1,1,1},
    }; 
    private static readonly int[,] _L4 = new int[,]
    {
        {1,0,0},
        {1,0,1},
        {1,1,1},
        {0,0,1},
    }; 
    private static readonly int[,] _L5 = new int[,]
    {
        {0,1,1},
        {0,1,0},
        {0,0,1},
        {1,1,1},
    };  
    private static readonly int[,] _L6 = new int[,]
    {
        {1,1,1},
        {1,0,0},
        {1,1,0},
        {1,1,0},
    }; 
    private static readonly int[,] _L7 = new int[,]
    {
        {1,1,1},
        {0,0,1},
        {0,0,1},
        {0,0,1},
    }; 
    private static readonly int[,] _L8 = new int[,]
    {
        {0,1,1},
        {0,1,1},
        {1,0,1},
        {1,1,1},
    }; 
    private static readonly int[,] _L9 = new int[,]
    {
        {0,1,1},
        {0,1,1},
        {0,0,1},
        {1,1,1},
    }; 
    private static readonly int[,] _L = new int[,]
    {
        {0,0,0},
        {0,0,0},
        {0,0,0},
        {0,0,0},
    }; 
    private static readonly int[,] _LRIGHT = new int[,]
    {
        {0,0,0},
        {0,1,0},
        {0,1,1},
        {0,1,0},
    }; 
    private static readonly int[,] _LLEFT = new int[,]
    {
        {0,0,0},
        {0,1,0},
        {1,1,0},
        {0,1,0},
    }; 
    private static readonly int[,] _LDOWN = new int[,]
    {
        {0,0,0},
        {0,0,0},
        {1,1,1},
        {0,1,0},
    }; 
    private static readonly int[,] _LDD = new int[,]
    {
        {1,1,1},
        {0,1,0},
        {1,1,1},
        {0,1,0},
    }; 
    private static readonly int[,] _LRR = new int[,]
    {
        {0,0,0},
        {1,1,0},
        {0,0,1},
        {0,1,0},
    }; 
    private static readonly int[,] _LRL = new int[,]
    {
        {0,0,0},
        {0,1,1},
        {1,0,0},
        {0,1,0},
    }; 
    private static readonly int[,] _L_A = new int[,]
    {
        {0,1,1},
        {1,0,1},
        {1,1,1},
        {1,0,1},
    }; 
    private static readonly int[,] _L_D = new int[,]
    {
        {1,1,0},
        {1,0,1},
        {1,0,1},
        {1,1,0},
    }; 
    private static readonly int[,] _L_S = new int[,]
    {
        {1,1,0},
        {1,1,1},
        {0,0,1},
        {1,1,1},
    }; 
    private static readonly int[,] _L_E = new int[,]
    {
        {1,1,1},
        {1,1,0},
        {1,0,0},
        {1,1,1},
    }; 
    private static readonly int[,] _L_Q = new int[,]
    {
        {0,1,0},
        {1,0,1},
        {1,0,1},
        {0,1,1},
    }; 
    private static readonly int[,] _LSPACE = new int[,]
    {
        {0,0,0},
        {0,0,0},
        {1,0,1},
        {1,1,1},
    }; 
    private static readonly int[,] _LEQUAL = new int[,]
    {
        {0,0,0},
        {0,1,1},
        {0,0,0},
        {0,1,1},
    };
    private static readonly int[,] _LX = new int[,]
    {
        {0,0,0},
        {1,0,1},
        {0,1,0},
        {1,0,1},
    };
    private static readonly int[,] _LP = new int[,]
    {
        {0,0,0},
        {1,0,1},
        {1,1,1},
        {0,1,0},
    };
    private static readonly int[,] _L_C = new int[,]
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
        Transform t = transform;
        _pool = new VoxelPool(voxel,t);
        _rotation = t.rotation;
    }

    void Start()
    {
        Text = text;
    }

    void Update()
    {
        if(isAnimated)
            foreach (GameObject v in _cubesOnScreen)
                v.transform.localScale = new Vector3(scale,scale,scale);
    }

    private void CreateText()
    {
        Transform t = transform;
        _pool.ReleaseAll();
        t.rotation = Quaternion.identity;
        _cubesOnScreen = new List<GameObject>();
        if (text != null)
        {
            for (int i = 0; i < Text.Length; i++)
            {
                _cubesOnScreen.AddRange(BuildLetter(text[i], Vector3.right * (4 * i)));
            }
        }

        t.rotation = _rotation;
    }

    private List<GameObject> BuildLetter(char c, Vector3 p)
    {
        switch (c)
        {
            case '0': return BuildLetterFromArray(p,_L0,4,3);
            case '1': return BuildLetterFromArray(p,_L1,4,3);
            case '2': return BuildLetterFromArray(p,_L2,4,3);
            case '3': return BuildLetterFromArray(p,_L3,4,3); 
            case '4': return BuildLetterFromArray(p,_L4,4,3); 
            case '5': return BuildLetterFromArray(p,_L5,4,3); 
            case '6': return BuildLetterFromArray(p,_L6,4,3); 
            case '7': return BuildLetterFromArray(p,_L7,4,3); 
            case '8': return BuildLetterFromArray(p,_L8,4,3); 
            case '9': return BuildLetterFromArray(p,_L9,4,3); 
            case 'A': return BuildLetterFromArray(p,_L_A,4,3); 
            case 'D': return BuildLetterFromArray(p,_L_D,4,3); 
            case 'S': return BuildLetterFromArray(p,_L_S,4,3); 
            case 'Q': return BuildLetterFromArray(p,_L_Q,4,3); 
            case 'E': return BuildLetterFromArray(p,_L_E,4,3); 
            case ' ': return BuildLetterFromArray(p,_LSPACE,4,3); 
            case '>': return BuildLetterFromArray(p,_LRIGHT,4,3); 
            case '<': return BuildLetterFromArray(p,_LLEFT,4,3); 
            case 'd': return BuildLetterFromArray(p,_LDOWN,4,3); 
            case 'y': return BuildLetterFromArray(p,_LDD,4,3); 
            case '=': return BuildLetterFromArray(p,_LEQUAL,4,3); 
            case 'u': return BuildLetterFromArray(p,_LRR,4,3); 
            case 'i': return BuildLetterFromArray(p,_LRL,4,3); 
            case 'x': return BuildLetterFromArray(p,_LX,4,3); 
            case 'p': return BuildLetterFromArray(p,_LP,4,3); 
            case 'C': return BuildLetterFromArray(p,_L_C,10,13); 
            default: return BuildLetterFromArray(p,_L,4,3); 
        }
    }

    private List<GameObject> BuildLetterFromArray(Vector3 p, int[,] l, int height, int width)
    {
        List<GameObject> res = new List<GameObject>();
        for (int i = 0; i < height; i++)
        for (int j = 0; j < width; j++)
            if (l[i, j] == 1)
            {
                GameObject v = _pool.Get();
                v.transform.localPosition = p + _R * j + _D * i;
                v.transform.rotation = Quaternion.identity;
                res.Add(v);
            }

        return res;
    }
    private class VoxelPool
    {
        private readonly GameObject _voxel;
        private readonly Transform _parent;
        private readonly Stack<GameObject> _freeVoxels;
        private readonly List<GameObject> _busyVoxels;
        private static readonly int _EXTEND_SIZE = 10;
        
        public VoxelPool(GameObject voxel, Transform parent)
        {
            _voxel = voxel;
            _parent = parent;
            _freeVoxels = new Stack<GameObject>();
            _busyVoxels = new List<GameObject>();
            Extend();
        }

        private void Extend()
        {
            for (int i = 0; i < _EXTEND_SIZE; i++)
            {
                GameObject v = Instantiate(
                    _voxel, 
                    Vector3.zero, 
                    Quaternion.identity, 
                    _parent
                    );
                v.SetActive(false);
                _freeVoxels.Push(v);
            }
        }

        public void Release(GameObject voxel)
        {
            _busyVoxels.Remove(voxel);
            voxel.SetActive(false);
            _freeVoxels.Push(voxel);
        }

        public void ReleaseAll()
        {
            for (int i = 0; i < _busyVoxels.Count; i++)
            {
                GameObject v = _busyVoxels[i];
                v.SetActive(false);
                _freeVoxels.Push(v);
            }
            _busyVoxels.Clear();
        }

        public GameObject Get()
        {
            if (_freeVoxels.Count == 0)
            {
                Extend();
                return Get();
            }
            else
            {
                GameObject v = _freeVoxels.Pop();
                v.SetActive(true);
                _busyVoxels.Add(v);
                return v;
            }
        }
    }
}
