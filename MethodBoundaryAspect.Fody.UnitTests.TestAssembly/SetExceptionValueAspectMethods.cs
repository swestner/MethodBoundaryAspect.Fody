﻿using System;
using System.Threading.Tasks;
using MethodBoundaryAspect.Fody.UnitTests.TestAssembly.Aspects;

namespace MethodBoundaryAspect.Fody.UnitTests.TestAssembly
{
    public class SetExceptionValueAspectMethods
    {
        public static object Result { get; set; }

        [SetExceptionValueAspect]
        public static void StaticMethodCall()
        {
            throw new InvalidOperationException("StaticMethodCall");
        }

        [SetExceptionValueAspect]
        public void InstanceMethodCall()
        {
            throw new InvalidOperationException("InstanceMethodCall");
        }

        [SetExceptionValueAspect]
        public async Task AsyncInstanceMethodCall()
        {
            await Task.Delay(1);
            throw new InvalidOperationException("AsyncInstanceMethodCall");
        }

        [SetExceptionValueAspect]
        public async Task AsyncStaticMethodCall()
        {
            await Task.Delay(1);
            throw new InvalidOperationException("AsyncStaticMethodCall");
        }
    }
}