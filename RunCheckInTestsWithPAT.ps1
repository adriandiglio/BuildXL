﻿Param(
 [Parameter(mandatory=$true)]
 [String]$1esPat,
 [Parameter(mandatory=$true)]
 [String]$cbPat,
 [Parameter(mandatory=$true)]
 [String]$ncPath,
 [Parameter(mandatory=$true)]
 [String]$msEngGitPat,
 [Parameter(mandatory=$true)]
 [String]$args
)
[Environment]::SetEnvironmentVariable("MSENG_GIT_PAT", $msEngGitPat, "Process")
[Environment]::SetEnvironmentVariable("VSS_NUGET_EXTERNAL_FEED_ENDPOINTS", "
{
    'endpointCredentials': [
        {'endpoint':'https://pkgs.dev.azure.com/1essharedassets/_packaging/BuildXL/nuget/v3/index.json', 'password':'$1esPat'}, 
        {'endpoint':'https://pkgs.dev.azure.com/cloudbuild/_packaging/BuildXL.Selfhost/nuget/v3/index.json', 'password':'$cbPat'}
    ]
}", "Process")

.\RunCheckInTests.cmd /lab $args /internal