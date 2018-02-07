#load "Generator.fs"
open System.IO;
open  Signals.Generator

let output = Path.Combine (__SOURCE_DIRECTORY__, "example")
Directory.CreateDirectory output
writeFiles output (Some "MyNamespace") (Some "UnityEngine") "AudioClip"
