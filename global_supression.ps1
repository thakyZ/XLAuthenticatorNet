using namespace System;
using namespace System.Collections;
using namespace System.Collections.Generic;
using namespace System.Collections.ObjectModel;
using namespace System.Diagnostics;
using namespace System.IO;
using namespace System.Linq;
using namespace System.Management;
using namespace System.Management.Automation;
using namespace System.Net;
using namespace System.Security.Cryptography;
using namespace System.Security.Principal;
using namespace System.ServiceProcess;
using namespace System.Text;
using namespace System.Text.Json;
using namespace System.Text.RegularExpressions;
using namespace Microsoft.Automation;
using namespace Microsoft.PowerShell.Commands;

[CmdletBinding()]
Param(
  # Specifies a path to the CSV or the current GlobalSuppression to import from.
  [Parameter(Mandatory = $True,
             Position = 0,
             ValueFromPipeline = $True,
             ValueFromPipelineByPropertyName = $True,
             HelpMessage = "Path to the CSV or the current GlobalSuppression to import from.")]
  [Alias("PSPath","PSInputPath","Input")]
  [ValidateNotNullOrEmpty()]
  [string]
  $InputPath,
  # Specifies a path to the CSV or the current GlobalSuppression to export to.
  [Parameter(Mandatory = $True,
             Position = 1,
             ValueFromPipeline = $True,
             ValueFromPipelineByPropertyName = $True,
             HelpMessage = "Path to the CSV or the current GlobalSuppression to export to.")]
  [Alias("PSOutputPath","Output")]
  [ValidateNotNullOrEmpty()]
  [string]
  $OutputPath
)

Begin {
  Function Get-KeyValuePair {
    [CmdletBinding()]
    [OutputType([Hashtable])]
    Param(
      [Parameter(Mandatory = $True,
                 HelpMessage = 'Provides the GroupCollection from a RegEx match.')]
      [ValidateNotNullOrEmpty()]
      [GroupCollection]
      $Groups,
      [Parameter(Mandatory = $True,
                 HelpMessage = 'The group index to fetch the keys from.')]
      [ValidateNotNull()]
      [int]
      $KeyIndex,
      [Parameter(Mandatory = $True,
                 HelpMessage = 'The group index to fetch the values from.')]
      [ValidateNotNull()]
      [int]
      $ValueIndex,
      [Parameter(Mandatory = $True,
                 HelpMessage = 'Matches the value to the specified key.')]
      [ValidateNotNull()]
      [string]
      $MatchCaptureIndexOfKey
    )

    Begin {
      [Hashtable] $Output = $Null;
      If ($Groups[$KeyIndex].Captures.Count -ne $Groups[$ValueIndex].Captures.Count) {
        Throw 'The length of the key capture collection and value capture collection are not equal.';
      }
    } Process {
      For ($I = 0; $I -lt $Groups[$KeyIndex].Captures.Count; $I++) {
        If ($Groups[$KeyIndex].Captures[$I].Value.Equals($MatchCaptureIndexOfKey)) {
          If ($Null -eq $Output) {
            $Output = @{
              Key = "$($Groups[$KeyIndex].Captures[$I].Value)";
              Value = "$($Groups[$ValueIndex].Captures[$I].Value)";
            };
          } Else {
            Write-Warning -Message "Found another key that matched the value of parameter -MatchCaptureIndexOfKey using first one only. Found: $($MatchCaptureIndexOfKey)";
          }
        }
      }
    } End {
      Write-Output -NoEnumerate -InputObject $Output;
    }
  }

  Function Test-IsEmpty {
    [CmdletBinding()]
    [OutputType([bool])]
    Param(
      [Parameter(Mandatory = $True,
                 Position = 0,
                 HelpMessage = 'The input object to check if it has any empty NoteProperties.')]
      [AllowNull()]
      [object]
      $InputObject
    )

    Begin {
      [bool] $Output = $True;
    } Process {
      If ($Null -ne $InputObject) {
        ForEach ($NoteProperty in @($InputObject | Get-Member -MemberType NoteProperty)) {
          [object] $Value = $InputObject.PSObject.Properties[$NoteProperty.Name].Value;
          If (($Value -isnot [string] -and $Null -ne $Value) -or ($Value -is [string] -and -not [string]::IsNullOrEmpty($Value))) {
            $Output = $False;
          }
        }
      }
    } End {
      Write-Output -NoEnumerate -InputObject $Output;
    }
  }

  Function Get-Values {
    [CmdletBinding()]
    [OutputType([object[]])]
    Param(
      [Parameter(Mandatory = $True,
                 Position = 0,
                 HelpMessage = 'The input object to check if it has any empty NoteProperties.')]
      [AllowNull()]
      [object]
      $InputObject
    )

    Begin {
      [object[]] $Output = @();
    } Process {
      If ($Null -ne $InputObject) {
        ForEach ($NoteProperty in @($InputObject | Get-Member -MemberType NoteProperty)) {
          $Output += $InputObject.PSObject.Properties[$NoteProperty.Name].Value;
        }
      } Else {
        $Output = $Null;
      }
    } End {
      Write-Output -NoEnumerate -InputObject $Output;
    }
  }

  Function Get-Keys {
    [CmdletBinding()]
    [OutputType([string[]])]
    Param(
      [Parameter(Mandatory = $True,
                 Position = 0,
                 HelpMessage = 'The input object to check if it has any empty NoteProperties.')]
      [AllowNull()]
      [object]
      $InputObject
    )

    Begin {
      [string[]] $Output = @();
    } Process {
      If ($Null -ne $InputObject) {
        ForEach ($NoteProperty in @($InputObject | Get-Member -MemberType NoteProperty)) {
          $Output += $NoteProperty.Name;
        }
      } Else {
        $Output = $Null;
      }
    } End {
      Write-Output -NoEnumerate -InputObject $Output;
    }
  }

  If ($Null -eq (Get-Command -Name 'ConvertTo-Hashtable' -ErrorAction SilentlyContinue)) {
    Function ConvertTo-Hashtable {
      [CmdletBinding()]
      [OutputType([Hashtable])]
      Param(
        [Parameter(Mandatory = $True,
                   Position = 0,
                   HelpMessage = 'The input object to check if it has any empty NoteProperties.')]
        [AllowNull()]
        [PSCustomObject]
        $InputObject
      )

      Begin {
        [Hashtable] $Output = $Null;
      } Process {
        If ($Null -ne $InputObject) {
          $Output = @{};
          ForEach ($Item in $InputObject.PSObject.Properties) {
            $Output[$Item.Name] = (Invoke-Expression -Command "[$($Item.TypeNameOfValue)]($($Item.Value))");
          }
        }
      } End {
        Write-Output -NoEnumerate -InputObject $Output;
      }
    }
  }

  If ($Null -eq (Get-Command -Name 'ConvertTo-HashtableArray' -ErrorAction SilentlyContinue)) {
    Function ConvertTo-HashtableArray {
      [CmdletBinding()]
      [OutputType([Hashtable[]])]
      Param(
        [Parameter(Mandatory = $True,
                   Position = 0,
                   HelpMessage = 'The input object to check if it has any empty NoteProperties.')]
        [AllowNull()]
        [PSCustomObject[]]
        $InputObject
      )

      Begin {
        [Hashtable[]] $Output = $Null;
      } Process {
        If ($Null -ne $InputObject) {
          $Output = @();
          ForEach ($Item in $InputObject) {
            $Output += (ConvertTo-Hashtable -InputObject $Item);
          }
        }
      } End {
        Write-Output -NoEnumerate -InputObject $Output;
      }
    }
  }

  [string] $Direction = 'None'; # Either 'CSV->GlobalSuppressions' or 'GlobalSuppressions->CSV'
  [FileInfo] $InputFile = (Get-Item -LiteralPath $InputPath -ErrorAction Stop);
  [FileInfo] $OutputFile = (Get-Item -LiteralPath $OutputPath -ErrorAction SilentlyContinue);
  If ($InputFile.Extension -eq '.csv' -and (Split-Path -Path $OutputPath -Extension) -eq '.csv') {
    Throw 'Parameter -InputPath and -OutputPath have the same extension.';
  } ElseIf ($InputFile.Extension -eq 'GlobalSuppressions.cs' -and (Split-Path -Path $OutputPath -Leaf) -eq 'GlobalSuppressions.cs') {
    Throw 'Parameter -InputPath and -OutputPath have the same extension.';
  } ElseIf ($InputFile.Extension -eq '.csv') {
    $Direction = 'CSV->GlobalSuppressions';
  } ElseIf ($InputFile.Name -eq 'GlobalSuppressions.cs') {
    $Direction = 'GlobalSuppressions->CSV';
  }
  If ($Null -ne $OutputFile) {
    [string] $Prompt = (Read-Host -Prompt "Replace file at `"$($OutputFile)`"? [y/N]");
    If ($Prompt -notmatch 'y(?:es)?') {
      Exit 0;
    }
  }
  [string[]] $GlobalSuppressionHeader = (@"
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;
"@ -split '\r?\n');
  [string[]] $InputText = (Get-Content -LiteralPath $InputFile);
  [List[[PSCustomObject]]] $CSVTable = [List[[PSCustomObject]]]::new();
  [List[[string]]] $OutputText = [List[[string]]]::new();
  [RegEx] $SuppressMessageRegex = [RegEx]::new('(?<=\[assembly: SuppressMessage\()"(.*?)", ?"(.*?)"(, ?(.*?) ?= ?"(.*?)")*(?=\)\])');
} Process {
  # Load Input
  If ($Direction -eq 'CSV->GlobalSuppressions') {
    [PSCustomObject[]] $Csv = (Import-Csv -LiteralPath $InputFile -Encoding 'Utf-8');

    # Un-Quote Each Cell
    For ($Row = 0; $Row -lt $Csv.Length; $Row++) {
      [PSPropertyInfo[]] $Properties = $Csv[$Row].PSObject.Properties;
      For ($Col = 0; $Col -lt $Properties.Length; $Col++) {
        $Csv[$Row].PSObject.Properties["$($Properties[$Col].Name)"].Value = $Properties[$Col].Value -replace '^""?"?', '' -replace '"?"?"$', '';
      }
    }

    $CSVTable.AddRange($Csv);
  } ElseIf ($Direction -eq 'GlobalSuppressions->CSV') {
    ForEach ($Line in $InputText) {
      If ([string]::IsNullOrEmpty($Line) -or [string]::IsNullOrWhitespace($Line)) {
        $CSVTable.Add([PSCustomObject]@{ Category = ''; CheckId = ''; Scope = ''; Target = ''; MessageId = ''; Justification = ''; Condition = ''; Comment = ''; });
      } Else {
        $CSVTable.Add([PSCustomObject]@{
          Category = $SuppressMessageRegex.IsMatch($Line) ? $SuppressMessageRegex.Match($Line).Groups[1].Value : '';
          CheckId = $SuppressMessageRegex.IsMatch($Line) ? $SuppressMessageRegex.Match($Line).Groups[2].Value : '';
          Scope = $SuppressMessageRegex.IsMatch($Line) ? (Get-KeyValuePair -Groups $SuppressMessageRegex.Match($Line).Groups -MatchCaptureIndexOfKey 'Scope' -KeyIndex 4 -ValueIndex 5).Value : '';
          Target = $SuppressMessageRegex.IsMatch($Line) ? (Get-KeyValuePair -Groups $SuppressMessageRegex.Match($Line).Groups -MatchCaptureIndexOfKey 'Target' -KeyIndex 4 -ValueIndex 5).Value : '';
          MessageId = $SuppressMessageRegex.IsMatch($Line) ? (Get-KeyValuePair -Groups $SuppressMessageRegex.Match($Line).Groups -MatchCaptureIndexOfKey 'MessageId' -KeyIndex 4 -ValueIndex 5).Value : '';
          Justification = $SuppressMessageRegex.IsMatch($Line) ? (Get-KeyValuePair -Groups $SuppressMessageRegex.Match($Line).Groups -MatchCaptureIndexOfKey 'Justification' -KeyIndex 4 -ValueIndex 5).Value : '';
          Condition = $Line.Contains('#if') -or $Line.Contains('#else') -or $Line.Contains('#endif') ? $Line : '';
          Comment = $Line.Contains('//') ? ($Line -split '(//)', 2)[2] : '';
        });
      }
    }
  } Else {
    Throw 'Failed to set direction variable!';
  }

  # Parse to Output
  If ($Direction -eq 'CSV->GlobalSuppressions') {
    $OutputText.AddRange($GlobalSuppressionHeader);
    ForEach ($Row in $CSVTable) {
      If (-not [string]::IsNullOrEmpty($Row.Comment)) {
        $OutputText.Add("//$($Row.Comment)");
      } ElseIf (-not [string]::IsNullOrEmpty($Row.Condition)) {
        $OutputText.Add($Row.Condition);
      } ElseIf (Test-IsEmpty -InputObject $Row) {
        $OutputText.Add([string]::Empty);
      } Else {
        [string] $Temp = "[assembly: SuppressMessage(`"$($Row.Category)`", `"$($Row.CheckId)`"";
        If (-not [string]::IsNullOrEmpty($Row.Scope)) {
          $Temp += ", Scope = `"$($Row.Scope)`""
        }
        If (-not [string]::IsNullOrEmpty($Row.Target)) {
          $Temp += ", Target = `"$($Row.Target)`""
        }
        If (-not [string]::IsNullOrEmpty($Row.MessageId)) {
          $Temp += ", MessageId = `"$($Row.MessageId)`""
        }
        If (-not [string]::IsNullOrEmpty($Row.Justification)) {
          $Temp += ", Justification = `"$($Row.Justification)`""
        }
        $Temp += ")]";
        $OutputText.Add($Temp);
      }
    }
  } ElseIf ($Direction -eq 'GlobalSuppressions->CSV') {
    # Remove the first four entires;
    For ($I = 0; $I -lt 6; $I++) {
      $CSVTable.RemoveAt(0);
    }
    <#
    # Remove any empty entires
    For ($I = 0; $I -lt $CSVTable.Count; $I++) {
      If (Test-IsEmpty -InputObject $CSVTable[$I]) {
        $CSVTable.RemoveAt($I);
        $I--;
      }
    }
    #>
  } Else {
    Throw 'Failed to set direction variable!';
  }
} End {
  If ($Direction -eq 'CSV->GlobalSuppressions') {
    $OutputText | Out-File -Force -LiteralPath $OutputPath;
  } ElseIf ($Direction -eq 'GlobalSuppressions->CSV') {
    $CSVTable | Export-Csv -LiteralPath $OutputPath -UseQuotes Always -NoTypeInformation -Encoding 'Utf-8';
  } Else {
    Throw 'Failed to set direction variable!';
  }
}
