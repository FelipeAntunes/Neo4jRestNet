﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Neo4jRestNet.CypherPlugin
{
	public class CypherQuery
	{
		List<Func<CypherStart, object>> _start = new List<Func<CypherStart, object>>();
		List<Func<CypherMatch, object>> _match = new List<Func<CypherMatch, object>>();
		List<Expression<Func<CypherWhere, object>>> _where = new List<Expression<Func<CypherWhere,object>>>();
		List<Func<CypherReturn, object>> _return = new List<Func<CypherReturn,object>>();
		List<Func<CypherOrderBy, object>> _orderBy = new List<Func<CypherOrderBy, object>>();
		String _skip = string.Empty;
		String _limit = string.Empty;

		public void Start(Func<CypherStart, object> start)
		{
			_start.Add(start);
		}

		public void Match(Func<CypherMatch, object> match)
		{
			_match.Add(match);
		}

		public void Where(Expression<Func<CypherWhere, object>> where)
		{
			_where.Add(where);
		}

		public void Return(Func<CypherReturn, object> cypherReturn)
		{
			_return.Add(cypherReturn);
		}

		public void OrderBy(Func<CypherOrderBy, object> cypherOrderBy)
		{
			_orderBy.Add(cypherOrderBy);
		}

		public void Skip(int skip)
		{
			_skip = string.Format(" SKIP {0}", skip);
		}
		
		public void Limit(int limit)
		{
			_limit = string.Format(" LIMIT {0}", limit);
		}

		public override string ToString()
		{
			StringBuilder sbToString = new StringBuilder();

			string label = "START";
			foreach (var s in _start)
			{
				sbToString.AppendFormat("{1}{0}", s.Invoke(new CypherStart()).ToString(), label);
				label = ",";
			}

			if (_match != null)
			{
				label = "MATCH";
				foreach (var m in _match)
				{
					sbToString.AppendFormat(" {1}{0}", m.Invoke(new CypherMatch()).ToString(), label);
					label = ",";
				}
			}

			if (_where != null)
			{
				label = "WHERE";
				foreach (var w in _where)
				{
					sbToString.AppendFormat(" {1} {0}", new ParseWhereLambda().Parse(w), label);
					label = string.Empty;
				}
			}

			if (_return != null)
			{
				label = "RETURN";
				foreach (var r in _return)
				{
					sbToString.AppendFormat(" {1}{0}", r.Invoke(new CypherReturn()).ToString(), label);
					label = ",";
				}
			}

			if (_orderBy != null)
			{
				label = "ORDER BY";
				foreach (var o in _orderBy)
				{
					sbToString.AppendFormat(" {1}{0}", o.Invoke(new CypherOrderBy()).ToString(), label);
					label = ",";
				}
			}

			// Append Skip
			sbToString.Append(_skip);

			// Append Limit
			sbToString.Append(_limit);
			return sbToString.ToString();
		}
	}
}
