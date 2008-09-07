﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NUnit.Framework;
using VIMControls;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;
using VIMControls.Controls.StackProcessor;
using Rhino.Mocks;

namespace Tests
{
    [TestFixture]
    public class VIMExpressionProcessorTest
    {
        private MockRepository repository;
        private IStackPanel stackPanel;

        [SetUp]
        public void Setup()
        {
            repository = new MockRepository();
            var textFactory = repository.DynamicMock<ITextFactory>();
            var stackPanelFactory = repository.DynamicMock<IStackPanelFactory>();
            var text = repository.DynamicMock<IText>();
            stackPanel = repository.DynamicMultiMock<IStackPanel>();
            var panelChildren = new List<IUIElement>();

            Services.Register<ITextFactory>(textFactory);
            Services.Register<IStackPanelFactory>(stackPanelFactory);

            textFactory.Expect(factory => factory.Create()).Return(text).Repeat.Any();
            stackPanelFactory.Expect(factory => factory.Create()).Return(stackPanel);
            stackPanel.Expect(a => a.Children).Return(panelChildren).Repeat.Any();

        }

        [Ignore]
        [Test]
        public void ShouldBeAbleToEditTextAndCompileAndRunIt()
        {
            var eProcessor = new ExpressionProcessor();
            eProcessor.Eval((IFuncExpression)VIMExpression.FromString("edit"));
            //do some magical editing stuff
            eProcessor.Push(new StringExpression("var i = 0; i++; return i;"));
            eProcessor.Eval((IFuncExpression)VIMExpression.FromString("compile"));
            eProcessor.Eval((IFuncExpression)VIMExpression.FromString("eval"));
        }

        [Ignore]
        [Test]
        public void RefreshStackViewGetsPostedAsMessageSoItsNotCalledAfterPassingControlFromResetCall()
        {
            Assert.Fail();
        }

        [Test]
        public void ProperlyHandlesMultipleInstancesOfOneVariable()
        {
            repository.ReplayAll();

            var eProcessor = new ExpressionProcessor();
            eProcessor.Push(new DoubleExpression(2));

            eProcessor.Push(new StringExpression("x"));
            eProcessor.Push(new DoubleExpression(3));
            eProcessor.Eval(StackOpExpression.Power);

            eProcessor.Push(new StringExpression("x"));
            eProcessor.Push(new DoubleExpression(2));
            eProcessor.Eval(StackOpExpression.Power);

            eProcessor.Eval(StackOpExpression.Add);
            eProcessor.Eval((IFuncExpression)VIMExpression.FromString("eval"));

            var expr = eProcessor.Pop();
            var numExpr = (INumericExpression) expr;

            Assert.AreEqual(12, numExpr.dVal);

            repository.VerifyAll();
        }

        [Test]
        public void InputEvalsLargeExpressions()
        {
            repository.ReplayAll();

            var eProcessor = new ExpressionProcessor();
            eProcessor.Push(new DoubleExpression(2));
            eProcessor.Push(new StringExpression("x"));
            eProcessor.Push(new DoubleExpression(3));
            eProcessor.Eval(StackOpExpression.Power);
            eProcessor.Push(new DoubleExpression(3));
            eProcessor.Eval(StackOpExpression.Multiply);
            eProcessor.Push(new DoubleExpression(5));
            eProcessor.Eval(StackOpExpression.Add);
            eProcessor.Eval((IFuncExpression)VIMExpression.FromString("eval"));
            var expr = eProcessor.Pop();
            var numExpr = (INumericExpression) expr;

            Assert.AreEqual(29, numExpr.dVal);

            repository.VerifyAll();
        }

        [Test]
        public void Input_4_5_x_add_eval_returns_9()
        {
            repository.ReplayAll();

            var eProcessor = new ExpressionProcessor();
            eProcessor.Push(new DoubleExpression(4));
            eProcessor.Push(new DoubleExpression(5));
            eProcessor.Push(new StringExpression("x"));
            eProcessor.Eval(StackOpExpression.Add);
            eProcessor.Eval((IFuncExpression)VIMExpression.FromString("eval"));
            var expr = eProcessor.Pop();

            Assert.IsInstanceOfType(typeof(INumericExpression), expr);
            var numExpr = (INumericExpression)expr;

            Assert.AreEqual(9, numExpr.dVal);

            repository.VerifyAll();
        }

        [Test]
        public void ShouldBeAbleToPushAnyNumberOfObjectsOnToStack()
        {
            stackPanel.RenderSizeChanged += null;
            var loadRaiser = LastCall.IgnoreArguments().GetEventRaiser();

            repository.ReplayAll();

            var eProcessor = new ExpressionProcessor();
            loadRaiser.Raise(new Size(200, 100));

            Enumerable.Repeat(0, 10).Do(i => eProcessor.Push(new DoubleExpression(4)));

            repository.VerifyAll();
        }
    }
}