#load "Generator.fs"
open System.IO
open  Signals.Generator



let folder = Path.Combine (__SOURCE_DIRECTORY__, "example")
Directory.CreateDirectory folder

writeFiles (Some "UnityEngine") "AudioClip" (Some "MyNamespace") folder
