#https://github.com/EnclaveConsulting/SANS-SEC505

function Convert-HexStringToByteArray {
    ################################################################
    #.Synopsis
    # Convert a string of hex data into a System.Byte[] array. An
    # array is always returned, even if it contains only one byte.
    #.Parameter String
    # A string containing hex data in any of a variety of formats,
    # including strings like the following, with or without extra
    # tabs, spaces, quotes or other non-hex characters:
    # 0x41,0x42,0x43,0x44
    # \x41\x42\x43\x44
    # 41-42-43-44
    # 41424344
    # The string can be piped into the function too.
    ################################################################
    [CmdletBinding()]
    Param ([Parameter(Mandatory = $True, ValueFromPipeline = $True)][String] $String)
 
    #Clean out whitespaces and any other non-hex crud.
    $String = $String.ToLower() -replace '[^a-f0-9\\,x\-\:]', "
 
#Try to put into canonical colon-delimited format.
$String = $String -replace '0x|\x|\-|,',':'
 
#Remove beginning and ending colons, and other detritus.
$String = $String -replace '^:+|:+$|x|\',"
 
    #Maybe there's nothing left over to convert...
    if ($String.Length -eq 0) { , @(); return }
 
    #Split string with or without colon delimiters.
    if ($String.Length -eq 1)
    { , @([System.Convert]::ToByte($String, 16))}
    elseif (($String.Length % 2 -eq 0) -and ($String.IndexOf(":") -eq -1))
    { , @($String -split '([a-f0-9]{2})' | foreach-object { if ($_) {[System.Convert]::ToByte($_, 16)}})}
    elseif ($String.IndexOf(":") -ne -1)
    { , @($String -split ':+' | foreach-object {[System.Convert]::ToByte($_, 16)})}
    else
    { , @()}
    #The strange ",@(...)" syntax is needed to force the output into an
    #array even if there is only one element in the output (or none).
}

exit

######################################################
# Example usage:
# Comparing Azure MD5 to Local MD5 Hash
######################################################
# Snippet:
#   Assumes $StorageContainer01
[Microsoft.WindowsAzure.Commands.Common.Storage.ResourceModel.AzureStorageBlob[]] $blob01ref = $null
$blob01ref = $StorageContainer01 | Set-AzureStorageBlobContent -File $LocalFile.FullName -Blob $LocalFile.Name;
$LocalFile = "C:\temp\a.out";
Write-Information "`tComparing MD5 Hashes before removing $($LocalFile.Name)"
$RemoteHash = $blob01ref[0].ICloudBlob.Properties.ContentMD5;
$LocalFileHash = Get-FileHash -LiteralPath $LocalFile.FullName -Algorithm MD5
$LocalHash = $LocalFileHash.Hash | Convert-HexStringToByteArray | ConvertTo-Base64

#Compare local to remote hash
if ($RemoteHash -eq $LocalHash) {
    Write-Information "`t`tRemoving Local file $($LocalFile.Name)";
    Remove-Item -LiteralPath $LocalFile.FullName -WhatIf:$true
}
else {
    Write-Error "Local and Remote Hashes are different $($LocalFile.Name)";
}