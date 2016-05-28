using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class PlayerMovement : MonoBehaviour {

    public GameObject DepthSrcManager;
    private MultiSourceManager depthManager;
    private ushort[] data;


    public int frontBound = 800;
    public int backBound = 1400;

    private int columnCount = 512; //width of the depth image

    void Start()
    {
        if (DepthSrcManager == null)
        {
            Debug.Log("Assign Game Object with Depth Source Manager");
        }
        else
        {
            depthManager = DepthSrcManager.GetComponent<MultiSourceManager>();
        }

        

    }


    void Update()
    {

        //Is there a depthManager attached?
        if (depthManager == null)
        {
            return;
        }

        data = depthManager.GetDepthData();

        //Just look at values between frontBound & backBound
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] < frontBound || data[i] > backBound)
            {
                data[i] = 10000;
            }
        }


        //Get the point where the nearest object is
        ushort maxValue = data.Min();
        int maxValueIndex = Array.IndexOf(data, maxValue);

        Debug.Log("Max Value of " + maxValue + " at index position " + maxValueIndex);

        //Get the two-dimensional position

        int yCoord = maxValueIndex / columnCount; //Row Position of Index
        int xCoord = maxValueIndex % columnCount; //Column Position

        int yFieldCoord = yCoord * 2;

        Vector3 newPosition = new Vector3(xCoord, 0, yFieldCoord);
        Debug.Log("New Position:" + newPosition.x + "," + newPosition.y + "," + newPosition.z);


        //Move the Stick around
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);






    }
}
