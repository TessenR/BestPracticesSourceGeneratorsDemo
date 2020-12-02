using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LoggingGenerator
{
  [Generator]
  public class LoggingProxyGenerator : ISourceGenerator
  {
    public void Initialize(GeneratorInitializationContext context) {}

    public void Execute(GeneratorExecutionContext context)
    {
      var namespaceName = "LoggingImplDefault";


      var compilation = context.Compilation;
      var loggingTargets = compilation.SyntaxTrees
        .SelectMany(x => x.GetRoot()
          .DescendantNodesAndSelf()
          .OfType<TypeDeclarationSyntax>());

      var targetTypes = new HashSet<ITypeSymbol>();
      foreach (var targetTypeSyntax in loggingTargets)
      {
        var semanticModel = compilation.GetSemanticModel(targetTypeSyntax.SyntaxTree);
        var hasLogAttribute = targetTypeSyntax.AttributeLists
          .SelectMany(x => x.Attributes)
          .Any(x => x.Name.ToString() == "Log");
        if (!hasLogAttribute)
          continue;

        if (targetTypeSyntax is not InterfaceDeclarationSyntax)
          continue;

        var targetType = semanticModel.GetDeclaredSymbol(targetTypeSyntax);
        targetTypes.Add(targetType);
      }

      foreach (var targetType in targetTypes)
      {
        var proxySource = GenerateProxy(targetType, namespaceName);
        context.AddSource($"{targetType.Name}.Logging.cs", proxySource);
      }
    }

    private string GenerateProxy(ITypeSymbol targetType, string namespaceName)
    {
      var allInterfaceMethods = targetType.AllInterfaces
        .SelectMany(x => x.GetMembers())
        .Concat(targetType.GetMembers())
        .OfType<IMethodSymbol>()
        .ToList();

      var fullQualifiedName = GetFullQualifiedName(targetType);

      var sb = new StringBuilder();
      var proxyName = targetType.Name.Substring(1) + "LoggingProxy";
      sb.Append($@"
using System;
using System.Text;
using NLog;
using System.Diagnostics;

namespace {namespaceName}
{{
  public static partial class LoggingExtensions
  {{
     public static {fullQualifiedName} WithLogging(this {fullQualifiedName} baseInterface)
       => new {proxyName}(baseInterface);
  }}

  public class {proxyName} : {fullQualifiedName}
  {{
    private readonly ILogger _logger = LogManager.GetLogger(""{targetType.Name}"");
    private readonly {fullQualifiedName} _target;
    public {proxyName}({fullQualifiedName} target)
      => _target = target;
");

      foreach (var interfaceMethod in allInterfaceMethods)
      {
        var containingType = interfaceMethod.ContainingType;
        var parametersList = string.Join(", ", interfaceMethod.Parameters.Select(x => $"{GetFullQualifiedName(x.Type)} {x.Name}"));
        var argumentLog = string.Join(", ", interfaceMethod.Parameters.Select(x => $"{x.Name} = {{{x.Name}}}"));
        var argumentList = string.Join(", ", interfaceMethod.Parameters.Select(x => x.Name));
        var isVoid = interfaceMethod.ReturnsVoid;
        var interfaceFullyQualifiedName = GetFullQualifiedName(containingType);
        sb.Append($@"
    {interfaceMethod.ReturnType} {interfaceFullyQualifiedName}.{interfaceMethod.Name}({parametersList})
    {{
{Log("LogLevel.Info", $"\"{interfaceMethod.Name} started...\"")}
{Log("LogLevel.Info", $"$\"  Arguments: {argumentLog}\"")}
      var sw = new Stopwatch();
      sw.Start();
      try
      {{
");

        sb.Append("        ");
        if (!isVoid)
        {
          sb.Append("var result = ");
        }
        sb.AppendLine($"_target.{interfaceMethod.Name}({argumentList});");
        sb.AppendLine("  " + Log("LogLevel.Info", $@"$""{interfaceMethod.Name} finished in {{sw.ElapsedMilliseconds}} ms"""));
        if (!isVoid)
        {
          sb.AppendLine("  " + Log("LogLevel.Info", "$\"Return value: {result}\""));
          sb.AppendLine("        return result;");
        }

        sb.Append($@"
      }}
      catch (Exception e)
      {{
  {Log("LogLevel.Error", "e.ToString()")}
        throw;
      }}
    }}");
      }

      sb.Append(@"
  }
}");
      return sb.ToString();

      string Log(string logLevel, string message)
      {
        return $"      _logger.Log({logLevel}, {message});";
      }
    }

    private static string GetFullQualifiedName(ISymbol symbol)
    {
      var containingNamespace = symbol.ContainingNamespace;
      if (!containingNamespace.IsGlobalNamespace)
        return containingNamespace.ToDisplayString() + "." + symbol.Name;

      return symbol.Name;
    }
  }
}