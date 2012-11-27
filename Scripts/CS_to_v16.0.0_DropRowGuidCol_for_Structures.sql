/************************************************/
-- 2012-11-27, v1
-- NOTE! No warranties. Use at own risk
-- ENSURE YOU HAVE A BACKUP OF YOUR DB
-- Desc: Used to drop rowguidcol from structure tables. This is a change introduced in v16.0.0
-- Might not be applicable to your db. It expects Structure tables for entities with GUID id
-- to have column [StructureId]:uniqueidentifier, rowguidcol which it will remove rowguidcol for
/************************************************/
--use [your_db_here]
go
declare @t table(r int identity, n nvarchar(max))
declare @s nvarchar(max)
declare @n int;

insert into @t
select 'alter table dbo.[' + t.name + '] alter column [' + c.name + '] drop rowguidcol;'
from sys.columns c 
inner join sys.tables t on c.object_id = t.object_id 
where c.name = 'StructureId' and c.is_rowguidcol = 1

set @n = 1;

print 'begin tran'
print 'go'
while @n <= (select max(r) from @t)
begin
	set @s = (select n from @t where r = @n)
	print @s
	set @n +=1;
end
print 'go'
print '--commit tran'
print '--rollback tran'