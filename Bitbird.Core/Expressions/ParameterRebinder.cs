﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

//https://blogs.msdn.microsoft.com/meek/2008/05/02/linq-to-entities-combining-predicates/

namespace Bitbird.Core.Expressions
{
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, Expression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, Expression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, Expression>();
        }
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, Expression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            return map.TryGetValue(p, out var replacement) ? replacement : p;
        }
    }

    public class ParameterRebinderDirect : ExpressionVisitor
    {
        private readonly Func<ParameterExpression, Expression> map;

        public ParameterRebinderDirect(Func<ParameterExpression, Expression> map)
        {
            this.map = map;
        }
        public static Expression ReplaceParameters(Expression exp, Func<ParameterExpression, Expression> map)
        {
            return new ParameterRebinderDirect(map).Visit(exp);
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            return map(p) ?? p;
        }
    }
}