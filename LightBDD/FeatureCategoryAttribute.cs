using System;

namespace LightBDD
{
    /// <summary>
    /// Feature category attribute that can be applied on feature test class.
    /// May be used to associate feature test class with specific categories.
    /// It is possible to apply multiple FeatureCategory attributes on given feature test class.
    ///
    /// If given implementation supports alternative category attributes, and both are applied on class, all of them would be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class FeatureCategoryAttribute : Attribute
    {
        /// <summary>
        /// Feature description.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Constructor accepting category name.
        /// </summary>
        public FeatureCategoryAttribute(string name)
        {
            Name = name;
        }
    }
}