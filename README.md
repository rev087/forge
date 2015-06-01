# forge

Procedural Modeling plugin for Unity3D.

## Creating procedural assets

Procedural Assets are created by subclassing `Forge.ProceduralAsset` and overriding the `Build()` method which should return a `Geometry` instance. `Forge.ProceduralAsset` itself subclasses `MonoBehaviour`, meaning that you can add the script to an empty _Game Object_.

Exposing public parameters works perfectly as a way to parameterize your procedural asset via the built-in Unity editor.

The classes are designed to work well with a node-based visual editor, where the connections are instances of `Geometry` being passed from one class instance to the next.

When appropriate, they can receive one ore more geometries as input by implementing `Input()`, have public fields to parameterise their actions and must implement `Output()` to pass on the resulting geometry.

### Primitives

- __Point__
- __Line__
- __Triangle__
- __Square__
- __Cuboid__
- __Circle__
- __Cylinder__
- __Hemisphere__
- __Sphere__

### Filters

- __Bridge__: Receives two sets of geometry A and B, and returns a new geometry with quad faces connecting every two vertices from each set. Basis for extrude operations.
- __Converge__: Adds a vertex at the specified point and creates triangle faces from every two original vertices to that new point. Creates shapes like pyramids and cones.
- __ExtractFaces__: Outputs only the faces (and their vertices) specified by an array of face indices.
- __ExtractVertices__: Outputs only the vertices specified by an array of face indices.
- __Fuse__: Fuses vertices together based on a threshold distance, creating contiguous surfaces and averaging their normals. Results in a "smooth" shaded surface. For now this is a naive, expensive O(n^2) operation.
- __Manipulate__: Manipulate the geometry basic transforms: _Position_, _Rotation_ and _Scale_.
- __Merge:__ Merges one or more geometries into one.
- __Mirror__: Outputs the mirrored image of a geometry along a specified axis.
- __Reverse__: Reverses the order of vertices in each triangle face, inverting the face normal.
- __Triangulate__: Triangulates a 2D polygon defined by the inputted vertices.