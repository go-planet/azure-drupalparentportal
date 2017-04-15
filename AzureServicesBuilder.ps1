
Add-AzureRmAccount


$UserName = Read-Host -Prompt "Azure Username? "
$SecurePassword = Read-Host -Prompt "Azure Password? " -AsSecureString

$Credentials = New-Object System.Management.Automation.PSCredential -ArgumentList $UserName, $SecurePassword

$BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($SecurePassword)
$PlainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)


$servicePlanName    = Read-Host -Prompt "What is the name of the app service plan? " # ContosoAppServicePlan
$serviceLocation    = Read-Host -Prompt "Where is the service hosted?" # "South Central US"
$serviceResourceG   = Read-Host -Prompt "What is the resource group name? " # ContosoAzureResourceGroup
$serviceTier        = Read-Host -Prompt "What service tier? Minimum must be Basic. " #Premium
$serviceWorkerSize  = Read-Host -Prompt "What is the worker size? " # Large
$serviceWorkerCount = Read-Host -Prompt "How many workers?" # 10

#############################################
# Create new, or get existing resource group
#############################################

#$resourceGroupName = "{resource-group-name}"
#$resourceGroupLocation = "{resource-group-location}"

$myResourceGroup = Get-AzureRmResourceGroup -Name $serviceResourceG -ea SilentlyContinue

if(!$myResourceGroup)
{
   Write-Output "Creating resource group: $serviceResourceG"
   $myResourceGroup = New-AzureRmResourceGroup -Name $serviceResourceG -Location $serviceLocation
}
else
{
   Write-Output "Resource group $serviceResourceG already exists:"
}
$myResourceGroup

#############################################
# Create an App Service Plan in an App Service Environment
#
# Name:              name of the app service plan.
# Location:          service plan location.
# ResourceGroupName: resource group that includes the newly created app service plan.
# Tier:              the desired pricing tier (Default is Free, other options are Shared, Basic, Standard, and Premium.)
# WorkerSize:        the size of workers (Default is small if the Tier parameter was specified as Basic, Standard, 
#                    or Premium. Other options are Medium, and Large.)
# NumberofWorkers:   the number of workers in the app service plan (Default value is 1).
#
# New-AzureRmAppServicePlan -Name ContosoAppServicePlan -Location "South Central US" -ResourceGroupName ContosoAzureResourceGroup -Tier Premium -WorkerSize Large -NumberofWorkers 10
#
#################################
New-AzureRmAppServicePlan -Name $servicePlanName -Location $serviceLocation -ResourceGroupName $serviceResourceG -Tier Basic -WorkerSize Small -NumberofWorkers 1

#############################################
# Create a new, or get existing server
#############################################

$serverName = Read-Host -Prompt "What is the SQL Server name?"
$serverVersion = "12.0"
$serverLocation = $serviceLocation
$serverResourceGroupName = $serviceResourceG

$serverAdmin = $UserName
$serverAdminPassword = $SecurePassword

$securePassword = $SecurePassword # ConvertTo-SecureString -String $serverAdminPassword -AsPlainText -Force
$serverCreds = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $serverAdmin, $securePassword

$myServer = Get-AzureRmSqlServer -ServerName $serverName -ResourceGroupName $serverResourceGroupName -ea SilentlyContinue
if(!$myServer)
{
   Write-Output "Creating SQL server: $serverName"
   $myServer = New-AzureRmSqlServer -ResourceGroupName $serverResourceGroupName -ServerName $serverName -Location $serverLocation -ServerVersion $serverVersion -SqlAdministratorCredentials $serverCreds
}
else
{
   Write-Output "SQL server $serverName already exists:"
}
$myServer

$serverFirewallRuleName = "BuilderCreatedRule"
$serverFirewallStartIp = Read-Host "What is your public IP?"
$serverFirewallEndIp = $serverFirewallStartIp

$myFirewallRule = Get-AzureRmSqlServerFirewallRule -FirewallRuleName $serverFirewallRuleName -ServerName $serverName -ResourceGroupName $serverResourceGroupName -ea SilentlyContinue

if(!$myFirewallRule)
{
   Write-host "Creating server firewall rule: $serverFirewallRuleName"
   $myFirewallRule = New-AzureRmSqlServerFirewallRule -ResourceGroupName $serverResourceGroupName -ServerName $serverName -FirewallRuleName $serverFirewallRuleName -StartIpAddress $serverFirewallStartIp -EndIpAddress $serverFirewallEndIp
}
else
{
   Write-host "Server firewall rule $serverFirewallRuleName already exists:"
}
$myFirewallRule

# Allow Azure services to access the server
$serverFirewallRuleName2 = "allowAzureServices"
$serverFirewallStartIp2 = "0.0.0.0"
$serverFirewallEndIp2 = "0.0.0.0"

$myFirewallRule2 = Get-AzureRmSqlServerFirewallRule -FirewallRuleName $serverFirewallRuleName2 -ServerName $serverName -ResourceGroupName $serverResourceGroupName -ea SilentlyContinue

if(!$myFirewallRule2)
{
   Write-host "Creating server firewall rule: $serverFirewallRuleName2"
   $myFirewallRule2 = New-AzureRmSqlServerFirewallRule -ResourceGroupName $serverResourceGroupName -ServerName $serverName -FirewallRuleName $serverFirewallRuleName2 -StartIpAddress $serverFirewallStartIp2 -EndIpAddress $serverFirewallEndIp2
}
else
{
   Write-host "Server firewall rule $serverFirewallRuleName2 already exists:"
}
$myFirewallRule2

#$resourceGroupName = "{resource-group-name}"
#$serverName = "{server-name}"

$databaseName = "AdventureWorksLT"
$databaseEdition = "Basic"
$databaseServiceLevel = "Basic"

$storageKeyType = "SharedAccessKey"
$storageUri = "https://sqldbtutorial.blob.core.windows.net/bacpacs/AdventureWorksLT.bacpac"
$storageKey = "?"

$importRequest = New-AzureRmSqlDatabaseImport -ResourceGroupName $resourceGroupName -ServerName $serverName -DatabaseName $databaseName -StorageKeytype $storageKeyType -StorageKey $storageKey -StorageUri $storageUri -AdministratorLogin $serverAdmin -AdministratorLoginPassword $securePassword -Edition $databaseEdition -ServiceObjectiveName $databaseServiceLevel -DatabaseMaxSizeBytes 5000000


Do {
     $importStatus = Get-AzureRmSqlDatabaseImportExportStatus -OperationStatusLink $importRequest.OperationStatusLink
     Write-host "Importing database..." $importStatus.StatusMessage
     Start-Sleep -Seconds 30
     $importStatus.Status
   }
   until ($importStatus.Status -eq "Succeeded")

$importStatus




#############################################
# Create a new storage account
#############################################

# Update with the name of your subscription.
$SubscriptionName = "YourSubscriptionName"

# Give a name to your new storage account. It must be lowercase!
$StorageAccountName = "msdpp.ParentPortalStorage"

# Choose "West US" as an example.
$Location = $serviceLocation

# Give a name to your new container.
$ContainerName = "imagecontainer"

# Add your Azure account to the local PowerShell environment.
Add-AzureAccount

# Set a default Azure subscription.
Select-AzureSubscription -SubscriptionName $SubscriptionName –Default

# Create a new storage account.
New-AzureStorageAccount –StorageAccountName $StorageAccountName -Location $Location

# Set a default storage account.
Set-AzureSubscription -CurrentStorageAccountName $StorageAccountName -SubscriptionName $SubscriptionName

# Create a new container.
New-AzureStorageContainer -Name $ContainerName -Permission Off

#############################################
# Create a new azure ad
#############################################
New-AzureRmADApplication -DisplayName "NewApplication" -HomePage "http://www.Contoso.com" -IdentifierUris "http://NewApplication"


# Azure AD provisioning



########################################
# BETTER Firewall Code
#Add-AzureRmAccount
#

#$serviceResourceG   = Read-Host -Prompt "What is the resource group name? " # ContosoAzureResourceGroup

#$serverFirewallRuleName = "BuilderCreatedRule"
#$serverName = Read-Host "What is the name of the SQL Server?"
#$serverFirewallStartIp = Read-Host "What is your public IP?"
#$serverFirewallEndIp = $serverFirewallStartIp

#Get-AzureRmSubscription
#$subscription = Read-Host "Which subscription Id to use?"

#Set-AzureRmContext -SubscriptionId $subscription

#$myResourceGroup = Get-AzureRmResourceGroup -Name $serviceResourceG -ea SilentlyContinue
#Write-host "Resource group result: " + $myResourceGroup

#$myFirewallRule = Get-AzureRmSqlServerFirewallRule -FirewallRuleName $serverFirewallRuleName -ServerName $serverName -ResourceGroupName $serverResourceGroupName -ea SilentlyContinue

#if(!$myFirewallRule)
#{
   #Write-host "Creating server firewall rule: $serverFirewallRuleName"
   #$myFirewallRule = New-AzureRmSqlServerFirewallRule -ResourceGroupName $serverResourceGroupName -ServerName $serverName -FirewallRuleName $serverFirewallRuleName -StartIpAddress $serverFirewallStartIp -EndIpAddress $serverFirewallEndIp
#}
#else
#{
   #Write-host "Server firewall rule $serverFirewallRuleName already exists:"
#}
#$myFirewallRule

## Allow Azure services to access the server
#$serverFirewallRuleName2 = "allowAzureServices"
#$serverFirewallStartIp2 = "0.0.0.0"
#$serverFirewallEndIp2 = "0.0.0.0"

#$myFirewallRule2 = Get-AzureRmSqlServerFirewallRule -FirewallRuleName $serverFirewallRuleName2 -ServerName $serverName -ResourceGroupName $serverResourceGroupName -ea SilentlyContinue

#if(!$myFirewallRule2)
#{
   #Write-host "Creating server firewall rule: $serverFirewallRuleName2"
   #$myFirewallRule2 = New-AzureRmSqlServerFirewallRule -ResourceGroupName $serverResourceGroupName -ServerName $serverName -FirewallRuleName $serverFirewallRuleName2 -StartIpAddress $serverFirewallStartIp2 -EndIpAddress $serverFirewallEndIp2
#}
#else
#{
   #Write-host "Server firewall rule $serverFirewallRuleName2 already exists:"
#}
#$myFirewallRule2

#$removeFirewallRule = Read-Host "Would you like to remove the firewall rule? [y]"

#if($removeFirewallRule == "y")
#{
    #Remove-AzureRmSqlServerFirewallRule -FirewallRuleName $serverFirewallRuleName -ServerName $serverName
#}
########################################