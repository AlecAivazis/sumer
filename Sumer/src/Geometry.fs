module Sumer.Geometry

// externals
open UnityEngine

// given a list of points, compute the convex hull, ignoring the y-axis.
let ConvexHull2D points =
    // we're going to compute the convex hull using the Graham Scan method
    // as described here: https://en.wikipedia.org/wiki/Graham_scan#Pseudocode

    // let's compute the lowest point, favoring points to the left in case of a tie
    let p0 = points |> List.reduce (fun (lowest: Vector2) (next: Vector2) ->
        match (lowest, next) with
        // if the new item is lower use it
        | (l, n) when n.y < l.y -> n
        // if they are at the same height but the new item is to the left use it
        | (l, n) when n.y = l.y && n.x < l.x -> n
        // otherwise the lowest still considered lower
        | _ -> lowest
    )

    printf "lowest point: (%f, %f)\n" p0.x p0.y

    // we now have to sort the incoming list of points by polar angle relative to p0
    // if two points have the same polar angle, we have to only include the furthest one

    // to do this, we're going to use a map to track if we've seen an angle before
    // and then turn that map into a list which will order the entries by angle in ascending order
    let polarCoods = (List.fold (fun (map: Map<float, Vector2>) (point: Vector2) ->
        // compute the polar coordinate of this point with p0 and convert into degrees
        let coords = atan2 (point.y - p0.y ) (point.x - p0.x) |> float |> (*) (180./System.Math.PI)

        match map.TryFind(coords) with
        // we haven't seen the coordinates before so use it
        | None -> map.Add(coords, point)
        // we've seen the coordinate and this new point is further away
        | Some otherPoint when Vector2.Distance(point, p0) > Vector2.Distance(otherPoint, p0) ->
            // replace the new point over the old point in the map
            map.Add(coords, point)
        // anything else we keep life as it is...
        | _ -> map
    // turn the map into a list which will sort it in ascending order by key
    ) Map.empty points) |> Map.toList

    // log the list of angles
    printf "angles %A\n" polarCoods

    // we're going to keep a list of points that we will build up as we walk
    // around the set. This structure must have stack semantics
    let result = []

    // for now just return the list we were given
    result
