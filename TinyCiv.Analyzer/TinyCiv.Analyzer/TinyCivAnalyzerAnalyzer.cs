using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace TinyCiv.Analyzer
{
    // Guide: https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/tutorials/how-to-write-csharp-analyzer-code-fix
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TinyCivAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "TinyCivAnalyzer";

        private const char Underscore = '_';

        private const string PrivateFieldCategory = "Naming";
        private const string PrivateFieldTitle = "Private field does not contain _";
        private const string PrivateFieldMessageFormat = "Private field '{0}' does not contain underscore '_'";
        private const string PrivatedFieldDescription = "Private field name does not contain underscore '_' ";

        private const string ParameterCountCategory = "Style";
        private const string ParameterCountTitle = "Method signature contains more parameters than allowed";
        private const string ParameterCountMessageFormat = "Method '{0}' signature contains '{1}' parameters. This project allows '{2}' parameters in method signature.";
        private const string ParameterCountDescription = "Method signature contains more parameters than allowed";

        private static readonly DiagnosticDescriptor _privateFieldRule = new DiagnosticDescriptor(DiagnosticId, PrivateFieldTitle, PrivateFieldMessageFormat, PrivateFieldCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: PrivatedFieldDescription);
        private static readonly DiagnosticDescriptor _methodParameterCountRule = new DiagnosticDescriptor(DiagnosticId, ParameterCountTitle, ParameterCountMessageFormat, ParameterCountCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: ParameterCountDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_privateFieldRule, _methodParameterCountRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeField, SymbolKind.Field);
            context.RegisterSymbolAction(AnalyzeMethodSignature, SymbolKind.Method);
        }

        private static void AnalyzeMethodSignature(SymbolAnalysisContext context)
        {
            const int allowedParameterCount = 3;

            var methodSymbol = context.Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return;
            }

            var parameterCount = methodSymbol.Parameters.Count();
            if (parameterCount <= allowedParameterCount)
            {
                return;
            }

            var diagnostic = Diagnostic.Create(_methodParameterCountRule, methodSymbol.Locations[0], methodSymbol.Name, parameterCount, allowedParameterCount);
            context.ReportDiagnostic(diagnostic);
        }

        private static void AnalyzeField(SymbolAnalysisContext context)
        {
            var field = context.Symbol as IFieldSymbol;
            if (field == null || field.DeclaredAccessibility != Accessibility.Private || field.IsConst)
            {
                return;
            }
            

            var hasUnderscore = field.Name.ToCharArray().FirstOrDefault() == Underscore;
            if (hasUnderscore)
            {
                return;
            }

            var diagnostic = Diagnostic.Create(_privateFieldRule, field.Locations[0], field.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
