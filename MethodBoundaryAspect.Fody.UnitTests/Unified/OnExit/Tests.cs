using System;
using System.Collections.Generic;
using FluentAssertions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace MethodBoundaryAspect.Fody.UnitTests.Unified.OnExit
{
    [TestFixture]
    public class Tests : UnifiedWeaverTestBase
    {
        private readonly Type _testType = typeof (TestMethods);

        [SetUp]
        public override void Setup()
        {
            base.Setup();
        }

        [Test]
        public void IfVoidEmptyMethodMethodIsWeaved_ThenPeVerifyShouldBeOk()
        {
            // Arrange
            var weaver = new ModuleWeaver();
            weaver.AddMethodFilter(_testType.FullName + ".VoidEmptyMethod");

            // Act
            weaver.Weave(WeaveDll);

            // Arrange
            var peVerifyResult = RunPeVerify();
            peVerifyResult.Should().Be(0);

            AssertUnifiedMethod(weaver.LastWeavedMethod);
        }

        [Test]
        public void IfIntMethodIsWeaved_ThenPeVerifyShouldBeOk()
        {
            // Arrange
            var weaver = new ModuleWeaver();
            weaver.AddMethodFilter(_testType.FullName + ".IntMethod");

            // Act
            weaver.Weave(WeaveDll);

            // Arrange
            var peVerifyResult = RunPeVerify();
            peVerifyResult.Should().Be(0);

            AssertUnifiedMethod(weaver.LastWeavedMethod);
        }

        [Test]
        public void IfVoidThrowMethodIsWeaved_ThenPeVerifyShouldBeOk()
        {
            // Arrange
            var weaver = new ModuleWeaver();
            weaver.AddMethodFilter(_testType.FullName + ".VoidThrowMethod");

            // Act
            weaver.Weave(WeaveDll);

            // Arrange
            var peVerifyResult = RunPeVerify();
            peVerifyResult.Should().Be(0);

            AssertUnifiedMethod(weaver.LastWeavedMethod, true);
        }

        [Test]
        public void IfIntMethodIntWithMultipleReturnIsWeaved_ThenPeVerifyShouldBeOk()
        {
            // Arrange
            var weaver = new ModuleWeaver();
            weaver.AddMethodFilter(_testType.FullName + ".IntMethodIntWithMultipleReturn");

            // Act
            weaver.Weave(WeaveDll);

            // Arrange
            var peVerifyResult = RunPeVerify();
            peVerifyResult.Should().Be(0);

            AssertUnifiedMethod(weaver.LastWeavedMethod);
        }

        private static void AssertUnifiedMethod(MethodDefinition method)
        {
            AssertUnifiedMethod(method, false);
        }

        private static void AssertUnifiedMethod(MethodDefinition method, bool methodThrows)
        {
            var allowedLoadOpCodes = new List<OpCode>
            {
                OpCodes.Ldloc_0,
                OpCodes.Ldloc_S,
            };

            var allowedStoreOpCodes = new List<OpCode>
            {
                OpCodes.Stloc_0,
                OpCodes.Stloc_S,
            };

            var instructions = method.Body.Instructions;
            instructions[0].OpCode.Should().Be(OpCodes.Nop);

            var lastIndex = instructions.Count - 1;

            instructions[lastIndex].OpCode.Should().Be(methodThrows ? OpCodes.Throw : OpCodes.Ret);
            if (method.ReturnType.Name == "Void")
            {
                if (methodThrows)
                {
                    instructions[lastIndex - 1].OpCode.Should().Match(x => allowedLoadOpCodes.Contains((OpCode) x));
                    instructions[lastIndex - 11].OpCode.Should().Match(x => allowedLoadOpCodes.Contains((OpCode) x));
                    instructions[lastIndex - 12].OpCode.Should().Match(x => allowedStoreOpCodes.Contains((OpCode) x));
                    instructions[lastIndex - 13].OpCode.Should().Be(OpCodes.Nop);
                }
                else
                {
                    instructions[lastIndex - 6].OpCode.Should().Be(OpCodes.Nop);
                    instructions[lastIndex - 7].OpCode.Should().Be(OpCodes.Nop);
                }
            }
            else
            {
                instructions[lastIndex - 1].OpCode.Should().Match(x => AllLdLocOpCodes.Contains((OpCode)x));

                instructions[lastIndex - 11].OpCode.Should().Be(OpCodes.Nop);
                instructions[lastIndex - 13].OpCode.Should().Be(OpCodes.Nop);
            }
        }
    }
}