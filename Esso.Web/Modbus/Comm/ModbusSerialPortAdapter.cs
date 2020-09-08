using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace IndustrialNetwork.Modbus.Comm
{
    public class ModbusSerialPortAdapter
    {

        private const int READ_BUFFER_SIZE = 256; 

        private const int WRITE_BUFFER_SIZE = 256; 

        private byte[] bufferReceiver = null;

        private string bufferMsgReceiver = null;

        private SerialPort serialPort;

        public ModbusSerialPortAdapter(SerialPort serialPort)
        {
            bufferReceiver = new byte[READ_BUFFER_SIZE];
            this.serialPort = serialPort;
            this.serialPort.ReadTimeout = 3000;
            this.serialPort.WriteTimeout = 3000;
        }

        public void Connect()
        {
            if (this.serialPort.IsOpen) this.serialPort.Close();
            this.serialPort.Open();
        }

        public void Close()
        {
            if (this.serialPort.IsOpen) this.serialPort.Close();
        }

        public byte[] Read()
        {
                if (this.serialPort.BytesToRead >= 5)
                {
                    bufferReceiver = new byte[this.serialPort.BytesToRead];
                    int result = this.serialPort.Read(bufferReceiver, 0, this.serialPort.BytesToRead);
                    this.serialPort.DiscardInBuffer();
                }
            return bufferReceiver;
        }

        public string ReadLine()
        {
                if (this.serialPort.BytesToRead >= 11)
                {
                    bufferMsgReceiver = this.serialPort.ReadLine();
                    this.serialPort.DiscardInBuffer();
                }
            return bufferMsgReceiver;
        }

        public void Write(byte[] data, int offset, int size)
        {
            this.serialPort.Write(data, offset, size);
            this.serialPort.DiscardOutBuffer();
        }

        public void WriteLine(string data)
        {
            this.serialPort.WriteLine(data);
            this.serialPort.DiscardOutBuffer();
        }
    }
}
