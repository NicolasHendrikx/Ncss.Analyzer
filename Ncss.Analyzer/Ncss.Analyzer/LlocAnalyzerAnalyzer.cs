using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Ncss.Analyzer
{
    /// <summary>
    /// Raise a warning diagnostic for member's body counting more than 20 statements.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LlocAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NCSSAnalyzer";
        private const int Threshold = 20;

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";
        
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(
                syntaxAnalysisContext =>  Parse(syntaxAnalysisContext.Node as MethodDeclarationSyntax, syntaxAnalysisContext), 
                SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(
                syntaxAnalysisContext => Parse(syntaxAnalysisContext.Node as PropertyDeclarationSyntax, syntaxAnalysisContext),
                SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(
                syntaxAnalysisContext => Parse(syntaxAnalysisContext.Node as IndexerDeclarationSyntax, syntaxAnalysisContext),
                SyntaxKind.IndexerDeclaration);
        }

        private void Parse(MethodDeclarationSyntax methodDeclaration, SyntaxNodeAnalysisContext syntaxTreeContext)
        {
            var stmtCount = CountStatements(methodDeclaration.Body);
            if (stmtCount >= Threshold)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    methodDeclaration.GetFirstToken().GetLocation(),
                    methodDeclaration.Identifier,
                    stmtCount
                );
                syntaxTreeContext.ReportDiagnostic(diagnostic);
            }
        }

        private void Parse(PropertyDeclarationSyntax propertyDeclaration, SyntaxNodeAnalysisContext syntaxTreeContext)
        {
            foreach (var accessorDeclaration in propertyDeclaration.DescendantNodes().OfType<AccessorDeclarationSyntax>())
            {
                var stmtCount = CountStatements(accessorDeclaration.Body);

                if (stmtCount >= Threshold)
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        accessorDeclaration.GetFirstToken().GetLocation(),
                        propertyDeclaration.Identifier + "." + accessorDeclaration.Keyword,
                        stmtCount
                    );
                    syntaxTreeContext.ReportDiagnostic(diagnostic);
                }
            }
        }

        private void Parse(IndexerDeclarationSyntax indexDeclaration, SyntaxNodeAnalysisContext syntaxTreeContext)
        {
            foreach (var accessorDeclaration in indexDeclaration.DescendantNodes().OfType<AccessorDeclarationSyntax>())
            {
                var stmtCount = CountStatements(accessorDeclaration.Body);

                if (stmtCount >= Threshold)
                {
                    var diagnostic = Diagnostic.Create(
                        Rule,
                        accessorDeclaration.GetFirstToken().GetLocation(),
                        $"this{indexDeclaration.ParameterList}." + accessorDeclaration.Keyword,
                        stmtCount
                    );
                    syntaxTreeContext.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static int CountStatements(BlockSyntax block)
        {
            var simpleStatementCount = block
                ?.DescendantNodes()
                ?.OfType<StatementSyntax>()
                ?.Where(stmt => !(stmt is BlockSyntax))
                ?.Count() ?? 0;

            var elseStatementCount = block
                ?.DescendantNodes()
                ?.OfType<IfStatementSyntax>()
                ?.Where(ifStmt => ifStmt.Else != null)
                ?.Count() ?? 0;

            return simpleStatementCount + elseStatementCount;

        } 
    }
}
