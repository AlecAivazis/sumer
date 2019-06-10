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

type RectangleTestCase = {
    name: string
    points: List<Vector2>
    expected: Rectangle
}


[<TestFixture>]
type GeometryTests () =

    [<Test>]
    member this.ConvexHull() =
        // a list of convex shapes
        let table: List<TestCase> = [
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
                name = "something a bit more complicated"
                points = [
                    Vector2(0.f, 2.f)
                    Vector2(1.f, 2.f)
                    Vector2(4.f, 2.f)
                    Vector2(0.f, 0.f)
                    Vector2(3.f, 1.f)
                    Vector2(1.f, 1.f)
                    Vector2(3.f, 2.f)
                    Vector2(4.f, 0.f)
                ]
                expected = [
                    Vector2(0.f, 0.f)
                    Vector2(4.f, 0.f)
                    Vector2(4.f, 2.f)
                    Vector2(3.f, 2.f)
                    Vector2(1.f, 2.f)
                    Vector2(0.f, 2.f)
                ]
            }
        ]

        for test in table do
            // compute the convex hull
            match ConvexHull2D(test.points) with
            | ConvexHull points ->
                Assert.That(points, Is.EquivalentTo(test.expected), test.name)

    // [<Test>]
    // member this.OrientedBoundingBox() =
    //     let rot45 = Rotate2D 45.0f

    //     let table: List<RectangleTestCase> = [
    //         {
    //             name = "convex shape"
    //             points = [
    //                 Vector2(0.f, 0.f)
    //                 Vector2(1.f, 0.f)
    //                 Vector2(1.f, 1.f)
    //                 Vector2(0.f, 1.f)
    //             ]
    //             expected = {
    //                 supports = {
    //                     top = Vector2.up
    //                     left = Vector2.zero
    //                     right = Vector2(1.f, 1.f)
    //                     bottom = Vector2.right
    //                 }
    //                 basisVectors = (Vector2.right, Vector2.up)
    //                 area = 1.0f
    //             }
    //         }
    //         // {
    //         //     name = "rotated shape"
    //         //     points = [
    //         //         rot45 (Vector2(0.f, 0.f))
    //         //         rot45 (Vector2(1.f, 0.f))
    //         //         rot45 (Vector2(1.f, 1.f))
    //         //         rot45 (Vector2(0.f, 1.f))
    //         //     ]
    //         //     expected = {
    //         //         supports = {
    //         //             bottom = Vector2(1.f, 0.f)
    //         //             right = Vector2(2.f, 1.f)
    //         //             top = Vector2(1.f, 2.f)
    //         //             left = Vector2(0.f, 1.f)
    //         //         }
    //         //         basisVectors = (Vector2.right, Vector2.up)
    //         //         area = 1.0f
    //         //     }
    //         // }
    //         {
    //             name = "concave shape"
    //             points = [
    //                 Vector2(0.f, 0.f)
    //                 Vector2(4.f, 0.f)
    //                 Vector2(4.f, 2.f)
    //                 Vector2(3.f, 2.f)
    //                 Vector2(3.f, 1.f)
    //                 Vector2(1.f, 1.f)
    //                 Vector2(1.f, 2.f)
    //                 Vector2(0.f, 2.f)
    //             ]
    //             expected = {
    //                 supports = {
    //                     top = Vector2(0.f, 2.f)
    //                     left = Vector2.zero
    //                     right = Vector2(4.f, 2.f)
    //                     bottom = Vector2(4.f, 0.f)
    //                 }
    //                 area = 1.0f
    //                 basisVectors = (Vector2.right, Vector2.up)
    //             }
    //         }
    //     ]

    //     for test in table do
    //         // compute the convex hull
    //         Assert.That(OrientedBoundingBox(test.points), Is.EqualTo(test.expected), test.name)

