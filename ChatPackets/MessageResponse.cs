using Network.Attributes;
using Network.Packets;

namespace Chat.Packets
{
    [PacketRequest(typeof(MessageRequest))]
    public class MessageResponse : ResponsePacket
    {
        public string response;
        public MessageResponse(string _response, MessageRequest request) : base(request) 
        {
            response = _response;
        }
    }
}
