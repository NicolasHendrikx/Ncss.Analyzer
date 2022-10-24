# Non-Comment Source Statements (NCSS) Analyzer for Roslyn

First attempt to compute the NCSS of a member's body. The analyzer triggers a diagnostic warning whenever a body NCSS is greater than 20 is computed.

The NCSS algorithm gives results closed, but not equal, to those of the Logicical Line of Code Count of Microsoft Metrics. One hypothesis that explains the difference is that we count `else` statements as actual statements, which does not seem to be the case for Microsoft's metric.