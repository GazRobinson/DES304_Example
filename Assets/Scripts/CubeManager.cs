using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    GameObject[,] cubeList = new GameObject[4,4];

    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = Vector3.zero;
        for (int y = 0; y < cubeList.GetLength(1); y++)
        {
            for (int x = 0; x < cubeList.GetLength(0); x++)
            {
                cubeList[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubeList[x, y].transform.position = position;
                position += Vector3.right * 1.1f;
            }
            position.x = 0.0f;
            position += Vector3.down * 1.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cubeList[Random.Range(0,4), Random.Range(0, 4)].SetActive(false);
        }
    }
}
