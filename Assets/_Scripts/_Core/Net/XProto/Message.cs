using System;
using System.Collections.Generic;
using System.Text;

namespace QGame.Core.XProto
{
    public abstract class Message
    {
        public virtual void Encode(ProtoStream stream)
        {
        }

        public virtual void Decode(ProtoStream stream)
        {
        }
    }
}
