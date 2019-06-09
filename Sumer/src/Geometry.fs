module Sumer.Geometry

// externals
open UnityEngine
open System.Drawing

// an enum to indicate turn directionality (cross-product direction)
type TurnDirection = CoLinear | Clockwise | CounterClockwise

// a single case union so users have to flag a list is actually built as a convex hull
type ConvexHull2D = ConvexHull of List<Vector2>


// turnDirection returns the direction of the turn created by the three provided points
let private turnDirection (a: Vector2) (b: Vector2) (c: Vector2) =
    match (b.y-a.y)*(c.x-b.x)-(b.x-a.x)*(c.y-b.y) with
    | 0.0f -> CoLinear
    | i when i < 0.0f -> CounterClockwise
    | _ -> Clockwise

// given a list of points, compute the convex hull, ignoring the y-axis.
let ConvexHull2D points =
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
    let mutable result = ConvexHull [p0]

    // visit each point
    for (_, point) in polarCoods  do
        // pull the contents out of the convex hull
        match result with
        | ConvexHull res ->
            // while there are elements left and the last 3 points make a
            // counter-clockwise turn
            while res.Length > 1 && (turnDirection point res.Head res.Tail.Head = CounterClockwise) do
                // pop the stack
                result <- ConvexHull res.Tail

            // add the point to the list
            result <- ConvexHull (point::res)

    // we're done
    result


// a rectangle used in tracking and computing bounding boxes
type Rectangle = {
    area: float
    basisVectors: Vector2 * Vector2
    supportingIndices: int * int * int * int
}

let private rectangleCoincidentWith (p1:Vector2) (p2:Vector2) (points: List<Vector2>): Rectangle =
    // compute the basis vectors in the space aligned with our two vectors
    let u1 = p2 - p1
    u1.Normalize()
    let u2 = u1 |> Vector2.Perpendicular

    // the 4 supporting vertices for the rectangle coincident with our points
    val bottomVertex Vector2
    val leftVertex Vector2
    val rightVertex Vector2
    val bottomVertex Vector2

    // find the minimum rectangle
    for point in points do 
        // convert the point to a coordinate system with the origin at p2
        // and basis vectors (u1, u2)
        let diff = point - p2
        let localSpace = Vector2(Vector2.Dot(u1, diff), Vector2.Dot(u2, diff))

        // 

    
    ()


// compute the oriented bounding box of a set of points in two dimensions
let MinAreaBox2D points = (
    // compute the convex hull of the points
    let (ConvexHull hull) = points |> ConvexHull2D

    // we're going to start at the last edge in the hull to avoid mod logic
    hull
)





