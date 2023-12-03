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
        private const string PrivateFieldMessageFormat = "Private field name '{0}' does not contain underscore '_'";
        private const string PrivatedFieldDescription = "Private fied name does not contain";

        private static readonly DiagnosticDescriptor PrivateFieldRule = new DiagnosticDescriptor(DiagnosticId, PrivateFieldTitle, PrivateFieldMessageFormat, PrivateFieldCategory, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: PrivatedFieldDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(PrivateFieldRule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeField, SymbolKind.Field);
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

            var diagnostic = Diagnostic.Create(PrivateFieldRule, field.Locations[0], field.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
