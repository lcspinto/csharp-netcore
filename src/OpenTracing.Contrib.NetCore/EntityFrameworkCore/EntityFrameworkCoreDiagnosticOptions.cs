﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OpenTracing.Contrib.NetCore.EntityFrameworkCore
{
    public class EntityFrameworkCoreDiagnosticOptions
    {
        // NOTE: Everything here that references any EFCore types MUST NOT be initialized in the constructor as that would throw on applications that don't reference EFCore.

        public const string DefaultComponent = "EFCore";

        private string _componentName = DefaultComponent;
        private List<Func<CommandEventData, bool>> _ignorePatterns;
        private Func<CommandEventData, string> _operationNameResolver;

        /// <summary>
        /// A list of delegates that define whether or not a given EF Core command should be ignored.
        /// <para/>
        /// If any delegate in the list returns <c>true</c>, the EF Core command will be ignored.
        /// </summary>
        public List<Func<CommandEventData, bool>> IgnorePatterns => _ignorePatterns ??= new List<Func<CommandEventData, bool>>();

        /// <summary>
        /// Allows changing the "component" tag of created spans.
        /// </summary>
        public string ComponentName
        {
            get => _componentName;
            set => _componentName = value ?? throw new ArgumentNullException(nameof(ComponentName));
        }

        /// <summary>
        /// Whether or not spans should be created if there is no currently active span.
        /// </summary>
        public bool StartRootSpans { get; set; } = true;

        /// <summary>
        /// A delegate that returns the OpenTracing "operation name" for the given command.
        /// </summary>
        public Func<CommandEventData, string> OperationNameResolver
        {
            get
            {
                if (_operationNameResolver == null)
                {
                    // Default value may not be set in the constructor because this would fail
                    // if the target application does not reference EFCore.
                    _operationNameResolver = (data) => "DB " + data.ExecuteMethod.ToString();
                }
                return _operationNameResolver;
            }
            set => _operationNameResolver = value ?? throw new ArgumentNullException(nameof(OperationNameResolver));
        }
    }
}
