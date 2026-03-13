using System.Text;
using System.IO;

namespace TextFileMerger
{
    class Program
    {
        private static readonly HashSet<string> IgnoredDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".git", ".svn", ".hg", ".vs", ".vscode", ".idea", "node_modules", "bower_components",
            "bin", "obj", "packages", "dist", "build", "out", "target", "vendor", "__pycache__", 
            ".tox", "venv", "env", ".env", "virtualenv", ".gradle", ".m2", "DerivedData", 
            ".next", ".nuxt", "coverage", ".pytest_cache", "logs", "tmp", "temp", "Debug",
            "Release", ".cache", ".sass-cache", ".eslintcache", ".terragrunt-cache", "__snapshots__",
            ".terraform", "Pods", ".symlinks", ".yarn", ".npm", ".cargo", ".Trash"
        };
        
        private static readonly HashSet<string> IgnoredFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
        };
        
        private static readonly HashSet<string> AllowedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Dockerfile", "Containerfile", "Makefile", "CMakeLists.txt", "Kconfig", "Kbuild",
            "Jenkinsfile", "Vagrantfile", "Procfile", "Gemfile", "Podfile", "Cartfile",
            "Pipfile", "Rakefile", "Brewfile", "Fastfile", "Appfile", "Capfile", "Guardfile",
            "Berksfile", "Cheffile", "Thorfile", "Snakefile", "Justfile", "Tiltfile",
            "Earthfile", "SConstruct", "SConscript", "BUCK", "WORKSPACE", "BUILD", "PKGBUILD",
            "APKBUILD", "dune", "dune-project", "dune-workspace", "Jamfile", "Jamrules",
            "meson.build", ".gitignore", ".gitattributes", ".gitconfig", ".gitmodules",
            ".editorconfig", ".npmrc", ".yarnrc", ".bazelrc", ".buckconfig", ".browserslistrc",
            ".stylelintrc", ".eslintrc", ".prettierrc", ".babelrc", ".terraformrc",
            ".tool-versions", ".env", ".env.local", ".env.production", ".bashrc", ".bash_profile", 
            ".zshrc", ".zprofile", ".zshenv", ".profile", ".inputrc", ".vimrc", ".tmux.conf",
            ".merlin", ".mailmap", ".dockerignore", ".npmignore", ".eslintignore",
            ".prettierignore", ".stylelintignore", ".slugignore", ".helmignore", ".clang-format",
            ".clang-tidy", ".flake8", ".pylintrc", ".coveragerc", ".Rprofile", ".Renviron",
            "LICENSE", "README", "AUTHORS", "CHANGELOG", "NEWS", "CODEOWNERS"
        };

        private static readonly HashSet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".text", ".asc", ".ans", ".nfo", ".diz", ".log", ".out", ".err", ".trace", ".lst", ".md", ".markdown", ".mdown", ".mkd", ".mkdn", ".mdx", ".rst", ".rest", ".adoc", ".asciidoc", ".org", ".norg", ".wiki", ".mediawiki", ".creole", ".textile", ".rdoc", ".pod", ".typ", ".tex", ".ltx", ".sty", ".cls", ".bib", ".bst", ".dtx", ".ins", ".ctx", ".rtf", ".man", ".roff", ".mdoc", ".me", ".ms", ".mom", ".mm", ".scdoc", ".texi", ".txi", ".info", ".mf", ".mp", ".ly", ".ily", ".abc", ".lyx", ".dox", ".doxygen", ".nw", ".fountain", ".saty", ".satyh", ".fb2", ".1", ".2", ".3", ".4", ".5", ".6", ".7", ".8", ".9", ".html", ".htm", ".xhtml", ".shtml", ".mhtml", ".sgml", ".sgm", ".xml", ".xsd", ".xsl", ".xslt", ".rng", ".rnc", ".sch", ".dbk", ".wsdl", ".xul", ".mml", ".xaml", ".axaml", ".xoml", ".xib", ".storyboard", ".resx", ".resw", ".rss", ".atom", ".opml", ".fo", ".csl", ".dita", ".ditamap", ".smil", ".gpx", ".kml", ".osm", ".gml", ".gexf", ".graphml", ".qgs", ".qlr", ".sld", ".mapcss", ".vrt", ".wkt", ".prj", ".drawio", ".bpmn", ".dmn", ".cmmn", ".ifcxml", ".urdf", ".xacro", ".sdf", ".world", ".rviz", ".launch", ".msg", ".srv", ".action", ".rosinstall", ".repos", ".iml", ".ipr", ".iws", ".edmx", ".sitemap", ".tmlanguage", ".tmpreferences", ".tmsnippet", ".tmtheme", ".sublime-project", ".sublime-settings", ".sublime-keymap", ".sublime-menu", ".sublime-theme", ".sublime-build", ".sublime-commands", ".sublime-completions", ".sublime-color-scheme", ".sublime-macro", ".sublime-mousemap", ".sublime-syntax", ".code-workspace", ".code-snippets", ".musicxml", ".opf", ".ncx", ".webloc", ".xspf", ".json", ".jsonc", ".json5", ".jsonl", ".ndjson", ".jsonld", ".geojson", ".topojson", ".har", ".webmanifest", ".avsc", ".pbtxt", ".prototxt", ".textproto", ".textpb", ".hjson", ".ron", ".yaml", ".yml", ".toml", ".kdl", ".cff", ".ini", ".cfg", ".conf", ".cnf", ".config", ".cf", ".properties", ".prop", ".prefs", ".opts", ".desktop", ".url", ".repo", ".list", ".sources", ".ovpn", ".nmconnection", ".reg", ".inf", ".theme", ".rasi", ".dic", ".aff", ".ldif", ".edi", ".x12", ".admx", ".adml", ".spdx", ".lock", ".patch", ".diff", ".rej", ".md5", ".sha1", ".sha256", ".sha512", ".sum", ".sarif", ".pem", ".pub", ".pacnew", ".pacsave", ".clixml", ".csv", ".tsv", ".tab", ".psv", ".ssv", ".sql", ".ddl", ".dml", ".psql", ".cql", ".kql", ".cypher", ".prql", ".rq", ".sparql", ".hql", ".pig", ".dbml", ".prisma", ".graphql", ".gql", ".graphqls", ".proto", ".thrift", ".avdl", ".idl", ".webidl", ".asn", ".asn1", ".fbs", ".capnp", ".yang", ".apib", ".raml", ".wadl", ".rdf", ".owl", ".ttl", ".n3", ".nt", ".nq", ".trig", ".qasm", ".sh", ".bash", ".zsh", ".ksh", ".csh", ".tcsh", ".fish", ".command", ".awk", ".gawk", ".mawk", ".sed", ".bat", ".cmd", ".btm", ".ps1", ".psm1", ".psd1", ".ps1xml", ".psc1", ".pssc", ".cdxml", ".vbs", ".wsf", ".wsc", ".hta", ".au3", ".ahk", ".tcl", ".tk", ".tm", ".exp", ".vim", ".pac", ".applescript", ".service", ".socket", ".timer", ".mount", ".automount", ".target", ".path", ".slice", ".scope", ".swap", ".link", ".network", ".netdev", ".rules", ".policy", ".css", ".scss", ".sass", ".less", ".styl", ".pcss", ".sss", ".pug", ".jade", ".ejs", ".eta", ".hbs", ".handlebars", ".mustache", ".liquid", ".twig", ".jinja", ".jinja2", ".j2", ".njk", ".nunjucks", ".tpl", ".tmpl", ".gotmpl", ".templ", ".ftl", ".ftlh", ".ftlx", ".vm", ".vtl", ".soy", ".st", ".tt", ".tt2", ".haml", ".slim", ".erb", ".rhtml", ".builder", ".jbuilder", ".rabl", ".latte", ".dust", ".swig", ".kit", ".mjml", ".marko", ".riot", ".tag", ".vue", ".svelte", ".svx", ".astro", ".wpy", ".nvue", ".qml", ".qmltypes", ".wxml", ".wxss", ".axml", ".acss", ".swan", ".ttml", ".ttss", ".ux", ".slint", ".epp", ".isml", ".razor", ".cshtml", ".vbhtml", ".heex", ".leex", ".eex", ".sface", ".jsp", ".jspx", ".jspf", ".tagx", ".tld", ".gsp", ".cfm", ".cfc", ".cfml", ".aspx", ".ascx", ".asax", ".master", ".skin", ".browser", ".ashx", ".asmx", ".svc", ".mxml", ".c", ".h", ".i", ".ii", ".cc", ".cp", ".cpp", ".cxx", ".c++", ".hpp", ".hxx", ".hh", ".h++", ".hp", ".ipp", ".inl", ".tcc", ".ixx", ".cppm", ".cu", ".cuh", ".cl", ".opencl", ".hip", ".ispc", ".ptx", ".m", ".swift", ".rs", ".go", ".zig", ".zon", ".d", ".di", ".jai", ".odin", ".v", ".vsh", ".nim", ".nimble", ".nims", ".pas", ".pp", ".dpr", ".lpr", ".dpk", ".dfm", ".lfm", ".inc", ".ada", ".adb", ".ads", ".gpr", ".f", ".for", ".ftn", ".f77", ".f90", ".f95", ".f03", ".f08", ".f18", ".fpp", ".cob", ".cbl", ".cpy", ".mod", ".def", ".ob2", ".obn", ".vala", ".vapi", ".e", ".bas", ".frm", ".ctl", ".vbp", ".vbw", ".pb", ".pbi", ".bi", ".rc", ".manifest", ".smali", ".baksmali", ".al", ".xpp", ".cs", ".csx", ".fs", ".fsi", ".fsx", ".fsscript", ".vb", ".java", ".kt", ".kts", ".scala", ".sc", ".groovy", ".gvy", ".gy", ".gsh", ".gradle", ".clj", ".cljs", ".cljc", ".edn", ".bb", ".aj", ".ceylon", ".xtend", ".boo", ".re", ".rei", ".res", ".resi", ".hs", ".lhs", ".hsc", ".cabal", ".purs", ".ml", ".mli", ".mll", ".mly", ".sml", ".sig", ".fun", ".cm", ".elm", ".erl", ".hrl", ".ex", ".exs", ".lfe", ".lisp", ".lsp", ".el", ".scm", ".ss", ".rkt", ".rktl", ".rktd", ".idr", ".lidr", ".agda", ".lagda", ".lean", ".hlean", ".thy", ".fst", ".fsti", ".mlw", ".tla", ".als", ".pony", ".gleam", ".roc", ".fut", ".kk", ".nix", ".janet", ".wren", ".factor", ".fth", ".4th", ".forth", ".io", ".self", ".apl", ".dyalog", ".ijs", ".bqn", ".k", ".q", ".pro", ".p", ".icn", ".red", ".reds", ".fan", ".stan", ".mo", ".mos", ".mzn", ".dzn", ".ozn", ".pddl", ".smt2", ".pml", ".smv", ".mcrl2", ".circom", ".py", ".pyw", ".pyi", ".pyx", ".pxd", ".pxi", ".pyde", ".ipynb", ".sage", ".tac", ".wsgi", ".asgi", ".rb", ".rbw", ".rake", ".gemspec", ".ru", ".thor", ".php", ".php3", ".php4", ".php5", ".php7", ".php8", ".phtml", ".phps", ".hack", ".lua", ".luau", ".tl", ".rockspec", ".moon", ".pl", ".pm", ".t", ".psgi", ".perl", ".raku", ".rakumod", ".rakudoc", ".p6", ".pm6", ".pod6", ".nqp", ".cr", ".ecr", ".dart", ".jl", ".r", ".rmd", ".rnw", ".rd", ".qmd", ".sas", ".do", ".ado", ".sps", ".wl", ".wls", ".nb", ".mpl", ".gs", ".gsx", ".abap", ".bal", ".hx", ".hxml", ".hxsl", ".as", ".brs", ".bs", ".nut", ".gd", ".gdshader", ".tscn", ".tres", ".gdextension", ".uc", ".sol", ".vy", ".move", ".cairo", ".scilla", ".cdc", ".clar", ".leo", ".yul", ".ligo", ".mligo", ".jsligo", ".religo", ".tact", ".mojo", ".ino", ".pde", ".spin", ".spin2", ".fnl", ".hy", ".nu", ".liq", ".ring", ".rexx", ".rx", ".prg", ".ch", ".w", ".rpy", ".ink", ".twee", ".bmx", ".js", ".mjs", ".cjs", ".jsx", ".ts", ".tsx", ".mts", ".cts", ".ets", ".jsm", ".ssjs", ".sjs", ".coffee", ".cjsx", ".litcoffee", ".iced", ".y", ".yy", ".l", ".ll", ".g", ".g4", ".bnf", ".ebnf", ".abnf", ".peg", ".pegjs", ".jj", ".jjt", ".cup", ".ne", ".ohm", ".lark", ".rl", ".td", ".mlir", ".wat", ".wast", ".lalrpop", ".pest", ".asdl", ".fsy", ".fsl", ".jflex", ".mk", ".mak", ".make", ".am", ".ac", ".m4", ".in", ".cmake", ".ninja", ".gn", ".gni", ".bzl", ".bazel", ".sbt", ".qbs", ".pri", ".qrc", ".ui", ".qdocconf", ".qhp", ".qhcp", ".qmlproject", ".proj", ".csproj", ".vbproj", ".fsproj", ".vcproj", ".vcxproj", ".vcxitems", ".filters", ".props", ".targets", ".ruleset", ".runsettings", ".pubxml", ".sqlproj", ".dbproj", ".dsp", ".dsw", ".cbp", ".jucer", ".xcscheme", ".sln", ".slnx", ".nuspec", ".tbd", ".wrap", ".spec", ".dsc", ".changes", ".ebuild", ".eclass", ".apkbuild", ".opam", ".xcconfig", ".pbxproj", ".entitlements", ".appxmanifest", ".pkgdef", ".pkgundef", ".wxs", ".wxi", ".wxl", ".wixproj", ".nsi", ".nsh", ".iss", ".isl", ".jam", ".bbappend", ".bbclass", ".godot", ".uproject", ".uplugin", ".unity", ".prefab", ".anim", ".meta", ".asmdef", ".asmref", ".uxml", ".uss", ".inputactions", ".rbxlx", ".rbxmx", ".hcl", ".tf", ".tfvars", ".tfbackend", ".tfstate", ".nomad", ".sentinel", ".cue", ".rego", ".jsonnet", ".libsonnet", ".dhall", ".pkl", ".kcl", ".ncl", ".star", ".sky", ".vcl", ".bicep", ".bicepparam", ".cwl", ".wdl", ".nf", ".dvc", ".sls", ".cil", ".eml", ".emlx", ".mbox", ".ics", ".ifb", ".vcs", ".vcf", ".po", ".pot", ".xliff", ".xlf", ".arb", ".strings", ".stringsdict", ".xcstrings", ".srt", ".vtt", ".ssa", ".ass", ".smi", ".sbv", ".lrc", ".m3u", ".m3u8", ".pls", ".asx", ".wax", ".wvx", ".edl", ".ffconcat", ".dot", ".gv", ".puml", ".plantuml", ".iuml", ".pu", ".mmd", ".mermaid", ".d2", ".nomnoml", ".msc", ".blockdiag", ".seqdiag", ".actdiag", ".nwdiag", ".rackdiag", ".packetdiag", ".feature", ".robot", ".http", ".bru", ".hurl", ".jmx", ".bats", ".tap", ".snap", ".lcov", ".gcov", ".trx", ".asm", ".s", ".a51", ".dts", ".dtsi", ".dtso", ".its", ".ld", ".lds", ".va", ".vams", ".vh", ".sv", ".svh", ".vhd", ".vhdl", ".ucf", ".xdc", ".sdc", ".qsf", ".qpf", ".lef", ".lib", ".spef", ".saif", ".vcd", ".scs", ".cir", ".sp", ".spice", ".ckt", ".net", ".gbr", ".ger", ".drl", ".xln", ".kicad_pro", ".kicad_prl", ".kicad_dru", ".kicad_sym", ".kicad_mod", ".kicad_sch", ".kicad_pcb", ".kicad_wks", ".gcode", ".ngc", ".scad", ".dxf", ".step", ".stp", ".iges", ".igs", ".ifc", ".amf", ".off", ".xyz", ".pts", ".glsl", ".vert", ".frag", ".geom", ".tesc", ".tese", ".comp", ".mesh", ".task", ".rgen", ".rchit", ".rmiss", ".rahit", ".rint", ".rcall", ".wgsl", ".hlsl", ".hlsli", ".fx", ".fxh", ".cg", ".cginc", ".slang", ".metal", ".shader", ".usf", ".ush", ".osl", ".sl", ".pov", ".rib", ".mi", ".mtl", ".gltf", ".wrl", ".vrml", ".x3d", ".dae", ".ma", ".usda", ".mtlx", ".pbrt", ".sfd", ".bdf", ".afm", ".fea", ".glyphs", ".glif", ".designspace", ".pfa", ".ps", ".eps", ".d.ts", ".d.mts", ".d.cts", ".blade.php", ".gradle.kts", ".cmake.in", ".app.src", ".lagda.md", ".lagda.tex", ".tfvars.json", ".pkr.hcl", ".csproj.user", ".vcxproj.filters", ".go.mod", ".go.sum", ".go.work", ".zig.zon", ".module.css", ".module.scss", ".module.sass", ".module.less", ".module.styl", ".stories.tsx", ".stories.jsx", ".stories.mdx", ".spec.ts", ".spec.tsx", ".spec.js", ".test.ts", ".test.tsx", ".test.js", ".config.js", ".config.cjs", ".config.mjs", ".config.ts", ".config.json", ".config.yaml", ".config.yml"
        };
        
        private static readonly StringBuilder Output = new StringBuilder();
            
        static void Main(string[] args)
        {
            string outputFilePath;
            if (args.Length < 1)
            {
                outputFilePath = "output.txt";
            }
            else
            {
                outputFilePath = args[0];
            }

            string outputFilePathAbs = Path.GetFullPath(outputFilePath);
            string currentDir = Directory.GetCurrentDirectory();

            try
            {
                ProcessDirectory(currentDir, currentDir, outputFilePathAbs);
                File.WriteAllText(outputFilePath, Output.ToString());

                Console.WriteLine($"Successfully created: {outputFilePath}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static bool HasAllowedExtension(string fileName)
        {
            int dotIndex = fileName.IndexOf('.');
            
            while (dotIndex>=0)
            {
                string extension = fileName.Substring(dotIndex);
                
                if (AllowedExtensions.Contains(extension))
                {
                    return true;
                }
                
                dotIndex = fileName.IndexOf('.', dotIndex + 1);
            }
            
            return false;
        }

        static void ProcessTextFile(string filePath, string rootDir)
        {
            try
            {
                string relativePath = filePath.Substring(rootDir.Length).TrimStart(Path.DirectorySeparatorChar);

                Output.AppendLine($"// ======== FILE: {relativePath} ========");
                Output.AppendLine(File.ReadAllText(filePath));
                Output.AppendLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ProcessDirectory(string currentDir, string rootDir, string outputFilePathAbs)
        {
            try
            {
                foreach (var filePath in Directory.GetFiles(currentDir))
                {
                    string fileName = Path.GetFileName(filePath);
                    string filePathAbs = Path.GetFullPath(filePath);

                    if (filePathAbs.Equals(outputFilePathAbs))
                    {
                        continue;
                    }

                    if (IgnoredFiles.Contains(fileName))
                    {
                        continue;
                    }

                    if (AllowedFileNames.Contains(fileName))
                    {
                        ProcessTextFile(filePath, rootDir);
                        continue;
                    }

                    if (HasAllowedExtension(fileName))
                    {
                        ProcessTextFile(filePath, rootDir);
                    }
                }

                foreach (var dir in Directory.GetDirectories(currentDir))
                {
                    string dirName = Path.GetFileName(dir);
                    if (IgnoredDirectories.Contains(dirName))
                    {
                        continue;
                    }

                    ProcessDirectory(dir, rootDir, outputFilePathAbs);
                }
            }
            catch (UnauthorizedAccessException) {}
            
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}