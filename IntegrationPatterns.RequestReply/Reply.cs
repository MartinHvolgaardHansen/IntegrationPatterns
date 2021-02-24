using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.RequestReply
{
	public class Reply
	{
		public string RequestId { get; set; }
		public object Data { get; set; }

		public Reply()
		{
		}

		public Reply(string requestId, object data)
		{
			RequestId = requestId;
			Data = data;
		}
	}
}
