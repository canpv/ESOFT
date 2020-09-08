using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IndustrialNetwork.Modbus.Comm;
using System.IO.Ports;

namespace IndustrialNetwork.Modbus.ASCII
{
    public partial class ModbusASCIIMaster : ModbusASCIIMessage, IModbusMaster
    {
        private const int DELAY = 100;// delay 100 ms
        private ModbusSerialPortAdapter SerialAdaper = null;
        private ModbusEthernetAdapter EthernetAdaper = null;

        public void Connection()
        {
            this.SerialAdaper.Connect();
        }

        public void Disconnection()
        {
            this.SerialAdaper.Close();
        }

        public ModbusASCIIMaster(string portName = "COM1", int baudRate = 96000, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            this.SerialAdaper = new ModbusSerialPortAdapter(new SerialPort(portName, baudRate, parity, dataBits, stopBits));
        }

        public byte[] ReadCoilStatus(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            string frame = this.ReadCoilStatusMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.SerialAdaper.WriteLine(frame);
            Thread.Sleep(DELAY);
            string buffReceiver = this.SerialAdaper.ReadLine();
            string tempStrg = buffReceiver.Substring(1, buffReceiver.Length - 2);
            byte[] messageReceived = Conversion.HexToBytes(tempStrg);
            if (buffReceiver.Length == 10) this.ModbusExcetion(messageReceived);
            byte[] data = new byte[messageReceived[2]];
            Array.Copy(messageReceived, 3, data, 0, data.Length);
            return IndustrialNetwork.Modbus.DataType.Boolean.ToByteArray(IndustrialNetwork.Modbus.DataType.Boolean.ToArray(data));
        }

        public byte[] ReadHoldingRegisters(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            string frame = this.ReadHoldingRegistersMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.SerialAdaper.WriteLine(frame);
            Thread.Sleep(DELAY);
            string buffReceiver = this.SerialAdaper.ReadLine();
            if (string.IsNullOrEmpty(buffReceiver)) return new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            string tempStrg = buffReceiver.Substring(1, buffReceiver.Length - 2);
            byte[] messageReceived = Conversion.HexToBytes(tempStrg);
            if (buffReceiver.Length == 10) this.ModbusExcetion(messageReceived);
            byte[] data = new byte[messageReceived[2]];
            Array.Copy(messageReceived, 3, data, 0, data.Length);
            return data;
        }

        public byte[] ReadInputRegisters(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            string frame = this.ReadInputRegistersMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.SerialAdaper.WriteLine(frame);
            Thread.Sleep(DELAY);
            string buffReceiver = this.SerialAdaper.ReadLine();
            string tempStrg = buffReceiver.Substring(1, buffReceiver.Length - 2);
            byte[] messageReceived = Conversion.HexToBytes(tempStrg);
            if (buffReceiver.Length == 10) this.ModbusExcetion(messageReceived);
            byte[] data = new byte[messageReceived[2]];
            Array.Copy(messageReceived, 3, data, 0, data.Length);
            return data;
        }

        public byte[] ReadInputStatus(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            string frame = this.ReadInputStatusMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.SerialAdaper.WriteLine(frame);
            Thread.Sleep(DELAY);
            string buffReceiver = this.SerialAdaper.ReadLine();
            string tempStrg = buffReceiver.Substring(1, buffReceiver.Length - 2);
            byte[] messageReceived = Conversion.HexToBytes(tempStrg);
            if (buffReceiver.Length == 10) this.ModbusExcetion(messageReceived);
            byte[] data = new byte[messageReceived[2]];
            Array.Copy(messageReceived, 3, data, 0, data.Length);
            return IndustrialNetwork.Modbus.DataType.Boolean.ToByteArray(IndustrialNetwork.Modbus.DataType.Boolean.ToArray(data));
        }

        public void SetModbusSerialPortAdapter(ModbusSerialPortAdapter iModbusSerialPortAdapter)
        {
            SerialAdaper = iModbusSerialPortAdapter;
        }

        public byte[] WriteMultipleCoils(byte slaveAddress, string startAddress, bool[] values)
        {
            string frame = this.WriteMultipleCoilsMessage(slaveAddress, startAddress, values);
            this.SerialAdaper.WriteLine(frame);
            Thread.Sleep(DELAY);
            string buffReceiver = this.SerialAdaper.ReadLine();
            string tempStrg = buffReceiver.Substring(1, buffReceiver.Length - 2);
            byte[] data = Conversion.HexToBytes(tempStrg);
            if (buffReceiver.Length == 10) this.ModbusExcetion(data);
            return data;
        }

        public byte[] WriteMultipleRegisters(byte slaveAddress, string startAddress, byte[] values)
        {
            string frame = this.WriteMultipleRegistersMessage(slaveAddress, startAddress, values);
            this.SerialAdaper.WriteLine(frame);
            Thread.Sleep(DELAY);
            string buffReceiver = this.SerialAdaper.ReadLine();
            string tempStrg = buffReceiver.Substring(1, buffReceiver.Length - 2);
            byte[] data = Conversion.HexToBytes(tempStrg);
            if (buffReceiver.Length == 10) this.ModbusExcetion(data);
            return data;
        }

        public byte[] WriteSingleCoil(byte slaveAddress, string startAddress, bool value)
        {
            string frame = this.WriteSingleCoilMessage(slaveAddress, startAddress, value);
            this.SerialAdaper.WriteLine(frame);
            Thread.Sleep(DELAY);
            string buffReceiver = this.SerialAdaper.ReadLine();
            string tempStrg = buffReceiver.Substring(1, buffReceiver.Length - 2);
            byte[] data = Conversion.HexToBytes(tempStrg);
            if (buffReceiver.Length == 10) this.ModbusExcetion(data);
            return data;
        }

        public byte[] WriteSingleRegister(byte slaveAddress, string startAddress, byte[] values)
        {
            string frame = this.WriteSingleRegisterMessage(slaveAddress, startAddress, values);
            this.SerialAdaper.WriteLine(frame);
            Thread.Sleep(DELAY);
            string buffReceiver = this.SerialAdaper.ReadLine();
            string tempStrg = buffReceiver.Substring(1, buffReceiver.Length - 2);
            byte[] data = Conversion.HexToBytes(tempStrg);
            if (buffReceiver.Length == 10) this.ModbusExcetion(data);
            return data;
        }
    }
}
