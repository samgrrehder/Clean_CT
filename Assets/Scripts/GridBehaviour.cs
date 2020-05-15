
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    public int columns = 100;
    public int rows = 100;    
    public int depth = 100;
    public int scale = 1;
    public int[,,] stage = new int[100, 100, 100];
    public GameObject gridPrefab;
    public Vector3 leftBottomLocation = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        if (gridPrefab) {
            GenerateGrid();
        } else {
            print("missing gridprefab, please assign");
        }
    }

    // Update is called once per frame
    void Update()
    {

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

        RemoveBlock(54, 54, 53);

    }
    void CreateBlock(int nx, int ny, int nz)
    {
        var block = GameObject.Find("bl-"+nx+"-"+ny+"-"+nz);
        if (block == null) {
            GameObject obj = Instantiate(gridPrefab, new Vector3(nx, ny, nz), Quaternion.identity);
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