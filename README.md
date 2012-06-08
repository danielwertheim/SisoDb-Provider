# SisoDb - Simple-Structure-Oriented-Db - a document-oriented db-provider for SQL-Server 2005-2012 & SQLCE4.

By using SisoDb you get to store and retrieve complete POCO-graphs without having to specify:

* any mappings
* or extending any interfaces 
* or extending any base-classes

_there's also no proxies_ etc. **It's just storage of your entities made simple**.

## Getting started
Either you can read about it here, in the [Wiki](https://github.com/danielwertheim/SisoDb-Provider/wiki/getting-started) or have a look at this [short getting started screencast](http://vimeo.com/41374802).

## NuGet
SisoDb is available via [NuGet](http://nuget.org/packages?q=sisodb).

### [PM> Install-Package SisoDb.Sql2005](http://nuget.org/packages/SisoDb.Sql2005)

### [PM> Install-Package SisoDb.Sql2008](http://nuget.org/packages/SisoDb.Sql2008)

### [PM> Install-Package SisoDb.Sql2012](http://nuget.org/packages/SisoDb.Sql2012)

### [PM> Install-Package SisoDb.SqlCe4](http://nuget.org/packages/SisoDb.SqlCe4)

## NuGet - Cache providers

### [PM> Install-Package SisoDb.AspWebCache](http://nuget.org/packages/SisoDb.AspWebCache)

### [PM> Install-Package SisoDb.MsMemoryCache](http://nuget.org/packages/SisoDb.MsMemoryCache)

## More information
Follow [@danielwertheim](http://twitter.com/danielwertheim) and [@sisodb](http://twitter.com/sisodb)

For documentation, news etc, look under the wiki or goto: http://sisodb.com

## License
License: [The MIT License (MIT)](http://www.opensource.org/licenses/mit-license.php)

## SemVer for versioning
SisoDb uses [SemVer](http://semver.org) for versioning.

## Contribute
Line feeds setting should be `core.autocrlf=false`.

Unit-tests are written using `NUnit` and integration tests are written using [Machine Specifications](https://github.com/machine/machine.specifications). To get the integration tests to work you need to change the connection strings in respective projects `App.config`

Pull request should be against the **Develop branch** and you should have done your work in a feature branch, merged to the develop branch via `git merge --no-ff my-feature-branch`.