/************************************************/
-- 2012-11-27, v1
-- NOTE! No warranties. Use at own risk
-- ENSURE YOU HAVE A BACKUP OF YOUR DB
-- Desc: Used to generate SQL for applying new v16.1.0 indices to Indexes-tables.
-- Might not be applicable to your db. It expects Indexes-tables for Strings and Texts
-- to have index: "IX_[entity]Strings_Q", "IX_[entity]Texts_Q" which will be dropped
-- and re-created. It will also add new index: "IX_[entity]Strings_SID", "IX_[entity]Texts_SID"
/************************************************/
--use [your_db_here]
go
declare @t table(r int identity, n nvarchar(max))
declare @s nvarchar(max)
declare @n int;

insert into @t
select 'drop index [' + i.name + '] on dbo.[' + t.name + '];'
from sys.indexes i
inner join sys.tables t on i.object_id = t.object_id
where i.name like 'IX_%Strings_Q' or i.name like 'IX_%Texts_Q';

insert into @t
select 'create index [' + i.name + '] on dbo.[' + t.name + ']([MemberPath] asc, [Value] asc);'
from sys.indexes i
inner join sys.tables t on i.object_id = t.object_id
where i.name like 'IX_%Strings_Q';

insert into @t
select 'create index [' + i.name + '] on dbo.[' + t.name + ']([MemberPath] asc) include([Value]);'
from sys.indexes i
inner join sys.tables t on i.object_id = t.object_id
where i.name like 'IX_%Texts_Q';

insert into @t
select 'create index [IX_' + t.name + '_SID] on dbo.[' + t.name + ']([StructureId] asc);'
from sys.indexes i
inner join sys.tables t on i.object_id = t.object_id
where i.name like 'IX_%Strings_Q' or i.name like 'IX_%Texts_Q';

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