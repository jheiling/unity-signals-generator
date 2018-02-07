#load "Generator.fs"
open System.IO;
open  Signals.Generator

let directory = Path.Combine (__SOURCE_DIRECTORY__, "example")
Directory.CreateDirectory directory
writeFiles directory (Some "MyNamespace") (Some "UnityEngine") "AudioClip"
