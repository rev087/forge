# forge
Procedural Generation toolkit for Unity3D

## Creating procedural assets

Procedural Assets are created by subclassing `Forge.ProceduralAsset` and overriding the `Build()` method which should return a `Geometry` instance. `Forge.ProceduralAsset` itself subclasses `MonoBehaviour`, meaning that you can add the script to an empty _Game Object_.

Exposing public parameters works perfectly as a way to parameterize your procedural asset via the built-in Unity editor.

The classes are designed to work well with a node-based visual editor, where the connections are instances of `Geometry` being passed from one class instance to the next.

When appropriate, they can receive one ore more geometries as input by implementing `Input()`, have public fields to parameterise their actions and must implement `Output()` to pass on the resulting geometry.

### Primitives

- __Square__
- __Cuboid__

### Filters

- __Manipulate__: Manipulate the geometry basic transforms: _Position_, _Rotation_ and _Scale_.
- __Merge:__ Merges one or more geometries into one.
- __Fuse__: Fuses vertices together based on a threshold distance, creating contiguous surfaces and averaging their normals. Results in a "smooth" shaded surface.
- __Reverse__: Reverses the order of vertices in each triangle face, inverting the face normal.
