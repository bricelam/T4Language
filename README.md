T4 Language for Visual Studio
=============================

Adds basic language support for .tt files.

The latest version is available on the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=bricelam.T4Language).

Screenshot
----------

![Text Template with syntax highlighting](.github/Screenshot.png)

Snippets
--------

The following code snippets are available.

Shortcut  | Code
--------- | -----------
assembly  | `<#@ assembly name="" #>`
import    | `<#@ import namespace="" #>`
include   | `<#@ include file="" #>`
parameter | `<#@ parameter name="MyParameter" type="System.String" #>`

Options
-------

You can customize the colors under **Tools > Options > Environment > Fonts and Colors**.

Display item      | Default
----------------- | -------
T4 Argument       | Red
T4 Argument Value | Blue
T4 Delimiter      | Yellow (background)
T4 Directive      | Maroon
T4 Operator (=)   | Blue

See also
--------

[Code Generation using T4 Text Templates](https://docs.microsoft.com/visualstudio/modeling/design-time-code-generation-by-using-t4-text-templates)
