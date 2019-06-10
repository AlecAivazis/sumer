module Sumer.Geometry

// externals
open UnityEngine

/// An enum to indicate turn directionality (cross-product direction)
type TurnDirection = CoLinear | Clockwise | CounterClockwise

/// A wrapper over List<Vector2> to flag list as a convex hull in standard form
type ConvexHull2D = ConvexHull of List<Vector2>

/// A wrapper over Vector2 * Vector2 interacting with hulls
type Edge = Edge of Vector2 * Vector2

// turnDirection returns the direction of the turn created by the three provided points
let private turnDirection (a: Vector2) (b: Vector2) (c: Vector2) =
    match (b.y-a.y)*(c.x-b.x)-(b.x-a.x)*(c.y-b.y) with
    | 0.0f -> CoLinear
    | i when i < 0.0f -> CounterClockwise
    | _ -> Clockwise

/// Compute the convex hull of an unordered list of points.
let ConvexHull2D (points: List<Vector2>) =
    // we're going to compute the convex hull using the Graham Scan method
    // as described here: https://en.wikipedia.org/wiki/Graham_scan#Pseudocode

    // let's compute the lowest point, favoring points to the left in case of a tie
    let p0 = points
            |> List.reduce (fun (lowest: Vector2) (next: Vector2) ->
                match (lowest, next) with
                // if the new item is lower use it
                | (l, n) when n.y < l.y -> n
                // if they are at the same height but the new item is to the left use it
                | (l, n) when n.y = l.y && n.x < l.x -> n
                // otherwise the lowest still considered lower
                | _ -> lowest
            )

    // we now have to sort the incoming list of points by polar angle relative to p0
    // if two points have the same polar angle, we have to only include the furthest one

    let mutable polarCoods =
        points
        // create a map of from polar coordinate to position
        |> List.fold (fun (map: Map<float32, Vector2>) (point: Vector2) ->
                // compute the polar coordinate of this point with p0 and convert into degrees
                let coords = atan2 (point.y - p0.y ) (point.x - p0.x)

                match map.TryFind(coords) with
                // we haven't seen the coordinates before so use it
                | None -> map.Add(coords, point)
                // we've seen the coordinate and this new point is further away
                | Some otherPoint when Vector2.Distance(point, p0) > Vector2.Distance(otherPoint, p0) ->
                    // replace the new point over the old point in the map
                    map.Add(coords, point)
                // anything else we keep life as it is...
                | _ -> map
            ) Map.empty
        // turn the map into a list which will sort it in ascending order by key
        |> Map.toList

    // if the first entry is the origin, remove it
    match polarCoods.Head with
    | (_, p) when p = p0 ->
        polarCoods <- polarCoods.Tail
    | _ -> ()

    // start the list of points with our initial one
    let mutable result = [p0]

    // visit each point
    for (_, point) in polarCoods  do
        // while there are elements left and the last 3 points make a
        // counter-clockwise turn
        while result.Length > 1 && (turnDirection point result.Head result.Tail.Head = CounterClockwise) do
            // pop the stack
            result <- result.Tail

        // add the point to the list
        result <- point::result

    // we're done
    ConvexHull (result |> List.rev)


type RectangleVertices = {
    bottom: Vector2
    left: Vector2
    right: Vector2
    top: Vector2
}

// a rectangle used in tracking and computing bounding boxes
type Rectangle = {
    basisVectors: Vector2 * Vector2
    supports: RectangleVertices
    area: float32
}

let Rotate2D (angle: float32) (vec: Vector2) : Vector2 =
    Vector2(
        (vec.x * Mathf.Cos angle) - (vec.y * Mathf.Sin angle),
        (vec.x * Mathf.Sin angle) + (vec.y * Mathf.Cos angle)
    )

// take a point in the coordinate system defined by u1,u2 at origin and transform it to standard
let private align (u1: Vector2) (u2: Vector2) (origin: Vector2) (point: Vector2) : Vector2 =
    // the location in local space
    let location = (point.x * u1) + (point.y * u2)

    // add the components along each standard axis to the origin
    Vector2 (
        origin.x + Vector2.Dot(location, Vector2.right),
        origin.y + Vector2.Dot(location, Vector2.up)
    )

let private boundingBoxAlongEdge (points: List<Vector2>) (Edge (p1, p2)): Rectangle =
    // compute the basis vectors in the space aligned with our two vectors
    let u1 = p2 - p1
    u1.Normalize()
    let u2 = u1 |> Vector2.Perpendicular

    // calculate the supports for the rectangle coincident with the two points in the local
    // coordinate system
    let supportsLocal = (
        points
        |> List.fold (fun (prev: RectangleVertices) (point: Vector2) ->
            // convert the point to a coordinate system with the origin at p2
            // and basis vectors (u1, u2)
            let diff = point - p2
            let localSpace = Vector2(Vector2.Dot(u1, diff), Vector2.Dot(u2, diff))

            // the vertex record we might change
            let mutable supports = prev

            // since we are garunteed that none of vertices are colinear, p2 must be the
            // lowest, right most point and therefor is already our bottom support

            // new right maximum OR same right but more top
            if localSpace.x > supports.right.x
                || localSpace.x = supports.right.x && localSpace.y > supports.right.y  then
                // update the right support to be this points
                supports <- { supports with right = localSpace }

            // new top maximum OR same top but more left
            if localSpace.y > supports.top.y
                || localSpace.y = supports.top.y && localSpace.x < supports.top.x  then
                // update the top support to be this points
                supports <- { supports with top = localSpace }

            // new left maximum OR same left but more bottom
            if localSpace.x < supports.left.x
                || localSpace.x = supports.left.x && localSpace.y < supports.left.y then
                // update the left support to be this points
                supports <- { supports with left = localSpace }

            // return the new supports taking this point into account
            supports

        ) {
            top = Vector2(0.f, 0.f)
            left = Vector2(0.f, 0.f)
            right = Vector2(0.f, 0.f)
            bottom = Vector2(0.f, 0.f)
        }
    )

    // partially binding the coodinate system
    let localAlign = align u1 u2 p2

    // return the rectangle we just computed
    {
        supports = {
            top = localAlign supportsLocal.top
            bottom = localAlign supportsLocal.bottom
            left = localAlign supportsLocal.left
            right = localAlign supportsLocal.right
        }
        basisVectors = (u1, u2)
        // since supports are in the local coordinate anchored along the bottom line,
        // system support.bottom.y = 0. Therefore the height is just the height of the top support
        area = (supportsLocal.right.x - supportsLocal.left.x) * supportsLocal.top.y
    }

/// Compute the oriented bounding box of a set of points in two dimensions.
let OrientedBoundingBox points =
    // grab the list of points that make the convex hull of what we were given
    match points |> ConvexHull2D with
    | ConvexHull hull ->
        hull
        // string the hull vertices together to form form edges and compute
        // the minimum rectangle that is incident with it
        |> List.mapi (fun i point ->
            // since we are going counter-clockwise, if we are at the end, next is the start
            let nextPoint = if i = hull.Length-1 then points.[0] else points.[i + 1]

            // compute the bounding box for the points oriented along the edge
            boundingBoxAlongEdge points (Edge (point, nextPoint))
        )
        // find the rectangle with the smallest area
        |> List.minBy (fun { area = area } ->  area)
