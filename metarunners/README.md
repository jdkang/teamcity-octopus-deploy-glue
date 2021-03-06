# TeamCity Metarunners

These set of XML [Meta Runners](https://confluence.jetbrains.com/display/TCD8/Working+with+Meta-Runner) are largely purpose-built wrappers to facilitate:

1. Not having to create many build configuration templates
2. Not having to disable build steps (which can be cumbersome to audit)

As such, these build runners largely wrap functionality of existing runners but offer "optionality" by **not failing the build** should no requisites be met. A further attempt to return fast should the build step not be needed is also made. As such, usually the build steps amount to ~1s of additional time per + additional time for CLI tools (e.g. nuget).

## Requirements
These are only assured to run on TeamCity `9.x` and agents running Powershell `3.0`+

## Reference

Script                                      | Wraps                         | Description
|:-----------------------------------------:|:-----------------------------:|:------------:
**NugetPack2.xml**                          | `Nuget.exe PACK`              | Packs `.nuspec` files in multiple folders into `.nupkg`
**NugetPushProGet.xml**                     | `Nuget.exe PUSH`              | Pushes `.nupkg` file(s) to various feeds (e.g. `/dev` vs `/main`) under the same base feed URL. Uses some logic to determine which package goes where based on name (e.g. pre-release)
**OctoCreateReleaseFromJsonMapping.xml**    | `Octo.exe create-release`     | Uses a JSON file checked into VC to create multiple Octopus Project Releases. This is in the case where a MSBuild Solution has a "one to many" relationship with Octopus Projects (e.g. service, web, database, etc)
**ReleaseNotesGenerateMarkdownFileSvn.xml** | `n/a`                         | Translates pending teamcity changes and other information into a boilerplate "release notes" markdown file to be consumed by Octopus Deploy when automaticlaly creating releases.

## Notes
### NugetPushProGet
This was designed around the multi-feed and URI scheme of [ProGet](http://inedo.com/proget). IMO, it's a very solid NuGet feed choice short of paying for Artifactory.

The seperation of pre-release and standard packages was designed to make upgrading **internal** nuget packages safer and easier. As such, this would involve selecting a "dev" and "main" nuget source depending. 

This is aided heavily by the fact ProGet (and Artifactory) allow the proxying of nuget.org packages. 

## OctoCreateReleaseFromJsonMapping
Assumes you have placed `octo.exe` in the [TeamCity Agent Tools directory](https://confluence.jetbrains.com/display/TCD8/Installing+Agent+Tools) as such `%teamcity.tool.octo%\octo.exe`

An example of the a JSON mapping file would look like:

```
{
    "sln": "name",
    "releases": [
        {
            "project": "Octopus Project Name",
            "nupkg": ["Package1","Package2"]
        }
    ]
}
```

Other details, such as release channel can be specified in the build step (e.g. via a build parameter).