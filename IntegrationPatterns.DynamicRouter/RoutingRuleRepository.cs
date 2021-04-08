using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationPatterns.DynamicRouter
{
	class RoutingRuleRepository
	{
		private IDictionary<int, RoutingRule> rules;

		public RoutingRuleRepository(IDictionary<int, RoutingRule> rules)
		{
			this.rules = rules;
		}

		public IDictionary<int, RoutingRule> Get()
		{
			return rules;
		}
	}
}
