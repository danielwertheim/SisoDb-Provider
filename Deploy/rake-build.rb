#Template v1.0.0
#--------------------------------------
# Dependencies
#--------------------------------------
require 'albacore'
#--------------------------------------
# Debug
#--------------------------------------
#ENV.each {|key, value| puts "#{key} = #{value}" }
#--------------------------------------
# Environment vars
#--------------------------------------
@env_solutionname = 'SisoDb-Provider'
@env_solutionfolderpath = "../Source"

@env_projectnameCore = 'SisoDb'
@env_projectnameSql2005 = 'SisoDb.Sql2005'
@env_projectnameSql2008 = 'SisoDb.Sql2008'
@env_projectnameSql2012 = 'SisoDb.Sql2012'
@env_projectnameSqlCe4 = 'SisoDb.SqlCe4'
@env_projectnameAspWebCache = 'SisoDb.AspWebCache'
@env_projectnameMsMemoryCache = 'SisoDb.MsMemoryCache'
@env_projectnameDynamic = 'SisoDb.Dynamic'
@env_projectnameGlimpse = 'SisoDb.Glimpse'
@env_projectnameMiniProfiler = 'SisoDb.MiniProfiler'

@env_buildfolderpath = 'build'
@env_assversion = "14.2.0"
@env_version = "#{@env_assversion}"
@env_buildversion = @env_version + (ENV['env_buildnumber'].to_s.empty? ? "" : ".#{ENV['env_buildnumber'].to_s}")
@env_buildconfigname = ENV['env_buildconfigname'].to_s.empty? ? "Release" : ENV['env_buildconfigname'].to_s

buildNameSuffix = "-v#{@env_buildversion}-#{@env_buildconfigname}"
@env_buildnameCore = "#{@env_projectnameCore}#{buildNameSuffix}"
@env_buildnameSql2005 = "#{@env_projectnameSql2005}#{buildNameSuffix}"
@env_buildnameSql2008 = "#{@env_projectnameSql2008}#{buildNameSuffix}"
@env_buildnameSql2012 = "#{@env_projectnameSql2012}#{buildNameSuffix}"
@env_buildnameSqlCe4 = "#{@env_projectnameSqlCe4}#{buildNameSuffix}"
@env_buildnameAspWebCache = "#{@env_projectnameAspWebCache}#{buildNameSuffix}"
@env_buildnameMsMemoryCache = "#{@env_projectnameMsMemoryCache}#{buildNameSuffix}"
@env_buildnameDynamic = "#{@env_projectnameDynamic}#{buildNameSuffix}"
@env_buildnameGlimpse = "#{@env_projectnameGlimpse}#{buildNameSuffix}"
@env_buildnameMiniProfiler = "#{@env_projectnameMiniProfiler}#{buildNameSuffix}"
#--------------------------------------
# Reusable vars
#--------------------------------------
coreOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameCore}"
sql2005OutputPath = "#{@env_buildfolderpath}/#{@env_projectnameSql2005}"
sql2008OutputPath = "#{@env_buildfolderpath}/#{@env_projectnameSql2008}"
sql2012OutputPath = "#{@env_buildfolderpath}/#{@env_projectnameSql2012}"
sqlCe4OutputPath = "#{@env_buildfolderpath}/#{@env_projectnameSqlCe4}"
aspWebCacheOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameAspWebCache}"
msMemoryCacheOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameMsMemoryCache}"
dynamicOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameDynamic}"
glimpseOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameGlimpse}"
miniProfilerOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameMiniProfiler}"
#--------------------------------------
# Albacore flow controlling tasks
#--------------------------------------
task :ci => [:installNuGetPackages, :buildIt, :copyIt, :testIt, :zipIt, :packIt]

task :local => [:buildIt, :copyIt, :testIt, :zipIt, :packIt]
#--------------------------------------
task :copyIt => [:copyCore, :copySql2005, :copySql2008, :copySql2012, :copySqlCe4, :copyAspWebCache, :copyMsMemoryCache, :copyDynamic, :copyGlimpse, :copyMiniProfiler]

task :testIt => [:unittests]

task :zipIt => [:zipCore, :zipSql2005, :zipSql2008, :zipSql2012, :zipSqlCe4, :zipAspWebCache, :zipMsMemoryCache, :zipDynamic, :zipGlimpse, :zipMiniProfiler]

task :packIt => [:packCore, :packSql2005, :packSql2008, :packSql2012, :packSqlCe4, :packAspWebCache, :packMsMemoryCache, :packDynamic, :packGlimpse, :packMiniProfiler]
#--------------------------------------
# Albacore tasks
#--------------------------------------
task :installNuGetPackages do
    FileList["#{@env_solutionfolderpath}/**/packages.config"].each { |filepath|
        sh "NuGet.exe i #{filepath} -o #{@env_solutionfolderpath}/packages"
    }
end

assemblyinfo :versionIt do |asm|
    sharedAssemblyInfoPath = "#{@env_solutionfolderpath}/SharedAssemblyInfo.cs"

    asm.input_file = sharedAssemblyInfoPath
    asm.output_file = sharedAssemblyInfoPath
    asm.version = @env_assversion
    asm.file_version = @env_buildversion
end

task :ensureCleanBuildFolder do
    FileUtils.rm_rf(@env_buildfolderpath)
    FileUtils.mkdir_p(@env_buildfolderpath)
end

msbuild :buildIt => [:ensureCleanBuildFolder, :versionIt] do |msb|
    msb.properties :configuration => @env_buildconfigname
    msb.targets :Clean, :Build
    msb.solution = "#{@env_solutionfolderpath}/#{@env_solutionname}.sln"
end

def copyProject(projectName, outputPath)
    FileUtils.mkdir_p(outputPath)
    FileUtils.cp_r(FileList["#{@env_solutionfolderpath}/Projects/#{projectName}/bin/#{@env_buildconfigname}/**"], outputPath)
end

task :copyCore do
    copyProject(@env_projectnameCore, coreOutputPath)
end

task :copySql2005 do
    copyProject(@env_projectnameSql2005, sql2005OutputPath)
end

task :copySql2008 do
    copyProject(@env_projectnameSql2008, sql2008OutputPath)
end

task :copySql2012 do
    copyProject(@env_projectnameSql2012, sql2012OutputPath)
end

task :copySqlCe4 do
    copyProject(@env_projectnameSqlCe4, sqlCe4OutputPath)
end

task :copyAspWebCache do
    copyProject(@env_projectnameAspWebCache, aspWebCacheOutputPath)
end

task :copyMsMemoryCache do
    copyProject(@env_projectnameMsMemoryCache, msMemoryCacheOutputPath)
end

task :copyDynamic do
    copyProject(@env_projectnameDynamic, dynamicOutputPath)
end

task :copyGlimpse do
    copyProject(@env_projectnameGlimpse, glimpseOutputPath)
end

task :copyMiniProfiler do
    copyProject(@env_projectnameMiniProfiler, miniProfilerOutputPath)
end

nunit :unittests do |nunit|
    nunit.command = "nunit-console.exe"
    nunit.options "/framework=v4.0.30319","/xml=#{@env_buildfolderpath}/NUnit-Report-#{@env_solutionname}-UnitTests.xml"
    nunit.assemblies FileList["#{@env_solutionfolderpath}/Tests/#{@env_projectnameCore}.**UnitTests/bin/#{@env_buildconfigname}/#{@env_projectnameCore}.**UnitTests.dll"]
end

def zipProject(zip, outputPath, buildName)
    zip.directories_to_zip outputPath
    zip.output_file = "#{buildName}.zip"
    zip.output_path = @env_buildfolderpath
end

zip :zipCore do |zip|
    zipProject(zip, coreOutputPath, @env_buildnameCore)
end

zip :zipSql2005 do |zip|
    zipProject(zip, coreOutputPath, @env_buildnameSql2005)
end

zip :zipSql2008 do |zip|
    zipProject(zip, coreOutputPath, @env_buildnameSql2008)
end

zip :zipSql2012 do |zip|
    zipProject(zip, sql2012OutputPath, @env_buildnameSql2012)
end

zip :zipSqlCe4 do |zip|
    zipProject(zip, sqlCe4OutputPath, @env_buildnameSqlCe4)
end

zip :zipAspWebCache do |zip|
    zipProject(zip, aspWebCacheOutputPath, @env_buildnameAspWebCache)
end

zip :zipMsMemoryCache do |zip|
    zipProject(zip, msMemoryCacheOutputPath, @env_buildnameMsMemoryCache)
end

zip :zipDynamic do |zip|
    zipProject(zip, dynamicOutputPath, @env_buildnameDynamic)
end

zip :zipGlimpse do |zip|
    zipProject(zip, glimpseOutputPath, @env_buildnameGlimpse)
end

zip :zipMiniProfiler do |zip|
    zipProject(zip, miniProfilerOutputPath, @env_buildnameMiniProfiler)
end

def packProject(cmd, projectname, basepath)
    cmd.command = "NuGet.exe"
    cmd.parameters = "pack #{projectname}.nuspec -version #{@env_version} -basepath #{basepath} -outputdirectory #{@env_buildfolderpath}"
end

exec :packCore do |cmd|
    packProject(cmd, @env_projectnameCore, coreOutputPath)
end

exec :packSql2005 do |cmd|
    packProject(cmd, @env_projectnameSql2005, sql2005OutputPath)
end

exec :packSql2008 do |cmd|
    packProject(cmd, @env_projectnameSql2008, sql2008OutputPath)
end

exec :packSql2012 do |cmd|
    packProject(cmd, @env_projectnameSql2012, sql2012OutputPath)
end

exec :packSqlCe4 do |cmd|
    packProject(cmd, @env_projectnameSqlCe4, sqlCe4OutputPath)
end

exec :packAspWebCache do |cmd|
    packProject(cmd, @env_projectnameAspWebCache, aspWebCacheOutputPath)
end

exec :packMsMemoryCache do |cmd|
    packProject(cmd, @env_projectnameMsMemoryCache, msMemoryCacheOutputPath)
end

exec :packDynamic do |cmd|
    packProject(cmd, @env_projectnameDynamic, dynamicOutputPath)
end

exec :packGlimpse do |cmd|
    packProject(cmd, @env_projectnameGlimpse, glimpseOutputPath)
end

exec :packMiniProfiler do |cmd|
    packProject(cmd, @env_projectnameMiniProfiler, miniProfilerOutputPath)
end