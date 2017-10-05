# Forge

Procedural Modeling plugin for Unity3D.

## Creating procedural assets

Procedural Assets are created by subclassing `Forge.ProceduralAsset` and overriding the `Build()` method (which should then `return` a `Geometry` instance). `Forge.ProceduralAsset` subclasses `MonoBehaviour`, thus meaning you can add the script to an empty _Game Object_.

Exposing public parameters works perfectly as a way to parameterize your procedural asset via the built-in Unity editor.

The classes are designed to interface well with a node-based visual editor (where the connections are instances of `Geometry` being passed from one class instance to the next).

When appropriate, they can receive one or more geometries as input by: 
  1. implementing `Input()`
  2. having public fields to parameterise their actions 
  3. and then implementing `Output()` to pass on the resulting geometry.

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

- __Bridge__: Connects adjacent polygons in the input geometry with triangle faces (Ex. two or more circles positioned vertically creates a cylinder).
- __Converge__: Adds a vertex at the specified point and creates triangle faces from every two original vertices to that new point (Creates shapes like pyramids and cones).
- __ExtractFaces__: Outputs only the faces, and their vertices, specified by an array of face indices.
- __ExtractVertices__: Outputs only the vertices specified by an array of face indices.
- __Fuse__: Fuses vertices together based on a threshold distance, creating contiguous surfaces and averaging their normals. Results in a "smooth" shaded surface. For now, this is a naive, expensive O(n^2) operation.
- __Manipulate__: Manipulate the geometry basic transforms: _Position_, _Rotation_ and _Scale_.
- __Merge:__ Merges one or more geometries into one.
- __Mirror__: Outputs the mirrored image of a geometry along a specified axis.
- __Polygonize__: Outputs a polygon with line segments connecting all the vertices of the input geometry in their index order.
- __Reverse__: Reverses the order of vertices in each triangle face, inverting the face normal.
- __Triangulate__: Triangulates a 2D polygon.
