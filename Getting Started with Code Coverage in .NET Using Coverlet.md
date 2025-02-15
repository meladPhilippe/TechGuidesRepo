# Coverlet Library: A Code Coverage Tool for .NET Applications
## Introduction
The Coverlet library is a popular code coverage tool for .NET applications, commonly used through two NuGet packages: `Coverlet.Collector` and `Coverlet.MSBuild`. These packages provide seamless integration with the .NET ecosystem, enabling developers to collect and analyze code coverage data efficiently.

## Coverlet.Collector
`Coverlet.Collector` integrates with the .NET Test Platform, which is used by the `dotnet test` command. It operates as a data collector during test execution, gathering code coverage data in the background.

### Steps to Use Coverlet.Collector  
Add the package to your project:  
`dotnet add package coverlet.collector`  
Run your tests with code coverage collection enabled:  
`dotnet test --collect:"XPlat Code Coverage"`  
 By default, it gathers coverage data using the Cobertura format and produces a file named coverage.cobertura.xml in a subfolder (named with a GUID) under the project’s TestResults directory.
 You can customize its behavior by appending additional parameters after the collector name (separated by semicolons).
 supported formats include: cobertura (default), opencover, json, lcov
 `dotnet test --collect:"XPlat Code Coverage;Format=opencover"`
 You can also provide a comma‑separated list of formats if you’d like multiple outputs, for example:
`dotnet test --collect:"XPlat Code Coverage;Format=json,lcov,cobertura"`
### Advanced Configuration  
Using a .runsettings File  
You can configure code coverage collection using a .runsettings file. For example:  
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat Code Coverage">
        <Configuration>
          <Format>lcov</Format>
          <OutputDirectory>coverage</OutputDirectory>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```
Run your tests with the settings file:  
`dotnet test --settings codecoverage.runsettings`  

## Coverlet.MSBuild
`Coverlet.MSBuild` integrates with MSBuild to collect code coverage data during the build process.

### Steps to Set Up Coverlet.MSBuild
1. Add the package to your project:
   `dotnet add package coverlet.msbuild`
2. Modify your .csproj file to enable coverage collection:
  
```xml
<PropertyGroup>
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutput>./coverage/</CoverletOutput>
  <CoverletOutputFormat>Cobertura</CoverletOutputFormat>
</PropertyGroup>
```
3. Run `dotnet test`  

## MSBuild Overview
MSBuild (Microsoft Build Engine) is the default build engine for .NET applications. It uses XML-based project files (e.g., .csproj, .vbproj) to define the structure of the project, including source files, dependencies, and build configurations.

### Key Concepts
Project File: The .csproj file defines the project structure, including source files, dependencies, and build configurations. MSBuild reads this file to determine the necessary actions.

### Targets and Tasks:

#### Tasks: The smallest units of work in MSBuild, representing individual actions like compiling code (Csc), copying files (Copy), or executing commands (Exec).

#### Targets: Groups of tasks. For example:

The Build target compiles the project.

The Test target runs unit tests.

The Publish target packages the application for deployment.

### Execution
When you run commands like dotnet build or dotnet test, the .NET CLI invokes MSBuild behind the scenes to execute the appropriate targets.

### Commmand
`dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover`

## How Coverlet Integrates with MSBuild
`Coverlet.MSBuild` adds custom tasks that run with the Test target.  

`Coverlet.Collector` registers itself as a data collector with the .NET Test Platform.  

### Why Don’t You See &lt;Target&gt; or &lt;Task&gt; Tags?  
When you run dotnet build or dotnet test, the .NET SDK automatically includes predefined targets and tasks (e.g., Build, Clean, Publish). To inspect the full build process, including implicit targets and tasks, you can generate a preprocessed MSBuild file using the following command:  
`dotnet msbuild -preprocess`  

## Configuring Coverage Gutters in VS Code  
To display code coverage information in Visual Studio Code using the Coverage Gutters extension, add the following to your settings.json file"  
```json
"coverage-gutters.coverageFileNames": [
    "lcov.info",
    "cov.xml",
    "coverage.xml",
    "cobertura.xml",
    "jacoco.xml",
    "coverage.cobertura.xml",
    "coverage.info"
],
"coverage-gutters.coverageReportFileName": "./KantarProfiles.Utility.API.Tests/TestResults/**/{coverage.info,lcov.info,coverage.xml,cobertura.xml}"
```
## Conclusion
By leveraging Coverlet and MSBuild, developers can seamlessly integrate code coverage into their .NET development workflow, ensuring robust and well-tested applications.
