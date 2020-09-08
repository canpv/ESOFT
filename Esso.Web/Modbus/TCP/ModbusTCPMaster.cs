using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IndustrialNetwork.Modbus.Comm;
using System.Net.Sockets;
using System.Net;

namespace IndustrialNetwork.Modbus.TCP
{
    public partial class ModbusTCPMaster : ModbusTCPMessage, IModbusMaster
    {
        private const int DELAY = 10;
        private ModbusEthernetAdapter EthernetAdaper = null;
        private ModbusSerialPortAdapter SerialAdaper = null;

        public ModbusTCPMaster() { }

        public ModbusTCPMaster(string ip, int port)
        {
            this.EthernetAdaper = new ModbusEthernetAdapter(ip, port);
        }

        public ModbusTCPMaster(string ip, short port, int connectTimeout)
        {
            this.EthernetAdaper = new ModbusEthernetAdapter(ip, port, connectTimeout);
        }

        public void Connection()
        {
            try
            {
                this.EthernetAdaper.Connect();
            }
            catch (System.Net.Sockets.SocketException ex)
            {
               
                throw ex;
            }            
        }

        public void Disconnection()
        {
            this.EthernetAdaper.Close();
        }

        public byte[] ReadCoilStatus(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            byte[] frame = this.ReadCoilStatusMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.EthernetAdaper.Write(frame);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.EthernetAdaper.Read();
            if (FUNCTION_01 != buffReceiver[7])
            {
                byte[] errorbytes = new byte[3];
                Array.Copy(buffReceiver, 6, errorbytes, 0, errorbytes.Length);
                this.ModbusExcetion(errorbytes);
            }
            int SizeByte = buffReceiver[8]; // Số lượng byte dữ liệu thu được.
            byte[] data = new byte[SizeByte];
            Array.Copy(buffReceiver, 9, data, 0, data.Length); // Dữ liệu cần lấy bắt đầu từ byte có chỉ số 9 trong buffReceive.            
            return IndustrialNetwork.Modbus.DataType.Boolean.ToByteArray(IndustrialNetwork.Modbus.DataType.Boolean.ToArray(data)); 
        }

        public byte[] ReadInputStatus(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            byte[] frame = this.ReadInputStatusMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.EthernetAdaper.Write(frame);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.EthernetAdaper.Read();
            if (FUNCTION_02 != buffReceiver[7])
            {
                byte[] errorbytes = new byte[3];
                Array.Copy(buffReceiver, 6, errorbytes, 0, errorbytes.Length);
                this.ModbusExcetion(errorbytes);
            }
            int SizeByte = buffReceiver[8]; // Số lượng byte dữ liệu thu được.
            byte[] data = new byte[SizeByte];
            Array.Copy(buffReceiver, 9, data, 0, data.Length); // Dữ liệu cần lấy bắt đầu từ byte có chỉ số 9 trong buffReceive.            
            return IndustrialNetwork.Modbus.DataType.Boolean.ToByteArray(IndustrialNetwork.Modbus.DataType.Boolean.ToArray(data)); 
        }

        public byte[] ReadHoldingRegisters(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            byte[] frame = this.ReadHoldingRegistersMessage(slaveAddress, startAddress, nuMBErOfPoints);
           
            this.EthernetAdaper.Write(frame);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.EthernetAdaper.Read();
            if (FUNCTION_03 != buffReceiver[7])
            {
                byte[] errorbytes = new byte[3];
                Array.Copy(buffReceiver, 6, errorbytes, 0, errorbytes.Length);
                this.ModbusExcetion(errorbytes);
            }
            int SizeByte = buffReceiver[8]; // Số lượng byte dữ liệu thu được.
            byte[] data = new byte[SizeByte];
            Array.Copy(buffReceiver, 9, data, 0, data.Length); // Dữ liệu cần lấy bắt đầu từ byte có chỉ số 9 trong buffReceive.            
            return data; 
        }

        public byte[] ReadInputRegisters(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            byte[] frame = this.ReadInputRegistersMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.EthernetAdaper.Write(frame);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.EthernetAdaper.Read();
            if (FUNCTION_04 != buffReceiver[7])
            {
                byte[] errorbytes = new byte[3];
                Array.Copy(buffReceiver, 6, errorbytes, 0, errorbytes.Length);
                this.ModbusExcetion(errorbytes);
            }
            int SizeByte = buffReceiver[8]; // Số lượng byte dữ liệu thu được.
            byte[] data = new byte[SizeByte];
            Array.Copy(buffReceiver, 9, data, 0, data.Length); // Dữ liệu cần lấy bắt đầu từ byte có chỉ số 9 trong buffReceive.            
            return data; 
        }

        public byte[] WriteSingleCoil(byte slaveAddress, string startAddress, bool value)
        {
            byte[] frame = this.WriteSingleCoilMessage(slaveAddress, startAddress, value);
            this.EthernetAdaper.Write(frame);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.EthernetAdaper.Read();
            if (FUNCTION_05 != buffReceiver[7])
            {
                byte[] errorbytes = new byte[3];
                Array.Copy(buffReceiver, 6, errorbytes, 0, errorbytes.Length);
                this.ModbusExcetion(errorbytes);
            }
            return buffReceiver;
        }

        public byte[] WriteMultipleCoils(byte slaveAddress, string startAddress, bool[] values)
        {
            byte[] frame = this.WriteMultipleCoilsMessage(slaveAddress, startAddress, values);
            this.EthernetAdaper.Write(frame);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.EthernetAdaper.Read();
            if (FUNCTION_15 != buffReceiver[7])
            {
                byte[] errorbytes = new byte[3];
                Array.Copy(buffReceiver, 6, errorbytes, 0, errorbytes.Length);
                this.ModbusExcetion(errorbytes);
            }
            return buffReceiver;
        }

        public byte[] WriteSingleRegister(byte slaveAddress, string startAddress, byte[] values)
        {
            byte[] frame = this.WriteSingleRegisterMessage(slaveAddress, startAddress, values);
            this.EthernetAdaper.Write(frame);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.EthernetAdaper.Read();
            if (FUNCTION_06 != buffReceiver[7])
            {
                byte[] errorbytes = new byte[3];
                Array.Copy(buffReceiver, 6, errorbytes, 0, errorbytes.Length);
                this.ModbusExcetion(errorbytes);
            }
            return buffReceiver;
        }

        public byte[] WriteMultipleRegisters(byte slaveAddress, string startAddress, byte[] values)
        {
            byte[] frame = this.WriteMultipleRegistersMessage(slaveAddress, startAddress, values);

            this.EthernetAdaper.Write(frame);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.EthernetAdaper.Read();
            if (FUNCTION_16 != buffReceiver[7])
            {
                byte[] errorbytes = new byte[3];
                Array.Copy(buffReceiver, 6, errorbytes, 0, errorbytes.Length);
                this.ModbusExcetion(errorbytes);
            }
            return buffReceiver;
        }


        public void SetModbusSerialPortAdapter(ModbusSerialPortAdapter iModbusSerialPortAdapter)
        {
            SerialAdaper = iModbusSerialPortAdapter;
        }

        public void sModbusEthernetAdapter(ModbusEthernetAdapter iModbusEthernetAdapter)
        {
            EthernetAdaper = iModbusEthernetAdapter;
        }

    }
}
