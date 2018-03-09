using System;
using System.Reflection;

namespace AutoFixture.Kernel
{
    /// <summary>
    /// A specification that evaluates specimen requests to see if they request a subclass
    /// of <see cref="MulticastDelegate"/>. 
    /// </summary>
    public class DelegateSpecification : IRequestSpecification
    {
        /// <summary>
        /// Evaluates a request for a specimen.
        /// </summary>
        /// <param name="request">The specimen request.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="request"/> is a subclass of <see cref="MulticastDelegate"/>;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// Test against MulticastDelegate instead of Delegate base class
        /// because Brad Abrams says that we "should pretend that [Delegate 
        /// and MulticaseDelegate] are merged and that only MulticastDelegate exists."
        /// http://blogs.msdn.com/b/brada/archive/2004/02/05/68415.aspx
        /// </remarks>
        public bool IsSatisfiedBy(object request)
        {
            return request is Type type && type.GetTypeInfo().IsSubclassOf(typeof(MulticastDelegate));
        }
    }
}