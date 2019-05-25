using QGame.Core.XProto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text; 

namespace QGame.Core.Net
{
    public class GameRequest
    {
        public static readonly byte MESSAGE_FLAG = 0x58;

        public int CmdId { get;  set; }
        public int Sequence { get;  set; }  
        public Message Body { get; set; }

        public GameRequest()
        {
        }

        public GameRequest(int cmdId, int sequence)
        {
            this.CmdId = cmdId;
            this.Sequence = sequence; 
        } 

        public byte[] Encode()
        {
            ProtoStream stream = new ProtoStream();

            stream.Put(MESSAGE_FLAG);
            stream.Position = 3;

           // stream.Write(1, CmdId);
          //  stream.Write(2, Sequence);

            stream.WriteInt(CmdId);
            stream.WriteInt(Sequence);

            if (Body != null)
            {
                Body.Encode(stream);
            }

            int len = stream.Position;
             
            stream.Put((byte)((len >> 8) & 0xff),1);
            stream.Put((byte)(len & 0xff),2);

            return stream.ToArray();
        }

        public override string ToString()
        {
            return "GameRequest [cmdId="
                    + CmdId
                    + ", sequence="
                    + Sequence
                    + ", body="
                    + (Body == null ? "null" : Body.ToString())
                    + "]";
        } 
    }
}
