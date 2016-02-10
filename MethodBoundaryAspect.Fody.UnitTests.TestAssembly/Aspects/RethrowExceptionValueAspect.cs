using System;
using System.Runtime.ExceptionServices;
using MethodBoundaryAspect.Fody.Attributes;

namespace MethodBoundaryAspect.Fody.UnitTests.TestAssembly.Aspects
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class WrapExceptionValueAspect : OnMethodBoundaryAspect
    {
        public string TranslationKey { get; set; }
        public Type ExceptionType { get; set; }
        public override void OnException(MethodExecutionArgs arg)
        {
            var exception = arg.Exception;

            if (exception.GetType() == ExceptionType)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }

            throw new WrappedException(TranslationKey, exception.Message, exception);
        }


    }

    public class WrappedException : Exception
    {
        public string TranslationKey { get; }
        public WrappedException(string translationKey, string message,  Exception ex) : base(message, ex) 
        {
            TranslationKey = translationKey;
        }
    }

}