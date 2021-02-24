using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.RequestReply
{
	public class Request
	{
		public string Id { get; set; }

		public Request()
		{
		}

		public Request(string id)
		{
			Id = id;
		}
	}
}
