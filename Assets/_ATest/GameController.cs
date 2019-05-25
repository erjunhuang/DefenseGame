using LitJson;
using QGame.Core.Event;
using QGame.Core.Model;
using QGame.Core.Net;
using QGame.Core.XProto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TowerDefense.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : DDOLSingleton<GameController>
{
    private readonly HttpClient _client = new HttpClient();
    public Button btn;
    // Use this for initialization
    void Awake () {
        //显示UI
        new TestOne().showPanel_();
        //事件模拟
        //StartCoroutine(NetUpdateGold());

        EventTriggerListener.Get(btn.gameObject).onClick = OnBtnClick;

        TcpClient();

        Socket();
    }


     void Update()
    {
        GameSocket.Instance.Process();
    }
    void TcpClient() {
        try
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("mid", "131224");
            parameters.Add("shareType", "11");

            StringBuilder buffer = new StringBuilder();
            if (!(parameters == null || parameters.Count == 0))
            {

                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }

            }
            byte[] bytes = Encoding.UTF8.GetBytes(buffer.ToString());
            _client.Post("http://qgtest.ode.cn/game/share.php?appid=1010&lmode=4#", bytes);
            //_client.Get("http://qgtest.ode.cn/game/share.php?appid=1010&shareType=11&mid=144701&lmode=4#");
        }
        catch (Exception)
        {
            Debug.Log("连接失败");
            return;
        }
        _client.CompletedEvent += delegate (HttpClient sender, Exception exception) {
            Debug.Log(_client.Response.Text);
            JsonData jsonData = JsonMapper.ToObject(_client.Response.Text);
        };
    }

    void Socket() {
        //192.168.1.158 11020
        //("45.40.192.88", 8888
        //GameSocket.Instance.Connect("192.168.1.158", 11020, 1);

        //GameSocket.Instance.Connect("45.40.192.88", 8888, 10);
        GameSocket.Instance.Connect("127.0.0.1", 10002, 10);
        Debug.Log(GameSocket.Instance.Connected);
        StartCoroutine(wait());
    }
    
    IEnumerator wait() {
        yield return new WaitForSeconds(1f);
        LoginVerifyReq loginVerifyReq = new LoginVerifyReq();
        loginVerifyReq.sesskey = "10748-1553675973-78664470d5fb3562bf6a1f9ec208eb43";
        loginVerifyReq.terminalType = 10;
        loginVerifyReq.uid = 10748;
        GameSocket.Instance.Send(0x1701, loginVerifyReq, messageCallback);
    }
    private void messageCallback(int result, GameResponse resp)
    {
        Debug.Log("result:" + result);
    }

    void OnBtnClick(PointerEventData eventData, GameObject go) {
        Debug.Log("点了我一下");
    }
    private IEnumerator NetUpdateGold()
    {
        int gold = 0;
        while (true)
        {
            gold++;
            yield return new WaitForSeconds(1.0f);
            //XEventBus.Instance.Post(EventId.AboveReturnClick, gold);
        }
    }

    void TestResManager()
    {

        float time = Environment.TickCount;
        for (int i = 1; i < 1000; i++)
        {

            //GameObject go = null;
            //直接加载
            //go = Instantiate(Resources.Load<GameObject>("Prefabs/Cube"));  //1
            //go.transform.position = UnityEngine.Random.insideUnitSphere * 20;

            //2正常加载
            //,()=> {Debug.Log("加载进度成功");}
            //go = Instantiate(ResManager.Instance.LoadInstance("Prefabs/Cube")) as GameObject;
            //go.transform.position = UnityEngine.Random.insideUnitSphere * 20;


            ////4、协程加载
            //ResManager.Instance.LoadCoroutine("Prefabs/Cube", (_obj) =>
            //{
            //    go = Instantiate(_obj) as GameObject;
            //    go.transform.position = UnityEngine.Random.insideUnitSphere * 20;
            //});
        }
    }

    private void OnDestroy()
    {
        GameSocket.Instance.close();
    }
}
