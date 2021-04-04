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

let truncateTable (conn: SqlConnection, tableName) =
    conn.Execute ("TRUNCATE TABLE " + tableName)

let deleteAllTableRecords (conn: SqlConnection, tableName) =
    conn.Execute ("DELETE FROM " + tableName)

let insertEntity (conn: SqlConnection, entity: 'T) : int =
    conn.Insert entity |> int

let insertEntities (conn: SqlConnection, list: 'T list) =
    list |> ofSeq |> conn.Insert |> ignore

let querySingleOrDefault<'T> (conn: SqlConnection, sql: string, args: obj) : 'T option =
    conn.QuerySingleOrDefault<'T> (sql, args)
    |> optionOfNullableRecord

let queryMultipleAsSeq<'T> (conn: SqlConnection, sql: string, args: obj) : 'T seq =
    conn.Query<'T> (sql, args)

let queryMultipleToList<'T> (conn: SqlConnection, sql: string, args: obj) : 'T list =
    queryMultipleAsSeq (conn, sql, args)
    |> Seq.toList
