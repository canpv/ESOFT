using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndustrialNetwork.Modbus.Comm;

namespace IndustrialNetwork.Modbus.ASCII
{
    public partial class ModbusASCIIMessage : BaseMessage
    {
        private const char Header = ':';// Start(:)
        private const char CR = '\r';  // CR
        private const char LF = '\n';  // LF
        private string Trailer = string.Format("{0}{1}", CR, LF);  // End(CRLF)

        private string Read(byte slaveAddress, ushort startAddress, byte functionCode, uint nuMBErOfPoints)
        {
            string frame = string.Format("{0:X2}", slaveAddress);
            frame += string.Format("{0:X2}", functionCode);
            frame += string.Format("{0:X4}", startAddress);
            frame += string.Format("{0:X4}", nuMBErOfPoints);
            byte[] bytes = Conversion.HexToBytes(frame);
            byte lrc = LRC(bytes);
            return Header + frame + lrc.ToString("X2") + Trailer;
        }

        private string Write(byte slaveAddress, ushort startAddress, byte functionCode, byte[] value)
        {
            string frame = string.Format("{0:X2}", slaveAddress); // Địa chỉ slave.
            frame += string.Format("{0:X2}", functionCode); // Mã hàm modbus.
            frame += string.Format("{0:X4}", startAddress); // Địa chỉ bắt đầu của coil.
            frame += string.Format("{0:X4}", value); // Dữ liệu cần ghi xuống coil.
            byte[] bytes = Conversion.HexToBytes(frame);
            byte lrc = LRC(bytes);
            return Header + frame + lrc.ToString("X2") + Trailer;
        }

        private string WriteAll(byte slaveAddress, ushort startAddress, byte functionCode, byte[] values)
        {
            string frame = string.Format("{0:X2}", slaveAddress); // Địa chỉ slave.
            frame += string.Format("{0:X2}", functionCode); // Mã hàm modbus.
            frame += string.Format("{0:X4}", startAddress); // Địa chỉ bắt đầu của coils.
            frame += string.Format("{0:X4}", (functionCode == 15) ? values.Length * 8 : values.Length/2); // Số lượng coils.
            frame += string.Format("{0:X2}", values.Length); // Số byte cần ghi.
            foreach (byte item in values)
            {
                frame += string.Format("{0:X2}", item);
            }
            byte[] bytes = Conversion.HexToBytes(frame);
            byte lrc = LRC(bytes);
            return Header + frame + lrc.ToString("X2") + Trailer;
        }

        private byte LRC(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("Tham số truyền vào không tồn tại phần tử nào");
            byte lrc = 0;
            foreach (byte b in data)
                lrc += b;
            lrc = (byte)((lrc ^ 0xFF) + 1);
            return lrc;
        }

        protected string ReadCoilStatusMessage(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            return this.Read(slaveAddress, ushort.Parse(startAddress), FUNCTION_01, nuMBErOfPoints);
        }

        protected string ReadInputStatusMessage(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            return this.Read(slaveAddress, ushort.Parse(startAddress), FUNCTION_02, nuMBErOfPoints);
        }

        protected string ReadHoldingRegistersMessage(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            return this.Read(slaveAddress, ushort.Parse(startAddress), FUNCTION_03, nuMBErOfPoints);
        }

        protected string ReadInputRegistersMessage(byte slaveAddress, string startAddress, ushort nuMBErOfPoints)
        {
            return this.Read(slaveAddress, ushort.Parse(startAddress), FUNCTION_04, nuMBErOfPoints);
        }

        protected string WriteSingleCoilMessage(byte slaveAddress, string startAddress, bool value)
        {
            short temp = 0;
            if (value == true)
            {
                temp = 255; // 0xFF00(55280) = 0xFF(250) Xor 0xFFFF(65535).
            }
            else
            {
                temp = 0;
            }
            byte[] values = IndustrialNetwork.Modbus.DataType.Int.ToByteArray(temp);
            return this.Write(slaveAddress, ushort.Parse(startAddress), FUNCTION_05, values);
        }

        protected string WriteMultipleCoilsMessage(byte slaveAddress, string startAddress, bool[] values)
        {
            byte[] data = IndustrialNetwork.Modbus.DataType.Boolean.ToByteArray(values);
            return this.WriteAll(slaveAddress, ushort.Parse(startAddress), FUNCTION_15, data);
        }

        protected string WriteSingleRegisterMessage(byte slaveAddress, string startAddress, byte[] values)
        {            
            return this.Write(slaveAddress, ushort.Parse(startAddress), FUNCTION_06, values);
        }

        protected string WriteMultipleRegistersMessage(byte slaveAddress, string startAddress, byte[] values)
        {            
            return this.WriteAll(slaveAddress, ushort.Parse(startAddress), FUNCTION_16, values);
        }

    }
}
