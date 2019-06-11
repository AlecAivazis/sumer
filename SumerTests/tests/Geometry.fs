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

type RectangleCorners = {
    name: string
    rectangle: Rectangle
    expected: Vector2 * Vector2 * Vector2 * Vector2
}


[<TestFixture>]
type GeometryTests () =
    [<Test>]
    member this.RectangleProperties() =
        // a rectangle to test
        let rectangle  = {
            Width = 1.f
            Height = 1.f
            Supports = {
                top = Vector2.up
                left = Vector2.zero
                right = Vector2(1.f, 1.f)
                bottom = Vector2.right
            }
            BasisVectors = (Vector2.right, Vector2.up)
        }

        // we should be able to compute the area
        Assert.That(rectangle.Area, Is.EqualTo(1))

        // as well as the center
        Assert.That(rectangle.Center.x, Is.EqualTo(0.5f))
        Assert.That(rectangle.Center.y, Is.EqualTo(0.5f))


    [<Test>]
    member this.RectangleCorners() =
        // the list of rectangles to test
        let table = [
            {
                name = "axis aligned square"
                rectangle  = {
                    Width = 1.f
                    Height = 1.f
                    Supports = {
                        top = Vector2.up
                        left = Vector2.zero
                        right = Vector2(1.f, 1.f)
                        bottom = Vector2.right
                    }
                    BasisVectors = (Vector2.right, Vector2.up)
                }
                expected = (
                            Vector2.up,
                            Vector2(1.f, 1.f),
                            Vector2.zero,
                            Vector2.right
                        )
            }
        ]

        for test in table do
                Assert.That(test.rectangle.Corners, Is.EqualTo(test.expected), test.name)


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
                    Supports = {
                        top = Vector2.up
                        left = Vector2.zero
                        right = Vector2(1.f, 1.f)
                        bottom = Vector2.right
                    }
                    BasisVectors = (Vector2.right, Vector2.up)
                    Width = 1.f
                    Height = 1.f
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
                    Supports = {
                        top = Vector2(0.f, 2.f)
                        left = Vector2.zero
                        right = Vector2(3.f, 2.f)
                        bottom = Vector2(3.f, 0.f)
                    }
                    Width = 3.f
                    Height = 2.f
                    BasisVectors = (Vector2.right, Vector2.up)
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
                    Supports = {
                        bottom = Vector2.right
                        right = Vector2(2.f, 1.f)
                        top = Vector2(1.f, 2.f)
                        left = Vector2.up
                    }
                    BasisVectors = ( Rotate2D (Mathf.PI/4.f) Vector2.right, Rotate2D (3.f*Mathf.PI/4.f) Vector2.right)
                    Width = 1.f/Mathf.Sqrt(2.f)
                    Height = 1.f/Mathf.Sqrt(2.f)
                }
            }
        ]

        for test in table do
            // compute the oriented bounding box
            let box = test.points |> OrientedBoundingBox2D

            // make sure we set the right basis vectors
            Assert.That(box.BasisVectors, Is.EqualTo(test.expected.BasisVectors), test.name)
            // make sure that we calculated a reasonably close area
            Assert.That(Mathf.Round(box.Height), Is.EqualTo(Mathf.Round(test.expected.Height)), test.name)
            Assert.That(Mathf.Round(box.Width), Is.EqualTo(Mathf.Round(test.expected.Width)), test.name)

            // make sure we grabbed the right supports
            let assertEqual a b =
                Assert.That(Mathf.Round(a), Is.EqualTo(Mathf.Round(b)), test.name)

            // make sure that each coordinate is roughly the same as its equivalent
            assertEqual box.Supports.top.y test.expected.Supports.top.y
            assertEqual box.Supports.left.x test.expected.Supports.left.x
            assertEqual box.Supports.left.y test.expected.Supports.left.y
            assertEqual box.Supports.right.x test.expected.Supports.right.x
            assertEqual box.Supports.right.y test.expected.Supports.right.y
            assertEqual box.Supports.bottom.x test.expected.Supports.bottom.x
            assertEqual box.Supports.bottom.y test.expected.Supports.bottom.y

