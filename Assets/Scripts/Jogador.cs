using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

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
    bool moved;
    bool sliding;
    Dictionary<string, object> ifDirections = new Dictionary<string, object>();
    // Update is called once per frame

    void Start()
    {
        ifDirections.Add("", "");
        ifDirections.Add("spiderPosN", new Vector3(0f, 0.25f, 0.35f));
        ifDirections.Add("spiderPosE", new Vector3(0.35f, 0.25f, 0f));
        ifDirections.Add("spiderPosS", new Vector3(0f, 0.25f, -0.35f));
        ifDirections.Add("spiderPosO", new Vector3(-0.35f, 0.25f, 0f));
        ifDirections.Add("directionN", new Vector3(0f, 0f, 1f));
        ifDirections.Add("directionE", new Vector3(1f, 0f, 0f));
        ifDirections.Add("directionS", new Vector3(0f, 0f, -1f));
        ifDirections.Add("directionO", new Vector3(-1f, 0f, 0f));
        ifDirections.Add("rotateTo1N", new int[] { 270, 1, 1 });
        ifDirections.Add("rotateTo1E", new int[] { 0, 1, -1 });
        ifDirections.Add("rotateTo1S", new int[] { 90, -1, -1 });
        ifDirections.Add("rotateTo1O", new int[] { 180, -1, 1 });
        ifDirections.Add("rotateTo-1N", new int[] { 90, -1, 1 });
        ifDirections.Add("rotateTo-1E", new int[] { 180, 1, 1 });
        ifDirections.Add("rotateTo-1S", new int[] { 270, 1, -1 });
        ifDirections.Add("rotateTo-1O", new int[] { 0, -1, -1 });
        ifDirections.Add("rotateTo2N", new int[] { 90, 0, 0 });
        ifDirections.Add("rotateTo2E", new int[] { 180, 0, 0 });
        ifDirections.Add("rotateTo2S", new int[] { 270, 0, 0 });
        ifDirections.Add("rotateTo2O", new int[] { 0, 0, 0 });
        ifDirections.Add("rotateTo-2N", new int[] { 270, 0, 0 });
        ifDirections.Add("rotateTo-2E", new int[] { 0, 0, 0 });
        ifDirections.Add("rotateTo-2S", new int[] { 90, 0, 0 });
        ifDirections.Add("rotateTo-2O", new int[] { 180, 0, 0 });
        timeBetweenMoves = 0.2f;
        Velocidade = 5f;
        doingSpider = false;
        moved = false;
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
        if (Vector3.Distance(tr.position, pos+spiderPos) <= 0.2f) {
            if (moved) { 
                GameObject.Find("bl-" + x + "-" + (y-1) + "-" + z)?.GetComponent<GridStats>().Call();
                CheckSlide();
            } else if (Time.time >= timestamp) { 
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
        } else {
            moved = true;
        }
    }
     
    void FixedUpdate()
    {
        UpdateDirection();
        if (doingSpider) {
            spiderPos = (Vector3)ifDirections["spiderPos" + (dirS)];            
        }
        tr.position = Vector3.MoveTowards(tr.position, pos+spiderPos, Time.fixedDeltaTime * Velocidade);

        if (sliding == true) {
            Quaternion target = Quaternion.Euler(-20, Math.Abs(tr.localEulerAngles.y) % 360, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1000);
        } else {
            Quaternion target = Quaternion.Euler(0, Math.Abs(tr.localEulerAngles.y) % 360, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1000);
        }
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
                    if (stage[x + eixoX, y - j, z + eixoZ] == 1) {
                        break;
                    }
                    height = -j;                   
                }
            }
            if (stage[x + eixoX, y + height, z + eixoZ] == 1 || (stage[x, y + height, z] == 1 && height == 1)) {
                return;
            }
            if (height < -1) { 
                print("become spider");
                height = -1;
                Spider();
                transform.Rotate(0, 180, 0);
            }
            direction = new Vector3(eixoX, height, eixoZ);
            pos += direction;
            UpdateCoords();

        }
    }

    void Push() //or Pull
    {
        bool eixoXP = Input.GetButtonDown("Horizontal");
        bool eixoZP = Input.GetButtonDown("Vertical");
        direction = (Vector3)ifDirections["direction" + (dirS)];
        Vector3 bF = new Vector3(direction.x, y, direction.z);

        if (stage[x + (int)bF.x, y, z + (int)bF.z]==1 && (eixoXP || eixoZP)) { //checks if there's a block ahead and if there's a direction pressed
            int eixoX = (int)Input.GetAxisRaw("Horizontal");
            int eixoZ = 0;
            if (eixoX == 0) {
                eixoZ = (int)Input.GetAxisRaw("Vertical");
            }
            //print(eixoX);
            if (eixoX == eixoZ || (Math.Abs((int)bF.x * eixoZ) == 1 || Math.Abs((int)bF.z * eixoX) == 1)) {
                return;
            }
            //with the direction pressed, we check if there's a block on the position the pushed block will be
            //if it pushes forward, it's +2 on that axis, else, it's +0 

            // positions                        player pos, block pos, future bp, future pp
            //looking right and pushing right,  PP x      , BP x+1   , FBP x+2  , FPP x 
            //looking right and pulling left,   PP x      , BP x+1   , FBP x    , FPP x-1
            
            //looking left and pulling right,   PP x      , BP x-1   , FBP x    , FPP x+1 
            //looking left and pushing left,    PP x      , BP x-1   , FBP x-2  , FPP x

            //eixoX is 1 if pressing forward, -1 if pressing backward
            //bF.x is 1 if looking forward, -1 if looking backward
            int fX = eixoX * (int)bF.x < 0 ? 0 : eixoX * Math.Abs((int)bF.x) * 2;
            int fZ = eixoZ * (int)bF.z < 0 ? 0 : eixoZ * Math.Abs((int)bF.z) * 2;
            if (fX == 0 && Math.Abs(eixoX) == 1 && stage[x + eixoX, y, z + eixoZ] == 1 ||
                fZ == 0 && Math.Abs(eixoZ) == 1 && stage[x + eixoX, y, z + eixoZ] == 1) {
                return;
            }
            Stack myStack = new Stack();
            var pushedBlock = GameObject.Find("bl-" + (x + (int)bF.x + 0*eixoX) + "-" + y + "-" + (z + (int)bF.z+0*eixoZ));
            //find all the blocks that will be pushed
            for (int count = 0; (pushedBlock = GameObject.Find("bl-" + (x + (int)bF.x + count*eixoX) + "-" + y + "-" + (z + (int)bF.z + count*eixoZ))) != null;count++) {
                myStack.Push(count);
            }
            foreach (int count in myStack) {
                //send move coroutine to the found blocks
                StartCoroutine(GameObject.Find("bl-" + (x + (int)bF.x + count * eixoX) + "-" + y + "-" + (z + (int)bF.z + count * eixoZ)).GetComponent<GridStats>().Move(new Vector3(x + fX + count * eixoX, y, z + fZ + count * eixoZ), 0));
            }
            //if pushing block, only push it and does not travel with it
            if (Math.Abs(fX) == 2 ||
                Math.Abs(fZ) == 2) {
                timestamp = Time.time + timeBetweenMoves * 0.5f;
                return;
            }
            height = 0;
            //if player falls after pulling block
            if (y > 0 && stage[x + eixoX, y, z + eixoZ] == 0 && stage[x + eixoX, y - 1, z + eixoZ] == 0) {
                height = -1;
                Spider();
            }
            pos += new Vector3(eixoX, height, eixoZ);
            timestamp = Time.time + timeBetweenMoves*1.2f;
            //transform.LookAt(new Vector3(pos.x, y, pos.z));
            UpdateCoords();
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

            // if                        Z+1         X+1         Z-1         X-1
            // direction is "north"      GO UP       MOVE RIGHT  FALL DOWN   MOVE LEFT
            // direction is "south"      FALL DOWN   MOVE LEFT   GO UP       MOVE RIGHT
            // direction is "east"       MOVE LEFT   GO UP       MOVE RIGHT  FALL DOWN
            // direction is "west"       MOVE RIGHT  FALL DOWN   MOVE LEFT   GO UP

            //if pressed "up", and there's nothing impeding it, go up
            if ((eixoZ == 1) &&
               ((dirS == "N" && stage[x, y, z + 1] == 1 && stage[x, y + 1, z + 1] == 0) ||
                (dirS == "E" && stage[x + 1, y, z] == 1 && stage[x + 1, y + 1, z] == 0) ||
                (dirS == "S" && stage[x, y, z - 1] == 1 && stage[x, y + 1, z - 1] == 0) ||
                (dirS == "O" && stage[x - 1, y, z] == 1 && stage[x - 1, y + 1, z] == 0))) {
                heightSpider = 1;
                transform.localScale = new Vector3(0.9f, 0.5f, 0.9f);
                spiderPos = new Vector3(0f, 0f, 0f);
                doingSpider = false;
                //stops grabbing ledge
            } else if (eixoZ == -1) {
                eixoZ = 0;
                for (int j = 0; j < y; j++) {
                    if (stage[x, y - j, z] == 1) {
                        heightSpider = -j + 1;
                        transform.localScale = new Vector3(0.9f, 0.5f, 0.9f);
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
                        transform.localScale = new Vector3(0.9f, 0.5f, 0.9f);
                        spiderPos = new Vector3(0f, 0f, 0f);
                        doingSpider = false;
                        break;
                    }
                }

            } else if ((eixoX == 1) &&
               ((dirS == "N" && stage[x + 1, y, z] == 0 && stage[x + 1, y + 1, z] == 0 && stage[x + 1, y, z + 1] == 1) ||
                (dirS == "E" && stage[x, y, z - 1] == 0 && stage[x, y + 1, z - 1] == 0 && stage[x + 1, y, z - 1] == 1) ||
                (dirS == "S" && stage[x - 1, y, z] == 0 && stage[x - 1, y + 1, z] == 0 && stage[x - 1, y, z - 1] == 1) ||
                (dirS == "O" && stage[x, y, z + 1] == 0 && stage[x, y + 1, z + 1] == 0 && stage[x - 1, y, z + 1] == 1))) {
                //move right, nothing to add
            } else if ((eixoX == 1) && 
               ((dirS == "N" && stage[x + 1, y, z] == 0 && stage[x + 1, y + 1, z] == 0 && stage[x + 1, y, z + 1] == 0 && stage[x + 1, y + 1, z + 1] == 0)  ||
                (dirS == "E" && stage[x, y, z - 1] == 0 && stage[x, y + 1, z - 1] == 0 && stage[x + 1, y, z - 1] == 0 && stage[x + 1, y + 1, z - 1] == 0)  ||
                (dirS == "S" && stage[x - 1, y, z] == 0 && stage[x - 1, y + 1, z] == 0 && stage[x - 1, y, z - 1] == 0 && stage[x - 1, y + 1, z - 1] == 0)  ||
                (dirS == "O" && stage[x, y, z + 1] == 0 && stage[x, y + 1, z + 1] == 0 && stage[x - 1, y, z + 1] == 0 && stage[x - 1, y + 1, z + 1] == 0))) {
                cornerX = 1;
                //move external right corner
            } else if ((eixoX == 1) &&
               ((dirS == "N" && stage[x + 1, y, z] == 1) ||
                (dirS == "E" && stage[x, y, z - 1] == 1) ||
                (dirS == "S" && stage[x - 1, y, z] == 1) ||
                (dirS == "O" && stage[x, y, z + 1] == 1))) {
                 cornerX = 2;
                //move internal right corner
            } else if ((eixoX == -1) &&
               ((dirS == "N" && stage[x - 1, y, z] == 0 && stage[x - 1, y + 1, z] == 0 && stage[x - 1, y, z + 1] == 1)  ||
                (dirS == "E" && stage[x, y, z + 1] == 0 && stage[x, y + 1, z + 1] == 0 && stage[x + 1, y, z + 1] == 1)  ||
                (dirS == "S" && stage[x + 1, y, z] == 0 && stage[x + 1, y + 1, z] == 0 && stage[x + 1, y, z - 1] == 1)  ||
                (dirS == "O" && stage[x, y, z - 1] == 0 && stage[x, y + 1, z - 1] == 0 && stage[x - 1, y, z - 1] == 1))) {
                //move left, nothing to add
            } else if ((eixoX == -1) &&
               ((dirS == "N" && stage[x - 1, y, z] == 0 && stage[x - 1, y + 1, z] == 0 && stage[x - 1, y, z + 1] == 0 && stage[x - 1, y + 1, z + 1] == 0)  ||
                (dirS == "E" && stage[x, y, z + 1] == 0 && stage[x, y + 1, z + 1] == 0 && stage[x + 1, y, z + 1] == 0 && stage[x + 1, y + 1, z + 1] == 0)  ||
                (dirS == "S" && stage[x + 1, y, z] == 0 && stage[x + 1, y + 1, z] == 0 && stage[x + 1, y, z - 1] == 0 && stage[x + 1, y + 1, z - 1] == 0)  ||
                (dirS == "O" && stage[x, y, z - 1] == 0 && stage[x, y + 1, z - 1] == 0 && stage[x - 1, y, z - 1] == 0 && stage[x - 1, y + 1, z - 1] == 0))) {
                cornerX = -1;
                //move external right corner
            } else if ((eixoX == -1) &&
               ((dirS == "N" && stage[x - 1, y, z] == 1) ||
                (dirS == "E" && stage[x, y, z + 1] == 1) ||
                (dirS == "S" && stage[x + 1, y, z] == 1) ||
                (dirS == "O" && stage[x, y, z - 1] == 1))) {
                cornerX = -2;
                //move internal right corner
            } else {
                //can't move!
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

            if (cornerX != 0) {
                int[] cornerParams = (int[])ifDirections["rotateTo" + Convert.ToString(cornerX) + (dirS)];
                rotateTo = cornerParams[0];
                eixoX = cornerParams[1];
                eixoZ = cornerParams[2];
                Quaternion target = Quaternion.Euler(0, rotateTo, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 1000);
            }
            direction = new Vector3(eixoX, heightSpider, eixoZ);
            timestamp = Time.time + timeBetweenMoves;
            pos += direction;
            UpdateCoords();
        }
    }

    void CheckSlide()
    {
        if (GameObject.Find("bl-" + x + "-" + (y - 1) + "-" + z)?.GetComponent<GridStats>().tipo == 3 &&
        doingSpider == false &&
        height == 0 &&
        pushing == false) {
            sliding = true;
            if (dirS == "N" && stage[x, y, z + 1] == 0) {
                pos += new Vector3(0f, 0f, 1f);
            }
            else if (dirS == "E" && stage[x + 1, y, z] == 0) {
                pos += new Vector3(1f, 0f, 0f);
            }
            else if (dirS == "S" && stage[x, y, z - 1] == 0) {
                pos += new Vector3(0f, 0f, -1f);
            }
            else if (dirS == "O" && stage[x - 1, y, z] == 0) {
                pos += new Vector3(-1f, 0f, 0f);
            } else {
                sliding = false;
                moved = false;
            }
        } else if (sliding) {
            for (int j = 0; j < y; j++) {
                if (stage[x, y - j, z] == 1) {
                    break;
                }
                if ((dirS == "N" && stage[x, y - j, z + 1] == 0 && stage[x, y - j - 1, z + 1] == 1 && stage[x, y - j - 1, z] == 0) ||
                    (dirS == "E" && stage[x + 1, y - j, z] == 0 && stage[x + 1, y - j - 1, z] == 1 && stage[x, y - j - 1, z] == 0) ||
                    (dirS == "S" && stage[x, y - j, z - 1] == 0 && stage[x, y - j - 1, z - 1] == 1 && stage[x, y - j - 1, z] == 0) ||
                    (dirS == "O" && stage[x - 1, y - j, z] == 0 && stage[x - 1, y - j - 1, z] == 1 && stage[x, y - j - 1, z] == 0)) {
                    height = -j - 1;
                    Spider();
                    break;
                }
                height = -j;
            }
            pos += new Vector3(0f, height, 0f);
            sliding = false;
        } else {
            moved = false;
        }
        UpdateCoords();
    }

    void UpdateDirection()
    {
        float rotY = Math.Abs(tr.localEulerAngles.y) % 360; 
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

    void UpdateCoords()
    {
        x = (int)pos.x;
        y = (int)pos.y;
        z = (int)pos.z;
    }
}
