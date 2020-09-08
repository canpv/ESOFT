using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace IndustrialNetwork.Modbus.Comm
{
    public interface IModbusMaster
    {

        void SetModbusSerialPortAdapter(ModbusSerialPortAdapter iModbusSerialPortAdapter);

        void Connection();

        void Disconnection();

        byte[] ReadCoilStatus(byte slaveAddress, string startAddress, ushort nuMBErOfPoints);

        byte[] ReadInputStatus(byte slaveAddress, string startAddress, ushort nuMBErOfPoints);

        byte[] ReadHoldingRegisters(byte slaveAddress, string startAddress, ushort nuMBErOfPoints);

        byte[] ReadInputRegisters(byte slaveAddress, string startAddress, ushort nuMBErOfPoints);

        byte[] WriteSingleCoil(byte slaveAddress, string startAddress, bool value);

        byte[] WriteMultipleCoils(byte slaveAddress, string startAddress, bool[] values);

        byte[] WriteSingleRegister(byte slaveAddress, string startAddress, byte[] values);

        byte[] WriteMultipleRegisters(byte slaveAddress, string startAddress, byte[] values);
    }
}
