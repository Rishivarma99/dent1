$ErrorActionPreference = "Stop"

$context = @"
devlog-toolkit is enabled.

To run a simple read-only code review:
- Agent: devlog-toolkit:review-code
- Skill: devlog-toolkit:review-angular-structure
"@

$obj = @{ additional_context = $context }
$obj | ConvertTo-Json -Compress

