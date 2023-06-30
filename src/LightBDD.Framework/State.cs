using System;

namespace LightBDD.Framework
{
    /// <summary>
    /// A helper struct designed to protect scenario state fields from being accessed without former initialization.
    /// </summary>
    public struct State<T>
    {
        private readonly T _value;
        /// <summary>
        /// Returns true if state is initialized, otherwise false.
        /// The default instance of <see cref="State{T}"/> type represents uninitialized state.
        /// </summary>
        public bool IsInitialized { get; }

        /// <summary>
        /// Creates state instance, initialized with <paramref name="value"/> value.
        /// The <see cref="IsInitialized"/> is always set to true, even if <paramref name="value"/> itself is null.
        /// The <paramref name="value"/> is retrievable with <see cref="GetValue"/> method or implicit cast.
        /// </summary>
        /// <param name="value"></param>
        public State(T value)
        {
            _value = value;
            IsInitialized = true;
        }

        /// <summary>
        /// Returns the held value, or throws <see cref="InvalidOperationException"/> if state is not initialized.
        /// </summary>
        /// <param name="memberName">If specified, the member name will be used in exception. If not specified, the state type will be used.</param>
        /// <exception cref="InvalidOperationException">Thrown if value is not initialized.</exception>
        public T GetValue(string memberName = null)
        {
            if (IsInitialized)
                return _value;
            throw new InvalidOperationException($"The {memberName ?? typeof(T).Name} state is not initialized.");
        }

        /// <summary>
        /// Returns the held value or <paramref name="defaultValue"/> if state is not initialized or held value is null (in case where <typeparamref name="T"/> is reference type).
        /// </summary>
        public T GetValueOrDefault(T defaultValue = default)
        {
            if (!IsInitialized)
                return defaultValue;
            return _value != null ? _value : defaultValue;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsInitialized ? _value?.ToString() : "<not set>";
        }

        /// <summary>
        /// Implicit cast, converting <paramref name="value"/> to initialized state instance.
        /// </summary>
        public static implicit operator State<T>(T value) => new(value);
        /// <summary>
        /// Implicit cast, retrieving value of <paramref name="state"/> or throwing <see cref="InvalidOperationException"/> if state is not initialized.
        /// </summary>
        /// <param name="state"></param>
        public static implicit operator T(State<T> state) => state.GetValue();
    }
}
