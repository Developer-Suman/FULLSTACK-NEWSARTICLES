# Define the path to the .NET project
$projectPath = "C:\MyWorkSpace\MVC\DATABASERELATION-PAGINATION\DatabaseRelationship-Pagination\DatabaseRelationship-Pagination.csproj"

# Define the output directory for the published files
$outputPath = "D:\Publish"

# Define the configuration (Debug/Release)
$configuration = "Release"

# Define the target framework (e.g., net6.0, net7.0)
$framework = "net7.0"

# Define the remote server details
$remoteUser = "Silicon-Soft"
$remoteHost = "192.168.1.178"
$remotePath = "/Users/publish"  # Make sure this directory exists and is correct
$password = "Silicon321"  # Replace with your actual password

# Run the publish command
dotnet publish $projectPath `
    -c $configuration `
    -f $framework `
    -o $outputPath

# Check if the publish command was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Project published successfully to $outputPath"
    
    # Define the path to pscp.exe
    $pscpPath = "C:\Program Files\PuTTY\pscp.exe"  # Update this path if necessary

    # Construct the scp command
    $scpCommand = "& `"$pscpPath`" -r -pw ${password} -batch `"$outputPath\*`" ${remoteUser}@${remoteHost}:${remotePath}"
    Write-Host "Running command: $scpCommand"
    Invoke-Expression $scpCommand
    
    # Check if the pscp command was successful
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Files successfully copied to ${remoteHost}:${remotePath}"
    } else {
        Write-Host "Failed to copy files to the remote server"
    }
} else {
    Write-Host "Failed to publish the project"
}
