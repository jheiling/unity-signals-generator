module Signals.Generator

open System
open System.IO



let firstToUpper text =
    if String.IsNullOrWhiteSpace text
    then text
    else Char.ToString (Char.ToUpper text.[0]) + if String.length text > 1 then text.Substring(1) else String.Empty

let appendImport import imports =
    match import with 
    | Some i -> if List.contains i imports then imports else imports @ [i]
    | None -> imports

let lines imports code namesp typeName =
    (if List.isEmpty imports 
     then [] 
     else (List.map (sprintf "using %s;") imports) @ [""; ""; ""]) @
    (match namesp with 
     | Some n -> [sprintf "namespace %s" n; "{"] @ List.map (sprintf "    %s") (code typeName) @ ["}"] 
     | None -> code typeName)

let writeFile className imports code folder namesp typeName = 
    let contents = lines imports code namesp typeName
    File.WriteAllLines (Path.Combine (folder, className typeName + ".cs"), List.toArray contents)



module Events =
    let className typeName = firstToUpper typeName + "Event"

    let imports = ["System"; "UnityEngine.Events"]

    let code typeName = [
        "[Serializable]"
        sprintf "public class %s : UnityEvent<%s> { }" (className typeName) typeName]

    let writeFile folder namesp typeNamesp = writeFile className (appendImport typeNamesp imports) code folder namesp



module Signals =
    let className typeName = firstToUpper typeName + "Signal"

    let imports = ["UnityEngine"; "Signals"]

    let code typeName = [
        sprintf "[CreateAssetMenu(menuName = \"Signals/%s\")]" (className typeName)
        sprintf "public class %s : Signal<%s, %s> { }" (className typeName) typeName (Events.className typeName)]

    let writeFile folder namesp typeNamesp = writeFile className (appendImport typeNamesp imports) code folder namesp



module SignalEditors =
    let className typeName = Signals.className typeName + "Editor"

    let imports = ["UnityEditor"; "Signals"]

    let code typeName = [
        sprintf "[CustomEditor(typeof(%s))]" (Signals.className typeName)
        sprintf "public class %s : SignalEditor<%s, %s>" (className typeName) typeName (Events.className typeName)
        "{"
        sprintf "    protected override %s ValueField(%s value)" typeName typeName
        "    {"
        "        return /*TODO*/(value);"
        "    }"
        "}"]

    let writeFile folder namesp typeNamesp = writeFile className (appendImport typeNamesp imports) code folder namesp



module SignalListeners =
    let className typeName = Signals.className typeName + "Listener"

    let imports = ["UnityEngine"; "Signals"]

    let code typeName = [
        sprintf "[AddComponentMenu(\"Signals/%s\")]" (className typeName)
        sprintf "public class %s : SignalListener<%s, %s, %s> { }" (className typeName) typeName (Events.className typeName) (Signals.className typeName)]

    let writeFile folder namesp typeNamesp = writeFile className (appendImport typeNamesp imports) code folder namesp



module ValueReferences =
    let className typeName = firstToUpper typeName + "ValueReference"

    let imports = ["System"; "Signals"]

    let code typeName = [
        "[Serializable]"
        sprintf "public class %s : ValueReference<%s, %s, %s>" (className typeName) typeName (Events.className typeName) (Signals.className typeName)
        "{"
        sprintf "public %s() { }" (className typeName)
        sprintf "public %s(%s localValue) : base(localValue) { }" (className typeName) typeName
        "}"]

    let writeFile folder namesp typeNamesp = writeFile className (appendImport typeNamesp imports) code folder namesp



module ValueReferenceDrawers =
    let className typeName = ValueReferences.className typeName + "Drawer"

    let imports = ["UnityEditor"; "Signals"]

    let code typeName = [
        sprintf "[CustomPropertyDrawer(typeof(%s))]" (ValueReferences.className typeName)
        sprintf "public class %s : ValueReferenceDrawer { }" (className typeName)]

    let writeFile = writeFile className imports code



let writeFiles folder namesp typeNamesp typeName =
    Events.writeFile folder namesp typeNamesp typeName
    Signals.writeFile folder namesp typeNamesp typeName
    SignalEditors.writeFile folder namesp typeNamesp typeName
    SignalListeners.writeFile folder namesp typeNamesp typeName
    ValueReferences.writeFile folder namesp typeNamesp typeName
    ValueReferenceDrawers.writeFile folder namesp typeName