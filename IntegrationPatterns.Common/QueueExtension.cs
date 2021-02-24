using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Common
{
	public static class QueueExtension
	{
		public static void Verify(this MessageQueue queue)
		{
			if (!MessageQueue.Exists(queue.Path))
				MessageQueue.Create(queue.Path);
		}

		public static void VerifyRecreate(this MessageQueue queue)
		{
			if (MessageQueue.Exists(queue.Path))
				MessageQueue.Delete(queue.Path);
			MessageQueue.Create(queue.Path);
		}

		public static void Delete(this MessageQueue queue)
		{
			MessageQueue.Delete(queue.Path);
		}
	}
}
