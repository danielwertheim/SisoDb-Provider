/************************************************/
-- 2012-12-03, v1
-- NOTE! No warranties. Use at own risk
-- ENSURE YOU HAVE A BACKUP OF YOUR DB
-- Desc: Used to fix issues with polygons.
-- 1) Try to make all invalid, valid
-- 2) If a polygon seems to be inverted it will be fixed

-- Config: [your_db_here], [structure name here]
/************************************************/
--use [your_db_here]
go
begin tran;

-- (1) Try to make them valid
update dbo.[structure name here]Spatial set Geo = Geo.MakeValid() where Geo.STIsValid() <> 1;

-- (2) Fix any inverted shapes
update dbo.[structure name here]Spatial set Geo = Geo.ReorientObject() where Geo.EnvelopeAngle() > 90;

-- (3) Check if any multipolygons seems to have been defined
select Geo.STGeometryType() from dbo.[structure name here]Spatial where Geo.STNumGeometries() > 1;

-- Highlight "commit tran" or "rollback tran" after you are satisfied
-- with the patch, and run only the highlighted part.

--commit tran
--rollback tran