/// <binding BeforeBuild='default' />

const fs = require("fs");
const gulp = require("gulp");
const js_yaml = require("js-yaml");
const plist = require("plist");
const execSync = require("child_process").execSync;

gulp.task("default", done => {
    fs.writeFileSync(
        "T4Language/Snippets/assembly.tmSnippet",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Snippets/assembly.tmSnippet.yml"))));
    fs.writeFileSync(
        "T4Language/Snippets/import.tmSnippet",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Snippets/import.tmSnippet.yml"))));
    fs.writeFileSync(
        "T4Language/Snippets/include.tmSnippet",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Snippets/include.tmSnippet.yml"))));
    fs.writeFileSync(
        "T4Language/Snippets/parameter.tmSnippet",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Snippets/parameter.tmSnippet.yml"))));
    fs.writeFileSync(
        "T4Language/Syntaxes/csharp.tmLanguage",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Syntaxes/csharp.tmLanguage.yml"))));
    fs.writeFileSync(
        "T4Language/Syntaxes/t4.tmLanguage",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Syntaxes/t4.tmLanguage.yml"))));
    fs.writeFileSync(
        "T4Language/Syntaxes/t4.tmTheme",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Syntaxes/t4.tmTheme.yml"))));

    const vsInstallDir = execSync(`"${process.env['ProgramFiles(x86)']}/Microsoft Visual Studio/Installer/vswhere.exe" -property installationPath`)
        .toString()
        .trim();
    execSync(`"${vsInstallDir}/VSSDK/VisualStudioIntegration/Tools/Bin/VsixColorCompiler.exe" T4Language/Themes.xml`);

    done();
});
