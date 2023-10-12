using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;

// Class responsible for detecting when the user confirms their selection. Implemented with UDP Client
public class SelectConfirmer : MonoBehaviour
{
    [SerializeField]
    private int port = 8999;

    private UdpClient _udpClient;
    private IPEndPoint _endPoint;

    private Thread th;
    private bool running = true;
    
    // Mutex object
    public System.Object obj;
    public bool selected = false;
    public bool frameHasPassed = false;

    // Start is called before the first frame update
    void Start()
    {
        obj = new System.Object();
        _endPoint = new IPEndPoint(IPAddress.Any, 0);
        _udpClient = new UdpClient(port);
        th = new Thread(ReceiveClickUDP);
        th.IsBackground = true;
        th.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Holds the user's click for 2 frames so that it can surely be detected by Selection Manager
        lock(obj)
        {
            if (frameHasPassed)
            {
                frameHasPassed = false;
                selected = false;
            }
            else if (selected)
            {
                frameHasPassed = true;
            }
        }
    }

    private void ReceiveClickUDP()
    {
        while (running)
        {
            try
            {
                byte[] received = _udpClient.Receive(ref _endPoint);
                byte[] actionData = new byte[1];
                actionData[0] = received[16];
                string action = Encoding.UTF8.GetString(actionData);
                // If the input action is the user lifting their finger, select
                if (action == "U")
                {
                    lock(obj)
                    {
                        selected = true;
                    }
                }
            }
            catch (SocketException e)
            {
                Debug.Log($"Thread probably interrupted. No worries, though! {e}");
            }
        }
    }
}
