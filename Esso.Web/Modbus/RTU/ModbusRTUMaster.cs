using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IndustrialNetwork.Modbus.Comm;
using System.IO.Ports;
using System.Timers;
using System.Diagnostics;

namespace IndustrialNetwork.Modbus.RTU
{
    public partial class ModbusRTUMaster : ModbusRTUMessage, IModbusMaster
    {
        private const int DELAY = 100;// delay 100 ms
        private ModbusSerialPortAdapter SerialAdaper = null;
        public ModbusRTUMaster(string portName = "COM1", int baudRate = 96000, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            this.SerialAdaper = new ModbusSerialPortAdapter(new SerialPort(portName, baudRate, parity, dataBits, stopBits));
        }
        public ModbusRTUMaster(ModbusSerialPortAdapter iModbusSerialPortAdapter = null)
        {
            this.SerialAdaper = iModbusSerialPortAdapter;
        }

        public void SetModbusSerialPortAdapter(ModbusSerialPortAdapter iModbusSerialPortAdapter)
        {
            SerialAdaper = iModbusSerialPortAdapter;
        }

        public void Connection()
        {
            this.SerialAdaper.Connect();
        }

        public void Disconnection()
        {
            this.SerialAdaper.Close();
        }

        public byte[] ReadCoilStatus(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            byte[] frame = this.ReadCoilStatusMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.SerialAdaper.Write(frame, 0, frame.Length);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.SerialAdaper.Read();
            if (buffReceiver.Length == 5) this.ModbusExcetion(buffReceiver);
            byte[] data = new byte[buffReceiver.Length - 5];
            Array.Copy(buffReceiver, 3, data, 0, data.Length);
            return IndustrialNetwork.Modbus.DataType.Boolean.ToByteArray(IndustrialNetwork.Modbus.DataType.Boolean.ToArray(data));
        }

        public byte[] ReadHoldingRegisters(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            byte[] frame = this.ReadHoldingRegistersMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.SerialAdaper.Write(frame, 0, frame.Length);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.SerialAdaper.Read();
            if (buffReceiver.Length == 5) this.ModbusExcetion(buffReceiver);
            byte[] data = new byte[buffReceiver.Length - 5];
            Array.Copy(buffReceiver, 3, data, 0, data.Length);
            return data;
        }

        public byte[] ReadInputRegisters(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            byte[] frame = this.ReadInputRegistersMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.SerialAdaper.Write(frame, 0, frame.Length);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.SerialAdaper.Read();
            if (buffReceiver.Length == 5) this.ModbusExcetion(buffReceiver);
            byte[] data = new byte[buffReceiver.Length - 5];
            Array.Copy(buffReceiver, 3, data, 0, data.Length);
            return data;
        }

        public byte[] ReadInputStatus(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            byte[] frame = this.ReadInputStatusMessage(slaveAddress, startAddress, nuMBErOfPoints);
            this.SerialAdaper.Write(frame, 0, frame.Length);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.SerialAdaper.Read();
            if (buffReceiver.Length == 5) this.ModbusExcetion(buffReceiver);
            byte[] data = new byte[buffReceiver.Length - 5];
            Array.Copy(buffReceiver, 3, data, 0, data.Length);
            return IndustrialNetwork.Modbus.DataType.Boolean.ToByteArray(IndustrialNetwork.Modbus.DataType.Boolean.ToArray(data));
        }

        public byte[] WriteMultipleCoils(byte slaveAddress, string startAddress, bool[] values)
        {
            byte[] frame = this.WriteMultipleCoilsMessage(slaveAddress, startAddress, values);
            this.SerialAdaper.Write(frame, 0, frame.Length);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.SerialAdaper.Read();
            if (buffReceiver.Length == 5) this.ModbusExcetion(buffReceiver);
            return buffReceiver;
        }

        public byte[] WriteMultipleRegisters(byte slaveAddress, string startAddress, byte[] values)
        {
            byte[] frame = this.WriteMultipleRegistersMessage(slaveAddress, startAddress, values);
            this.SerialAdaper.Write(frame, 0, frame.Length);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.SerialAdaper.Read();
            if (buffReceiver.Length == 5) this.ModbusExcetion(buffReceiver);
            return buffReceiver;
        }

        public byte[] WriteSingleCoil(byte slaveAddress, string startAddress, bool value)
        {
            byte[] frame = this.WriteSingleCoilMessage(slaveAddress, startAddress, value);
            this.SerialAdaper.Write(frame, 0, frame.Length);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.SerialAdaper.Read();
            if (buffReceiver.Length == 5) this.ModbusExcetion(buffReceiver);
            return buffReceiver;
        }

        public byte[] WriteSingleRegister(byte slaveAddress, string startAddress, byte[] values)
        {
            byte[] frame = this.WriteSingleRegisterMessage(slaveAddress, startAddress, values);
            this.SerialAdaper.Write(frame, 0, frame.Length);
            Thread.Sleep(DELAY);
            byte[] buffReceiver = this.SerialAdaper.Read();
            if (buffReceiver.Length == 5) this.ModbusExcetion(buffReceiver);
            return buffReceiver;
        }
    }
}
