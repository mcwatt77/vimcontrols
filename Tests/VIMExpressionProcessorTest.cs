using System;
using System.Collections.Generic;
using System.Windows;
using NUnit.Framework;
using VIMControls;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;
using VIMControls.Controls.StackProcessor;

namespace Tests
{
    [TestFixture]
    public class VIMExpressionProcessorTest
    {
        [Ignore]
        [Test]
        public void Input_4_5_x_add_eval_returns_9()
        {
            Services.Register<ITextFactory>(new TestTextFactory());
            Services.Register<IStackPanelFactory>(new TestStackPanelFactory());

            var eProcessor = new ExpressionProcessor();
            eProcessor.Process(new DoubleExpression(4));
            eProcessor.Process(new DoubleExpression(5));
            eProcessor.Process(new StringExpression("x"));
            eProcessor.Eval(new BinaryMathExpression((d0, d1) => d0 + d1));
            eProcessor.Eval((IFuncExpression)VIMExpression.FromString("eval"));
            var expr = eProcessor.Pop();

            Assert.IsInstanceOfType(typeof(INumericExpression), expr);
            var numExpr = (INumericExpression)expr;

            Assert.AreEqual(9, numExpr.dVal);
        }
    }

    public class TestTextFactory : ITextFactory
    {
        public IText Create()
        {
            return new TestText();
        }
    }

    public class TestText : IText
    {
        public event Action<SizeChangedInfo> RenderSizeChanged;
        public string Text { get; set; }
        public double Height { get; set; }
    }

    public class TestStackPanel : IStackPanel
    {
        private readonly List<IUIElement> _children = new List<IUIElement>();
        public event Action<SizeChangedInfo> RenderSizeChanged;
        public bool Fill { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public IList<IUIElement> Children { get { return _children; }}
        public bool ApplyBorders { get; set; }
    }

    public class TestStackPanelFactory : IStackPanelFactory
    {
        public IStackPanel Create()
        {
            return new TestStackPanel();
        }
    }
}