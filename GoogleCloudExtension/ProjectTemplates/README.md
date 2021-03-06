# Project templates for the Visual Studio extension
This directory contains all of the project templates to be used in the extension.

## Procedure to publish, or update, an project template
To publish, or update, a project template it is recommended to just use the
[publish_all_templates.sh](../tools/publish_all_templates.sh) script. This script will enumerate all of the templates in this 
directory and publish them to
the right location under the extension codebase.

The typical set of steps then is:
* Do your modifications to the template, code, MyTemplate.vstemplate, etc...
* Use the [publish_all_templates.sh](../tools/publish_all_templates.sh) script to publish the changes to the extension 
codebase.
* Build the extension in Visual Studio and start it.
* Use the experimental Visual Studio instance to create a new project based on your changed template, verify that the template 
expands to what you expect. Verify that the project produced from the template builds.
* Repeat.
* Enable the template for the right version of VS.
  + .NET Core templates will have to be done twice, one for VS 2015 and another for VS 2017. You also have to ensure that
    the template is only enabled for the right version of vs.
  + .NET 4.x templates should be enabled in both VS 2015 and 2017.

### Control visibility of the templates for VS 2015
Since VS 2015 shows all templates by default you don't have to do anything special to show the templates in VS 2015.

If you wish to prevent the template from showing in VS 2015 you need to set the `<Hidden>` attribute for the template to 
`true`. This attribute is set under the `<TemplateData>` attribute in the `MyTemplate.vstemplate` file for the template.

### Control visibility of the template for VS 2017
In order for VS 2017 to use the new template, it needs to be added to the `.vstman` manifest with all of the VS 2017 visible 
templates for the extension. This file is located at 
[ProjectTemplates](../GoogleCloudExtension/ProjectTemplates/templateManifest0.noloc.vstman). You must copy the 
`<TemplateData>` element from the `MyTemplate.vstemplate` file for the new template into the manifest.

If, on the other hand, you wish to prevent the template from showing in VS 2017 you just have to skip this step. VS 2017 will 
only show the templates that are added to the `.vstman` manifest.

## Creating .NET Core templates
Because we wish to support both VS 2015 and 2017 and because they use different build systems, each .NET Core template will 
have to be done twice. One template for VS 2015 using `project.json`, ensuring that is hidden in VS 2017. The other template 
for VS 2017 using the `.csproj` build system and hidden from VS 2015.

The code for the templates should be identical otherwise, the `.csproj` and `project.json` approaches are equivalent.

## To add package references to an existing template
The procedure to add a new package reference to an existing template differs between an ASP.NET Core 1.0 app and an ASP.NET 
4.x app.

### Adding a package reference to an ASP.NET 4.x app
Adding a package reference to an ASP.NET 4.x project is a very involved process because you need to reproduce what Visual 
Studio normally does when adding the package. 

The best way to know what changes are needed is to create a new project from the template and, using Visual Studio, add the 
package. Doing a diff with the unchanged project will show you of the changes you need to port to the template. These changes 
usually involve:
* Adding a reference to the new package, and the packages it depends on, to the `packages.config` file for the project.
* Adding all the necessary `<Reference>` elements to the `.csproj` file. These should include the full identity of the package 
and the `<HintPath>`.
* Ensure that you add all of the new `<Error>` entries in the `<Target Target Name="EnsureNuGetPackageBuildImports" 
BeforeTargets="PrepareForBuild">` entry. These entries add new `msbuild` targets required by the new packages.
  + It is possible that you don't have any new `<Error>` entries if the new package doesn't add new targets, that is fine.

### Adding a package reference to an ASP.NET Core 1.0 app
To add a package reference to an ASP.NET Core 1.0 app you just need to add the reference to the `project.json` in the 
project's template. Make sure that you never include the `project.lock.json` file in the template to ensure that the project 
will be restored when opened by Visual Studio.

Once you are done testing the template and it look as you intend, check in all of the changes, including the produced .zip 
files under the extension's codebase.

## Supporting templates for VS 2017 and VS 2015
VS 2017 has completely changed the way custom project and item templates are enumerated. In VS 2015 the directory where the 
extension is installed was inspected to see what templates were present. Now in VS 2017 a manifest is used to discover what 
templates are present, see [here](https://docs.microsoft.com/en-us/visualstudio/extensibility/upgrading-custom-project-and-
item-templates-for-visual-studio-2017) for more details.

We _exploit_ the new search mechanism to show those templates that are VS 2015 or 2017 specific hidden when installed on the 
incompatible version of VS. The templates that are to be shown only on VS 2015 are **not** added to the `.vstman` manifest, 
while the templates that are to be shown only on VS 2017 are marked as hidden in their template files but added to the 
`.vstman` file. This technique is used for .NET Core projects. In VS 2015 .NET Core projects use the `project.json` build 
system, while in VS 2017 the .NET Core projects use the `.csproj` build system. We cannot have a single .NET Core template 
that would work in both versions of VS.
