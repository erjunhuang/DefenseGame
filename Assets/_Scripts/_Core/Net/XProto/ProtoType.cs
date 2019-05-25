using System;
using System.Collections.Generic;
using System.Text;

namespace QGame.Core.XProto
{
    public class ProtoDefine
    {
        public static readonly int TAG_TYPE_BITS = 3;
        public static readonly int TAG_TYPE_MASK = 0x7;
        public static int DEFAULT_BUFFER_SIZE = 256;
    }
    public enum ProtoType : byte
    {
        VarInt = 0,
        VarLong,
        String,
        Object,
        VarIntList,
        VarLongList,
        StringList,
        ObjectList
    }
}
