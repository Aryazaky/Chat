using System.Collections.Generic;
using Network.Attributes;
using Network.Packets;

namespace Chat.Packets
{
	public class MessageRequest : RequestPacket
	{
		public MessageOwner messageOwner;
		public string message;
		public MessageRequest(MessageOwner _messageOwner, string _message)
		{
			messageOwner = _messageOwner;
			message = _message;
		}
	}

	public class MessageOwner
	{
		public string name;
		public MessageOwner(string _name)
		{
			name = _name;
		}
	}
}

