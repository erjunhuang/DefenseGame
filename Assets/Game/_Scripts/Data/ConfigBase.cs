using QGame.Core.XProto;
using System;
using System.Collections.Generic;
using System.Text;

namespace QGame.Core.Config
{
    public abstract class ConfigBase : Message
    {
        public int Id { get; set; }

        public override void Encode(ProtoStream stream)
        {
            base.Encode(stream);
            stream.WriteInt(Id);
        }

        public override void Decode(ProtoStream stream)
        {
            base.Decode(stream);
            Id = stream.ReadInt();
        }
    }
}
