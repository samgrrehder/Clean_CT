using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlaCam : MonoBehaviour
{
    public GameObject Player;
    Vector3 distCompensar;
    // Start is called before the first frame update
    void Start()
    {
        distCompensar = transform.position - Player.transform.position;
        transform.position = Player.transform.position + distCompensar;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.transform.position + distCompensar;
        // float eixoX = Input.GetAxis("Horizontal") * 20;
        // float eixoY = Input.GetAxis("Vertical") * 20;

        // Vector3 direcao = new Vector3(eixoX, eixoY, 0);

        // transform.Translate(direcao * Time.deltaTime);
    }
}
