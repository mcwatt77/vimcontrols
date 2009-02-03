using System;
using System.Collections.Generic;
using System.Linq;

namespace VIMControls.Controls.StackProcessor.MathExpressions
{
    public class StoExpression : IFuncExpression
    {
        private const string _rpnStoGuid = "{56A84837-1459-48a5-96A5-26A9F532657F}";
        private Dictionary<string, Guid> _nameLookups = new Dictionary<string, Guid>();

        public static IExpression GetValue(string name)
        {
            var rpnGuid = new Guid(_rpnStoGuid);
            var nameLookups = rpnGuid.Load<Dictionary<string, Guid>>();
            if (nameLookups == null) return null;
            return !nameLookups.ContainsKey(name) ? null : nameLookups[name].Load<IExpression>();
        }

        public IExpression Eval(IEnumerable<IExpression> args)
        {
            var name = args.First().ToString();

            var guid = Guid.NewGuid();
            var rpnGuid = new Guid(_rpnStoGuid);
            _nameLookups = rpnGuid.Load<Dictionary<string, Guid>>() ?? new Dictionary<string, Guid>();
            if (!_nameLookups.ContainsKey(name))
            {
                _nameLookups[args.First().ToString()] = guid;
                _nameLookups.Persist(rpnGuid);
            }
            else
                guid = _nameLookups[name];

            args.Last().Persist(guid);
            return null;
        }

        public int StackArgs
        {
            get { return 2; }
        }
    }
}