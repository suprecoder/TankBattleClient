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
    bool gamestart=false;
    public Transform player;
    PlayerInfo pinfo;
    int[] playerid = new int[4];
    int playernum = 0;
    void addPlayer(PlayerInfo p)
    {
        if (p==null || p.users==null || p.users.Count < 1) return;
        //Debug.Log(p.users.Count);
        PlayerRoot.myself = p.users[0].Id;
        Debug.Log("myself"+ PlayerRoot.myself);

        for(int i = 0; i < p.users.Count; i++)
        {
            GameObject o;
            if (!PlayerRoot.dic.TryGetValue(p.users[i].Id,out o))
            {
                Debug.Log("创建gameobject：" + p.users[i].Id);
                PlayerRoot.dic[p.users[i].Id] = GameObject.Instantiate<GameObject>(player.gameObject,transform);
                PlayerRoot.dic[p.users[i].Id].SetActive(true);
                if(PlayerRoot.myself== p.users[i].Id)
                {
                    Debug.Log("myobj" + p.users[i].Id);
                    PlayerRoot.obj_me = PlayerRoot.dic[p.users[i].Id];
                }
                playerid[playernum] = p.users[i].Id;
                playernum++;
            }      
        }

    }
    // Use this for initialization
    void Start()
    {
        Debug.Log("start");
        //GameObject other = GameObject.Instantiate<GameObject>(gameObject);

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
            // Debug.Log("rec length" + msg.Length);
            byte[] a = new byte[msg.Length - 4];
            for (int i = 4; i < msg.Length; i++)
            {
                a[i - 4] = msg[i];
            }
            //Debug.Log(a);
            string s = System.Text.Encoding.ASCII.GetString(a);
           // Debug.Log(s);
            pinfo = PlayerInfo.CreateFromJSON(s);
            gamestart = true;
            /*
            pos = new Vector3[p.users.Count];
            for (int i = 1; i < p.users.Count; i++)
            {
                pos[i].x = p.users[i].X;
                pos[i].y = p.users[i].Id+0.1f;
                pos[i].z = p.users[i].Y;
            }
            pinfo=p;*/
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
        addPlayer(pinfo);
        Move();
    }
    void Move()
    {
        if (!gamestart) return;
        //Debug.Log("length" + pos.Length);
        for (int i = 0; i < playernum; i++)
        {
            GameObject g;
            if (PlayerRoot.dic.TryGetValue(playerid[i], out g))
            {
                Vector3 pos=new Vector3(0,0,0);
                
                //GameObject g = PlayerRoot.dic[(int)pos[i].y];
                for(int j = 0; j < pinfo.users.Count; j++)
                {
                    if (pinfo.users[j].Id == playerid[i])
                    {
                        pos.x = pinfo.users[j].X;
                        pos.y = g.transform.localPosition.y;
                        pos.z = pinfo.users[j].Y;
                        float speed = 5;
                        float step = speed * Time.deltaTime;
                        g.transform.localPosition = new Vector3(Mathf.Lerp(g.transform.localPosition.x, pos.x, step), Mathf.Lerp(g.transform.localPosition.y, pos.y, step), Mathf.Lerp(g.transform.localPosition.z, pos.z, step));
        
                    }
                }
               // Debug.Log("" + pos.x + "," + pos.z);
                
            }
            else
            {
                Debug.Log(""+i+":"+pinfo.users[i].Id);
            }
        }
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