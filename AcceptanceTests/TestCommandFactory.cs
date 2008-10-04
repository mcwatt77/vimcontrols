using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VIMControls;
using VIMControls.Input;
using VIMControls.Interfaces;

namespace AcceptanceTests
{
    public class TestCommandFactory : IFactory<ICommand>
    {
        public List<LambdaExpression> RequestedExpressions { get; private set; }

        public TestCommandFactory()
        {
            RequestedExpressions = new List<LambdaExpression>();
        }

        public ICommand Create(params object[] @params)
        {
            return new TestCommand((LambdaExpression)@params[0], this);
        }

        public void AddCall(LambdaExpression expression)
        {
            RequestedExpressions.Add(expression);
        }
    }

    public class TestCommand : ICommand
    {
        private readonly LambdaExpression _expression;
        private readonly TestCommandFactory _factory;
        private readonly ICommand _cmd;

        public TestCommand(LambdaExpression expression, TestCommandFactory factory)
        {
            _expression = expression;
            _factory = factory;
            _cmd = new Command(expression);
        }

        public void Invoke(ICommandable commandable)
        {
            try
            {
                _factory.AddCall(_expression);
                _cmd.Invoke(commandable);
            }
            catch(Exception e)
            {
                throw new Exception("Failed executing " + _expression + " on: " + e.Message);
            }
        }
    }
}
