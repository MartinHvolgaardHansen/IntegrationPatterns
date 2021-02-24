using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.RequestReply
{
	static class Data
	{
		private static IDictionary<string, string> data = new Dictionary<string, string>
		{
			{ "first", "First data" },
			{ "second", "Second data" },
			{ "third", "Third data" }
		};

		public static string Get(string key)
		{
			return data[key];
		}
	}
}
