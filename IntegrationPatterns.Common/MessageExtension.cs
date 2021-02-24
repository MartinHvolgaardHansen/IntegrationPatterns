using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Common
{
	public static class MessageExtension
	{
		public static void AttachFormatter(this Message message, Type[] targetTypes)
		{
			message.Formatter = new XmlMessageFormatter(targetTypes);
		}
	}
}
