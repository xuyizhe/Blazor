// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using System;
using System.Linq;

namespace Microsoft.AspNetCore.Blazor.Build.Core.RazorCompilation.Engine
{
    /// <summary>
    /// Directs a <see cref="DocumentWriter"/> to use <see cref="BlazorIntermediateNodeWriter"/>.
    /// </summary>
    internal class BlazorCodeTarget : CodeTarget
    {
        private BlazorComponentTagHelperTargetExtension _blazorComponentTagHelperTargetExtension
            = new BlazorComponentTagHelperTargetExtension();

        public override IntermediateNodeWriter CreateNodeWriter()
            => new BlazorIntermediateNodeWriter();

        public override TExtension GetExtension<TExtension>()
        {
            if (typeof(TExtension) == typeof(IDefaultTagHelperTargetExtension))
            {
                return (TExtension)(object)_blazorComponentTagHelperTargetExtension;
            }

            throw new ArgumentException($"Unsupported extension: {typeof(TExtension).FullName}");
        }

        public override bool HasExtension<TExtension>()
            => throw new NotImplementedException();

        class BlazorComponentTagHelperTargetExtension : IDefaultTagHelperTargetExtension
        {
            public void WriteTagHelperBody(CodeRenderingContext context, DefaultTagHelperBodyIntermediateNode node)
            {
                ((BlazorIntermediateNodeWriter)context.NodeWriter).BeginComponent(context, node);
            }

            public void WriteTagHelperCreate(CodeRenderingContext context, DefaultTagHelperCreateIntermediateNode node)
            {
            }

            public void WriteTagHelperExecute(CodeRenderingContext context, DefaultTagHelperExecuteIntermediateNode node)
            {
                ((BlazorIntermediateNodeWriter)context.NodeWriter).EndComponent(context);
            }

            public void WriteTagHelperHtmlAttribute(CodeRenderingContext context, DefaultTagHelperHtmlAttributeIntermediateNode node)
            {
                ((BlazorIntermediateNodeWriter)context.NodeWriter).AddComponentProperty(context, node.AttributeName, node);
            }

            public void WriteTagHelperProperty(CodeRenderingContext context, DefaultTagHelperPropertyIntermediateNode node)
            {
                ((BlazorIntermediateNodeWriter)context.NodeWriter).AddComponentProperty(context, node.PropertyName, node);
            }

            public void WriteTagHelperRuntime(CodeRenderingContext context, DefaultTagHelperRuntimeIntermediateNode node)
            {
            }
        }
    }
}
