namespace Sumer.Tests

open System
open NUnit.Framework
open UnityEngine
open Sumer.Geometry

type TestCase = {
    name: string
    points: List<Vector3>
}

[<TestFixture>]
type GeometryTests () =

    [<Test>]
    member this.JarvisMarch_ConvexHulls() =
        // a list of convex shapes
        let shapes = [
            {
                name = "Square";
                points = [
                    Vector3(0.f, 0.f, 0.f);
                    Vector3(0.f, 0.f, 1.f);
                    Vector3(1.f, 0.f, 1.f);
                    Vector3(0.f, 0.f, 1.f)
                ]
            }
        ]

        for test in shapes do
            // compute the convex hull
            let hull = ConvexHull(test.points)

            // make sure that the reslt is the same as what we put in
            Assert.That(hull, Is.EquivalentTo(test.points))

