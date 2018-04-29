using System;

namespace LightBDD.Framework.Expectations
{

    /// <summary>
    /// Interface representing expectation composer.
    /// </summary>
    public interface IExpectationComposer
    {
        /// <summary>
        /// Negates the expectation that is going to be composed.
        /// It can be used multiple times, flipping the negation flag every time.
        /// </summary>
        IExpectationComposer Not { get; }
        /// <summary>
        /// Composes the expectation from <paramref name="expectation"/> parameter and the negation state defined by <see cref="Not"/> property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectation"></param>
        /// <returns></returns>
        Expectation<T> Compose<T>(Expectation<T> expectation);

        /// <summary>
        /// This method should not be used as it does not describe an expectation.
        /// </summary>
        [Obsolete("This is not a valid expectation method", true)]
        bool Equals(object x);
    }
}