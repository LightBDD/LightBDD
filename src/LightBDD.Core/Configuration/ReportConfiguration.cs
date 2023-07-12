﻿using System;
using System.Collections;
using System.Collections.Generic;
using LightBDD.Core.Reporting;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize report generation.
    /// </summary>
    //TODO: LightBDD 4.x revisit
    public class ReportConfiguration : FeatureConfiguration, IEnumerable<IReportGenerator>
    {
        private readonly List<IReportGenerator> _generators = new();
        private IFileAttachmentsManager _fileAttachmentsManager = NoFileAttachmentsManager.Instance;

        /// <summary>
        /// File Attachments Manager
        /// </summary>
        public IFileAttachmentsManager GetFileAttachmentsManager() => _fileAttachmentsManager;

        /// <summary>
        /// Adds <paramref name="generator"/> to report generator collection.
        /// </summary>
        /// <param name="generator">Generator to add.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="generator"/> is <c>null</c>.</exception>
        public ReportConfiguration Add(IReportGenerator generator)
        {
            ThrowIfSealed();
            if (generator == null)
                throw new ArgumentNullException(nameof(generator));
            _generators.Add(generator);
            return this;
        }

        /// <summary>
        /// Removes specified, previously configured generator.
        /// </summary>
        /// <param name="generator">Generator instance to remove.</param>
        /// <returns>Self.</returns>
        public ReportConfiguration Remove(IReportGenerator generator)
        {
            ThrowIfSealed();
            _generators.Remove(generator);
            return this;
        }

        /// <summary>
        /// Removes all previously configured report generators.
        /// </summary>
        /// <returns>Self.</returns>
        public ReportConfiguration Clear()
        {
            ThrowIfSealed();
            _generators.Clear();
            return this;
        }

        /// <summary>
        /// Sets <paramref name="manager"/> as a default file attachments manager to be used by LightBDD. The manager can be retrieved by <see cref="GetFileAttachmentsManager"/> method call.
        /// </summary>
        /// <param name="manager">Manager to set</param>
        /// <returns></returns>
        public ReportConfiguration UpdateFileAttachmentsManager(IFileAttachmentsManager manager)
        {
            ThrowIfSealed();

            _fileAttachmentsManager = manager ?? throw new ArgumentNullException(nameof(manager));

            return this;
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IReportGenerator> GetEnumerator()
        {
            return _generators.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}