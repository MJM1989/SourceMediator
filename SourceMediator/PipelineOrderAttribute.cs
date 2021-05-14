using System;

namespace SourceMediator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PipelineOrderAttribute : Attribute
    {
        public int Order { get; }

        public PipelineOrderAttribute(int order)
        {
            Order = order;
        }
    }
}