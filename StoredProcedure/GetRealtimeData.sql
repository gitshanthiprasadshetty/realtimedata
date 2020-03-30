GO
/****** Object:  StoredProcedure [dbo].[GetRealtimeData]    Script Date: 12-Dec-19 12:00:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--PLEASE NOTE: DO NOT CHANGE THE ORDER OF SELECT STATEMENT. IF NEW COLUMNS ARE REQUIRED PLEASE ADD AT THE END
ALTER PROCEDURE [dbo].[GetRealtimeData]
@startdate varchar(20),
@enddate varchar(20),
@acceptablesla int
as
DECLARE @DataFromDate VARCHAR(50)
DECLARE @DataToDate VARCHAR(50)
begin
	SET @DataFromDate = @startdate
	SET @DataToDate = @enddate

	select 
			t.Skill,
			SUM(t.ActiveTime) as ActiveTime, 
			SUM(t.HoldTime) as HoldTime, 
			SUM(t.AcwTime) as ACWTime, 
			COUNT(1) as TotalCallsInEachSkill,
			SUM(t.CallsAnsWithnSLA) as CallsAnsWithnSLA,
			SUM(QueueTime) as QueueTime
			from (
			select 
			Case when tmac.QueueTime < @acceptablesla then 1 else 0 end as CallsAnsWithnSLA, 
			tmac.ActiveTime as ActiveTime,
			tmac.HoldTime as HoldTime,
			tmac.AcwTime as AcwTime,
			Skill,
			tmac.QueueTime as QueueTime
			from TMAC_Interactions tmac
			where tmac.Skill is not null and tmac.Skill !='' 
			and ClosedDateTime >= @DataFromDate and ClosedDateTime <= @DataToDate ) t
			group by t.Skill
end