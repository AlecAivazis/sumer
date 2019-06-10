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

    [<Test>]
    member this.OrientedBoundingBox2D() =
        let table: List<RectangleTestCase> = [
            {
                name = "convex shape"
                points = [
                    Vector2(0.f, 0.f)
                    Vector2(1.f, 0.f)
                    Vector2(1.f, 1.f)
                    Vector2(0.f, 1.f)
                ]
                expected = {
                    supports = {
                        top = Vector2.up
                        left = Vector2.zero
                        right = Vector2(1.f, 1.f)
                        bottom = Vector2.right
                    }
                    basisVectors = (Vector2.right, Vector2.up)
                    area = 1.0f
                }
            }
            {
                name = "concave shape"
                points = [
                    Vector2(0.f, 0.f)
                    Vector2(3.f, 0.f)
                    Vector2(3.f, 2.f)
                    Vector2(2.f, 2.f)
                    Vector2(2.f, 1.f)
                    Vector2(1.f, 1.f)
                    Vector2(1.f, 2.f)
                    Vector2(0.f, 2.f)
                ]
                expected = {
                    supports = {
                        top = Vector2(0.f, 2.f)
                        left = Vector2.zero
                        right = Vector2(3.f, 2.f)
                        bottom = Vector2(3.f, 0.f)
                    }
                    area = 6.0f
                    basisVectors = (Vector2.right, Vector2.up)
                }
            }
            {
                name = "rotated shape"
                points = [
                    Vector2(1.f, 0.f)
                    Vector2(2.f, 1.f)
                    Vector2(1.f, 2.f)
                    Vector2(0.f, 1.f)
                ]
                expected = {
                    supports = {
                        bottom = Vector2.right
                        right = Vector2(2.f, 1.f)
                        top = Vector2(1.f, 2.f)
                        left = Vector2.up
                    }
                    basisVectors = ( Rotate2D (Mathf.PI/4.f) Vector2.right, Rotate2D (3.f*Mathf.PI/4.f) Vector2.right)
                    area = 2.f
                }
            }
        ]

        for test in table do
            // compute the oriented bounding box
            let box = test.points |> OrientedBoundingBox2D

            // make sure we set the right basis vectors
            Assert.That(box.basisVectors, Is.EqualTo(test.expected.basisVectors), test.name)
            // make sure that we calculated a reasonably close area
            Assert.That(Mathf.Round(box.area), Is.EqualTo(Mathf.Round(test.expected.area)), test.name)

            // make sure we grabbed the right supports
            let assertEqual a b =
                Assert.That(Mathf.Round(a), Is.EqualTo(Mathf.Round(b)), test.name)

            // make sure that each coordinate is roughly the same as its equivalent
            assertEqual box.supports.top.y test.expected.supports.top.y
            assertEqual box.supports.left.x test.expected.supports.left.x
            assertEqual box.supports.left.y test.expected.supports.left.y
            assertEqual box.supports.right.x test.expected.supports.right.x
            assertEqual box.supports.right.y test.expected.supports.right.y
            assertEqual box.supports.bottom.x test.expected.supports.bottom.x
            assertEqual box.supports.bottom.y test.expected.supports.bottom.y

