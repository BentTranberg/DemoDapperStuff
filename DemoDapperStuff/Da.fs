module Da

open System
open System.Data.SqlClient
open Dapper
open Dapper.Contrib
open Dapper.Contrib.Extensions

let queryMultipleAsSeq<'T> (conn: SqlConnection, sql: string, args: obj) : 'T seq =
    conn.Query<'T> (sql, args)

let queryMultipleToList<'T> (conn: SqlConnection, sql: string, args: obj) : 'T list =
    queryMultipleAsSeq (conn, sql, args)
    |> Seq.toList
