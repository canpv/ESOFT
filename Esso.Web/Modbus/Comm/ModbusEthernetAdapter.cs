using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using IndustrialNetwork.Modbus.INException;

namespace IndustrialNetwork.Modbus.Comm
{
    public class ModbusEthernetAdapter
    {

        private const int READ_BUFFER_SIZE = 2048; // .

        private const int WRITE_BUFFER_SIZE = 2048; // .

        private byte[] bufferReceiver = null;
        private byte[] bufferSender = null;

        //private IPEndPoint server = null;

        private Socket mSocket = null;

        private string IP = "127.0.0.1";
        private int Port = 502;
        private int ConntectTimeout = 3000;


        public ModbusEthernetAdapter(string ip = "127.0.0.1", int port = 502)
        {
            this.IP = ip;
            this.Port = port;

        }

        public ModbusEthernetAdapter(string ip, short port, int conntectTimeout)
            : this(ip, port)
        {
            this.SetTimeout(conntectTimeout);
        }

        public void Connect()
        {
            try
            {
                this.mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.bufferReceiver = new byte[READ_BUFFER_SIZE];
                this.bufferSender = new byte[WRITE_BUFFER_SIZE];
                this.mSocket.SendBufferSize = READ_BUFFER_SIZE;
                this.mSocket.ReceiveBufferSize = WRITE_BUFFER_SIZE;
                IPEndPoint server = new IPEndPoint(IPAddress.Parse(this.IP), this.Port);
                this.mSocket.Connect(server);
                ////this.mSocket.NoDelay = false;
                //newsock.BeginConnect(server, new AsyncCallback(Connected), newsock);                
            }
            catch (SocketException ex)
            {
                throw ex;
            }

        }

        public void Close()
        {
            if (this.mSocket == null) return;
            if (this.mSocket.Connected)
            {
                this.mSocket.Close();
            }
        }

        public void SetTimeout(int conntectTimeout)
        {
            this.ConntectTimeout = conntectTimeout;
        }

        public int Write(byte[] frame)
        {
            return this.mSocket.Send(frame, frame.Length, SocketFlags.None);
        }

        public byte[] Read()
        {
            NetworkStream ns = new NetworkStream(this.mSocket);

            if (ns.CanRead)
            {
                this.mSocket.ReceiveTimeout = 10000;//TODO
                int rs = this.mSocket.Receive(this.bufferReceiver, this.bufferReceiver.Length, SocketFlags.None);
            }
            return this.bufferReceiver;
        }





        private Socket client;

        private void Connected(IAsyncResult iar)
        {
            client = (Socket)iar.AsyncState;
            try
            {
                client.EndConnect(iar);
                //conStatus.Text = "Connected to: " + client.RemoteEndPoint.ToString();
                client.BeginReceive(bufferReceiver, 0, READ_BUFFER_SIZE, SocketFlags.None,
                              new AsyncCallback(ReceiveData), client);
            }
            catch (SocketException ex)
            {
                throw ex;
            }
        }

        private void ReceiveData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int recv = remote.EndReceive(iar);
            //string stringData = Encoding.ASCII.GetString(data, 0, recv);
            //results.Items.Add(stringData);
        }

        private void SendData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
            remote.BeginReceive(this.bufferSender, 0, WRITE_BUFFER_SIZE, SocketFlags.None,
                          new AsyncCallback(ReceiveData), remote);
        }

    }
}
