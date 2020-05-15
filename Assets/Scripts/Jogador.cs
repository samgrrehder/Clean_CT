using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Jogador : MonoBehaviour
{

    public float Velocidade = 7f;
    Vector3 pos;
    private Transform tr;
    public GameObject Grid;
    public int x;
    public int y;
    public int z;
    public bool doingSpider;
    public bool pushing;
    Vector3 spiderPos;
    Vector3 spiderScale;
    public int height;
    public int[,,] stage;
    Vector3 direction;
    public float timeBetweenMoves;
    private float timestamp;
    public String dirS;
    // Update is called once per frame

    void Start()
    {
        timeBetweenMoves = 0.2f;
        Velocidade = 5f;
        doingSpider = false;
        stage = Grid.GetComponent<GridBehaviour>().stage;
        transform.position = new Vector3(51, 51, 51);
        tr = transform;
        pos = tr.position;
        spiderPos = new Vector3(0, 0, 0);
        x = (int)tr.position.x;
        y = (int)tr.position.y;
        z = (int)tr.position.z;
}
    void Update()
    {
        if (Vector3.Distance(tr.position, pos+spiderPos) <= 0.2f && Time.time >= timestamp)
        {
            if (doingSpider) {
                SpiderMove();
            } else {
                pushing = Input.GetButton("Pushing");
                if (pushing) {
                    Push();
                } else {
                    Move();
                }
            }
        }
    }
     
    void FixedUpdate()
    {
        if (doingSpider) {
            GetDirection();
            if (dirS == "N") {
                spiderPos = new Vector3(0f, 0.25f, 0.35f);
            } else if (dirS == "E") {
                spiderPos = new Vector3(0.35f, 0.25f, 0f);
            } else if (dirS == "S") {
                spiderPos = new Vector3(0f, 0.25f, -0.35f);
            } else {
                spiderPos = new Vector3(-0.35f, 0.25f, 0f);
            }
        }
        tr.position = Vector3.MoveTowards(tr.position, pos+spiderPos, Time.fixedDeltaTime * Velocidade);
    }

    private void OnAnimatorMove()
    {
        x = (int)pos.x;
        y = (int)pos.y + 1;
        z = (int)pos.z;
    }

    void Move()
    {
        height = 0;
        bool eixoXP = Input.GetButton("Horizontal");
        bool eixoZP = Input.GetButton("Vertical");
        /*Grid.GetComponent<GridBehaviour>().stage*/

        if (eixoXP || eixoZP) {
            int eixoX = (int)Input.GetAxisRaw("Horizontal");
            int eixoZ = 0;
            if (eixoX == 0) {
                eixoZ = (int)Input.GetAxisRaw("Vertical");
            }
            GetDirection();
            if (Input.GetButton("Looking") || (dirS != "E" && eixoX == 1)  || (dirS != "O" && eixoX == -1)  || (dirS != "N" && eixoZ == 1)  || (dirS != "S" && eixoZ == -1) ){
                float rotateTo = eixoX == 1 ? 90 : eixoX == -1 ? 270 : eixoZ == 1 ? 0 : 180;
                Quaternion target = Quaternion.Euler(0, rotateTo, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1000);
                timestamp = Time.time + timeBetweenMoves/4;
                return;
            }

            if (stage[x + eixoX, y, z + eixoZ] == 1 && stage[x + eixoX, y + 1, z + eixoZ] == 0) {
                height = 1;
            }
            else if (stage[x + eixoX, y, z + eixoZ] == 0) {
                for (int j = 0; j < y; j++) {
                    //print(j);
                    if (stage[x + eixoX, y - j, z + eixoZ] == 1) {
                        break;
                    }
                    height = -j;                   
                }
                //print("height " + height);
            } else {
                /*print("flw");*/
            }
            if (stage[x + eixoX, y + height, z + eixoZ] == 1 || (stage[x, y + height, z] == 1 && height == 1)) {
                /*print("1111");*/
                return;
            }
            //tr.LookAt(new Vector3(pos.x, y, pos.z));    
            if (height < -1) { //se for -1, só cai, 0 só vai, 1 sobe
                print("become spider");
                height = -1;
                Spider();
                transform.Rotate(0, 180, 0);
            }
            direction = new Vector3(eixoX, height, eixoZ);
            pos += direction;
            x = (int)pos.x;
            y = (int)pos.y;
            z = (int)pos.z;

        }
    }

    void Push()
    {
        bool eixoXP = Input.GetButtonDown("Horizontal");
        bool eixoZP = Input.GetButtonDown("Vertical");
        /*Grid.GetComponent<GridBehaviour>().stage*/
        GetDirection();
        if (dirS == "N") {
            direction = new Vector3(0f, direction.y, 1f);
        } else if (dirS == "E") {
            direction = new Vector3(1f, direction.y, 0f);
        } else if (dirS == "S") {
            direction = new Vector3(0f, direction.y, -1f);
        } else {
            direction = new Vector3(-1f, direction.y, 0f);
        }
        Vector3 bF = new Vector3(direction.x, y, direction.z);

        if (stage[x + (int)bF.x, y, z + (int)bF.z]==1 && (eixoXP || eixoZP)) { //verifica se o bloco a frente existe e se apertou alguma direção
            int eixoX = (int)Input.GetAxisRaw("Horizontal");
            int eixoZ = 0;
            if (eixoX == 0) {
                eixoZ = (int)Input.GetAxisRaw("Vertical");
            }
            //print(eixoX);
            if (eixoX == eixoZ || (Math.Abs((int)bF.x * eixoZ) == 1 || Math.Abs((int)bF.z * eixoX) == 1)) {
                return;
            }
            //com a direção apertada em mãos, verificamos se existe algum bloco na futura posição do bloco
            //se apertar para frente, a posição deve ser x + 2, se apertar para trás, deve ser x

            //                                                                  jogador   bloco    futuro bloco futuro jogador                                                                            
            //se estiver olhando para a direita e quiser empurrar para a direita,  PJ é x, PB é x+1, PFB é x + 2
            //se estiver olhando para a direita e quiser empurrar para a esquerda, PJ é x, PB é x+1, PFB é x,    PJF é x-1 (eixoX vai ser -1)

            //se estiver olhando para a esquerda e quiser empurrar para a direita, PJ é x, PB é x-1, PFB é x,    PJF é x+1 (eixoX vai ser +1)
            //se estiver olhando para a esquerda e quiser empurrar para a esquerda,PJ é x, PB é x-1, PFB é x-2
            //eixo X é 1 se tiver apertado pra frente, -1 se tiver apertado pra trás
            //bf.x é 1 se tiver olhando pra frente, -1 se tiver olhando pra trás
            int fX = eixoX * (int)bF.x < 0 ? 0 : eixoX * Math.Abs((int)bF.x) * 2;
            int fZ = eixoZ * (int)bF.z < 0 ? 0 : eixoZ * Math.Abs((int)bF.z) * 2;
            if (fX == 0 && Math.Abs(eixoX) == 1 && stage[x + eixoX, y, z + eixoZ] == 1 ||
                fZ == 0 && Math.Abs(eixoZ) == 1 && stage[x + eixoX, y, z + eixoZ] == 1) {
                return;
            }
            bool mexeu = false;
            var pushedBlock = GameObject.Find("bl-" + (x + (int)bF.x + 0*eixoX) + "-" + y + "-" + (z + (int)bF.z+0*eixoZ));
            for (int count = 0; (pushedBlock = GameObject.Find("bl-" + (x + (int)bF.x + count*eixoX) + "-" + y + "-" + (z + (int)bF.z + count*eixoZ))) != null;count++) {
                /*print("bl-" + (x + (int)bF.x + count) + "-" + y + "-" + (z + (int)bF.z + count));*/
                mexeu = true;
                pushedBlock.GetComponent<GridStats>().StartCoroutine(pushedBlock.GetComponent<GridStats>().Move(new Vector3(x + fX + count*eixoX, y, z + fZ + count*eixoZ), 0));
            }
            if (mexeu == false) {
                return;
            }
            //se estiver empurrando o bloco, somente empurra o bloco, não vai pra frente ou agarra
            if (stage[x + eixoX, y + height, z + eixoZ] == 1 ||
                Math.Abs(fX) == 2 ||
                Math.Abs(fZ) == 2) {
                return;
            }
            if (y > 0 && stage[x + eixoX, y, z + eixoZ] == 0 && stage[x + eixoX, y - 1, z + eixoZ] == 0) {
                height = -1;
                Spider();
            }
            pos += new Vector3(eixoX, height, eixoZ);
            //transform.LookAt(new Vector3(pos.x, y, pos.z));
            x = (int)pos.x;
            y = (int)pos.y;
            z = (int)pos.z;
            //print(x+"-"+y+"-"+z);
        }
    }

    void Spider()
    {
        doingSpider = true;        
        transform.localScale = new Vector3(0.85f, 0.25f, 0.25f);
        
    }

    void SpiderMove()
    {
        bool eixoXP = Input.GetButton("Horizontal");
        bool eixoZP = Input.GetButton("Vertical");
        int heightSpider = 0;
        int cornerX = 0;
        int rotateTo = 0;
        /*Grid.GetComponent<GridBehaviour>().stage*/

        if (eixoXP || eixoZP) {
            /*if (Math.Abs(direction.x) == 1) { */
            int eixoX = (int)Input.GetAxisRaw("Horizontal");
            int eixoZ = 0;
            if (eixoX == 0) {
                eixoZ = (int)Input.GetAxisRaw("Vertical");
            }
            GetDirection();

            // SE                           Z+1         X+1         Z-1         X-1      
            // se direção for "NORTE"       SOBE        DIREITA     CAI         ESQUERDA 
            // se direção for "SUL"         CAI         ESQUERDA    SOBE        DIREITA
            // se direção for "LESTE"       ESQUERDA    SOBE        DIREITA     CAI
            // se direção for "OESTE"       DIREITA     CAI         ESQUERDA    SOBE

            //se apertou para cima, tem bloco na direção a frente do jogador e não tem bloco em cima, tem que subir
            //print(dirS + ":" + (x+1) + "-" + (y) + "-" + (z));
            //print(dirS + ":" + (x+1) + "-" + (y+1) + "-" + (z));
            if ((eixoZ == 1) &&
               ((dirS == "N" && stage[x, y, z + 1] == 1 && stage[x, y + 1, z + 1] == 0) ||
                (dirS == "E" && stage[x + 1, y, z] == 1 && stage[x + 1, y + 1, z] == 0) ||
                (dirS == "S" && stage[x, y, z - 1] == 1 && stage[x, y + 1, z - 1] == 0) ||
                (dirS == "O" && stage[x - 1, y, z] == 1 && stage[x - 1, y + 1, z] == 0))) {
                heightSpider = 1;
                transform.localScale = new Vector3(1f, 0.5f, 1f);
                spiderPos = new Vector3(0f, 0f, 0f);
                doingSpider = false;
                //sai do spider
            } else if (eixoZ == -1) {
                eixoZ = 0;
                for (int j = 0; j < y; j++) {
                    if (stage[x, y - j, z] == 1) {
                        heightSpider = -j + 1;
                        transform.localScale = new Vector3(1f, 0.5f, 1f);
                        spiderPos = new Vector3(0f, 0f, 0f);
                        doingSpider = false;
                        break;
                    }
                    if (y - j >= 0) {
                        if ((dirS == "N" && stage[x, y - j, z + 1] == 0 && stage[x, y - j - 1, z + 1] == 1 && stage[x, y - j - 1, z] == 0) ||
                            (dirS == "E" && stage[x + 1, y - j, z] == 0 && stage[x + 1, y - j - 1, z] == 1 && stage[x, y - j - 1, z] == 0) ||
                            (dirS == "S" && stage[x, y - j, z - 1] == 0 && stage[x, y - j - 1, z - 1] == 1 && stage[x, y - j - 1, z] == 0) ||
                            (dirS == "O" && stage[x - 1, y - j, z] == 0 && stage[x - 1, y - j - 1, z] == 1 && stage[x, y - j - 1, z] == 0)) {
                            print("here i am");
                            heightSpider = -j - 1;
                            break;
                        }
                    }
                    if (y - j - 1 == 0) {
                        heightSpider = -j;
                        transform.localScale = new Vector3(1f, 0.5f, 1f);
                        spiderPos = new Vector3(0f, 0f, 0f);
                        doingSpider = false;
                        break;
                    }
                }
                // SE APERTAR PRA DIREITA E NÃO FOR CANTINHO:
                // O BLOCO A DIREITA DO JOGADOR NÃO DEVE EXISTIR
                // O BLOCO A DIREITA E ACIMA DO JOGADOR NÃO DEVE EXISTIR
                // O BLOCO A DIREITA E A FRENTE DO JOGADOR DEVE EXISTIR

                //SE APERTAR PRA DIREITA E FOR CANTINHO
                // O BLOCO A DIREITA DO JOGADOR NÃO DEVE EXISTIR
                // O BLOCO A DIREITA E ACIMA DO JOGADOR NÃO DEVE EXISTIR
                // O BLOCO A DIREITA E A FRENTE DO JOGADOR NÃO DEVE EXISTIR
                // O BLOCO A FRENTE, A DIREITA E ACIMA DO JOGADOR NÃO DEVE EXISTIR

            } else if ((eixoX == 1) &&
               ((dirS == "N" && stage[x + 1, y, z] == 0 && stage[x + 1, y + 1, z] == 0 && stage[x + 1, y, z + 1] == 1) ||
                (dirS == "E" && stage[x, y, z - 1] == 0 && stage[x, y + 1, z - 1] == 0 && stage[x + 1, y, z - 1] == 1) ||
                (dirS == "S" && stage[x - 1, y, z] == 0 && stage[x - 1, y + 1, z] == 0 && stage[x - 1, y, z - 1] == 1) ||
                (dirS == "O" && stage[x, y, z + 1] == 0 && stage[x, y + 1, z + 1] == 0 && stage[x - 1, y, z + 1] == 1))) {
            } else if ((eixoX == 1) && 
               ((dirS == "N" && stage[x + 1, y, z] == 0 && stage[x + 1, y + 1, z] == 0 && stage[x + 1, y, z + 1] == 0 && stage[x + 1, y + 1, z + 1] == 0)  ||
                (dirS == "E" && stage[x, y, z - 1] == 0 && stage[x, y + 1, z - 1] == 0 && stage[x + 1, y, z - 1] == 0 && stage[x + 1, y + 1, z - 1] == 0)  ||
                (dirS == "S" && stage[x - 1, y, z] == 0 && stage[x - 1, y + 1, z] == 0 && stage[x - 1, y, z - 1] == 0 && stage[x - 1, y + 1, z - 1] == 0)  ||
                (dirS == "O" && stage[x, y, z + 1] == 0 && stage[x, y + 1, z + 1] == 0 && stage[x - 1, y, z + 1] == 0 && stage[x - 1, y + 1, z + 1] == 0))) {
                cornerX = 1;
            } else if ((eixoX == 1) &&
               ((dirS == "N" && stage[x + 1, y, z] == 1) ||
                (dirS == "E" && stage[x, y, z - 1] == 1) ||
                (dirS == "S" && stage[x - 1, y, z] == 1) ||
                (dirS == "O" && stage[x, y, z + 1] == 1))) {
                 cornerX = 2;
            } else if ((eixoX == -1) &&
               ((dirS == "N" && stage[x - 1, y, z] == 0 && stage[x - 1, y + 1, z] == 0 && stage[x - 1, y, z + 1] == 1)  ||
                (dirS == "E" && stage[x, y, z + 1] == 0 && stage[x, y + 1, z + 1] == 0 && stage[x + 1, y, z + 1] == 1)  ||
                (dirS == "S" && stage[x + 1, y, z] == 0 && stage[x + 1, y + 1, z] == 0 && stage[x + 1, y, z - 1] == 1)  ||
                (dirS == "O" && stage[x, y, z - 1] == 0 && stage[x, y + 1, z - 1] == 0 && stage[x - 1, y, z - 1] == 1))) {
                //print("canmoveesq" + dirS); 
                //print((x - 1) + "-" + (y) + "-" + (z + 1));
            } else if ((eixoX == -1) &&
               ((dirS == "N" && stage[x - 1, y, z] == 0 && stage[x - 1, y + 1, z] == 0 && stage[x - 1, y, z + 1] == 0 && stage[x - 1, y + 1, z + 1] == 0)  ||
                (dirS == "E" && stage[x, y, z + 1] == 0 && stage[x, y + 1, z + 1] == 0 && stage[x + 1, y, z + 1] == 0 && stage[x + 1, y + 1, z + 1] == 0)  ||
                (dirS == "S" && stage[x + 1, y, z] == 0 && stage[x + 1, y + 1, z] == 0 && stage[x + 1, y, z - 1] == 0 && stage[x + 1, y + 1, z - 1] == 0)  ||
                (dirS == "O" && stage[x, y, z - 1] == 0 && stage[x, y + 1, z - 1] == 0 && stage[x - 1, y, z - 1] == 0 && stage[x - 1, y + 1, z - 1] == 0))) {
                cornerX = -1;
            } else if ((eixoX == -1) &&
               ((dirS == "N" && stage[x - 1, y, z] == 1) ||
                (dirS == "E" && stage[x, y, z + 1] == 1) ||
                (dirS == "S" && stage[x + 1, y, z] == 1) ||
                (dirS == "O" && stage[x, y, z - 1] == 1))) {
                cornerX = -2;
            } else {
                print("não mexe, porra");
                return;
            }

            if (dirS == "S") {
                eixoX *= -1;
                eixoZ *= -1;
            } else if (dirS == "E") {
                int aux = eixoX;
                eixoX = eixoZ;
                eixoZ = aux*-1;                     
            } else if (dirS == "O") {
                int aux = eixoX;
                eixoX = eixoZ*-1;
                eixoZ = aux; 
            }
            if (cornerX == 1) {
                if (dirS == "N") {
                    rotateTo = 270;
                    eixoZ = 1;
                }
                else if (dirS == "E") {
                    rotateTo = 0;
                    eixoX = 1;
                }
                else if (dirS == "S") {
                    rotateTo = 90;
                    eixoZ = -1;
                }
                else {
                    rotateTo = 180;
                    eixoX = -1;
                }
            } else if (cornerX == -1) {
                if (dirS == "N") {
                    rotateTo = 90;
                    eixoZ = 1;
                }
                else if (dirS == "E") {
                    rotateTo = 180;
                    eixoX = 1;
                }
                else if (dirS == "S") {
                    rotateTo = 270;
                    eixoZ = -1;
                }
                else {
                    rotateTo = 0;
                    eixoX = -1;
                }
            } else if (cornerX == 2) {
                eixoX = 0;
                eixoZ = 0;
                if (dirS == "N") {
                    rotateTo = 90;
                }
                else if (dirS == "E") {
                    rotateTo = 180;
                }
                else if (dirS == "S") {
                    rotateTo = 270;
                }
                else {
                    rotateTo = 0;
                }
            } else if (cornerX == -2) {
                eixoX = 0;
                eixoZ = 0;
                if (dirS == "N") {
                    rotateTo = 270;
                }
                else if (dirS == "E") {
                    rotateTo = 0;
                }
                else if (dirS == "S") {
                    rotateTo = 90;
                }
                else {
                    rotateTo = 180;
                }
            }
            if (cornerX != 0) {
                Quaternion target = Quaternion.Euler(0, rotateTo, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1000);
            }
            direction = new Vector3(eixoX, heightSpider, eixoZ);
            timestamp = Time.time + timeBetweenMoves;
            pos += direction;
            x = (int)pos.x;
            y = (int)pos.y;
            z = (int)pos.z;

        }
    }

    void GetDirection()
    {
        float rotY = Math.Abs(tr.localEulerAngles.y) % 360;
        //print(tr.localEulerAngles.y);
        //print(rotY);
        dirS = "N";
        if (80 <= rotY && rotY <= 100) {
            dirS = "E";
        }
        if (170 <= rotY && rotY <= 190) {
            dirS = "S";
        }
        if (260 <= rotY && rotY <= 280) {
            dirS = "O";
        }
    }
}