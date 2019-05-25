using System;
using System.Collections.Generic;
using System.Text;
namespace QGame.Core.XProto
{
    public class ProtoStream : ProtoBuffer
    {
        private static readonly Encoding CHARSET = Encoding.UTF8;
       
        public ProtoStream() : base() { }

        public ProtoStream(byte[] bytes) : base(bytes) { }


        public void WriteFixedShort(int value)
        {
            Put((byte)(value & 0xff));
            Put((byte)((value >> 8) & 0xff));
        }
        public void WriteFixedShort(int value, int pos)
        {
            Put((byte)(value & 0xff), pos++);
            Put((byte)((value >> 8) & 0xff), pos);
        }

        public short ReadFixedShort()
        {
            short value = (short)(Get() & 0xff);
            value += (short)((Get() & 0xff) << 8);
            return value;
        }

        private void WriteRawVarint32(uint value)
        {
            while ((value & 0xFFFFFF80) != 0L)
            {
                Put((byte)(value & 0x7F | 0x80));
                value = value >> 7;
            }
            Put((byte)(value & 0x7F));
        }

        private void WriteRawVarint64(ulong value)
        {
            while (((ulong)value & 0xFFFFFFFFFFFFFF80L) != 0L)
            {
                Put((byte)(value & 0x7F | 0x80));
                value = value >> 7;
            }
            Put((byte)(value & 0x7F));
        }


        public void WriteInt(int value)
        {
            WriteRawVarint32(EncodeZigZag32(value));
        }

        public void WriteLong(long value)
        {
            WriteRawVarint64(EncodeZigZag64(value));
        }

        public void WriteString(String val)
        {
            if (string.IsNullOrEmpty(val))
            {
                WriteInt(0);
                return;
            }
            byte[] bytes = CHARSET.GetBytes(val);
            WriteInt(bytes.Length);
            Put(bytes);
        }

        public int ReadInt()
        {
            return DecodeZigZag32(ReadRawVarint32());
        }

        private uint ReadRawVarint32()
        {
            int shift = 0;
            uint result = 0;
            while (shift < 32)
            {
                byte b = Get();
                result |= (uint)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }
                shift += 7;
            }
            throw new ArgumentOutOfRangeException("Variable length quantity is too long");
        }

        public long ReadLong()
        {
            return DecodeZigZag64(ReadRawVarint64());
        }

        public ulong ReadRawVarint64()
        {
            int shift = 0;
            ulong result = 0;
            while (shift < 64)
            {
                byte b = Get();
                result |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }
                shift += 7;
            }
            throw new ArgumentOutOfRangeException("Variable length quantity is too long");
        }

        public string ReadString()
        {
            int len = ReadInt();

            if (len > 0)
            {
                byte[] temp = GetBytes(len);
                return CHARSET.GetString(temp);
            }
            return null;
        }

        public List<int> ReadIntList()
        {
            int count = ReadInt();
            List<int> list = new List<int>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(ReadInt());
            }

            return list;
        }

        public List<long> ReadLongList()
        {
            int count = ReadInt();
            List<long> list = new List<long>(count);

            for (int i = 0; i < count; i++)
            {
                list.Add(ReadLong());
            }

            return list;
        }

        public List<string> ReadStringList()
        {
            int count = ReadInt();
            List<string> list = new List<string>();

            for (int i = 0; i < count; i++)
            {
                list.Add(ReadString());
            }

            return list;
        }



        public int Write(int tag, int val)
        {
            if (val != 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.VarInt);
                WriteInt(tag);
                WriteInt(val);
                return 1;
            }
            return 0;
        }

        public int Write(int tag, String val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.String);

                WriteInt(tag);
                WriteString(val);
                return 1;
            }
            return 0;
        }

        public int Write(int tag, long val)
        {
            if (val != 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.VarLong);

                WriteInt(tag);
                WriteLong(val);
                return 1;
            }
            return 0;
        }


        public int WriteIntList(int tag, List<int> list)
        {
            if (list != null && list.Count > 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.VarIntList);

                WriteInt(tag);
                WriteInt(list.Count);

                foreach (var i in list)
                {
                    WriteInt(i);
                }
                return 1;
            }
            return 0;
        }

        public int WriteLongList(int tag, List<long> list)
        {
            if (list != null && list.Count > 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.VarLongList);

                WriteInt(tag);
                WriteInt(list.Count);

                foreach (var i in list)
                {
                    WriteLong(i);
                }
                return 1;
            }
            return 0;
        }

        public int WriteStringList(int tag, List<string> list)
        {
            if (list != null && list.Count > 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.StringList);

                WriteInt(tag);
                WriteInt(list.Count);

                foreach (var i in list)
                {
                    WriteString(i);
                }
                return 1;
            }
            return 0;
        }



        public void ReadUnknow(int tagAngType)
        {
            ReadUnknow((ProtoType)(tagAngType & ProtoDefine.TAG_TYPE_MASK));
        }

        public void ReadUnknow(ProtoType type)
        {
            switch (type)
            {
                case ProtoType.VarInt:
                    {
                        ReadInt();
                        break;
                    }
                case ProtoType.VarLong:
                    {
                        ReadLong();
                        break;
                    }
                case ProtoType.String:
                    {
                        ReadString();
                        break;
                    }
                case ProtoType.VarIntList:
                    {
                        ReadIntList();
                        break;
                    }
                case ProtoType.VarLongList:
                    {
                        ReadLongList();
                        break;
                    }
                case ProtoType.StringList:
                    {
                        ReadStringList();
                        break;
                    }
                case ProtoType.Object:
                    {
                        ReadUnknowObject();
                        break;
                    }
                case ProtoType.ObjectList:
                    {
                        ReadUnknowObjectList();
                        break;
                    }
                default:
                    break;
            }
        }

        private void ReadUnknowObject()
        {
            int fields = Get() & 0xff;
            fields += (Get() & 0xff) << 8;

            int tag = 0;
            while (fields-- > 0)
            {
                tag = ReadInt();
                ReadUnknow((ProtoType)tag);
            }
        }

        private void ReadUnknowObjectList()
        {
            int count = ReadInt();

            for (int i = 0; i < count; i++)
            {
                ReadUnknowObject();
            }
        }


        //
        public static int DecodeZigZag32(uint n)
        {
            return (int)(n >> 1) ^ -(int)(n & 1);
        }

        public static long DecodeZigZag64(ulong n)
        {
            return (long)(n >> 1) ^ -(long)(n & 1);
        }

        public static uint EncodeZigZag32(int n)
        {
            return (uint)((n << 1) ^ (n >> 31));
        }

        public static ulong EncodeZigZag64(long n)
        {
            return (ulong)((n << 1) ^ (n >> 63));
        }


        public int Write<T>(int tag, T val) where T : Message
        {
            if (val != null)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.Object);

                WriteInt(tag);
                val.Encode(this);
                return 1;
            }
            return 0;
        }


        public T ReadObject<T>() where T : Message
        {
            T item = Activator.CreateInstance<T>();
            item.Decode(this);
            return item;
        }

        public List<T> ReadObjectList<T>() where T : Message
        {
            int size = ReadInt();
            if (size <= 0)
                return null;

            List<T> list = new List<T>();

            for (int i = 0; i < size; i++)
            {
                list.Add(ReadObject<T>());
            }

            return list;
        }
        public int WriteObjectList<T>(int tag, List<T> list) where T : Message
        {
            if (list != null && list.Count > 0)
            {
                tag = ((tag << ProtoDefine.TAG_TYPE_BITS) + (byte)ProtoType.ObjectList);

                WriteInt(tag);
                WriteInt(list.Count);

                foreach (var i in list)
                {
                    i.Encode(this);
                }
                return 1;
            }
            return 0;
        }
 
    }
}
