using Navigator.Containers;
using NUnit.Framework;

namespace Navigator.Test
{
    [TestFixture]
    public partial class ContainerTest
    {
        [Test]
        public void IfInterceptorRegisteredItsInterfacesAreAvailableOnFinalObject()
        {
            var container = new Container();
            container.Register(typeof(PrimaryClass), typeof(InterceptorClass), ContainerRegisterType.Intercept);

            var primaryClass = container.Get<PrimaryClass>();

            var testInterface = (ITestInterface) primaryClass;
            Assert.AreEqual(InterceptorClass.DoSomethingReturn, testInterface.DoSomething());
        }

        [Test]
        public void IfInterceptorHasInterceptorBothSetsOfInterfacesAvailableOnFinalObject()
        {
            var container = new Container();
            container.Register(typeof(PrimaryClass), typeof(InterceptorClass), ContainerRegisterType.Intercept);
            container.Register(typeof(InterceptorClass), typeof(SecondInterceptor), ContainerRegisterType.Intercept);

            var primaryClass = container.Get<PrimaryClass>();

            var testInterface = (ITestInterface) primaryClass;
            Assert.AreEqual(InterceptorClass.DoSomethingReturn, testInterface.DoSomething());

            var secondInterface = (ISecondInterface) primaryClass;
            Assert.AreEqual(SecondInterceptor.StillDoItReturn, secondInterface.StillDoIt());
        }

        public void IfRegistrationOccursTwiceOnSameTypeSecondRegistrationOverridesFirst()
        {}

        public void IfTwoInterceptorsAreRegisteredOnSameTypeBothSetsOfInterfacesAreAvailableOnFinalObject()
        {}

        public void IfAnInstanceObjectIsSuppliedToATypeTheInterceptorGetsTheSameObject()
        {}
    }

    public partial class ContainerTest
    {
        public class PrimaryClass
        {}

        public class InterceptorClass : ITestInterface
        {
            public const int DoSomethingReturn = 42;

            public int DoSomething()
            {
                return DoSomethingReturn;
            }
        }

        public class SecondInterceptor : ISecondInterface
        {
            public const int StillDoItReturn = 24;

            public int StillDoIt()
            {
                return StillDoItReturn;
            }
        }

        public interface ISecondInterface
        {
            int StillDoIt();
        }

        public interface ITestInterface
        {
            int DoSomething();
        }
        
    }
}