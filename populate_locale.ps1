using namespace System;
using namespace System.IO;
using namespace System.Linq;
using namespace System.Text.RegularExpressions;
using namespace Microsoft.PowerShell.Commands;

[CmdletBinding()]
Param(
  [Parameter(Mandatory = $False,
             HelpMessage = 'Prints to console and not writing to file.')]
  [switch]
  $DryRun
)

Begin {
  [DirectoryInfo] $LocaleFolder = [DirectoryInfo]::new((Join-Path -Path $PSScriptRoot -ChildPath (Join-Path -Path 'Resources' -ChildPath 'Loc')));

  If (-not $LocaleFolder.Exists) {
    Throw 'Could not find locale folder';
  }

  [FileInfo[]] $LocaleFiles = (Get-ChildItem -LiteralPath $LocaleFolder -File -Filter '*.json');
  [FileInfo[]] $CSFiles = (Get-ChildItem -LiteralPath $PSScriptRoot -File -Filter '*.cs' -Recurse -Exclude @('obj','bin','.git','.vs','.idea'));
  [PSCustomObject[]] $Pairs = @();
  [string] $Regex = 'Loc\.Localize\((?:(?: ?\+ ?)?(?:"|nameof\()(.*?)[")])+, "(.*?)"\);?';
} Process {
  ForEach ($CSFile in $CSFiles) {
    [string[]] $Content = (Get-Content -LiteralPath $CSFile.FullName);

    [MatchInfo[]] $Selected = ($Content | Select-String -Pattern $Regex -AllMatches);

    If ($Null -eq $Selected.Matches -or $Selected.Matches.Count -eq 0) {
      Continue;
    }

    ForEach ($Match in $Selected.Matches) {
      If ($Match.Groups.Count -ne 3) {
        Write-Output -InputObject ($Match | Get-Member) | Out-Host;
        Write-Warning -Message "Failed to find any matches dispite matching in file at path `"$($CSFile)`" at line $($Match.Line).";
        Continue;
      }

      [PSCustomObject] $Item = [PSCustomObject]::new();
      [string] $Key = [string]::Empty;

      ForEach ($Capture in $Match.Groups[1].Captures) {
        [string] $KeyPart = $Capture.Value;

        if ($KeyPart.Contains('.')) {
          $KeyPart = $KeyPart.Split('.')[-1];
        }

        $Key += $KeyPart;
      }

      $Item | Add-Member -Name 'Key' -Value $Key -MemberType NoteProperty;
      $Item | Add-Member -Name 'Value' -Value $Match.Groups[2].Value -MemberType NoteProperty;
      $Pairs += $Item;
    }
  }

  ForEach ($LocaleFile in $LocaleFiles) {
    [Hashtable] $Content = $Null;

    If ($PSVersionTable.PSVersion.Major -ge 6) {
      $Content = (Get-Content -LiteralPath $LocaleFile | ConvertFrom-Json -Depth 100 -AsHashtable);
    } Else {
      $Content = @{};
      (ConvertFrom-Json -InputObject (Get-Content -LiteralPath $LocaleFile.FullName -Raw)).PSObject.Properties | Foreach {
        $Content[$_.Name] = $_.Value
      }
    }

    If ($Null -eq $Content) {
      Write-Warning -Message "Failed to parse locale file at path `"$($LocaleFile.FullName)`".";
      Continue;
    }

    ForEach ($Pair in @($Pairs)) {
      If (-not $Content.ContainsKey($Pair.Key)) {
        $Content.Add($Pair.Key, $Pair.Value);
      }
    }
    If ($DryRun) {
      Write-Output -InputObject ((ConvertTo-Json -Depth 100 -InputObject $Content) -replace '\\\\\\?(.)','\$1');
    } Else {
      [string] $JsonText = (ConvertTo-Json -Depth 100 -InputObject $Content);
      [string] $TransformedEscapes = ($JsonText -replace '\\\\\\?(.)','\$1');
      Out-File -LiteralPath $LocaleFile.FullName -InputObject $TransformedEscapes;
    }
  }
} End {
}