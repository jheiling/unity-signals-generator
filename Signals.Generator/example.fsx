#load "Generator.fs"
open System.IO;
open  Signals.Generator

writeFiles (Path.Combine (__SOURCE_DIRECTORY__, "test")) (Some "Signals.Common") None "int"