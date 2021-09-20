-- =============================================
-- Author:<Anish>
-- Version:<>
-- =============================================
Create Procedure Get_AsyncSkillDashboard(
@p_Date AS VARCHAR(20)
@p_skillExtn AS VARCHAR(20)
)
AS
DECLARE @DataFromDate VARCHAR(50)
DECLARE @DataToDate VARCHAR(50)
BEGIN
	SET @DataFromDate = CONCAT(@p_Date,'000000')
	SET @DataToDate = CONCAT(@p_Date, '235959') 
	DECLARE @AgentStatus TABLE(agent_id varchar(50), case_id varchar(50),sla_id int, time_stamp datetimeoffset(0), sla_given int, sla_elapsed int, sla_met int, [status] varchar(50))
	select agent_id,acs.case_id,sla_id,time_stamp,sla_given,sla_elapsed,sla_met,CCM.WorkCodeName as [status] 
	from tmac_async_chat_sla ACS inner join tmac_async_chat_history tcs on acs.case_id = tcs.case_id left join GBL_CallWorkCodeMapping CCM on ACS.sla_status = CCM.WorkCode
	where FORMAT(inserted_datetime, 'yyyyMMddHHmmss')>=@DataFromDate and FORMAT(inserted_datetime, 'yyyyMMddHHmmss')<=@DataToDate 

	Select tab.TotalResponseMet as TotalMetFirstResponse, tab.TotalResponseNotMet as TotalNotMetFirstResponse, Avg(tab.TotalResponseMet) as AverageFirstResponse
	FROM(
	SELECT
	TotalResponseMet = (SELECT Count(case_id) FROM @AgentStatus WHERE sla_id = 'SLA_1' and sla_met=1 and case_id = TA.case_id),
	TotalResponseNotMet = (SELECT Count(case_id) FROM @AgentStatus WHERE sla_id = 'SLA_1' and sla_met != 1 and case_id = TA.case_id),
	FROM tmac_async_chat_history with(nolock) TA
	where FORMAT(inserted_datetime, 'yyyyMMddHHmmss')>=@DataFromDate and FORMAT(inserted_datetime, 'yyyyMMddHHmmss')<=@DataToDate and queue_id=@p_skillExtn
	) tab
END