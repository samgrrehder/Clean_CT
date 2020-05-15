using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GridStats : MonoBehaviour
{
    public int x = 0;
    public int y = 0;
    public int z = 0;
    public int velocidade = 1;
    public Vector3 pos;
    public GameObject Grid;
    public int[,,] stage;
    public int height;
    public Coroutine lastRoutine = null;
    GameObject pushedBlock = null;
    // Start is called before the first frame update
    void Start()
    {      
        velocidade = 5;
        stage = Grid.GetComponent<GridBehaviour>().stage;
        pos = transform.position;        
    }

    public void updateLocation()
    {
        x = (int)Math.Round(transform.position.x);
        y = (int)Math.Round(transform.position.y);
        z = (int)Math.Round(transform.position.z);
        gameObject.name = "bl-" + x + "-" + y + "-" + z;
    }

    public void SendCheckFallUp(int depend, int fall)
    {
        GameObject.Find("bl-" + (x + 1) + "-" + (y + 1) + "-" + (z))?.GetComponent<GridStats>().CheckFall(depend, fall);
        GameObject.Find("bl-" + (x - 1) + "-" + (y + 1) + "-" + (z))?.GetComponent<GridStats>().CheckFall(depend, fall);
        GameObject.Find("bl-" + (x) + "-" + (y + 1) + "-" + (z))?.GetComponent<GridStats>().CheckFall(depend, fall);
        GameObject.Find("bl-" + (x) + "-" + (y + 1) + "-" + (z + 1))?.GetComponent<GridStats>().CheckFall(depend, fall);
        GameObject.Find("bl-" + (x) + "-" + (y + 1) + "-" + (z - 1))?.GetComponent<GridStats>().CheckFall(depend, fall);
    }

    public IEnumerator Move(Vector3 newPos, int depend)
    {
        if (lastRoutine != null) {
            transform.position = pos;
            StopCoroutine(lastRoutine);
            updateLocation();
        }        

        if (height == 0) {
            stage[x, y, z] = 0;
        }

        while (Vector3.Distance(transform.position, newPos) != 0) {
            gameObject.name = "bl-" + (int)(transform.position.x) + "-" + (int)(transform.position.y) + "-" + (int)(transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * velocidade);
            yield return null;
        }

        pos = newPos;
        stage[(int)pos.x, (int)pos.y, (int)pos.z] = 1;

        if (height == 0) {
            SendCheckFallUp(depend, 0);
            if ((int)pos.y > 0) {
                CheckFall(depend, 0);
            }
        }
        height = 0;
        updateLocation();
    }

    void CheckFall(int depend, int fall)
    {
        updateLocation();

        if (stage[x, y - 1, z + 1] <= 0 &&
            stage[x, y - 1, z - 1] <= 0 &&
            stage[x, y - 1, z] <= 0 &&
            stage[x + 1, y - 1, z] <= 0 &&
            stage[x - 1, y - 1, z] <= 0) {
            if (fall == 0) {
                print("shakeshakeshake"); 
                lastRoutine = StartCoroutine(Shake(depend));
                return;
            }
            for (int j = 1; j <= y; j++) {
                if ((stage[x, y - j, z + 1] != 0 ||
                        stage[x, y - j, z - 1] != 0 ||
                        stage[x, y - j, z] != 0 ||
                        stage[x + 1, y - j, z] != 0 ||
                        stage[x - 1, y - j, z] != 0)) {
                    height = -j + 1;
                    break;
                }
                height = -j;
            }
            if (height < 0) {
                Fall(depend);
                return;
            }
            stage[(int)pos.x, (int)pos.y, (int)pos.z] = 1;
        }
    }

    public IEnumerator Shake(int depend)
    {
        stage[x, y, z] = -1;
        print(x + "" + (int)pos.x + "-" + y + (int)pos.y + "-" + z + (int)pos.z + "---" + stage[x, y - 1, z + 1] + stage[x, y - 1, z - 1] + stage[x, y - 1, z] + stage[x + 1, y - 1, z] + stage[x - 1, y - 1, z]);  
        SendCheckFallUp(depend + 1, 0);
        float countDown = 1f;
        Quaternion target = Quaternion.Euler(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));
        for (int i = 0; i < 10000; i++) {
            while (countDown >= 0) {
                target = Quaternion.Euler(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1000);
                transform.position = new Vector3(pos.x + (UnityEngine.Random.Range(-0.02f, 0.02f)), pos.y + (UnityEngine.Random.Range(-0.02f, 0.02f)), pos.z + (UnityEngine.Random.Range(-0.02f, 0.02f)));
                countDown -= Time.smoothDeltaTime;
                yield return null;
            }
        }
        stage[x, y, z] = 1;
        target = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1000);
        transform.position = pos;
        if (depend == 0) {
            CheckFall(depend, 1);
        }
    }

    void CheckDependantFall(int depend, int dependHeight)
    {
        updateLocation();

        if (stage[x, y - 1, z + 1] <= 0 &&
            stage[x, y - 1, z - 1] <= 0 &&
            stage[x, y - 1, z] <= 0 &&
            stage[x + 1, y - 1, z] <= 0 &&
            stage[x - 1, y - 1, z] <= 0) {
            for (int j = 1; j <= -dependHeight; j++) {
                if ((stage[x, y - j, z + 1] != 0 ||
                        stage[x, y - j, z - 1] != 0 ||
                        stage[x, y - j, z] != 0 ||
                        stage[x + 1, y - j, z] != 0 ||
                        stage[x - 1, y - j, z] != 0)) {
                    height = -j + 1;
                    break;
                }
                height = -j;
            }
            Fall(depend);
            return;
        }
        stage[(int)pos.x, (int)pos.y, (int)pos.z] = 1;
    }

    public void Fall(int depend)
    {
        stage[(int)pos.x, (int)pos.y, (int)pos.z] = 0;
        GameObject.Find("bl-" + (x + 1) + "-" + (y + 1) + "-" + (z))?.GetComponent<GridStats>().CheckDependantFall(depend + 1, height);
        GameObject.Find("bl-" + (x - 1) + "-" + (y + 1) + "-" + (z))?.GetComponent<GridStats>().CheckDependantFall(depend + 1, height);
        GameObject.Find("bl-" + (x) + "-" + (y + 1) + "-" + (z))?.GetComponent<GridStats>().CheckDependantFall(depend + 1, height);
        GameObject.Find("bl-" + (x) + "-" + (y + 1) + "-" + (z + 1))?.GetComponent<GridStats>().CheckDependantFall(depend + 1, height);
        GameObject.Find("bl-" + (x) + "-" + (y + 1) + "-" + (z - 1))?.GetComponent<GridStats>().CheckDependantFall(depend + 1, height); 
        StartCoroutine(Wait(pos + new Vector3(0, height, 0)));
        StartCoroutine(Move(pos + new Vector3(0, height, 0), depend));
    }

    public IEnumerator Wait(Vector3 newPos)
    {
        float countDown = 0.05f;
        Quaternion target = Quaternion.Euler(UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f), UnityEngine.Random.Range(-0.8f, 0.8f));
        for (int i = 0; i < 500; i++) {
            while (countDown >= 0) {
                countDown -= Time.smoothDeltaTime;
                yield return null;
            }
        }
        stage[(int)newPos.x, (int)newPos.y, (int)newPos.z] = -1;
        
    }


}