using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Router
{
	public class RoutedMessage
	{
		public string Sender { get; set; }
		public string Destination { get; set; }
		public object Body { get; set; }
	}
}
