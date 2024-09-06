# Define the path to the .NET project
$projectPath = "C:\MyWorkSpace\FULLSTACK\NewAndArticles\BackEnd\MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY\MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY.csproj"

# Define the output directory for the published files
$outputPath = "D:\Publish"

# Define the configuration (Debug/Release)
$configuration = "Release"

# Define the target framework (e.g., net6.0, net7.0)
$framework = "net7.0"

# Run the publish command
dotnet publish $projectPath `
    -c $configuration `
    -f $framework `
    -o $outputPath

# Check if the publish command was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Project published successfully to $outputPath"
} else {
    Write-Host "Failed to publish the project"
}
