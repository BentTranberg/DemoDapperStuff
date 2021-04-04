module DemoDapperStuff.Main

open System
open System.Data.SqlClient
open Dapper
open Dapper.Contrib
open Dapper.Contrib.Extensions

module Da =

    let queryMultipleAsSeq<'T> (conn: SqlConnection, sql: string, args: obj) : 'T seq =
        conn.Query<'T> (sql, args)

    let queryMultipleToList<'T> (conn: SqlConnection, sql: string, args: obj) : 'T list =
        queryMultipleAsSeq (conn, sql, args)
        |> Seq.toList

let connectionString =
    // @"Server=.\SqlExpress;Database=DemoDb;User Id=sa;Password=password;"
    @"Server=.\SqlExpress;Database=DemoDb;Trusted_Connection=True;"

// Table names. Use brackets around table names, so that they never fail.
let [<Literal>] tableUser = "[User]" // "User" is a keyword, so the brackets are needed here.

[<Table (tableUser); CLIMutable>]
type EntUser =
    {
        Id: int
        UserName: string
        Role: string
    }

let getUsers () =
    use conn = new SqlConnection(connectionString)
    (conn, "SELECT * FROM " + tableUser, null)
    |> Da.queryMultipleToList<EntUser>

[<EntryPoint>]
let main _ =
    getUsers ()
    |> List.iter (fun user -> printfn "Id=%d User=%s" user.Id user.UserName)
    printfn "Done."
    Console.ReadKey() |> ignore
    0
