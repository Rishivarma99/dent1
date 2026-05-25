$ErrorActionPreference = 'Stop'

function Write-HookResponse {
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Response
    )

    Write-Output ($Response | ConvertTo-Json -Compress)
}

try {
    $rawInput = [Console]::In.ReadToEnd()

    if ([string]::IsNullOrWhiteSpace($rawInput)) {
        Write-HookResponse @{ permission = 'allow' }
        exit 0
    }

    $hookInput = $rawInput | ConvertFrom-Json
    $command = ''
    if ($null -ne $hookInput -and $null -ne $hookInput.command) {
        $command = [string]$hookInput.command
    }

    if ($command -notmatch '(?i)\bgit(?:\.exe)?\b' -or $command -notmatch '(?i)\bcommit(?:\s|$)') {
        Write-HookResponse @{ permission = 'allow' }
        exit 0
    }

    $protectedFiles = @(
        'api/dent1.api/dent1.api.csproj'
    )

    $stagedFiles = git diff --cached --name-only --diff-filter=ACMR
    if (-not $?) {
        Write-HookResponse @{
            permission    = 'deny'
            user_message  = 'Could not inspect staged files, so the sensitive-file commit check blocked this commit.'
            agent_message = 'The commit safeguard could not run `git diff --cached --name-only` successfully.'
        }
        exit 0
    }

    # Normalize staged paths so Windows and Git path styles compare consistently.
    $matchedFiles = @(
        $stagedFiles |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
            ForEach-Object { $_.Trim().Replace('\', '/').TrimStart('.', '/').ToLowerInvariant() } |
            Where-Object { $protectedFiles -contains $_ } |
            Sort-Object -Unique
    )

    if ($matchedFiles.Count -eq 0) {
        Write-HookResponse @{ permission = 'allow' }
        exit 0
    }

    $displayFiles = ($matchedFiles | ForEach-Object { "- $_" }) -join "`n"

    Write-HookResponse @{
        permission    = 'ask'
        user_message  = "This commit includes protected file(s):`n$displayFiles`n`nContinue only if you intended to commit them."
        agent_message = 'A protected staged file was detected before git commit.'
    }
    exit 0
}
catch {
    Write-HookResponse @{
        permission    = 'deny'
        user_message  = 'The sensitive-file commit check failed, so the commit was blocked.'
        agent_message = $_.Exception.Message
    }
    exit 0
}
