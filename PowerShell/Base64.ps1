function ConvertTo-Base64 {
   
    [CmdletBinding()]
    Param ([Parameter(Mandatory = $True, ValueFromPipeline = $True)][String] $String)
 
    [System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes($String))
 
}

function ConvertFrom-Base64 {
   
    [CmdletBinding()]
    Param ([Parameter(Mandatory = $True, ValueFromPipeline = $True)][String] $base64string)
 
    [System.Text.Encoding]::ASCII.GetString(  [System.Convert]::FromBase64String($base64string))
 
}

#########################################################
# Example usage and test
#########################################################
$normalstring = 'Base64 is a group of similar binary-to-text encoding schemes that represent binary data in an ASCII string format by translating it into a radix-64 representation. The term Base64 originates from a specific MIME content transfer encoding.';
$expectedencodedoutput = 'QmFzZTY0IGlzIGEgZ3JvdXAgb2Ygc2ltaWxhciBiaW5hcnktdG8tdGV4dCBlbmNvZGluZyBzY2hlbWVzIHRoYXQgcmVwcmVzZW50IGJpbmFyeSBkYXRhIGluIGFuIEFTQ0lJIHN0cmluZyBmb3JtYXQgYnkgdHJhbnNsYXRpbmcgaXQgaW50byBhIHJhZGl4LTY0IHJlcHJlc2VudGF0aW9uLiBUaGUgdGVybSBCYXNlNjQgb3JpZ2luYXRlcyBmcm9tIGEgc3BlY2lmaWMgTUlNRSBjb250ZW50IHRyYW5zZmVyIGVuY29kaW5nLg==';

if ( ($normalstring | ConvertTo-Base64 ) -eq $expectedencodedoutput) {
    Write-Host -ForegroundColor Green "Passed";
}
else {
    Write-Host -ForegroundColor Red "Failed";
}

if (($expectedencodedoutput | ConvertFrom-Base64) -eq $normalstring) {
    Write-Host -ForegroundColor Green "Passed";
}
else {
    Write-Host -ForegroundColor Red "Failed";
}