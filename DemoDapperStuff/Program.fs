module DemoDapperStuff.Main

open System
open System.Data.SqlClient
open Dapper
open Dapper.Contrib
open Dapper.Contrib.Extensions

let connectionString =
    // @"Server=.\SqlExpress;Database=DemoDb;User Id=sa;Password=password;"
    @"Server=.\SqlExpress;Database=DemoDb;Trusted_Connection=True;"

// Table names. Use brackets around table names, so that they never fail.
let [<Literal>] tableUser = "[User]" // "User" is a keyword, so the brackets are needed here.

[<Table (tableUser); CLIMutable>]
type EntUser = { Id: int; UserName: string; Role: string }

let truncateUsersTable () =
    use conn = new SqlConnection(connectionString)
    Da.truncateTable (conn, tableUser)

let addUser (user: EntUser) =
    use conn = new SqlConnection(connectionString)
    Da.insertEntity (conn, user) |> ignore

let addUsers (users: EntUser list) =
    use conn = new SqlConnection(connectionString)
    Da.insertEntities (conn, users) |> ignore

let changeUserRoleByName (userName: string) (newRole: string) =
    use conn = new SqlConnection(connectionString)
    let user =
        (conn, "SELECT * FROM " + tableUser + " WHERE UserName = @userName", {| userName = userName |})
        |> Da.querySingleOrDefault<EntUser>
    match user with
    | Some user ->
        let user = { user with Role = newRole }
        Da.updateEntity<EntUser> (conn, user)
    | None -> false

let getUsers () =
    use conn = new SqlConnection(connectionString)
    (conn, "SELECT * FROM " + tableUser, null)
    |> Da.queryMultipleToList<EntUser>

let getUsersWithRole (role: string) =
    use conn = new SqlConnection(connectionString)
    (conn, "SELECT * FROM " + tableUser + " WHERE Role = @role", {| role = role |})
    |> Da.queryMultipleToList<EntUser>

let getUserByName (userName: string) =
    use conn = new SqlConnection(connectionString)
    (conn, "SELECT * FROM " + tableUser + " WHERE UserName = @userName", {| userName = userName |})
    |> Da.querySingleOrDefault<EntUser>

// Just a note about SQL injection attacks, in case you wonder about the SQL used above.
// The arguments to the query are supplied in the recommended way. In case you wonder
// about the use of the tableUser to construct the query, this is a literal string. As
// such it can't be used to modify the query string during program execution.

[<EntryPoint>]
let main _ =
    truncateUsersTable () |> ignore // Start with a clean slate.

    printfn "Add our first user."
    addUser { Id = 0; UserName = "Frodo"; Role = "Baggins" }

    printfn "Add more users."
    let add u r = { Id = 0; UserName = u; Role = r }
    addUsers [
        add "Aragorn" "king"
        add "Isildur" "king"
        add "Sauron" "evil"
        add "Arwen" "elf"
        add "Galadriel" "elf"
        add "Bilbo" "BAGGINS"
    ]

    printfn "List of all users"
    getUsers ()
    |> List.iter (fun user -> printfn "  Id=%d User=%s" user.Id user.UserName)

    printfn "List of role Baggins."
    getUsersWithRole "baggins"
    |> List.iter (fun user -> printfn "  Id=%d User=%s Role=%s" user.Id user.UserName user.Role)

    let findUser userName =
        printfn "Find %s." userName
        match getUserByName userName with
        | Some user -> printfn "  Id=%d User=%s Role=%s" user.Id user.UserName user.Role
        | None -> printfn "  User %s not found." userName
    findUser "Frodo"
    findUser "Nobody"

    printfn "Make Frodo a hero."
    match changeUserRoleByName "Frodo" "hero" with
    | true -> printfn "Frodo is a hero."
    | false -> printfn "Can't make Frodo a hero."

    printfn "Done."
    Console.ReadKey() |> ignore
    0
