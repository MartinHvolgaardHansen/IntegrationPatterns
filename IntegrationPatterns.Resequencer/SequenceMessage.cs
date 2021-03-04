using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.Resequencer
{
	public class SequenceMessage
	{
		public Guid Id { get; set; }
		public int Sequence { get; set; }
		public int SequenceMax { get; set; }
		public object Payload { get; set; }
	}
}
