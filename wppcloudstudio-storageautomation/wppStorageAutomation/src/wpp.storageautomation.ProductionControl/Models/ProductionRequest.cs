// <copyright file="ProductionRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Wpp.StorageAutomation.Common;

namespace Wpp.StorageAutomation
{
    /// <summary>
    /// Production request.
    /// </summary>
    public class ProductionRequest
    {
        /// <summary>
        /// Gets or sets the production store identifier.
        /// </summary>
        /// <value>
        /// The production store identifier.
        /// </value>
        public string ProductionStoreId { get; set; }

        /// <summary>
        /// Gets or sets the production store URI.
        /// </summary>
        /// <value>
        /// The production store URI.
        /// </value>
        public string ProductionStoreUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the production.
        /// </summary>
        /// <value>
        /// The name of the production.
        /// </value>
        public string ProductionName { get; set; }

        /// <summary>
        /// Gets or sets the directory tree.
        /// </summary>
        /// <value>
        /// The directory tree.
        /// </value>
        public List<DirectoryTree> DirectoryTree { get; set; }

        /// <summary>
        /// Gets or sets the tokens.
        /// </summary>
        /// <value>
        /// The tokens.
        /// </value>
        public List<Token> Tokens { get; set; }
    }

    /// <summary>
    /// Directory tree.
    /// </summary>
    public class DirectoryTree
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ItemType Type { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the sub items.
        /// </summary>
        /// <value>
        /// The sub items.
        /// </value>
        public List<SubItem> SubItems { get; set; }
    }

    /// <summary>
    /// Token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets the production token.
        /// </summary>
        /// <value>
        /// The production token.
        /// </value>
        public string ProductionToken { get; set; }
    }

    /// <summary>
    /// Sub item.
    /// </summary>
    public class SubItem
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ItemType Type { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the sub items.
        /// </summary>
        /// <value>
        /// The sub items.
        /// </value>
        public List<SubItem> SubItems { get; set; }
    }
}
