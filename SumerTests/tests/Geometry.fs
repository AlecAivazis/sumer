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

        // look for the tests that don't pass
        let failures = shapes |> List.filter (fun test ->
            // compute the convex hull
            let hull = ConvexHull(test.points)

            // include this test case in the list if the hull is not the same as points
            test.points != hull
        )

        // make sure we didn't get any failures
        Assert.That(failures.Length, Is.EqualTo(0))
