# CrystalDocumenter - External Documentation Tool

![Static Badge](https://img.shields.io/badge/Language-C%23-blue?style=flat-square&logo=sharp)

![Static Badge](https://img.shields.io/badge/License-LGPLv3-orange?style=flat-square&logo=gnuemacs)

![CrystalDocs Logo](banner.png)

## Crystal Documenter Project Goals

Crystal Documenter is designed to simplify the documentation process for C# assemblies by generating external intermediary files. These files enable contributors to document types without requiring access to the source code, sidestepping legal concerns related to protected works.

With Crystal Documenter, you can easily create detailed documentation that can be published on GitHub Pages or other static sites and generate XMLDoc files compatible with all major IDEs. This facilitates seamless collaboration and enhances the quality of assembly documentation.

## Usage

Crystal Documenter is still in an early pre-release state and fields or types may change. Please do not use Crystal Documenter in a production environment at this time, consider this a preview-only release.

To learn more about how Crystal Documenter works simply pass the `--help` switch to the built binary and it will output a list of switches.

To build intermediary files, pages and an XMLDoc file from an assembly pass it in as an argument like so: `-i [file_location].dll`. This will create a folder in the default location `Documentation` and spit out a bunch of json in the `intermediary` directory, html files in `pages` an empty XMLDoc in [file_name].xml and two index files for browsing. There won't be much to look at until you edit the json files and run the program again with the same arguments. The second (and all subsequent) runs will cause Crystal Documenter to read in the json files first and then build the pages and other files from those, it will also output any new types it finds in the assembly that aren't documented with intermediary files yet.

To control where the output files are made pass in `-o [folder_name]` and Crystal Documenter will use that folder instead of `Documentation`
