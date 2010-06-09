//-------------------------------------------------------------------------------------------------
// <copyright file="ApplicationSettings.cs" company="Konesans Limited">
// Copyright (C) 2010 Konesans Limited.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Konesans.Dts.Tools.ExpressionTester
{
    using Konesans.Dts.Tools.Common;

    /// <summary>
    /// Application settings.
    /// </summary>
    internal class ApplicationSettings
    {
        /// <summary>
        /// Private property member.
        /// </summary>
        private SettingsManager.RecentFiles recentFiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettings"/> class.
        /// </summary>
        internal ApplicationSettings()
        {
            this.recentFiles = string.Empty;
        }

        /// <summary>
        /// Gets the recent files collection.
        /// </summary>
        /// <value>The recent files.</value>
        public SettingsManager.RecentFiles RecentFiles
        {
            get { return this.recentFiles; }
        }
    }
}
