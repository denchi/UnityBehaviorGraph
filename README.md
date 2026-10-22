# Behaviour Graph

Behaviour Graph runs task graphs stored as ScriptableObjects. Execution links control flow, typed ports carry values, and a `Flowgraph` component starts the graph.

Package: `com.deathbygravitystudio.behaviour.graph` 0.1.0 (`Behaviour Graph`).

## Features

- **Graph assets.** `Graph` is an `ILayer` (from Behaviour Vars) with a root `Node`. Create one from **Assets > Create > Behaviours/Graph**, or from **Behaviour > Graph > Create Graph Asset...**.
- **Nodes and tasks.** Each `Node` holds one `Task`. Nodes, tasks, inputs, and outputs are sub-assets of the graph. A `Composite` task stores child nodes. Creating one adds a child node titled Start with a `Start` task. If that composite's node already has a parent, it also adds a child titled Exit with an `End` task. The root composite has no parent, so a new graph asset only gets the Start child.
- **Ports from attributes.** `[Task]`, `[Input]`, and `[Output]` declare the title, `ValueType`, and order of a task's ports. The editor builds each task's `Input` and `Output` objects from `[Input]` and `[Output]`. `[Func]` and `[Event]` are declared the same way and are not applied to the built-in tasks.
- **Editor.** **Window > Graph Window** lays out nodes, draws links, and assigns a task type from a context menu. **Behaviour > Graph > New Task Script...** writes a new `Task` subclass. The editor assembly is `Behaviour.Graph.Editor` (Editor only).
- **One-shot runtime start.** `Flowgraph` copies the graph's layer values into `RuntimeValueData` and calls `RuntimeGraphData.start()`, which follows the root `Start` task's `Sync` output. `RuntimeGraphData.update` is empty, and `Flowgraph` does not tick the graph, so a run is the synchronous `sync` chain from that start call.
- **Two kinds of link.** A port with `ValueType.None` is an execution link: `RuntimeTaskData.sync` invokes a non-public instance method on the downstream task. Any other `ValueType` is a value pull: `getValue` invokes a non-public instance method on the upstream task and expects a `RuntimeValueData`. The method name must match the port title exactly.
- **Built-in tasks** for constants, math, graph variables, logging, and spawning a prefab. The table below is what each one actually runs.

## Unity version

This repository does not declare a Unity version. `package.json` has no `unity` or `unityRelease` field, and there is no `ProjectSettings/ProjectVersion.txt`.

The layout is a Package Manager package with assembly definitions. `Behaviour.Graph.Editor` sets `includePlatforms` to `Editor`.

## Installation

The runtime assembly references `Behaviour.Vars`. The editor assembly also references `Behaviour.Vars.Editor`. Both come from [UnityBehaviorVars](https://github.com/denchi/UnityBehaviorVars). `package.json` depends on `com.deathbygravitystudio.behaviour.vars` at version `0.1.0`. That version is not a Git URL, so add both packages in the project manifest.

In `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.deathbygravitystudio.behaviour.vars": "https://github.com/denchi/UnityBehaviorVars.git",
    "com.deathbygravitystudio.behaviour.graph": "https://github.com/denchi/UnityBehaviorGraph.git"
  }
}
```

Or, in **Window > Package Manager > Add package from git URL**, add the Vars URL first, then:

```
https://github.com/denchi/UnityBehaviorGraph.git
```

No other sibling package is required to compile this one.

## Quick start

1. Create a graph with **Behaviour > Graph > Create Graph Asset...**. The asset's root task is a `Composite` whose only child is a node titled Start.
2. Add a `Flowgraph` to a GameObject and assign that graph. Open **Window > Graph Window** and select the GameObject. The window loads `Flowgraph.graph`. Selecting a graph asset in the Project window does not: that path calls `setGraph` with the window's current graph. With no graph loaded, use the Graph field in the window, or **Create New Layer**.
3. Right-click the canvas and choose **New**, then right-click the node and choose **Set script**. Connect an output to an input by dragging from the output anchor to the input anchor. Execution links use `ValueType.None` (drawn thicker). The window does not check that linked `ValueType`s match.
4. `Flowgraph` does not check play mode. When the component enables — adding it to an active GameObject, or entering Play Mode with that object active — `OnEnable` runs:

```csharp
runtimeGraph = Behaviours.Graph.Runtime.Utils.createWithGraph(graph, gameObject);
runtimeGraph.start();
```

`start` calls `sync("Sync")` on the runtime data of the `Start` task inside the root composite. That invokes the private method on whatever task is linked to `Start`'s `Sync` output. The method name is the **input** title.

A task that logs a pulled value and then continues, matching `LogValue` in this package:

```csharp
using UnityEngine;
using Behaviours.Graph.Runtime;
using Behaviours.Runtime;

namespace Behaviours.Graph
{
    [Task(Title = "Log Value")]
    [Input(Title = "Sync", Order = 0)]
    [Input(Title = "Value", Type = ValueType.Other, Order = 1)]
    [Output(Title = "Sync", Order = 0)]
    public class LogValue : Task
    {
        void Sync(RuntimeTaskData runtimeData)
        {
            RuntimeValueData value = runtimeData.getValue("Value");

            if (value != null)
                Debug.LogFormat("Value : {0} = {1}", value.type, value.ToObject());
            else
                Debug.Log("Value is NULL");

            runtimeData.sync("Sync");
        }
    }
}
```

`getValue("Value")` looks up a method named `Value` on the task connected to that input. `sync("Sync")` looks up a method named `Sync` on the task connected to that output. Both methods take one `RuntimeTaskData` argument. Value methods return `RuntimeValueData`.

`LogValue` is already in the package (`Behaviours.Graph.LogValue`). **Behaviour > Graph > New Task Script...** scaffolds the same shape and writes the file where you choose. **Set script** only lists `Task` subclasses from the assembly that contains `Task` (`Behaviour.Graph`), so a script saved outside this package does not appear there.

Constants such as `Behaviours.Graph.Const.Primitive.Float` implement `Value` and can feed `LogValue`'s `Value` input. `Var` reads a named entry from `RuntimeGraphData.values`, which `createWithGraph` fills from the graph's `ILayer.values` (`Bool`, `Integer`, `Float`, `String`, and `Other`).

## Built-in tasks

Execution methods below are the private methods the runtime can call because the name matches a port title.

| Task | Ports | What the methods do |
| --- | --- | --- |
| `Start` | output `Sync` | Entry marker. `RuntimeGraphData.start` fires this output. |
| `End` | inputs `Sync`, `result` (`Other`) | Ports only. |
| `If` | inputs `Sync`, `Expression` (`Bool`); outputs `OnTrue`, `OnFalse` | Ports only. No branch method. |
| `Delay` | input `Sync`, output `Sync`, field `time` | Ports and a float field. Nothing reads `time` or waits. |
| `Composite` | input `Sync`; outputs `Sync`, `Result` (`Other`) | Holds `nodes`. `RuntimeGroupData` builds child runtime tasks from those nodes and treats a `Start` child as the default task. |
| `ForEach` | inputs `Sync`, `Array`; outputs `Tick`, `OnFinished`, `Value` | `Sync` reads `Array`, keeps the first element, and fires `Tick`. `Value` returns that element. `OnFinished` is never fired. |
| `LogValue` | inputs `Sync`, `Value`; output `Sync` | Logs `Value.ToObject()` and fires `Sync`. |
| `Add`, `Sub`, `Mul` | inputs `A`, `B` (`Float`); output `Result` (`Float`) | `Result` is A + B, A − B, or A × B. A missing input counts as `0` for add/sub and `1` for mul. |
| `Pow` | inputs `A`, `Power` (`Float`); output `Result` (`Float`) | `Result` is `Mathf.Pow`. A missing input counts as `1`. |
| `Div` | inputs `A`, `B` (`Float`); outputs `Sync`, `Result` (`Float`) | `Result` returns A + B (missing inputs count as `1`). There is no divide. |
| `Random` | inputs `a`, `b`; output `result` | The method is named `Result`, so a pull on `result` does not call it. |
| `Bool`, `Int`, `Float`, `String`, `Vector2` | output `Value` | `Value` returns the serialized constant as `RuntimeValueData`. |
| `BoolArray`, `IntArray`, `FloatArray`, `StringArray` | output `Value` (`Array`) | `Value` returns a list of `RuntimeValueData`. |
| `Var` | output `value`; field `valueId` | `value` returns `findRuntimeValue(valueId)` on the running graph. |
| `VarFloat` | inputs `Set`, `Value` (`Float`); outputs `Sync`, `Value` (`Float`); field `valueId` | `Set` copies the input float onto that runtime value and fires `Sync`. `Value` returns it. |
| `SetVar` | inputs `Sync`, `value`; output `Sync`; field `valueId` | The handler is named `Set`, so a `Sync` link does not call it. |
| `Spawn` (`Behaviours.Graph.Unity`) | input `Sync`; outputs `Sync`, `OnFailed`, `gameObject`; field `prefab` | `Sync` instantiates `prefab` and fires `Sync`. The method that returns the instance is named `Value`, so a pull on `gameObject` does not call it. `OnFailed` is never fired. |

`ValueType` lives in Behaviour Vars (`Behaviours.ValueType`): `Bool`, `Integer`, `Float`, `String`, `Other`, `Array`, `Vector2`, `Vector3`, `MinMax2D`, `MinMax3D`, `Prefab`, and `None`.

## Sibling packages

These repositories are separate behaviour packages from Death By Gravity Studio. They share Behaviour Vars. This package's assemblies reference only `Behaviour.Vars` and `Behaviour.Vars.Editor`.

| Repository | Package | Role relative to this one |
| --- | --- | --- |
| [UnityBehaviorVars](https://github.com/denchi/UnityBehaviorVars) | `com.deathbygravitystudio.behaviour.vars` | Required. `Graph` extends `Behaviours.ILayer`. Runtime values are `Behaviours.Runtime.RuntimeValueData`. The graph window subclasses `GenericWindow` and uses `EditorBehaviourGUI` and `ValueGUI` from the Vars editor assembly. |
| [UnityBehaviorHFSM](https://github.com/denchi/UnityBehaviorHFSM) | `com.deathbygravitystudio.behaviour.hfsm` | Sibling hierarchical state-machine package. It also depends on Vars. This package does not reference it. The new-task script template includes `using Behaviours.HFSM.Runtime;`, and that line is unused by the graph assemblies. |
| [UnityBehaviorTree](https://github.com/denchi/UnityBehaviorTree) | `com.deathbygravitystudio.behaviour.tree` | Sibling behaviour-tree package. It also depends only on Vars. This package does not reference it. |
| [UnityBehaviorDialogs](https://github.com/denchi/UnityBehaviorDialogs) | `com.deathbygravitystudio.behaviour.dialog` | Sibling dialog package. It depends on Vars, HFSM, and `com.unity.timeline`. It does not depend on this graph package. |

## Licence

[MIT](LICENSE). Copyright (c) 2026 Denis Chiriac / Death By Gravity Studio.
