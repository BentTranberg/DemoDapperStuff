module Da

open System
open System.Data.SqlClient
open Dapper
open Dapper.Contrib
open Dapper.Contrib.Extensions

let ofSeq (s: 'T seq) = new ResizeArray<'T>(s)

/// Maps a nullable record (or possibly other types) to option.
let optionOfNullableRecord (record: 'T) : 'T option =
    match record |> box with
    | null -> None
    | _ -> Some record

// Warning: TRUNCATE will effectively reset the table. What does that mean?
//          A table with an autoincrementing Id will start counting from 1 again.
//          If you haven't planned for that scenario, this can ruin the logical
//          relation between entities in tables, since you'll be reusing Id and
//          similar, so think twice before using this.
let truncateTable (conn: SqlConnection, tableName) =
    conn.Execute ("TRUNCATE TABLE " + tableName)

// Note: This can take a long time for large tables, but unlike TRUNCATE this
//       will not reset the autoincrement counter, so you will continue to have
//       unique Id's and avoid potentially messing up relations.
let deleteAllTableRecords (conn: SqlConnection, tableName) =
    conn.Execute ("DELETE FROM " + tableName)

let insertEntity (conn: SqlConnection, entity: 'T) : int =
    conn.Insert entity |> int

let insertEntities (conn: SqlConnection, list: 'T list) =
    list |> ofSeq |> conn.Insert |> ignore

let updateEntity<'T when 'T : not struct> (conn: SqlConnection, entity: 'T) : bool =
    conn.Update<'T> entity // id

let querySingleOrDefault<'T> (conn: SqlConnection, sql: string, args: obj) : 'T option =
    conn.QuerySingleOrDefault<'T> (sql, args)
    |> optionOfNullableRecord

let queryMultipleAsSeq<'T> (conn: SqlConnection, sql: string, args: obj) : 'T seq =
    conn.Query<'T> (sql, args)

let queryMultipleToList<'T> (conn: SqlConnection, sql: string, args: obj) : 'T list =
    queryMultipleAsSeq (conn, sql, args)
    |> Seq.toList
