NavMesh allows character to move around to certain areas

Tutorial on how to have Nav Mesh.

You need Obstacle, Floor, and Character


1. Go to Window -> AI -> Navigation. This will open Navigation tab on the right panel
2. Click on the Floor and Obstacle, Go to `Navigation` -> `Object` Tab ->  Check Navigation Static -> Navigation Area (Not Walkable)
3. Add `Nav Mesh Agent` to the character. Character should also have `Rigidbody` and `Capsule Collider`.
4. Add Script to Player.