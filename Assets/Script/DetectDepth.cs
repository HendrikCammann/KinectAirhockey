﻿using UnityEngine; 
using System.Collections;
using Windows.Kinect;
using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

public class DetectDepth : MonoBehaviour {

    public GameObject DepthSrcManager;
    private MultiSourceManager depthManager;
    private ushort[] distances;
    private int concatStringInt;
    public bool SetupFinished = false;
    public float latency = 1f;
    private int lastXcoord = 0;
    private int lastYcoord = 0;


    // Use this for initialization
    void Start () {
        if (DepthSrcManager == null)
        {
            Debug.Log("Assign Game Object with Depth Source Manager");
        }
        else
        {
            depthManager = DepthSrcManager.GetComponent<MultiSourceManager>();
        }
    }

    int lastDist = 0;
    //int counter = 0;
	// Update is called once per frame
	void Update () {
        if (depthManager == null)
        {
            return;
        }

        distances = depthManager.GetDepthData();
        int aktDist = distances[4000];

        for(int i = 0; i < distances.Length; i++)
        {
            if(distances[i] < 800 || distances[i] > 1400)
            {
                distances[i] = 10000;
            }
        }

        /*int width = 512;
        int[] getArray;
        int[] getArray2;

        if (!SetupFinished)
        {
            int maxValue = distances.Min();
            int maxIndex = distances.ToList().IndexOf(distances.Min());

            getArray = checkForContrast(maxIndex, maxValue, width, distances);
            int xCoord2 = getArray[0];
            int ýCoord2 = getArray[1];

            String xCoord2toString = xCoord2.ToString();
            String yCoord2toString = ýCoord2.ToString();

            String concatString = yCoord2toString + xCoord2toString;
            concatStringInt = Int32.Parse(concatString);
            SetupFinished = true;
        }

        getArray2 = getElementsAroundHighValue(concatStringInt, width, distances);
        
        for(int j = 0; j < distances.Length; j++)
        {
            if(!getArray2.Contains(j))
            {
                distances[j] = 0;
            }
        }

        int newMaxValue = (int) distances.Min();
        int newMaxIndex = (int)distances.ToList().IndexOf(distances.Min());
        concatStringInt = newMaxIndex;
        int yCoord = (int)(newMaxIndex / width);
        int xCoord = newMaxIndex % width;
        */
        int maxValue;
        int maxIndex;

        maxValue = (int) distances.Min();
        maxIndex = (int) distances.ToList().IndexOf(distances.Min());

        //int height = 424;
        int width = 512;

        //y-Coord
        int yCoord = (int)(maxIndex / width);
  
        //x-Coord
        int xCoord = (int)(maxIndex % width);
        //Debug.Log("x= " + xCoord);
        //Debug.Log("y= " + yCoord);
        int xCoordOffset = 0;
        int yCoordOffset = 0;

        xCoordOffset = (xCoord - lastXcoord);
        yCoordOffset = (yCoord - lastYcoord);
        //Debug.Log("xOffset = " + xCoordOffset);
        //Debug.Log("yOffset = " + yCoordOffset);
        lastXcoord = xCoord;
        lastYcoord = yCoord;        

        /*
        int offset = (lastDist - aktDist) * 2;
        if(offset > 400)
        {
            offset = 500;
        }

        lastDist = aktDist;
        */
        //Debug.Log(offset);
        
        if (distances == null)
        {
            return;
        }

        float ElapsedTime = 0.0f;
        float FinishTime = 60f;

        Vector3 oldPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        //Vector3 newPos = new Vector3(xCoord, gameObject.transform.position.y, yCoord);
        Vector3 newPos = new Vector3(gameObject.transform.position.x + xCoordOffset, 0, gameObject.transform.position.z + yCoordOffset);
        // 0.3f = maxDistanceDelta => Pro Call 0.3 Units in Richtung Ziel -> Optimal Zeitwert mitreinmultiplizieren TODO
        // eventuell Lerp benutzen
        ElapsedTime += Time.deltaTime;
        float start = Time.time;
       
        float length = Vector3.Distance(oldPos, newPos);
        //gameObject.transform.position = Vector3.Lerp(oldPos, newPos, ElapsedTime / FinishTime);
        //gameObject.transform.position = Vector3.MoveTowards(oldPos, newPos, latency * Time.deltaTime);
        gameObject.transform.position = newPos;
        //gameObject.transform.position = Vector3.MoveTowards(oldPos, newPos,0.2f);

        Debug.Log(oldPos + " old");
        Debug.Log(newPos + " new");

     
    } 
    
    public int[] checkForContrast(int indexOfLargestInArray, int largestInArray, int width, ushort[]array)
    {
        bool inArrayBoundsFirst = true;
        bool inArrayBoundsSecond = true;
        bool inArrayBoundsThird = true;
        int indexForTest = indexOfLargestInArray;
        int xValueInPicUpwards = 0;
        int xValueInPicDownwards = 0;
        int xValueInPicLeft = 0;
        int[] returnArray = null;

        while (inArrayBoundsFirst)
        {
            indexForTest = indexForTest - width;

            if (indexForTest < 0)
            {
                inArrayBoundsFirst = false;
                break;
            }

            int aktValue = array[indexForTest];
            int contrast = largestInArray - aktValue;

            if(contrast > 200)
            {
                xValueInPicUpwards = indexForTest;
                break;
            }
        }

        while(inArrayBoundsSecond)
        {
            indexForTest = indexForTest + width;

            if(indexForTest > array.Length)
            {
                inArrayBoundsSecond = false;
                break;
            }

            if(indexForTest > 217088)
            {
                indexForTest = 217087;
            }

            int aktValue2 = array[indexForTest];
            int contrast2 = largestInArray - aktValue2;

            if(contrast2 > 200)
            {
                xValueInPicDownwards = indexForTest;
                break;
            }
        }

        Debug.Log(xValueInPicDownwards + "Down");
        Debug.Log(xValueInPicUpwards + "Up");

        double indexOfNumberInbetweenConstrastDropsDouble =(xValueInPicDownwards + xValueInPicUpwards) / 2;

        Debug.Log(indexOfNumberInbetweenConstrastDropsDouble + " indexDropsDouble");

        int indexOfNumerInbetweenConstrastDrops = (int) Math.Ceiling(indexOfNumberInbetweenConstrastDropsDouble);

        double helper = indexOfNumerInbetweenConstrastDrops / 2;
        int rowInPicture = (int)Math.Floor(helper);

        while(inArrayBoundsThird)
        {
            indexOfNumerInbetweenConstrastDrops = indexOfNumerInbetweenConstrastDrops - 1;
            if(indexOfNumerInbetweenConstrastDrops% width == 0)
            {
                inArrayBoundsThird = false;
                break;
            }

            if(indexOfNumerInbetweenConstrastDrops > 217088)
            {
                indexOfNumerInbetweenConstrastDrops = 217087;
            }

            int aktValue3 = array[indexOfNumerInbetweenConstrastDrops];
            var contrast3 = largestInArray - aktValue3;

            if(contrast3 > 200)
            {
                xValueInPicLeft = indexOfNumerInbetweenConstrastDrops % width;
                break;
            }

            int xValueForTrade = xValueInPicLeft;
            int yValueForTrade = rowInPicture;

            returnArray[0] = xValueForTrade;
            returnArray[1] = yValueForTrade;
        }

        return returnArray;
    } 

    public int[] getElementsAroundHighValue(int indexOfLargestInArray, int width, ushort[] array)
    {
        int numSelectedRows = 50;
        int numSelectedCols = 50;
        int[] neededIndexes = null;

        int startingXValue = indexOfLargestInArray - 1 - width;

        for(int i=0; i < numSelectedRows; i++)
        {
            if(i==0)
            {
                for(int j = 0; j < numSelectedCols; j++)
                {
                    int index = startingXValue + j;
                    int length = neededIndexes.Length;
                    neededIndexes[length - 1] = index;
                }
            }
            else
            {
                startingXValue += width;
                for(int k = 0; k < numSelectedCols; k++)
                {
                    int index2 = startingXValue + k;
                    int length = neededIndexes.Length;
                    neededIndexes[length - 1] = index2;
                }
            }
        }

        return neededIndexes;
    }
}

