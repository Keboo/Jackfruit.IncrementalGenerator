﻿using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace Jackfruit.IncrementalGenerator
{
    internal static class RoslynHelpers
    {
        public enum MemberKind
        {
            Option = 0,
            Argument,
            Service
        }

        public class CommandDetails
        {
            public CommandDetails(string nspace, Detail detail, Dictionary<string, Detail> memberDetails)
            {
                Namespace = nspace;
                Detail = detail;
                MemberDetails = memberDetails;
            }
            public string Namespace { get; }
            public Detail Detail { get; }
            public Dictionary<string, Detail> MemberDetails { get; }
        }

        public class Detail
        {
            private string description = "";
            private MemberKind memberKind;
            private string? typeName = "";
            private string argDisplayId = "";
            private bool required;

            public Detail(string id, string name, string? typeName = null)
            {
                Id = id;
                Name = char.ToUpperInvariant(name[0]) + name.Substring(1);
                TypeName = typeName;
            }

            public string Id { get; }
            public string Name { get; set; }
            public string Description
            {
                get => description;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        description = value;
                    }
                }
            }
            public string[] Aliases { get; set; } = new string[] { };
            public MemberKind MemberKind
            {
                get => memberKind;
                set
                {
                    if (value != MemberKind.Option)
                    {
                        memberKind = value;
                    }
                }
            }
            public string? TypeName
            {
                get => typeName;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        typeName = value;
                    }
                }
            }
            public string ArgDisplayName
            {
                get => argDisplayId;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        argDisplayId = value;
                    }
                }
            }
            public bool Required
            {
                get => required;
                set
                {
                    if (value)
                    {
                        required = value;
                    }
                }
            }
        }

        public const string CommandKey = "__commandKey__";

        public static IMethodSymbol? MethodOrCandidateSymbol(SemanticModel semanticModel, SyntaxNode? expression)
        {
            if (expression == null) { return null; }
            var symbolInfo = semanticModel.GetSymbolInfo(expression);
            return symbolInfo.Symbol is IMethodSymbol methodSymbol
                ? methodSymbol
                : symbolInfo.CandidateSymbols.FirstOrDefault() is IMethodSymbol candidate
                    ? candidate
                    : null;
        }

        public static CommandDetails BasicDetails(this IMethodSymbol methodSymbol)
        {
            var nspace = methodSymbol.ContainingNamespace.ToString();

            var commandDetail = new Detail(methodSymbol.ToDisplayString(), methodSymbol.Name, methodSymbol.ReturnType.ToString());

            var details = new Dictionary<string, Detail>();

            foreach (var param in methodSymbol.Parameters)
            {
                details[param.Name] = new Detail(param.Name, param.Name, param.Type.ToString());
                if (param.Name.EndsWith("Arg"))
                {
                    details[param.Name].MemberKind = MemberKind.Argument;
                    details[param.Name].Name = details[param.Name].Name.Substring(0, param.Name.Length - 3);
                }
                else if (param.Type.IsAbstract)  // Test that this is true for interfaces
                {
                    details[param.Name].MemberKind = MemberKind.Service;
                }
            }
            return new CommandDetails(nspace, commandDetail, details);
        }

        public static void AddDescFromXmlDocComment(XDocument xDoc, Dictionary<string, Detail> details)
        {
            foreach (var element in xDoc.Root.Elements("param"))
            {
                var paramName = element.Attribute("name");
                if (paramName is not null)
                {
                    if (details.TryGetValue(paramName.Value, out var paramDetail))
                    { paramDetail.Description = element.Value.Trim(); }
                }
            }
        }

        public static void AddDescFromXmlDocComment(XDocument xDoc, Detail commandDetail)
        {
            var summaryElement = xDoc.Root.Element("summary");
            commandDetail.Description =
                    summaryElement is null
                    ? commandDetail.Description
                    : summaryElement.Value.Trim();
        }

        public static Dictionary<string, Detail> AddDetailsFromAttributes(
            IMethodSymbol methodSymbol,
            Detail commandDetail,
            Dictionary<string, Detail> details)
        {
            AddToDetail(methodSymbol.GetAttributes(), commandDetail);
            foreach (var param in methodSymbol.Parameters)
            {
                if (details.TryGetValue(param.Name, out var detail))
                {
                    AddToDetail(param.GetAttributes(), detail);
                }
            }

            return details;
        }

        private static void AddToDetail(IEnumerable<AttributeData> attributes, Detail detail)
        {
            foreach (var attrib in attributes)
            {
                if (attrib.AttributeClass is null) { continue; }
                switch (attrib.AttributeClass.Name)
                {
                    case "DescriptionAttribute":
                    case "Description":
                        var arg = attrib.ConstructorArguments.FirstOrDefault();
                        detail.Description = arg.Value?.ToString() ?? "";
                        break;

                    case "AliasesAttribute":
                    case "Aliases":
                        var arg1 = attrib.ConstructorArguments.FirstOrDefault();
                        detail.Aliases =
                            arg1.Kind == TypedConstantKind.Array
                                ? arg1.Values.Select(x => x.Value is null ? "" : x.Value.ToString()).ToArray()
                                : arg1.Value is null
                                    ? new string[] { }
                                    : new string[] { arg1.Value.ToString() };
                        break;

                    case "ArgumentAttribute":
                    case "Argument":
                        detail.MemberKind = MemberKind.Argument;
                        break;

                    case "OptionArgumentNameAttribute":
                    case "OptionArgumentName":
                        var arg3 = attrib.ConstructorArguments.FirstOrDefault();
                        detail.ArgDisplayName = arg3.Value?.ToString() ?? "";
                        break;

                    case "RequiredAttribute":
                    case "Required":
                        detail.Required = true;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
