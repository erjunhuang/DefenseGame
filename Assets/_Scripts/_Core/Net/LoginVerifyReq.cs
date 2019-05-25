using System;
using System.Text;
using System.Collections.Generic;
using QGame.Core.XProto;

namespace QGame.Core.Model
{
    /// <summary> 登录请求 </summary>
    public class LoginVerifyReq : Message
    {
        public string sesskey { get; set; }

        public int terminalType { get; set; }

        public int uid { get; set; }
        public override void Decode(ProtoStream stream){
            base.Decode(stream);

            int tagAndType = 0;
            int fieldCount = stream.ReadFixedShort();

            while ( fieldCount-- > 0 ) {
                tagAndType = stream.ReadInt();

                switch ((tagAndType >> ProtoDefine.TAG_TYPE_BITS)) {
                    case 1:{
                            sesskey = stream.ReadString();
                        break;
                    }
                    case 2:{
                            terminalType = stream.ReadInt();
                        break;
                    }
                    case 3:
                        {
                            uid = stream.ReadInt();
                            break;
                        }
                    default:{
                        stream.ReadUnknow(tagAndType);
                        break;
                    }
                }
            }
        }

        public override void Encode(ProtoStream buffer)
        {
            base.Encode(buffer);

            int fieldCount = 0;
            int pos = buffer.Position;

            buffer.WriteFixedShort(0);

            fieldCount += buffer.Write(1, sesskey);
            fieldCount += buffer.Write(2, terminalType);
            fieldCount += buffer.Write(3, uid);

            if (fieldCount > 0)
                buffer.WriteFixedShort(fieldCount,pos);
        }
    }
}