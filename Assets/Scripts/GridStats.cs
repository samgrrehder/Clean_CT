using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

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
    public int tipo;
    int stop;
    public Coroutine lastRoutine = null;
    public bool isPlayerPulling = false;
    public bool slide = false;
    // Start is called before the first frame update
    void Start()
    {
        Grid = GameObject.Find("Grid");
        stop = 0;
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
        isPlayerPulling = false;
        if (Vector3.Distance(GameObject.Find("Jogador").GetComponent<Jogador>().transform.position, newPos) < 0.1f) {
            isPlayerPulling = true;
        }
        Vector3 slidePos = new Vector3(0, 0, 0);
        stop = 1;
        if (lastRoutine != null) {
            transform.position = pos;
            StopCoroutine(lastRoutine);
            updateLocation();
        }

        if (height == 0) {
            stage[x, y, z] = 0;
            if (GameObject.Find("bl-" + newPos.x + "-" + (newPos.y - 1) + "-" + newPos.z)?.GetComponent<GridStats>().tipo == 3 || tipo == 3) {
                slidePos = newPos;
                // is in 1, moves to 2? slides to 3 : 2 - 1 = 1   2 + 1 = 3                            
                // is in 2, moves to 1? slides to 0 : 1 - 2 = -1  1 + (-1) = 0
                slide = true;
                if (newPos.x != pos.x) {
                    slidePos.x = newPos.x + newPos.x - pos.x;
                }
                if (newPos.z != pos.z) {
                    slidePos.z = newPos.z + (newPos.z - pos.z);
                }
            }
        }

        while (Vector3.Distance(transform.position, newPos) != 0) {
            gameObject.name = "bl-" + (int)(transform.position.x) + "-" + (int)(transform.position.y) + "-" + (int)(transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime * velocidade);
            yield return null;
        }

        pos = newPos;
        stop = 0;
        stage[(int)pos.x, (int)pos.y, (int)pos.z] = 1;

        if (height == 0) {
            SendCheckFallUp(depend, 0);
            if ((int)pos.y > 0) {
                CheckFall(depend, 0);
            }
        }
        height = 0;
        updateLocation();
        if (slide) {
            if (GameObject.Find("bl-" + newPos.x + "-" + (newPos.y - 1) + "-" + newPos.z) != null &&
            !isPlayerPulling &&
            (GameObject.Find("bl-" + slidePos.x + "-" + slidePos.y + "-" + slidePos.z) == null ||
             GameObject.Find("bl-" + slidePos.x + "-" + slidePos.y + "-" + slidePos.z).GetComponent<GridStats>().slide == true)) {
                slide = false;
                StartCoroutine(Move(slidePos, 0));
            } else {
                slide = false;
            }
        }
    }

    public IEnumerator PlatMove(Vector3 newPos)
    {
        Transform plat = gameObject.transform.Find("Platform");
        MeshRenderer platRend = plat.GetComponent<MeshRenderer>();
        Color currentColor = platRend.material.color;
        Vector3 platNewPos = newPos + plat.position;
        //platRend.material.SetColor("_Color", new Vector4(1, 0, 0, 1));
        //topRend.material.SetColor("_Color", new Vector4(1, 0, 0, 1));
        float cD = 0.45f;
        float tP = 0f;
        float totCD = cD;
        while (cD >= 0) {
            foreach (Transform child in transform) {
                child.GetComponent<MeshRenderer>().material.SetFloat("_Metallic", tP/totCD);
                child.GetComponent<MeshRenderer>().material.color = Color.Lerp(new Vector4(1, 0, 0, 1), currentColor, cD/totCD);                
            }
            cD -= Time.smoothDeltaTime;
            tP += Time.smoothDeltaTime;
            yield return null;
        }
        Jogador obj = GameObject.Find("Jogador").GetComponent<Jogador>();
        while (Vector3.Distance(plat.position, platNewPos) != 0) {
            if (Vector3.Distance(plat.position, platNewPos) < 0.55f &&
                obj.x == x && obj.y == (y+1) && obj.z == z) {
                print("morreu");
                Scene scene = SceneManager.GetActiveScene(); 
                SceneManager.LoadScene(scene.name);
            }
            platNewPos = new Vector3(transform.position.x, platNewPos.y, transform.position.z);
            plat.position = Vector3.MoveTowards(plat.position, platNewPos, Time.deltaTime * 10);
            yield return null;
        }
        cD = 1f;
        tP = 1f;
        totCD = 1f;
        for (int i = 0; i < (int)totCD*10000; i++) {
            while (cD >= 0) {
                foreach (Transform child in transform)
                {
                    child.GetComponent<MeshRenderer>().material.SetFloat("_Metallic", tP/totCD);
                    child.GetComponent<MeshRenderer>().material.color = Color.Lerp(currentColor, new Vector4(1, 0, 0, 1), cD / totCD);
                }
                cD -= Time.smoothDeltaTime;
                tP -= (Time.smoothDeltaTime/4);
                yield return null;
            }
        }
        platNewPos = plat.position - newPos;
        float disTot = Vector3.Distance(plat.position, platNewPos);
        float porc = Vector3.Distance(platNewPos, plat.position) / disTot * 255 * 1.5f;
        while (Vector3.Distance(plat.position, platNewPos) != 0) {
            porc = Vector3.Distance(platNewPos, plat.position) / disTot * 255 / 1.5f;
            porc = 255 - ((255 - porc)*1.25f);
            if (porc < 0) {
                porc = 0;
            }
            foreach (Transform child in plat) {
                Color32 col = child.GetComponent<MeshRenderer>().material.GetColor("_Color");
                col.a = (byte)porc;
                child.GetComponent<MeshRenderer>().material.SetColor("_Color", col);
                col = child.Find("Cone").GetComponent<MeshRenderer>().material.GetColor("_Color");
                col.a = (byte)porc;
                child.Find("Cone").GetComponent<MeshRenderer>().material.SetColor("_Color", col);
            }
            platNewPos = new Vector3(transform.position.x, platNewPos.y, transform.position.z);
            plat.position = Vector3.MoveTowards(plat.position, platNewPos, Time.deltaTime);
            yield return null;
        }
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
            stop = 0;
            Fall(depend);
            return;
        } else {
        }
        
    }

    public IEnumerator Shake(int depend)
    {
        if (stop == 1) {
            yield break;
        }
        stop = 1;
        stage[x, y, z] = -1;
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
        stop = 0;
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
        if (stop == 1) {
            return;
        }
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

    public void Call()
    {
        if (tipo == 1) { return; }
        if (tipo == 2) {
            tipo = 1;
            StartCoroutine(PlatMove(new Vector3(0, 0.8f, 0)));  
        }
    }
}