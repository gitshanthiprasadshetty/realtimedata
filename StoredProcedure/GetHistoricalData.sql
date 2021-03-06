/*    ==Scripting Parameters==

    Source Server Version : SQL Server 2008 R2 (10.50.4042)
    Source Database Engine Edition : Microsoft SQL Server Express Edition
    Source Database Engine Type : Standalone SQL Server

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Standard Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [DOCMT1_TH]
GO
/****** Object:  StoredProcedure [dbo].[Get_HistoricalData]    Script Date: 2/7/2018 3:17:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER Procedure [dbo].[Get_HistoricalData](
@Date as Varchar(8),
@StartTime as Varchar(6),
@EndTime as Varchar(6),
@Id as Varchar(20),
@Type as Varchar(10),
@acceptableSL as int
)
As
Begin
	IF(@Type = 'skill')
	Begin
		-- load this table data andkeep in-mem loc , then on refresh reload
		select  @Date as [Date],
		@Id as SkillID,
		Tab1.SkillName as SkillName,
		SUM(Tab1.TotalInteraction) as TotalInteraction,
		SUM(Tab1.SpeedOfAnswer) / ISNULL(SUM(Tab1.TotalInteraction),1) as AvgSpeedOfAnswer,
		SUM(ISNULL(Tab1.AbandCalls,0)) as AbandCalls,
		ISNULL(SUM(Tab1.AbandTime),0) / COALESCE(CASE When SUM(Tab1.AbandCalls) = 0 then 1 else SUM(Tab1.AbandCalls) end, 1) as AvgAbandTime,
		SUM(Tab1.TalkTime) / ISNULL(SUM(Tab1.TotalInteraction),1)  as AvgTalkTime,
		SUM(Tab1.TalkTime) as TotalACDTime,
		SUM(Tab1.TotalACWTime) as TotalACWTime,
		ISNULL(SUM(Tab1.FlowIn),0) as FlowIn,
		ISNULL(SUM(Tab1.FlowOut),0) as FlowOut,
		0 as AvgStaffedTime,
		0 as AvgStaffedTime,
		--Isnull(TAB2.TotalStaffedTime,0) as TotalStaffedTime
		100 * ( SUM(Tab1.SLCount) / Isnull(SUM(Tab1.TotalInteraction),1)) as SLPercentage
		from (
		Select TS.SkillName , COUNT(1) as TotalInteraction , TI.AgentID, TI.CreatedDateTime,
		SpeedOfAnswer = SUM(DATEDIFF(SECOND,CONVERT(datetime, STUFF(STUFF(STUFF(TWQH.CreateDate+TWQH.CreatedTime, 9, 0, ' '), 12, 0, ':'), 15, 0, ':')),
		CONVERT(datetime,STUFF(STUFF(STUFF(TI.CreatedDateTime, 9, 0, ' '), 12, 0, ':'), 15, 0, ':')))),
		
		AbandTime = (Select TOP 1  SUM(DATEDIFF(SECOND,CONVERT(datetime, STUFF(STUFF(STUFF(CreateDate+CreatedTime, 9, 0, ' '), 12, 0, ':'), 15, 0, ':')),
		CONVERT(datetime,STUFF(STUFF(STUFF(RouteDate+RouteTime, 9, 0, ' '), 12, 0, ':'), 15, 0, ':')))) AS AbandTime From TMAC_WorkQueueHistory 
		where Reason = 'timedout' and CreateDate  = @Date and CreatedTime between @StartTime and @EndTime),

		AbandCalls =  (SELECT COUNT(1) FROM TMAC_WorkQueueHistory 
		where Reason = 'timedout' and CreateDate  = @Date and CreatedTime between @StartTime and @EndTime),

		TI.ActiveTime as TalkTime,
		TotalACWTime  = CASE WHEN TI.Channel IN ('Voice','Chat','TextChat','FBPost','FBPrivate','Email','Fax')  THEN TI.AcwTime ELSE 0 END,
		FlowIn  = CASE WHEN TI.Direction='in' THEN 1 ELSE 0 END,
		FlowOut  = CASE WHEN TI.Direction='out' THEN 1 ELSE 0 END,
		SLCount = CASE When QueueTime <= @acceptableSL THEN 1 ELSE 0 END

		from TMAC_Skills TS WITH(NOLOCK)
		Inner Join TMAC_Interactions TI WITH(NOLOCK)
		On TI.Skill = TS.SkillExtension 
		Inner Join TMAC_WorkQueueHistory TWQH WITH(NOLOCK)
		on TI.SessionID = TWQH.ItemID
		where TI.Skill = @Id and TI.CreatedDateTime >= @Date + @StartTime and TI.CreatedDateTime <= @Date + @EndTime 
		Group by  TI.AgentID, TI.CreatedDateTime,TS.SkillName,TWQH.Reason,TI.ActiveTime,TI.AcwTime,TI.Direction,QueueTime,TI.Channel
		) AS Tab1	
		Group by Tab1.SkillName--,Tab1.AbandCalls,Tab1.SLCount, Tab1.TotalInteraction
	End

	IF(@Type = 'agent')
	Begin
		Select 
		Isnull(SUM(Tab1.TotalInteraction),0) TotalInteraction,
		Isnull(SUM(Tab1.TotalInteractionTime),0) TotalInteractionTime,
		AvgInteractionTime = (Isnull(SUM(Tab1.TotalInteractionTime),0) / case when SUM(Tab1.TotalInteraction) = 0 then 1 else SUM(Tab1.TotalInteraction) end),
		Isnull(SUM(Tab1.TotalChat),0)TotalChat,
		Isnull(SUM(Tab1.TotalChatTime),0) TotalChatTime,
		AvgChatTime = (Isnull(SUM(Tab1.TotalChatTime),0) / case when SUM(Tab1.TotalChat)= 0 then 1 else SUM(Tab1.TotalChat) end),
		Isnull(SUM(Tab1.TotalAudioIP),0)TotalAudioIP,
		Isnull(SUM(Tab1.TotalAudioIPTime),0) TotalAudioIPTime,
		AvgAudioIPTime = (Isnull(SUM(Tab1.TotalAudioIPTime),0) / case when SUM(Tab1.TotalAudioIP)= 0 then 1 else SUM(Tab1.TotalAudioIP) end)
		FROM (Select 
		TotalInteraction = CASE WHEN Channel IN ('Voice','Chat','TextChat','FBPost','FBPrivate','Email','Fax') THEN  1 ELSE 0 END,
		TotalInteractionTime = CASE WHEN Channel IN ('Voice','Chat','TextChat','FBPost','FBPrivate','Email','Fax') THEN  ActiveTime ELSE 0 END,
		TotalChat  = CASE WHEN Channel IN ('Chat','TextChat') THEN 1 ELSE 0 END,
		TotalChatTime  = CASE WHEN Channel IN ('Chat','TextChat') THEN ActiveTime ELSE 0 END,
		TotalAudioIP  = CASE WHEN SubChannel='AudioIP' THEN 1 ELSE 0 END,
		TotalAudioIPTime  = CASE WHEN SubChannel='AudioIP' THEN ActiveTime ELSE 0 END
		From TMAC_Interactions S with(nolock) 		
		Where CreatedDateTime>= @Date+@StartTime AND CreatedDateTime<=@Date+@EndTime AND S.AgentId = @Id
		) Tab1
	End
End




