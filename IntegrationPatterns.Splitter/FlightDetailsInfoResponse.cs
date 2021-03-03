using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Splitter
{
	public class FlightDetailsInfoResponse
	{
		public Flight Flight { get; set; }
		public Passenger Passenger { get; set; }
		public List<Luggage> Luggages { get; set; }
	}
}
