
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    public int columns = 100;
    public int rows = 100;    
    public int depth = 100;
    public int scale = 1;
    int count;
    public int[,,] stage = new int[100, 100, 100];
    public GameObject block;
    public GameObject spikeBlock;
    public GameObject iceBlock;
    public Vector3 leftBottomLocation = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        block = GameObject.Find("Block");
        spikeBlock = GameObject.Find("SpikeBlock");
        iceBlock = GameObject.Find("IceBlock");
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int i = 50; i < 62; i++) {
            int count = 0;
            for (int j = 50; j < 60; j++) {
                for (int k = 50+count*2; k < 60; k++) {
                    CreateBlock(i, j, k);
                }
                count++;
            }
        }

        for (int i = 50; i < 60; i++) {
            for (int j = 50; j < 57; j++) {
                for (int k = 60; k < 61; k++) {
                    CreateBlock(i, j, k);
                }
            }
        }
/*
        CreateBlock(55, 56, 61);
        CreateBlock(54, 57, 61);
        CreateBlock(56, 57, 61);
        CreateBlock(53, 58, 61);
        CreateBlock(55, 58, 61);
        CreateBlock(57, 58, 61);
        CreateBlock(56, 59, 61);
        CreateBlock(54, 59, 61);
        CreateBlock(55, 60, 61);*/

        for (int i=0; i<5; i++) {
            int cont = 0;
            for (int k=55-i; k <= 55+i; k++) {
                if (cont%2 == 0) {}
                    CreateBlock(k, i+55, 61);
                cont++;
            }
        }

        for (int i=0; i<4; i++) {
            int cont = 0;
            for (int k=55-i; k <= 55+i; k++) {
                if (cont%2 == 0) {}
                    CreateBlock(k, 55+8-i, 61);                
                cont++;
            }
        }

        for (int i=0; i<3; i++) {
            int cont = 0;
            for (int k=55-i; k <= 55+i; k++) {
                if (cont%2 == 0) {}
                    RemoveBlock(k, i+57, 61);
                cont++;
            }
        }

        for (int i=0; i<2; i++) {
            int cont = 0;
            for (int k=55-i; k <= 55+i; k++) {
                if (cont%2 == 0) {}
                    RemoveBlock(k, 57+4-i, 61);                
                cont++;
            }
        }

        for (int i = 53; i < 57; i++) {
            for (int j = 50; j < 56; j++) {
                for (int k = 53; k < 57; k++) {
                    CreateBlock(i, j, k);
                }
            }
        }
        for (int i = 53; i < 57; i++) {
            for (int j = 50; j < 56; j++) {                
                for (int k = 53; k < 57; k++) {
                    CreateBlock(i, j, k);
                }
            }
        }
        for (int j = 56; j < 58; j++) {
            for (int k = 53; k < 57; k++) {
                CreateBlock(54, j, k);
            }
        }
        for (int i = 53; i < 57; i++) {
            for (int j = 56; j < 58; j++) {
                CreateBlock(i, j, 54);
            }
        }
        CreateBlock(62, 50, 50);
        CreateBlock(56, 55, 59);
        CreateBlock(56, 56, 53);
        CreateBlock(56, 57, 53);
        CreateBlock(52, 57, 53);
        CreateBlock(51, 56, 53);
        CreateBlock(51, 55, 53);
        CreateBlock(51, 54, 53);
        CreateBlock(51, 53, 53);
        CreateBlock(51, 52, 53);
        CreateBlock(51, 51, 53);
        CreateBlock(50, 0, 50);

        CreateBlock(59, 53, 51);
        CreateBlock(59, 53, 52);
        CreateBlock(59, 53, 53);
        CreateBlock(60, 53, 51);
        CreateBlock(60, 53, 53);
        CreateBlock(61, 53, 51);
        CreateBlock(61, 53, 52);
        CreateBlock(61, 53, 53);
        CreateBlock(53, 51, 51, "SpikeBlock");
        CreateBlock(55, 51, 51, "SpikeBlock");

        for (int i = 53; i < 59; i++) {
            for (int j = 46; j < 50; j++) {
                CreateBlock(i, 50, j, "IceBlock");
            }
        }
        CreateBlock(52, 50, 49);
        CreateBlock(59, 50, 49);

        for (int i = 53; i < 57; i++) {
            for (int j = 41; j < 45; j++) {
                CreateBlock(i, 50, j, "IceBlock");
            }
        }

        for (int i = 51; i < 59; i++) {
            for (int j = 40; j < 51; j++) {
                CreateBlock(i, 48, j);
            }
        }

        CreateBlock(51, 49, 49);
        CreateBlock(51, 48, 48);



        CreateBlock(57, 51, 51, "IceBlock");
        CreateBlock(58, 51, 51, "IceBlock");
        CreateBlock(59, 51, 51, "IceBlock");
        CreateBlock(60, 51, 51, "IceBlock");
        CreateBlock(61, 51, 51, "IceBlock");
        CreateBlock(62, 51, 51, "IceBlock");
        CreateBlock(63, 51, 51, "IceBlock");
        CreateBlock(57, 50, 51);
        CreateBlock(58, 50, 51);
        CreateBlock(59, 50, 51);
        CreateBlock(60, 50, 51);
        CreateBlock(61, 50, 51);
        CreateBlock(62, 50, 51);
        CreateBlock(63, 50, 51);
        CreateBlock(64, 50, 51);
        CreateBlock(65, 50, 51);
        CreateBlock(66, 50, 51);
        CreateBlock(67, 50, 51);

        RemoveBlock(54, 54, 53);

    }
    void CreateBlock(int nx, int ny, int nz, string newBlock = "Block")
    {
        GameObject block = GameObject.Find(newBlock);
        if (GameObject.Find("bl-"+nx+"-"+ny+"-"+nz) == null) {
            GameObject obj = Instantiate(block, new Vector3(nx, ny, nz), Quaternion.identity);
            obj.name = "bl-"+nx+"-"+ny+"-"+nz;
            obj.transform.SetParent(gameObject.transform);
            obj.GetComponent<GridStats>().x = nx;
            obj.GetComponent<GridStats>().y = ny;
            obj.GetComponent<GridStats>().z = nz;
            stage[nx, ny, nz] = 1;
        }
    }

    void RemoveBlock(int nx, int ny, int nz)
    {
        var block = GameObject.Find("bl-"+nx+"-"+ny+"-"+nz);
        if (block != null) {
            Destroy(block);
            stage[nx, ny, nz] = 0;
        }
    }
}