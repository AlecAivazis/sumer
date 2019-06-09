namespace Sumer.Tests

open System
open NUnit.Framework
open UnityEngine
open Sumer.Geometry

type TestCase = {
    name: string
    points: List<Vector2>
    expected: List<Vector2>
}

[<TestFixture>]
type GeometryTests () =

    [<Test>]
    member this.ConvexHull() =
        // a list of convex shapes
        let table = [
            {
                name = "convex shape"
                points = [
                    Vector2(0.f, 0.f)
                    Vector2(0.f, 1.f)
                    Vector2(1.f, 1.f)
                    Vector2(1.f, 0.f)
                ]
                // a convex shape, is its convex hull
                expected = [
                    Vector2(0.f, 0.f)
                    Vector2(1.f, 0.f)
                    Vector2(1.f, 1.f)
                    Vector2(0.f, 1.f)
                ]
            }
            {
                name="concave shape"
                points = [
                    Vector2(0.f, 0.f)
                    Vector2(1.f, 0.f)
                    Vector2(0.5f, 0.5f)
                    Vector2(1.f, 1.f)
                    Vector2(0.f, 1.f)
                ]
                expected = [
                    Vector2(0.f, 0.f)
                    Vector2(1.f, 0.f)
                    Vector2(1.f, 1.f)
                    Vector2(0.f, 1.f)
                ]
            }
            {
                name = "doesn't include origin twice"
                points = [
                    Vector2(0.f, 0.f)
                    Vector2(1.f, 1.f)
                    Vector2(1.f, 0.f)
                ]
                expected = [
                    Vector2(0.f, 0.f)
                    Vector2(1.f, 0.f)
                    Vector2(1.f, 1.f)
                ]
            }
        ]

        for test in table do
            // compute the convex hull
            Assert.That(ConvexHull2D(test.points), Is.EquivalentTo(test.expected), test.name)

    [<Test>]
    member this.OrientedBoundingBox() =
        let table = [
            {
                name = "convex shape"
                points = [
                    Vector2(0.f, 0.f)
                    Vector2(1.f, 0.f)
                    Vector2(1.f, 1.f)
                    Vector2(0.f, 1.f)
                ]
                expected = [
                    Vector2(0.f, 0.f)
                    Vector2(1.f, 0.f)
                    Vector2(1.f, 1.f)
                    Vector2(0.f, 1.f)
                ]
            }
            {
                name = "rotated shape"
                points = [
                    Vector2(0.f, 1.f)
                    Vector2(1.f, 2.f)
                    Vector2(3.f, 1.f)
                    Vector2(1.f, 0.f)
                ]
                expected = [
                    Vector2(0.f, 1.f)
                    Vector2(1.f, 2.f)
                    Vector2(3.f, 1.f)
                    Vector2(1.f, 0.f)
                ]
            }
            {
                name = "concave shape"
                points = [
                    Vector2(0.f, 0.f)
                    Vector2(0.f, 2.f)
                    Vector2(1.f, 2.f)
                    Vector2(1.f, 1.f)
                    Vector2(3.f, 1.f)
                    Vector2(3.f, 2.f)
                    Vector2(4.f, 2.f)
                    Vector2(4.f, 0.f)
                ]
                expected = [
                    Vector2(0.f, 0.f)
                    Vector2(0.f, 2.f)
                    Vector2(4.f, 2.f)
                    Vector2(4.f, 0.f)
                ]
            }
        ]

        for test in table do
            // compute the convex hull
            Assert.That(OrientedBoundingBox2D(test.points), Is.EquivalentTo(test.expected), test.name)

