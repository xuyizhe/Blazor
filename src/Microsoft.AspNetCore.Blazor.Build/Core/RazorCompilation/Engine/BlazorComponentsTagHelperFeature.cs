// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language;

namespace Microsoft.AspNetCore.Blazor.Build.Core.RazorCompilation.Engine
{
    internal class BlazorComponentsTagHelperFeature : ITagHelperFeature
    {
        private IEnumerable<BlazorComponentDescriptor> _componentDescriptors;

        public BlazorComponentsTagHelperFeature(IEnumerable<BlazorComponentDescriptor> componentDescriptors)
        {
            _componentDescriptors = componentDescriptors;
        }

        public RazorEngine Engine { get; set; }

        public IReadOnlyList<TagHelperDescriptor> GetDescriptors()
        {
            return _componentDescriptors.Select(descriptor =>
            {
                var builder = TagHelperDescriptorBuilder.Create("BlazorComponentKind", $"__Generated__{descriptor.TypeName}", descriptor.AssemblyName);
                builder.SetTypeName(typeof(object).FullName);
                builder.DisplayName = descriptor.TypeName + "TagHelper";
                builder.TagMatchingRule(rule =>
                {
                    rule.TagName = descriptor.TypeName;
                });
                foreach (var property in descriptor.PropertiesWithTypeNames)
                {
                    builder.BindAttribute(rule =>
                    {
                        rule.Name = property.Key;
                        rule.SetPropertyName(property.Key);
                        rule.TypeName = property.Value;
                    });
                }
                return builder.Build();
            }).ToList();
        }

        class DefaultTagHelperDescriptor : TagHelperDescriptor
        {
            public DefaultTagHelperDescriptor(
                string kind,
                string name,
                string assemblyName,
                string displayName,
                string documentation,
                string tagOutputHint,
                TagMatchingRuleDescriptor[] tagMatchingRules,
                BoundAttributeDescriptor[] attributeDescriptors,
                AllowedChildTagDescriptor[] allowedChildTags,
                Dictionary<string, string> metadata,
                RazorDiagnostic[] diagnostics)
                : base(kind)
            {
                Name = name;
                AssemblyName = assemblyName;
                DisplayName = displayName;
                Documentation = documentation;
                TagOutputHint = tagOutputHint;
                TagMatchingRules = tagMatchingRules;
                BoundAttributes = attributeDescriptors;
                AllowedChildTags = allowedChildTags;
                Diagnostics = diagnostics;
                Metadata = metadata;
            }
        }

        class DefaultTagMatchingRuleDescriptor : TagMatchingRuleDescriptor
        {
            public DefaultTagMatchingRuleDescriptor(
                string tagName,
                string parentTag,
                TagStructure tagStructure,
                RequiredAttributeDescriptor[] attributes,
                RazorDiagnostic[] diagnostics)
            {
                TagName = tagName;
                ParentTag = parentTag;
                TagStructure = tagStructure;
                Attributes = attributes;
                Diagnostics = diagnostics;
            }
        }
    }
}
