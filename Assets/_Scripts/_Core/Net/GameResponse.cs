using QGame.Core.XProto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text; 

namespace QGame.Core.Net
{
    public class GameResponse
    {
        public int CmdId { get; private set; }
        public int Sequence { get; private set; }
        public int Result { get; private set; }
        public byte[] Body { get; private set; }

        public GameResponse(int cmdId, int sequence, int result)
        {
            this.CmdId = cmdId;
            this.Sequence = sequence;
            this.Result = result;
        }

        public GameResponse(byte[] bytes)
        {
            ProtoStream stream = new ProtoStream(bytes);
            CmdId = stream.ReadInt();
            Sequence = stream.ReadInt();
            Result = stream.ReadInt();

            Body = stream.GetOverplus();
        }

        public override string ToString()
        {
            return "GameResponse [cmdId="
                    + CmdId
                    + ", sequence="
                    + Sequence
                    + ", result="
                    + Result
                    + ", body="
                    + (Body == null ? "null" : Body.Length.ToString())
                    + "]";
        }


        public static GameResponse Decode(byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                return new GameResponse(bytes);
            }
            return null;
        }

        public T GetBody<T>() where  T : Message
        {
            if (Body == null || Body.Length == 0)
                return default(T);

            var t = Activator.CreateInstance<T>();
            t.Decode(new ProtoStream(Body));
            return t;
        }
    }
}
