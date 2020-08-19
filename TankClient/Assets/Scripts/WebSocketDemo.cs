using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using HybridWebSocket;
using System;

[System.Serializable]
public class cuser
{
    public int Id;
    public float X;
    public float Y;
}
[System.Serializable]
public class PlayerInfo
{
    public List<cuser> users;
    public List<int> outter;

    public static PlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonString);
    }

    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.
}
public class WebSocketDemo : MonoBehaviour
{

    WebSocket ws = WebSocketFactory.CreateInstance("ws://localhost:9001");
    Vector3 pos = new Vector3(0,0,0);


    // Use this for initialization
    void Start()
    {

        // Create WebSocket instance

        // Add OnOpen event listener
        ws.OnOpen += () =>
        {
            Debug.Log("WS connected!");
            Debug.Log("WS state: " + ws.GetState().ToString());

            //ws.Send(Encoding.UTF8.GetBytes("Hello from Unity 3D!"));
        };

        // Add OnMessage event listener
        ws.OnMessage += (byte[] msg) =>
        {
            Debug.Log("rec length" + msg.Length);
            byte[] a = new byte[msg.Length - 4];
            for (int i = 4; i < msg.Length; i++)
            {
                a[i - 4] = msg[i];
            }
            string s = System.Text.Encoding.ASCII.GetString(a);
            PlayerInfo p = PlayerInfo.CreateFromJSON(s);
            pos.x = p.users[0].X;
            pos.z = p.users[0].Y;
            //ws.Close();
        };

        // Add OnError event listener
        ws.OnError += (string errMsg) =>
        {
            Debug.Log("WS error: " + errMsg);
        };

        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log("WS closed with code: " + code.ToString());
        };

        // Connect to the server
        ws.Connect();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        { 
            sendMove(0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            sendMove(270);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            sendMove(180);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            sendMove(90);
        }
        Move();
    }
    void Move()
    {
        pos.x= GetComponent<Transform>().position.y+pos.x;
        pos.y = GetComponent<Transform>().position.y;
        pos.z=GetComponent<Transform>().position.y+pos.z;

        float speed = 5;
        float step = speed * Time.deltaTime;
        gameObject.transform.localPosition = new Vector3(Mathf.Lerp(gameObject.transform.localPosition.x, pos.x, step), Mathf.Lerp(gameObject.transform.localPosition.y, pos.y, step), Mathf.Lerp(gameObject.transform.localPosition.z, pos.z, step));
        //GetComponent<Transform>().position = pos;
    }
    void sendMove(int angle)
    {
        while (ws.GetState() != WebSocketState.Open)
        {
            //Debug.Log("un ready");
        }
        Debug.Log("-----------------------" + ws.GetState());
        byte[] bytes = new byte[12];
        bytes[0] = 0;
        bytes[1] = 0;
        bytes[2] = 1;
        bytes[3] = 0;
        bytes[4] = 0;
        bytes[5] = 0;
        bytes[6] = 1;
        bytes[7] = 0; //header
        byte[] ag=intToBytes(angle);
        for (int i = 8; i < 12; i++)
        {
            bytes[i] = ag[i-8];
        }
        ws.Send(bytes);
    }
    public byte[] intToBytes(int value)
    {
        byte[] src = new byte[4];
        src[3] = (byte)((value >> 24) & 0xFF);
        src[2] = (byte)((value >> 16) & 0xFF);
        src[1] = (byte)((value >> 8) & 0xFF);//高8位
        src[0] = (byte)(value & 0xFF);//低位
        return src;
    }

}