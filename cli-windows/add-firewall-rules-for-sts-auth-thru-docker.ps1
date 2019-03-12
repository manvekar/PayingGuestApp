param([switch]$Elevated)
function Check-Admin {
$currentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
$currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
}
if ((Check-Admin) -eq $false)  {
if ($elevated)
{
# could not elevate, quit
}
 
else {
 
Start-Process powershell.exe -Verb RunAs -ArgumentList ('-noprofile -noexit -file "{0}" -elevated' -f ($myinvocation.MyCommand.Definition))
}
exit
}

try {
  Get-NetFirewallRule -DisplayName FoodAppDocker -ErrorAction Stop
  Write-Host "Rule found"
}
  catch [Exception] {
  New-NetFirewallRule -DisplayName FoodAppOnContainers-Inbound -Confirm -Description "FoodApp Inbound Rule for port range 6100-6150" -LocalAddress Any -LocalPort 6100-6150 -Protocol tcp -RemoteAddress Any -RemotePort Any -Direction Inbound
  New-NetFirewallRule -DisplayName FoodAppOnContainers-Outbound -Confirm -Description "FoodApp Outbound Rule for port range 6100-6150" -LocalAddress Any -LocalPort 6100-6150 -Protocol tcp -RemoteAddress Any -RemotePort Any -Direction Outbound
}