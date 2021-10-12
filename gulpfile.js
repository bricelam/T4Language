/// <binding BeforeBuild='default' />

const fs = require("fs");
const gulp = require("gulp");
const js_yaml = require("js-yaml");
const plist = require("plist");

gulp.task("default", done => {
    fs.writeFileSync(
        "T4Language/Syntaxes/csharp.tmLanguage",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Syntaxes/csharp.tmLanguage.yml"))));
    fs.writeFileSync(
        "T4Language/Syntaxes/t4.tmLanguage",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Syntaxes/t4.tmLanguage.yml"))));
    fs.writeFileSync(
        "T4Language/Syntaxes/t4.tmTheme",
        plist.build(js_yaml.load(fs.readFileSync("T4Language/Syntaxes/t4.tmTheme.yml"))));
    done();
});
