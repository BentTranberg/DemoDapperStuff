# DemoDapperStuff

This can help F# developers figure out how to use Dapper and Dapper.Contrib.

I believe Dapper.Contrib is rather essential for Dapper. So much that Dapper
really isn't complete without Dapper.Contrib. Don't worry, they're in the
same repo on GitHub, and it's reliable. The download counts for both are in
the millions, so this is good stuff.

There are also other Dapper.* packages that are worth considering, if you
have needs beyond CRUD and other basics. Go to https://dapper-tutorial.net/
and click on the 3rd Party Libraries link on the top. On the left you will
see a list of libraries, including Dapper.Contrib. If you select a library,
you will find documentation for that library.

If you want to run this program, then create a database named DemoDb in your
MS SQL (Express?) Server, and a table named User in that database. The
fields should be, in quasi-SQL :

Id int, not null, autoincrement, identity=yes
UserName varchar(max), not null
Role varchar(max), not null

Later I also want to demonstrate how I use SchemaZen to keep database schemas
in sync with the source code. SchemaZen can generate complete database
creation scripts from a database's schema, and also the reverse - it can
create a database from the creation scripts that it has created earlier. My
source normally contains creation scripts, and these scripts and the
developer's local database can be updated both ways. Given this
functionality, it is also easy to quickly view a diff between the source
scripts and temporary scripts generated from the local DB.