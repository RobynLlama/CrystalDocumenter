# Contributing

- [Style](#style)
  - [Naming](#naming)
  - [General](#general)
- [Commits / Pull requests](#commits--pull-requests)

## Style

### Naming

The Crystal Documenter Project follows standard C# naming conventions:

- **Pascal Case**: Used for public members, including class names, method names, and properties.
  - Example: `PublicMethod`, `MyProperty`, `CustomerAccount`

- **Camel Case**: Used for local variables and method parameters and private fields within a class.
  - Example: `localVariable`, `inputParameter`, `privateField`

- **Underscore Prefix**: Optionally for private fields within a class.
  - Example: `_privateField`

For more detailed guidelines, you can refer to Microsoft's official documentation on [C# Identifier Names](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)

### General

- Always have trailing line endings on for all files. This is a setting you can enable in every IDE. This will confuse GIT if you don't do it
- **For markdown**:
  - Follow the style guides of a quality linter such as `DavidAnson.vscode-markdownlint`
- **For Source files**:
  - Try to avoid nesting by using early returns under negative checks instead of branches under positive checks. Example:
  - Indents are treated as two spaces, please configure your IDE to use this setting to avoid mangling commits with extra information

Example of leaving early rather than nesting:

  ```cs
  //Do not
  If (thing)
  {
    //do stuff here
  }

  //Do
  {
    If (!thing)
      return;

    //do stuff here
  }
  ```

## Commits / Pull requests

- Be descriptive of what your commits do.
- Follow [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) to help keep track of what you are changing both for yourself and others that wish to read your code
- **Do Not** open a PR for style-only changes. It is inevitable that some style violations will slip thru the cracks. Alert project maintainers via an Issue and allow them to correct style issues.
- **Do Not** commit changes to packages.lock.json, discuss with project maintainers if new packages are needed or updates are required
- **Avoid** committing changes to .gitignore since this will effect everyone's instance if merged. Use `git rm <file>` instead to ask git to stop tracking a file locally
