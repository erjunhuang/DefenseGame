using System;
using System.Collections.Generic; 
using System.Text;

namespace QGame.Core.XProto
{
    public class ProtoUtils
    {
        private static readonly char[] HEX_DIGITS = 
        {
            '0', '1', '2', '3', '4', '5', '6', '7',
			'8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        public static String Hex(byte[] bytes)
        {
            char[] result = new char[bytes.Length * 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                result[i * 2] = HEX_DIGITS[(bytes[i] >> 4) & 0xf];
                result[i * 2 + 1] = HEX_DIGITS[bytes[i] & 0xf];
            }
            return new String(result);
        }

        public static byte[] DecodeHex(String hex)
        {
            if (hex == null)
                throw new ArgumentException("hex == null");
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Unexpected hex string: " + hex);

            byte[] result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                int d1 = decodeHexDigit(hex[i * 2]) << 4;
                int d2 = decodeHexDigit(hex[i * 2 + 1]);
                result[i] = (byte)(d1 + d2);
            }
            return result;
        }

        private static int decodeHexDigit(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;
            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            throw new ArgumentException("Unexpected hex digit: " + c);
        }

        public static string Dump(ProtoStream stream)
        {
            StringBuilder sb = new StringBuilder();
            dump(sb,stream, 0);
            return sb.ToString();
        }

        private static void dump(StringBuilder sb,ProtoStream stream, int level)
        {
            sb.AppendLine();
            short fieldCount = stream.ReadFixedShort();

            while (fieldCount-- > 0)
            {
                dumpField(sb,stream, level);
            } 
        }

        private static void dumpField(StringBuilder sb, ProtoStream stream, int level)
        {
            int tagAndType = stream.ReadInt();
            int tag = (tagAndType >> ProtoDefine.TAG_TYPE_BITS);
            ProtoType type = (ProtoType)(tagAndType & ProtoDefine.TAG_TYPE_MASK);

            for (int i = 0; i < level; i++)
            {
                sb.Append("\t");
            }

            sb.AppendFormat("T:{0}\tTYPE:{1} = ", tag, type);

            switch (type)
            {
                case ProtoType.VarInt:
                    {
                        sb.Append(stream.ReadInt());
                        break;
                    }
                case ProtoType.VarLong:
                    {
                        sb.Append(stream.ReadLong());
                        break;
                    }
                case ProtoType.String:
                    {
                        sb.Append(stream.ReadString());
                        break;
                    }
                case ProtoType.Object:
                    {
                        dump(sb,stream, level + 1);
                        break;
                    }

                case ProtoType.VarIntList:
                    {
                        int count = stream.ReadInt();
                        sb.Append(" COUNT:" + count + "[");

                        for (int i = 0; i < count; i++)
                        {
                            sb.Append(stream.ReadInt());
                            if (i < count - 1)
                                sb.Append(",");
                        }

                        sb.Append("]");

                        break;
                    }
                case ProtoType.VarLongList:
                    {
                        int count = stream.ReadInt();
                        sb.Append(" COUNT:" + count + "[");

                        for (int i = 0; i < count; i++)
                        {
                            sb.Append(stream.ReadLong());
                            if (i < count - 1)
                                sb.Append(",");
                        }

                        sb.Append("]");

                        break;
                    }
                case ProtoType.StringList:
                    {
                        int count = stream.ReadInt();
                        sb.Append(" COUNT:" + count + "[");

                        for (int i = 0; i < count; i++)
                        {
                            sb.Append(stream.ReadString());
                            if (i < count - 1)
                                sb.Append(",");
                        }

                        sb.Append("]");

                        break;
                    }
                case ProtoType.ObjectList:
                    {
                        int count = stream.ReadInt();
                        sb.Append(" COUNT:" + count + "[");

                        for (int i = 0; i < count; i++)
                        {
                            dump(sb,stream, level + 1);
                        }

                        sb.Append("\t]");

                        break;
                    }
                default:
                    break;
            }
            sb.AppendLine();
        }
    }
}
