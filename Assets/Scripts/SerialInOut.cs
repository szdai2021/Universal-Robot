using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;


public class SerialInOut : MonoBehaviour
{
    public string COM = "COM5";
    public int value;
    public int SendVal = -200; // ForTesting
    public int Button;
    public int speed = 0;
    public int rotaryValue = 0;

    private List<int> speedList = new List<int>();

    SerialPort sp;
    Thread ReadThread;

    void Start()
    {
        sp = new SerialPort(COM, 115200);
        sp.ReadTimeout = 2000;
        sp.WriteTimeout = 2000;
        sp.WriteTimeout = 2000;
        sp.Parity = Parity.None;
        sp.DataBits = 8;
        sp.StopBits = StopBits.One;
        sp.RtsEnable = true;
        sp.Handshake = Handshake.None;
        sp.NewLine = "\n";  // Need this or ReadLine() fails

        try
        {
            sp.Open();
        }
        catch (SystemException f)
        {
            print("FAILED TO OPEN PORT");

        }
        if (sp.IsOpen)
        {
            print("SerialOpen!");

            ReadThread = new Thread(new ThreadStart(ReadSerial));
            ReadThread.Start();

        }
    }
    void ReadSerial()
    {
        while (ReadThread.IsAlive)
        {
            try
            {
                if (sp.BytesToRead > 1)
                {

                    string indata = sp.ReadLine();
                    string[] splits = indata.Split(' ');
                    value = int.Parse(splits[0]);

                    Button = int.Parse(splits[1]);

                    rotaryValue = int.Parse(splits[2]);
                }
            }
            catch (SystemException f)
            {
                print(f);
                ReadThread.Abort();
            }

        }
    }

    public void SetSlider(int val)
    {
        try
        {
            SendVal = val;
            sp.WriteLine(val.ToString());
        }
        catch (SystemException f)
        {
            print("ERROR::A message failed to send");
        }
    }

    public void SetSliderRalative(int val)
    {
        try
        {
            SendVal += val;
            sp.WriteLine(val.ToString());
        }
        catch (SystemException f)
        {
            print("ERROR::A message failed to send");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetSlider(0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetSlider(SendVal);
        }

        if (speedList.Count < 3)
        {
            speed = 0;
            speedList.Add(value);
        }
        else
        {
            speedList.RemoveAt(0);
            speedList.Add(value);

            if ((speedList[1] > speedList[0] & speedList[1] < speedList[2]) | (speedList[1] < speedList[0] & speedList[1] > speedList[2]))
            {
                speed = (int)Mathf.Round(((speedList[2] - speedList[1]) + (speedList[1] - speedList[0])) / 2.0f);
            }
            else
            {
                speed = 0;
            }
        }
    }
        void OnApplicationQuit()
        {
            ReadThread.Abort();
        }
    }
